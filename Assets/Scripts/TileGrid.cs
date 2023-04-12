using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//WFC tile grid
namespace HelloWorld
{
    public class TileGrid : MonoBehaviour
    {
        public GameObject gridCellObject;
        public int size = 8;
        public float tileSize = 1;
        public float setDelay = .5f;
        public float startDelay = 1f;
        public List<GameObject> inputTiles;
        public List<GameObject> lowestEntropyCells = new();

        public GameObject[,] gridCell;
        private GameObject selectedRandomCell;

        public enum LookDirection
        { UP, DOWN, LEFT, RIGHT };

        private WaitForSeconds wSetTileDelay;

        private void Awake()
        {
            gridCell = new GameObject[size, size];
            wSetTileDelay = new(setDelay);
        }

        private void Start()
        {
            InitializeWave();
        }

        private void InitializeWave()
        {
            Vector3 pos;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    pos = new(tileSize * x - (size * tileSize / 2 - .5f) + transform.position.x, tileSize * y - (size * tileSize / 2 - .5f) + transform.position.y);

                    gridCell[x, y] = Instantiate(gridCellObject, pos, Quaternion.identity, transform);
                    gridCell[x, y].GetComponent<GridCell>().xIndex = x;
                    gridCell[x, y].GetComponent<GridCell>().yIndex = y;
                }
            }

            StartCoroutine(CollapseWave());
        }

        private IEnumerator CollapseWave()
        {
            yield return new WaitForSeconds(startDelay);

            while (!IsWaveCollapsed())
            {
                //Observation Phase
                lowestEntropyCells = GetLowestEntropyCells();
                selectedRandomCell = lowestEntropyCells[UnityEngine.Random.Range(0, lowestEntropyCells.Count - 1)];

                //Collapse Tile
                if (selectedRandomCell.GetComponent<GridCell>().IsCellNotConflict())
                {
                    selectedRandomCell.GetComponent<GridCell>().SelectTile();
                }
                else
                {
                    var x = selectedRandomCell.GetComponent<GridCell>().xIndex;
                    var y = selectedRandomCell.GetComponent<GridCell>().yIndex;
                    Debug.LogWarning("Conflict on cell " + x + " " + y);
                    ResetWave();
                }

                //Propagation Phase
                PropagateConstraints(selectedRandomCell);
                yield return wSetTileDelay;
            }
        }

        public void PropagateConstraints(GameObject cell)
        {
            int x = cell.GetComponent<GridCell>().xIndex;
            int y = cell.GetComponent<GridCell>().yIndex;

            foreach (var item in inputTiles)
            {
                if (cell.GetComponent<GridCell>().selectedTileID == item.GetComponent<InputTile>().id)
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

        public void PropagateToCell(int x, int y, GameObject item, LookDirection direction)
        {
            switch (direction)
            {
                case LookDirection.UP:
                    y++;
                    if (!gridCell[x, y].GetComponent<GridCell>().isDefinite)
                    {
                        gridCell[x, y].GetComponent<GridCell>().Propagate(item.GetComponent<InputTile>().compatibleTop);
                    }
                    break;

                case LookDirection.DOWN:
                    y--;
                    if (!gridCell[x, y].GetComponent<GridCell>().isDefinite)
                    {
                        gridCell[x, y].GetComponent<GridCell>().Propagate(item.GetComponent<InputTile>().compatibleBottom);
                    }
                    break;

                case LookDirection.RIGHT:
                    x++;
                    if (!gridCell[x, y].GetComponent<GridCell>().isDefinite)
                    {
                        gridCell[x, y].GetComponent<GridCell>().Propagate(item.GetComponent<InputTile>().compatibleRight);
                    }
                    break;

                case LookDirection.LEFT:
                    x--;
                    if (!gridCell[x, y].GetComponent<GridCell>().isDefinite)
                    {
                        gridCell[x, y].GetComponent<GridCell>().Propagate(item.GetComponent<InputTile>().compatibleLeft);
                    }
                    break;
            }
        }

        private bool IsWaveCollapsed()
        {
            foreach (var cell in gridCell)
            {
                if (!cell.GetComponent<GridCell>().isDefinite)
                {
                    Debug.Log("Wave not Fully Collapsed...");
                    return false;
                }
            }
            Debug.Log("Wave Fully Collapsed...");
            return true;
        }

        private List<GameObject> GetLowestEntropyCells()
        {
            //Get lowest entropy cells to collapse
            List<GameObject> lowestEntropyCellsSelected = new();
            int lowestEntropy = int.MaxValue;
            int entropy;

            //Check all cells
            //If new lowest clear then add
            //If same just add
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (!gridCell[x, y].GetComponent<GridCell>().isDefinite)
                    {
                        entropy = gridCell[x, y].GetComponent<GridCell>().entropy;

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
            StopAllCoroutines();
            ResetAllCells();
            InitializeWave();
        }

        public void ResetWave(int newSize, float newTileSize, List<CellData> newCellData)
        {
            Debug.Log("Resetting Wave...");
            StopAllCoroutines();
            this.size = newSize;
            this.tileSize = newTileSize;

            ResetAllCells();
            gridCell = new GameObject[size, size];

            Vector3 pos;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    pos = new(tileSize * x - (size * tileSize / 2 - .5f) + transform.position.x, tileSize * y - (size * tileSize / 2 - .5f) + transform.position.y);

                    gridCell[x, y] = Instantiate(gridCellObject, pos, Quaternion.identity, transform);
                    gridCell[x, y].GetComponent<GridCell>().xIndex = x;
                    gridCell[x, y].GetComponent<GridCell>().yIndex = y;
                }
            }

            //int xMax = size;
            int yMax = size;
            int xIndex = 0;
            int yIndex = 0;

            GameObject selectObject;

            foreach (var item in newCellData)
            {
                if (yIndex >= yMax)
                {
                    xIndex++;
                    yIndex = 0;
                }
                foreach (var tile in inputTiles)
                {
                    if (item.selectedTileID == tile.GetComponent<InputTile>().id)
                    {
                        Debug.Log(tile.GetComponent<InputTile>().id);
                        selectObject = tile;
                        gridCell[xIndex, yIndex].GetComponent<GridCell>().SelectTile(tile);
                    }
                }
                Debug.Log(item + " " + xIndex + " " + yIndex);
                yIndex++;
            }
        }

        public void SetDelaySet(float value)
        {
            wSetTileDelay = new(value);
        }

        public void SetGridSize(float value)
        {
            //TODO
            StopAllCoroutines();
            ResetAllCells();
            size = Convert.ToInt32(value);
            gridCell = new GameObject[size, size];
            InitializeWave();
        }

        private void ResetAllCells()
        {
            foreach (var item in gridCell)
            {
                item.GetComponent<GridCell>().ResetCell();
            }
        }
    }
}

//private IEnumerator CollapseAllRandom()
//{
//    //Old random collapse
//    yield return new WaitForSeconds(1);
//    for (int i = 0; i < size; i++)
//    {
//        for (int j = 0; j < size; j++)
//        {
//            gridCell[i, j].GetComponent<GridCell>().SelectTile();
//            yield return null;
//        }
//        yield return null;
//    }
//}