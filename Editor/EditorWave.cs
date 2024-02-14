using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR

using Unity.EditorCoroutines.Editor;

#endif

//Editor WFC tile grid
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

        public float tileSize = 1f;

        private GameObject gridCellObject;
        private GameObject tempGameObject;

        private EditorCell[,] gridCell;
        private EditorCell selectedRandomCell;
        private List<EditorCell> lowestEntropyCells = new();

        private EditorCoroutine coroutine;
        private readonly EditorWaitForSeconds editorWait = new(0f);

        private bool isDone = true;

        public enum LookDirection
        { UP, DOWN, LEFT, RIGHT };

        public void InitializeWave()
        {
            if (allTileInputs.Count != 0)
            {
                Vector3 pos;
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
                coroutine = EditorCoroutineUtility.StartCoroutineOwnerless(CollapseWave());
            }
            else
            {
                Debug.LogWarning("Input tiles are empty...");
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
                selectedRandomCell = lowestEntropyCells[UnityEngine.Random.Range(0, lowestEntropyCells.Count - 1)];

                //Collapse Tile
                if (selectedRandomCell.IsCellNotConflict())
                {
                    selectedRandomCell.SelectTile();
                }
                else
                {
                    var x = selectedRandomCell.xIndex;
                    var y = selectedRandomCell.yIndex;
                    isDone = true;
                    Debug.LogWarning("Conflict on cell " + x + " " + y);
                    //Stop();
                    //ResetWave();
                    yield break;
                }

                //Propagation Phase
                PropagateConstraints(selectedRandomCell);
                yield return editorWait;
            }
            isDone = true;
            yield break;
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

        public void PropagateConstraints(EditorCell cell)
        {
            int x = cell.xIndex;
            int y = cell.yIndex;

            if (y + 1 > -1 && y + 1 < tileSizeY)
                PropagateToCell(x, y, cell.selectedTileInput, LookDirection.UP);

            if (y - 1 > -1 && y - 1 < tileSizeY)
                PropagateToCell(x, y, cell.selectedTileInput, LookDirection.DOWN);

            if (x + 1 > -1 && x + 1 < tileSizeX)
                PropagateToCell(x, y, cell.selectedTileInput, LookDirection.RIGHT);

            if (x - 1 > -1 && x - 1 < tileSizeX)
                PropagateToCell(x, y, cell.selectedTileInput, LookDirection.LEFT);
        }

        public void PropagateToCell(int x, int y, TileInput item, LookDirection direction)
        {
            switch (direction)
            {
                case LookDirection.UP:
                    y++;
                    if (gridCell[x, y].IsNotDefiniteState())
                    {
                        gridCell[x, y].Propagate(item.compatibleTop);
                    }
                    break;

                case LookDirection.DOWN:
                    y--;
                    if (gridCell[x, y].IsNotDefiniteState())
                    {
                        gridCell[x, y].Propagate(item.compatibleBottom);
                    }
                    break;

                case LookDirection.RIGHT:
                    x++;
                    if (gridCell[x, y].IsNotDefiniteState())
                    {
                        gridCell[x, y].Propagate(item.compatibleRight);
                    }
                    break;

                case LookDirection.LEFT:
                    x--;
                    if (gridCell[x, y].IsNotDefiniteState())
                    {
                        gridCell[x, y].Propagate(item.compatibleLeft);
                    }
                    break;
            }
        }

        private bool IsWaveCollapsed()
        {
            foreach (var cell in gridCell)
            {
                if (cell.IsNotDefiniteState())
                {
                    //Debug.Log("Wave not Fully Collapsed...");
                    return false;
                }
            }
            Debug.Log("Wave Fully Collapsed...");
            return true;
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
            //Debug.Log("# of lowest entropy cells: " + lowestEntropyCells.Count.ToString());
            return lowestEntropyCellsSelected;
        }

        public void ResetWave()
        {
            Debug.Log("Resetting Wave...");
            ResetAllCells();
            InitializeWave();
        }

        public void RemoveGridCells()
        {
            EditorCell[] cell = GetComponentsInChildren<EditorCell>();
            foreach (var item in cell)
            {
                DestroyImmediate(item.gameObject);
            }
        }

        private void CreateGridCellsAndInitialize()
        {
            gridCell = new EditorCell[tileSizeX, tileSizeY];
            if (gridCellObject != null)
            {
                Vector3 pos;
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
            }
        }

        public void InitializeGridCells()
        {
            gridCell = new EditorCell[tileSizeX, tileSizeY];
        }

        private void ResetAllCells()
        {
            foreach (var item in gridCell)
            {
                item.ResetAndInitializeCell(allTileInputs);
            }
        }

        public void Stop()
        {
            if (coroutine != null)
                EditorCoroutineUtility.StopCoroutine(coroutine);
            isDone = true;
        }

        public void ClearCells()
        {
            //Stop();
            List<EditorCell> child = GetComponentsInChildren<EditorCell>().ToList();
            foreach (var item in child)
            {
                DestroyImmediate(item.gameObject);
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

            // Get all the first child objects of the parent
            for (int i = 0; i < childCount; i++)
            {
                childTransform = transform.GetChild(i);

                // Move child's children to the new parent
                grandChildCount = childTransform.childCount;
                for (int j = 0; j < grandChildCount; j++)
                {
                    grandChildTransform = childTransform.GetChild(j);
                    grandChildTransform.SetParent(newParentObject.transform);
                }

                // Detach the child from the parent
                childTransform.SetParent(this.transform);
            }

            DeleteEditorGameObjects();
        }

        private void DeleteEditorGameObjects()
        {
            DestroyImmediate(this.gameObject);
        }

        public bool IsDone()
        {
            return isDone;
        }

        public void EnsureGridCellObject(GameObject newGridCellObject)
        {
            gridCellObject = newGridCellObject;
        }

    }
}