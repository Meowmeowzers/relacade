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
    public class EditorTileGrid : MonoBehaviour
    {
        [SerializeField] public TileInputSet tileInputSet;
        [SerializeField] public List<TileInput> allTileInputs;

        [Range(2, 50)]
        [SerializeField] public int size = 8;
        [SerializeField] public float tileSize = 1;

        private GameObject gridCellObject;
        private GameObject tempGameObject;

        private EditorGridCell[,] gridCell;
        private EditorGridCell selectedRandomCell;
        private List<EditorGridCell> lowestEntropyCells = new();
        private List<TileInput> uniqueTileInputs;

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

        public void PropagateConstraints(EditorGridCell cell)
        {
            int x = cell.xIndex;
            int y = cell.yIndex;

            if (y + 1 > -1 && y + 1 < size)
                PropagateToCell(x, y, cell.selectedTileInput, LookDirection.UP);

            if (y - 1 > -1 && y - 1 < size)
                PropagateToCell(x, y, cell.selectedTileInput, LookDirection.DOWN);

            if (x + 1 > -1 && x + 1 < size)
                PropagateToCell(x, y, cell.selectedTileInput, LookDirection.RIGHT);

            if (x - 1 > -1 && x - 1 < size)
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
            //Debug.Log("# of lowest entropy cells: " + lowestEntropyCells.Count.ToString());
            return lowestEntropyCellsSelected;
        }

        public void ResetWave()
        {
            Debug.Log("Resetting Wave...");
            ResetAllCells();
            InitializeWave();
        }

        public void LoadWaveFromData(int newSize, float newTileSize, List<CellData> newCellData, List<TileInput> newTileInputs)
        {
            Debug.Log("Resetting Wave...");
            StopAllCoroutines();

            tileInputSet = null;
            size = newSize;
            tileSize = newTileSize;
            allTileInputs.Clear();
            allTileInputs.AddRange(newTileInputs);

            RemoveGridCells();
            CreateGridCellsAndInitialize();

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
        
        public void RemoveGridCells()
        {
            EditorGridCell[] cell = GetComponentsInChildren<EditorGridCell>();
            foreach(var item in cell)
            {
                DestroyImmediate(item.gameObject);
            }
        }

        private void CreateGridCellsAndInitialize()
        {
            gridCell = new EditorGridCell[size, size];
            if (gridCellObject != null)
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
            foreach (var item in gridCell)
            {
                item.ResetAndInitializeCell(allTileInputs);
            }
        }

        public void Stop()
        {
            if(coroutine != null)
                EditorCoroutineUtility.StopCoroutine(coroutine);
            isDone = true;
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
            //Delete the editor gameobjects
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

        public EditorGridCell[,] GetCells()
        {
            return gridCell;
        }
    }
}