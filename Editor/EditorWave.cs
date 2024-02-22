using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

using Unity.EditorCoroutines.Editor;

#endif

namespace HelloWorld.Editor
{
	public class EditorWave : MonoBehaviour
	{
		public TileInputSet tileInputSet;
		public List<TileInput> allTileInputs;
		public List<EditorCell> fixedTiles;

		[Range(0, 80)]
		public int tileSizeX = 0;

		[Range(0, 80)]
		public int tileSizeY = 0;

		[Min(1)]
		public float tileSize = 1f;

		[SerializeField] private bool isDone = true;
		[SerializeField] private bool isInitialized = false;
		[SerializeField] private bool hasFixed = false; // used outside as serialized property

		[SerializeField] private GridRow gridCell; // serializing solves the problem of data persistence (it needs to be serialized? before it can be saved?)

		private GameObject gridCellObject;
		private GameObject tempGameObject;

		private EditorCell selectedRandomCell;
		private List<EditorCell> lowestEntropyCells = new();

		private EditorCoroutine coroutine;
		private readonly EditorWaitForSeconds editorWait = new(0f);

		public enum LookDirection
		{ UP, DOWN, LEFT, RIGHT };

		public void StartCollapse()
		{
			coroutine = EditorCoroutineUtility.StartCoroutineOwnerless(CollapseWave());
		}

		public void StartCollapseFixedTiles()
		{
			coroutine = EditorCoroutineUtility.StartCoroutineOwnerless(CollapseFixedTiles());
		}
		
		public void Stop()
		{
			if (coroutine != null)
				EditorCoroutineUtility.StopCoroutine(coroutine);
			isDone = true;
		}

		public void InitializeWave()
		{
			if (allTileInputs.Count != 0 && tileInputSet != null && isDone)
			{
				RemoveGridCells();
				hasFixed = false;

				Vector3 pos;
				gridCellObject = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.gatozhanya.relacade/Objects/EditorGridCell.prefab");
				gridCell = new(tileSizeX);

				for (int y = 0; y < tileSizeY; y++)
				{
					gridCell.row.Add(new());

					for (int x = 0; x < tileSizeX; x++)
					{
						pos = new(tileSize * x - (tileSizeX * tileSize / 2 - .5f) + transform.position.x, tileSize * y - (tileSizeY * tileSize / 2 - .5f) + transform.position.y);

						tempGameObject = Instantiate(gridCellObject, pos, Quaternion.identity, transform);
						gridCell.row[x].column.Add(tempGameObject);
						gridCell.row[x].column[y].GetComponent<EditorCell>().xIndex = x;
						gridCell.row[x].column[y].GetComponent<EditorCell>().yIndex = y;
						gridCell.row[x].column[y].GetComponent<EditorCell>().Initialize(allTileInputs);
						EditorUtility.SetDirty(gridCell.row[x].column[y]);
					}
				}
				isInitialized = true;
			}
			else
			{
				Debug.LogWarning("Tile set is empty...");
				isInitialized = false;
			}
		}

		private IEnumerator CollapseWave()
		{
			isDone = false;
			
			if (!CheckTiles()) yield break;
			
			while (!IsWaveCollapsed())
			{
				//Observation Phase
				lowestEntropyCells = GetLowestEntropyCells();
				selectedRandomCell = lowestEntropyCells[UnityEngine.Random.Range(0, lowestEntropyCells.Count)];

				//Collapse Tile
				if (selectedRandomCell.IsCellNotConflict())
				{
					selectedRandomCell.SelectRandomTile();
				}
				else
				{
					Debug.LogWarning("Conflict on cell " + selectedRandomCell.xIndex + " " + selectedRandomCell.yIndex);
					isDone = true;
					yield break;
				}

				//Propagation Phase
				PropagateConstraints(selectedRandomCell);
				yield return editorWait;
			}
			EditorUtility.SetDirty(gameObject); // to enable saving of modified gameobjects in scene
			isDone = true;
		}
		
		private IEnumerator CollapseFixedTiles()
		{
			if (!CheckTiles() && hasFixed) yield break;

			fixedTiles = GetFixedCells();

			foreach (var cell in fixedTiles)
			{
				cell.SelectFixedTile();
				PropagateConstraints(cell);
				yield return editorWait;
			}
		}
		
		public void SetSize(int x, int y, float size)
		{
			if (!isDone) return;

			tileSizeX = x;
			tileSizeY = y;
			tileSize = size;
		}
		
		public void StartResetCells()
		{
			if (allTileInputs.Count < 1 && isInitialized)
			{
				Debug.LogWarning("No tile set or no tiles in tile set");
				return;
			}
			ResetCells();
		}
		
		public void FinalizeGrid()
		{
			GameObject newParentObject = new("Generated Content"); ;
			newParentObject.transform.position = this.transform.position;

			int childCount = this.transform.childCount;
			int grandChildCount;
			Transform childTransform;
			Transform grandChildTransform;

			for (int i = 0; i < childCount; i++)
			{
				childTransform = transform.GetChild(i);
				grandChildCount = childTransform.childCount;

				for (int j = 0; j < grandChildCount; j++)
				{
					grandChildTransform = childTransform.GetChild(j);
					grandChildTransform.SetParent(newParentObject.transform);
				}

				childTransform.SetParent(this.transform);
			}

			DestroyImmediate(this.gameObject);
		}

		private bool CheckTiles()
		{
			foreach (var item in allTileInputs)
			{
				if (item == null)
				{
					Stop();
					Debug.Log("A tile is null or not configured. Please check again before generating.....");
					return false;
				}
			}
			return true;
		}

		private void PropagateConstraints(EditorCell cell)
		{
			int x = cell.xIndex;
			int y = cell.yIndex;

			if (y + 1 > -1 && y + 1 < tileSizeY)
				PropagateToCell(x, y, cell.selectedTile, LookDirection.UP);

			if (y - 1 > -1 && y - 1 < tileSizeY)
				PropagateToCell(x, y, cell.selectedTile, LookDirection.DOWN);

			if (x + 1 > -1 && x + 1 < tileSizeX)
				PropagateToCell(x, y, cell.selectedTile, LookDirection.RIGHT);

			if (x - 1 > -1 && x - 1 < tileSizeX)
				PropagateToCell(x, y, cell.selectedTile, LookDirection.LEFT);
		}

		private void PropagateToCell(int x, int y, TileInput item, LookDirection direction)
		{
			switch (direction)
			{
				case LookDirection.UP:
					y++;
					if (gridCell.row[x].column[y].GetComponent<EditorCell>().IsNotDefiniteState() && !gridCell.row[x].column[y].GetComponent<EditorCell>().IsFixed())
					{
						gridCell.row[x].column[y].GetComponent<EditorCell>().PropagateWith(item.compatibleTop);
					}
					break;

				case LookDirection.DOWN:
					y--;
					if (gridCell.row[x].column[y].GetComponent<EditorCell>().IsNotDefiniteState() && !gridCell.row[x].column[y].GetComponent<EditorCell>().IsFixed())
					{
						gridCell.row[x].column[y].GetComponent<EditorCell>().PropagateWith(item.compatibleBottom);
					}
					break;

				case LookDirection.RIGHT:
					x++;
					if (gridCell.row[x].column[y].GetComponent<EditorCell>().IsNotDefiniteState() && !gridCell.row[x].column[y].GetComponent<EditorCell>().IsFixed())
					{
						gridCell.row[x].column[y].GetComponent<EditorCell>().PropagateWith(item.compatibleRight);
					}
					break;

				case LookDirection.LEFT:
					x--;
					if (gridCell.row[x].column[y].GetComponent<EditorCell>().IsNotDefiniteState() && !gridCell.row[x].column[y].GetComponent<EditorCell>().IsFixed())
					{
						gridCell.row[x].column[y].GetComponent<EditorCell>().PropagateWith(item.compatibleLeft);
					}
					break;
			}
		}

		private List<EditorCell> GetLowestEntropyCells()
		{
			List<EditorCell> lowestEntropyCellsSelected = new();
			int lowestEntropy = int.MaxValue;
			int entropy;

			for (int y = 0; y < tileSizeY; y++)
			{
				for (int x = 0; x < tileSizeX; x++)
				{
					if (gridCell.row[x].column[y].GetComponent<EditorCell>().IsNotDefiniteState())
					{
						entropy = gridCell.row[x].column[y].GetComponent<EditorCell>().entropy;

						if (entropy < lowestEntropy)
						{
							lowestEntropy = entropy;
							lowestEntropyCellsSelected.Clear();
							lowestEntropyCellsSelected.Add(gridCell.row[x].column[y].GetComponent<EditorCell>());
						}
						else if (entropy == lowestEntropy)
						{
							lowestEntropyCellsSelected.Add(gridCell.row[x].column[y].GetComponent<EditorCell>());
						}
					}
				}
			}
			return lowestEntropyCellsSelected;
		}

		private List<EditorCell> GetFixedCells()
		{
			List<EditorCell> fixedCells = new();

			for (int x = 0; x < tileSizeX; x++)
			{
				for (int y = 0; y < tileSizeY; y++)
				{
					if (gridCell.row[x].column[y].GetComponent<EditorCell>().IsFixed())
					{
						fixedCells.Add(gridCell.row[x].column[y].GetComponent<EditorCell>());
					}
				}
			}
			return fixedCells;
		}

		private void RemoveGridCells()
		{
			EditorCell[] cell = GetComponentsInChildren<EditorCell>();
			foreach (var item in cell)
			{
				DestroyImmediate(item.gameObject);
			}
		}

		private void ResetCells()
		{
			for (int x = 0; x < tileSizeX; x++)
			{
				for (int y = 0; y < tileSizeY; y++)
				{
					gridCell.row[x].column[y].GetComponent<EditorCell>().ResetAndInitializeCell(allTileInputs);
				}
			}
		}

		public bool CheckIfSameSize(int tempX, int tempY, float tempCellSize)
		{
			if (tempX == tileSizeX && tempY == tileSizeY && tempCellSize == tileSize)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		
		private bool IsWaveCollapsed()
		{
			for (int x = 0; x < gridCell.row.Count; x++)
			{
				for (int y = 0; y < gridCell.row[x].column.Count; y++)
				{
					if (gridCell.row[x].column[y].GetComponent<EditorCell>().IsNotDefiniteState()) return false;
				}
			}
			return true;
		}

		//public void CheckCellObject()
		//{
		//	gridCellObject = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.gatozhanya.relacade/Objects/EditorGridCell.prefab");
		//}

		public bool IsDone()
		{
			return isDone;
		}

		public bool IsInitialized()
		{
			return isInitialized;
		}
	}
}