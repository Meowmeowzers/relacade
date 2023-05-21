using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Unity.EditorCoroutines.Editor;
using System.Collections;

//Editor WFC tile grid
namespace HelloWorld
{
    public class EditorTileGrid : MonoBehaviour
    {
        public TileInputSet tileInputSet;
        public List<TileInput> allTileInputs;
        public List<TileInput> uniqueTileInputs;

        [Range(2, 50)]
        public int size = 8;
        public float tileSize = 1;

        public GameObject gridCellObject;
        private List<EditorGridCell> lowestEntropyCells = new();

        public EditorGridCell[,] gridCell;
        private EditorGridCell selectedRandomCell;
        private GameObject tempGameObject;

        private readonly EditorWaitForSeconds editorWait = new(0f);

        EditorCoroutine coroutine;

        public enum LookDirection
        { UP, DOWN, LEFT, RIGHT };

        public void InitializeWave()
        {            
            if(allTileInputs.Count != 0)
            {
                Vector3 pos;

                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        pos = new(tileSize * x - (size * tileSize / 2 - .5f) + transform.position.x, tileSize * y - (size * tileSize / 2 - .5f) + transform.position.y);

                        tempGameObject = Instantiate(gridCellObject, pos, Quaternion.identity, transform);
                        gridCell[x, y] = tempGameObject.GetComponent<EditorGridCell>();
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
                    Debug.LogWarning("Conflict on cell " + x + " " + y);
                    //Stop();
                    //ResetWave();
                    yield break;
                }

                //Propagation Phase
                PropagateConstraints(selectedRandomCell);
                yield return editorWait;
            }
        }

        public void PropagateConstraints(EditorGridCell cell)
        {
            int x = cell.xIndex;
            int y = cell.yIndex;

            foreach (var item in allTileInputs)
            {
                //I dont remeber why id needs to match
                if (cell.selectedTileID == item.id)
                {
                    if (y + 1 > -1 && y + 1 < size)
                        PropagateToCell(x, y, item, LookDirection.UP);
                    if (y - 1 > -1 && y - 1 < size)
                        PropagateToCell(x, y, item, LookDirection.DOWN);
                    if (x + 1 > -1 && x + 1 < size)
                        PropagateToCell(x, y, item, LookDirection.RIGHT);
                    if (x - 1 > -1 && x - 1 < size)
                        PropagateToCell(x, y, item, LookDirection.LEFT);
                    break;
                }
            }
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
                //TODO: change to use function instead of direct access to isDefinite
                if (cell.IsNotDefiniteState())
                {
                    //Debug.Log("Wave not Fully Collapsed...");
                    return false;
                }
            }
            //Debug.Log("Wave Fully Collapsed...");
            return true;
        }

        private List<EditorGridCell> GetLowestEntropyCells()
        {
            List<EditorGridCell> lowestEntropyCellsSelected = new();
            int lowestEntropy = int.MaxValue;
            int entropy;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
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
            Debug.Log("# of lowest entropy cells: " + lowestEntropyCells.Count.ToString());
            return lowestEntropyCellsSelected;
        }

        public void ResetWave()
        {
            Debug.Log("Resetting Wave...");
            ResetAllCells();
            InitializeWave();
        }

        public void ResetWave(int newSize, float newTileSize, List<CellData> newCellData)
        {
            Debug.Log("Resetting Wave...");
            StopAllCoroutines();
            size = newSize;
            tileSize = newTileSize;

            ResetAllCells();
            InitializeGridCells();

            Vector3 pos;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    pos = new(tileSize * x - (size * tileSize / 2 - .5f) + transform.position.x, tileSize * y - (size * tileSize / 2 - .5f) + transform.position.y);

                    tempGameObject = Instantiate(gridCellObject, pos, Quaternion.identity, transform);
                    gridCell[x, y] = tempGameObject.GetComponent<EditorGridCell>();
                    gridCell[x, y].xIndex = x;
                    gridCell[x, y].yIndex = y;
                }
            }

            int yMax = size;
            int xIndex = 0;
            int yIndex = 0;

            foreach (var item in newCellData)
            {
                if (yIndex >= yMax)
                {
                    xIndex++;
                    yIndex = 0;
                }
                foreach (var tile in allTileInputs)
                {
                    if (item.selectedTileID == tile.id)
                    {
                        gridCell[xIndex, yIndex].SelectTile(tile);
                    }
                }
                //Debug.Log(item + " " + xIndex + " " + yIndex);
                yIndex++;
            }
        }
        
        public void InitializeGridCells()
        {
            gridCell = new EditorGridCell[size, size];
        }

        public void SetGridSize(float value)
        {
            Stop();
            ResetAllCells();
            size = Convert.ToInt32(value);
            InitializeGridCells();
            InitializeWave();
        }

        private void ResetAllCells()
        {
            if(gridCell != null)
            {
                foreach (var item in gridCell)
                {
                    item.ResetCell(allTileInputs);
                }
            }
        }

        public void Stop()
        {
            EditorCoroutineUtility.StopCoroutine(coroutine);
        }

        public void ClearCells()
        {
            //Stop();
            List<EditorGridCell> child = GetComponentsInChildren<EditorGridCell>().ToList();
            foreach (var item in child)
            {
                DestroyImmediate(item.gameObject);
            }
        }

        public void InitializeWaveNoStart()
        {
            Vector3 pos;
            gridCell = new EditorGridCell[size, size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    pos = new(tileSize * x - (size * tileSize / 2 - .5f) + transform.position.x, tileSize * y - (size * tileSize / 2 - .5f) + transform.position.y);

                    tempGameObject = Instantiate(gridCellObject, pos, Quaternion.identity, transform);
                    gridCell[x, y] = tempGameObject.GetComponent<EditorGridCell>();
                    gridCell[x, y].xIndex = x;
                    gridCell[x, y].yIndex = y;
                    gridCell[x, y].Initialize(allTileInputs);
                }
            }
        }

        public void GetInputTilesFromSet()
        {
            allTileInputs.Clear();
            foreach(var item in tileInputSet.AllInputTiles)
            {
                allTileInputs.Add(item);
            }
        }

        public void FinalizeGrid()
        {
            GameObject newParentObject = new GameObject("Generated Content  "); ;
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
            //Delete the editor gameobjects
            DestroyImmediate(this.gameObject);
        }

        public void ReLoadTileInputsFromSet()
        {
            allTileInputs.Clear();
            allTileInputs.AddRange(tileInputSet.AllInputTiles);
        }
    }
}