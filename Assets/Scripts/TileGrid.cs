using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//WFC tile grid
//Notes:
//  Did not cached WaitForSeconds to make it changeable during runtime
public class TileGrid : MonoBehaviour
{
    public GameObject gridCellObject;
    public int _size;
    public float _tileSize;
    public float setDelay = .5f;
    public float startDelay = 1f;
    public List<GameObject> inputTiles;
    public List<GameObject> lowestEntropyCells = new();

    private GameObject[,] _gridCell;
    private GameObject _selectedRandomCell;

    public enum LookDirection { UP, DOWN, LEFT, RIGHT };

    private void Awake()
    {
        _gridCell = new GameObject[_size, _size];
    }

    private void Start()
    {
        InitializeWave();
    }

    private void InitializeWave()
    {
        Vector3 pos;
        for (int y = 0; y < _size; y++)
        {
            for (int x = 0; x < _size; x++)
            {
                pos = new((_tileSize * x) - ((_size * _tileSize / 2) - .5f) + transform.position.x, (_tileSize * y) - ((_size * _tileSize / 2) - .5f) + transform.position.y);

                _gridCell[x, y] = Instantiate(gridCellObject, pos, Quaternion.identity, transform);
                _gridCell[x, y].GetComponent<GridCell>().xIndex = x;
                _gridCell[x, y].GetComponent<GridCell>().yIndex = y;
            }
        }
        //StartCoroutine(CollapseAllRandom());
        StartCoroutine(CollapseWave());
    }

    private IEnumerator CollapseWave()
    {
        yield return new WaitForSeconds(startDelay);
        while (!IsWaveCollapsed())
        {

            //Observation Phase
            //Observe lowest entropy cells
            lowestEntropyCells = GetLowestEntropyCells();
            _selectedRandomCell = lowestEntropyCells[UnityEngine.Random.Range(0, lowestEntropyCells.Count - 1)];

            //Collapse Tile
            if (_selectedRandomCell.GetComponent<GridCell>().CheckEntropy() == 0)
            {
                Debug.LogWarning("Conflict on cell " +
                _selectedRandomCell.GetComponent<GridCell>().xIndex + " " +
                _selectedRandomCell.GetComponent<GridCell>().yIndex);
                ResetWave();
            }
            else
            {
                _selectedRandomCell.GetComponent<GridCell>().SelectTile();
            }

            //Propagation Phase
            PropagateConstraints(_selectedRandomCell);
            yield return new WaitForSeconds(setDelay);
        }
    }

    public void PropagateConstraints(GameObject cell)
    {
        //Get selected tile of cell
        //Find the corresponding tile from superset
        //Then propagate to adjacent cells

        int x = cell.GetComponent<GridCell>().xIndex;
        int y = cell.GetComponent<GridCell>().yIndex;

        //Debug.Log("Cell " + x + " " + y + " Tile ID: " + cell.GetComponent<GridCell>().selectedTileID);

        foreach (var item in inputTiles)
        {
            if (cell.GetComponent<GridCell>().selectedTileID == item.GetComponent<InputTile>().id)
            {
                if (y + 1 > -1 && y + 1 < _size) PropagateToCell(x, y, item, LookDirection.UP);
                if (y - 1 > -1 && y - 1 < _size) PropagateToCell(x, y, item, LookDirection.DOWN);
                if (x + 1 > -1 && x + 1 < _size) PropagateToCell(x, y, item, LookDirection.RIGHT);
                if (x - 1 > -1 && x - 1 < _size) PropagateToCell(x, y, item, LookDirection.LEFT);
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
                if (!_gridCell[x, y].GetComponent<GridCell>().isDefinite)
                {
                    _gridCell[x, y].GetComponent<GridCell>().Propagate(item.GetComponent<InputTile>().compatibleTop);
                }
                break;

            case LookDirection.DOWN:
                y--;
                if (!_gridCell[x, y].GetComponent<GridCell>().isDefinite)
                {
                    _gridCell[x, y].GetComponent<GridCell>().Propagate(item.GetComponent<InputTile>().compatibleBottom);
                }
                break;

            case LookDirection.RIGHT:
                x++;
                if (!_gridCell[x, y].GetComponent<GridCell>().isDefinite)
                {
                    _gridCell[x, y].GetComponent<GridCell>().Propagate(item.GetComponent<InputTile>().compatibleRight);
                }
                break;

            case LookDirection.LEFT:
                x--;
                if (!_gridCell[x, y].GetComponent<GridCell>().isDefinite)
                {
                    _gridCell[x, y].GetComponent<GridCell>().Propagate(item.GetComponent<InputTile>().compatibleLeft);
                }
                break;
        }
    }

    private bool IsWaveCollapsed()
    {
        foreach (var cell in _gridCell)
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
        for (int y = 0; y < _size; y++)
        {
            for (int x = 0; x < _size; x++)
            {
                if (!_gridCell[x, y].GetComponent<GridCell>().isDefinite)
                {
                    entropy = _gridCell[x, y].GetComponent<GridCell>().entropy;

                    if (entropy < lowestEntropy)
                    {
                        lowestEntropy = entropy;
                        lowestEntropyCellsSelected.Clear();
                        lowestEntropyCellsSelected.Add(_gridCell[x, y]);
                    }
                    else if (entropy == lowestEntropy)
                    {
                        lowestEntropyCellsSelected.Add(_gridCell[x, y]);
                    }
                }
            }
        }
        Debug.Log("# of lowest entropy cells: " + lowestEntropyCells.Count.ToString());
        return lowestEntropyCellsSelected;
    }

    private IEnumerator CollapseAllRandom()
    {
        //Old random collapse
        yield return new WaitForSeconds(1);
        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                _gridCell[i, j].GetComponent<GridCell>().SelectTile();
                yield return null;
            }
            yield return null;
        }
    }

    public void ResetWave()
    {
        Debug.Log("Resetting Wave...");
        StopAllCoroutines();
        foreach (var item in _gridCell)
        {
            item.GetComponent<GridCell>().ResetCell();
        }
        InitializeWave();
    }

    public void SetDelaySet(float value)
    {
        setDelay = value;
    }
    public void SetGridSize(float value)
    {
        //TODO
    }
}