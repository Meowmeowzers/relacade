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
		public InputTileSet inputTileSet;
		public List<InputTile> inputTiles;
		public List<EditorCell> fixedTiles;

		[Range(0, 80)]
		public int gridSizeX = 0;

		[Range(0, 80)]
		public int gridSizeY = 0;

		[Min(1)]
		public float tileSize = 1f;

		[SerializeField] private bool isDone = true;
		[SerializeField] private bool isInitialized = false;
		[SerializeField] private bool hasFixed = false; // used outside as serialized property

		[SerializeField] private GridSize gridCells; // serializing solves the problem of data persistence (it needs to be serialized? before it can be saved?)

		private EditorCell selectedCell;
		private List<EditorCell> lowestEntropyCells = new();
		private static GameObject tempGameObject;

		private EditorCoroutine coroutine;
		private readonly EditorWaitForSeconds editorWait = new(0f);

		public enum LookDirection
		{ UP, DOWN, LEFT, RIGHT };

		public void StartCollapse()
		{
			coroutine = EditorCoroutineUtility.StartCoroutineOwnerless(CCollapseWave());
		}

		public void StartCollapseFixed()
		{
			coroutine = EditorCoroutineUtility.StartCoroutineOwnerless(CollapseFixedTiles());
		}
		
		public void StopCollapse()
		{
			if (coroutine != null)
				EditorCoroutineUtility.StopCoroutine(coroutine);
			isDone = true;
		}

		public void InitializeWave()
		{
			isInitialized = false;

			if (inputTileSet == null) { Debug.LogWarning("Tile set is missing..."); return; }
			if (inputTileSet.allInputTiles.Count < 1) { Debug.LogWarning("Tile set is empty. Configure it first."); return; }

			if (isDone)
			{
				RemoveGridCells();
				hasFixed = false;

				Vector3 pos;
				GameObject cellPrefab;
				gridCells = new(gridSizeX);

				for (int y = 0; y < gridSizeY; y++)
				{
					gridCells.row.Add(new());

					for (int x = 0; x < gridSizeX; x++)
					{
						pos = new(tileSize * x - (gridSizeX * tileSize / 2 - .5f) + transform.position.x, tileSize * y - (gridSizeY * tileSize / 2 - .5f) + transform.position.y);

						cellPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.gatozhanya.relacade/Objects/GridCell.prefab");

						tempGameObject = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
						gridCells.row[x].column.Add(tempGameObject);
						gridCells.row[x].column[y].GetComponent<EditorCell>().xIndex = x;
						gridCells.row[x].column[y].GetComponent<EditorCell>().yIndex = y;
						gridCells.row[x].column[y].GetComponent<EditorCell>().Initialize(inputTiles);
						EditorUtility.SetDirty(gridCells.row[x].column[y]);
					}
				}
				isInitialized = true;
			}
		}

		private IEnumerator CCollapseWave()
		{
			isDone = false;
			
			if (!CheckTiles()) yield break;
			
			while (!IsWaveCollapsed())
			{
				//Observation Phase
				lowestEntropyCells = GetLowestEntropyCells();
				selectedCell = lowestEntropyCells[UnityEngine.Random.Range(0, lowestEntropyCells.Count)];

				//Collapse Tile
				if (selectedCell.IsCellNotConflict())
				{
					selectedCell.SelectRandomTile();
				}
				else
				{
					Debug.LogWarning("Conflict on cell " + selectedCell.xIndex + " " + selectedCell.yIndex);
					isDone = true;
					yield break;
				}

				//Propagation Phase
				PropagateConstraints(selectedCell);
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
				if (!cell.IsNotDefiniteState()) continue;

				cell.SelectFixedTile();
				PropagateConstraints(cell);
				yield return editorWait;
			}
		}
		
		public void SetFields(int x, int y, float size, InputTileSet set)
		{
			if (!isDone) return;

			gridSizeX = x;
			gridSizeY = y;
			tileSize = size;
			inputTileSet = set;
		}
		
		public void ResetCells()
		{
			if (inputTiles.Count < 1 && isInitialized)
			{
				Debug.LogWarning("No tile set or no tiles in tile set");
				return;
			}

			for (int x = 0; x < gridSizeX; x++)
			{
				for (int y = 0; y < gridSizeY; y++)
				{
					if (gridCells.row[x].column[y].GetComponent<EditorCell>())
						gridCells.row[x].column[y].GetComponent<EditorCell>().ResetAndInitializeCell(inputTiles);
				}
			}
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
			foreach (var item in inputTiles)
			{
				if (item == null)
				{
					StopCollapse();
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

			if (y + 1 > -1 && y + 1 < gridSizeY)
				PropagateToCell(x, y, cell.selectedTile, LookDirection.UP);

			if (y - 1 > -1 && y - 1 < gridSizeY)
				PropagateToCell(x, y, cell.selectedTile, LookDirection.DOWN);

			if (x + 1 > -1 && x + 1 < gridSizeX)
				PropagateToCell(x, y, cell.selectedTile, LookDirection.RIGHT);

			if (x - 1 > -1 && x - 1 < gridSizeX)
				PropagateToCell(x, y, cell.selectedTile, LookDirection.LEFT);
		}

		private void PropagateToCell(int x, int y, InputTile item, LookDirection direction)
		{
			switch (direction)
			{
				case LookDirection.UP:
					y++;
					if (gridCells.row[x].column[y].GetComponent<EditorCell>().IsNotDefiniteState() && !gridCells.row[x].column[y].GetComponent<EditorCell>().IsFixed())
					{
						gridCells.row[x].column[y].GetComponent<EditorCell>().PropagateWith(item.compatibleTop);
					}
					break;

				case LookDirection.DOWN:
					y--;
					if (gridCells.row[x].column[y].GetComponent<EditorCell>().IsNotDefiniteState() && !gridCells.row[x].column[y].GetComponent<EditorCell>().IsFixed())
					{
						gridCells.row[x].column[y].GetComponent<EditorCell>().PropagateWith(item.compatibleBottom);
					}
					break;

				case LookDirection.RIGHT:
					x++;
					if (gridCells.row[x].column[y].GetComponent<EditorCell>().IsNotDefiniteState() && !gridCells.row[x].column[y].GetComponent<EditorCell>().IsFixed())
					{
						gridCells.row[x].column[y].GetComponent<EditorCell>().PropagateWith(item.compatibleRight);
					}
					break;

				case LookDirection.LEFT:
					x--;
					if (gridCells.row[x].column[y].GetComponent<EditorCell>().IsNotDefiniteState() && !gridCells.row[x].column[y].GetComponent<EditorCell>().IsFixed())
					{
						gridCells.row[x].column[y].GetComponent<EditorCell>().PropagateWith(item.compatibleLeft);
					}
					break;
			}
		}

		private List<EditorCell> GetLowestEntropyCells()
		{
			List<EditorCell> lowestEntropyCellsSelected = new();
			int lowestEntropy = int.MaxValue;
			int entropy;

			for (int y = 0; y < gridSizeY; y++)
			{
				for (int x = 0; x < gridSizeX; x++)
				{
					if (gridCells.row[x].column[y].GetComponent<EditorCell>().IsNotDefiniteState())
					{
						entropy = gridCells.row[x].column[y].GetComponent<EditorCell>().entropy;

						if (entropy < lowestEntropy)
						{
							lowestEntropy = entropy;
							lowestEntropyCellsSelected.Clear();
							lowestEntropyCellsSelected.Add(gridCells.row[x].column[y].GetComponent<EditorCell>());
						}
						else if (entropy == lowestEntropy)
						{
							lowestEntropyCellsSelected.Add(gridCells.row[x].column[y].GetComponent<EditorCell>());
						}
					}
				}
			}
			return lowestEntropyCellsSelected;
		}

		private List<EditorCell> GetFixedCells()
		{
			List<EditorCell> fixedCells = new();

			for (int x = 0; x < gridSizeX; x++)
			{
				for (int y = 0; y < gridSizeY; y++)
				{
					if (gridCells.row[x].column[y].GetComponent<EditorCell>().IsFixed())
					{
						fixedCells.Add(gridCells.row[x].column[y].GetComponent<EditorCell>());
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

		public bool CheckIfSameSize(int tempX, int tempY, float tempCellSize, InputTileSet tempSet)
		{
			if (tempX == gridSizeX && tempY == gridSizeY && tempCellSize == tileSize && tempSet == inputTileSet)
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
			for (int x = 0; x < gridCells.row.Count; x++)
			{
				for (int y = 0; y < gridCells.row[x].column.Count; y++)
				{
					if (gridCells.row[x].column[y].GetComponent<EditorCell>().IsNotDefiniteState()) return false;
				}
			}
			return true;
		}

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