using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

		[Range(2, 80)]
		public int tileSizeX = 8;

		[Range(2, 80)]
		public int tileSizeY = 8;

		[Min(1)]
		public float tileSize = 1f;

		private GameObject gridCellObject;
		private GameObject tempGameObject;

		private EditorCell[,] gridCell;
		private EditorCell selectedRandomCell;
		private List<EditorCell> lowestEntropyCells = new();

		private EditorCoroutine coroutine;
		private readonly EditorWaitForSeconds editorWait = new(0f);

		private bool isDone = true;
		private bool isInitialized = false;
		[SerializeField] private bool hasFixed = false; // used outside as serialized property

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
				gridCell = new EditorCell[tileSizeX, tileSizeY];

				for (int y = 0; y < tileSizeY; y++)
				{
					for (int x = 0; x < tileSizeX; x++)
					{
						pos = new(tileSize * x - (tileSizeX * tileSize / 2 - .5f) + transform.position.x, tileSize * y - (tileSizeY * tileSize / 2 - .5f) + transform.position.y);

						tempGameObject = Instantiate(gridCellObject, pos, Quaternion.identity, transform);

						gridCell[x, y] = tempGameObject.GetComponent<EditorCell>();
						gridCell[x, y].xIndex = x;
						gridCell[x, y].yIndex = y;
						gridCell[x, y].Initialize(allTileInputs);
					}
				}
				isInitialized = true;
			}
			else
			{
				Debug.LogWarning("Tile set is empty...");
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
			isDone = true;
		}
		
		private IEnumerator CollapseFixedTiles()
		{
			if (!CheckTiles()) yield break;

			var fixedTiles = GetFixedCells();

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
		
		public void ResetCells()
		{
			//Debug.Log("Resetting Wave...");
			ResetAllCells();
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
					if (gridCell[x, y].IsNotDefiniteState() && !gridCell[x, y].IsFixed())
					{
						gridCell[x, y].PropagateWith(item.compatibleTop);
					}
					break;

				case LookDirection.DOWN:
					y--;
					if (gridCell[x, y].IsNotDefiniteState() && !gridCell[x, y].IsFixed())
					{
						gridCell[x, y].PropagateWith(item.compatibleBottom);
					}
					break;

				case LookDirection.RIGHT:
					x++;
					if (gridCell[x, y].IsNotDefiniteState() && !gridCell[x, y].IsFixed())
					{
						gridCell[x, y].PropagateWith(item.compatibleRight);
					}
					break;

				case LookDirection.LEFT:
					x--;
					if (gridCell[x, y].IsNotDefiniteState() && !gridCell[x, y].IsFixed())
					{
						gridCell[x, y].PropagateWith(item.compatibleLeft);
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
					if (gridCell[x, y].IsNotDefiniteState())
					{
						entropy = gridCell[x, y].entropy;

						if (entropy < lowestEntropy)
						{
							lowestEntropy = entropy;
							lowestEntropyCellsSelected.Clear();
							lowestEntropyCellsSelected.Add(gridCell[x, y]);
						}
						else if (entropy == lowestEntropy)
						{
							lowestEntropyCellsSelected.Add(gridCell[x, y]);
						}
					}
				}
			}
			return lowestEntropyCellsSelected;
		}

		private List<EditorCell> GetFixedCells()
		{
			List<EditorCell> fixedCells = new();

			for (int y = 0; y < tileSizeY; y++)
			{
				for (int x = 0; x < tileSizeX; x++)
				{
					if (gridCell[x, y].IsFixed())
					{
						fixedCells.Add(gridCell[x, y]);
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

		private void ResetAllCells()
		{
			foreach (var item in gridCell)
			{
				item.ResetAndInitializeCell(allTileInputs);
			}
		}

		public bool CheckIfSameSize(int tempX, int tempY, float tempCellSize)
		{
			if (tempX != tileSizeX || tempY != tileSizeY || tempCellSize != tileSize)
			{
				isInitialized = false;
				return false;
			}
			else
			{
				isInitialized = true;
				return true;
			}
		}
		
		private bool IsWaveCollapsed()
		{
			foreach (var cell in gridCell)
			{
				if (cell.IsNotDefiniteState()) return false;
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