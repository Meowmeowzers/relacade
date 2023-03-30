using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public GameObject gridCellObject;
    [SerializeField] private GameObject[,] _gridCell;
    [SerializeField] private int _size;
    [SerializeField] private float _tileSize;
    [SerializeField] private float _setSpeed = .5f;
    [SerializeField] private float _startDelay = 1f;
    public List<GameObject> inputTiles;
    
    public enum LookDirection { UP, DOWN, LEFT, RIGHT };

    public List<GameObject> _lowestEntropyCells = new();
    GameObject _selectedRandomCell;

    private void Awake()
    {
        _gridCell = new GameObject[_size, _size];    
    }

    void Start()
    {
        InitializeWave();
    }

    void InitializeWave()
    {
        Vector3 pos;
        for (int y = 0; y < _size; y++)
        {
            for (int x = 0; x < _size; x++)
            {
                pos = new((_tileSize * x) - ((_size * _tileSize / 2) - .5f), (_tileSize * y) - ((_size * _tileSize / 2) - .5f));

                _gridCell[x, y] = Instantiate(gridCellObject, pos, Quaternion.identity, transform);
                _gridCell[x, y].transform.parent = transform;
                _gridCell[x, y].GetComponent<GridCell>().yIndex = y;
                _gridCell[x, y].GetComponent<GridCell>().xIndex = x;

                //Debug.Log("Initialized Cell at [ " + i + " , " + j + " ]");
            }
        }
        //StartCoroutine(CollapseAllRandom());
        StartCoroutine(CollapseWave());
    }

    IEnumerator CollapseWave()
    {
        yield return new WaitForSeconds(_startDelay);
        while (!IsWaveCollapsed())
        {
            //Observation Phase
            //Observe lowest entropy cells
            Debug.Log(_lowestEntropyCells);
            _lowestEntropyCells = GetLowestEntropyCells();
            _selectedRandomCell = _lowestEntropyCells[UnityEngine.Random.Range(0, _lowestEntropyCells.Count - 1)];
            _selectedRandomCell.GetComponent<GridCell>().SelectTile();

            //Propagation Phase
            PropagateConstraints(_selectedRandomCell);
            yield return new WaitForSeconds(_setSpeed);
        }
    }

    public void PropagateConstraints(GameObject cell)
    {
        //Find the corresponding tile from superset
        //Then compare to selected cell
        Debug.Log("Cell " +
            cell.GetComponent<GridCell>().xIndex + " " +
            cell.GetComponent<GridCell>().yIndex + " ID: " +
            cell.GetComponent<GridCell>().selectedTileID);

        //Todo: Check before propagate

        int x = cell.GetComponent<GridCell>().xIndex;
        int y = cell.GetComponent<GridCell>().yIndex;

        foreach (var item in inputTiles)
        {
            Debug.Log(item);
            if (cell.GetComponent<GridCell>().selectedTileID == item.GetComponent<InputTile>().id)
            {
                if (y + 1 > -1 && y + 1 < _size)
                    PropagateToCell(x, y, item, LookDirection.UP);
                if (y - 1 > -1 && y - 1 < _size)
                    PropagateToCell(x, y, item, LookDirection.DOWN);
                if (x + 1 > -1 && x + 1 < _size)
                    PropagateToCell(x, y, item, LookDirection.RIGHT);
                if (x - 1 > -1 && x - 1 < _size)
                    PropagateToCell(x, y, item, LookDirection.LEFT);
                break;
            }
        }
    }

    public void PropagateToCell(int x, int y, GameObject item, LookDirection direction)
    {
        //wag mag clear kung definite na
        //naikalat sa buong grid ayusin
        switch (direction)
        {
            case LookDirection.UP:
                y += 1;
                if(!_gridCell[x, y].GetComponent<GridCell>().isDefinite)
                {
                    _gridCell[x, y].GetComponent<GridCell>().Propagate(item.GetComponent<InputTile>().compatibleTop);
                }
                break;
            case LookDirection.DOWN:
                y -= 1;
                if (!_gridCell[x, y].GetComponent<GridCell>().isDefinite)
                {
                    _gridCell[x, y].GetComponent<GridCell>().Propagate(item.GetComponent<InputTile>().compatibleBottom);
                }
                break;
            case LookDirection.RIGHT:
                x += 1;
                if (!_gridCell[x, y].GetComponent<GridCell>().isDefinite)
                {
                    _gridCell[x, y].GetComponent<GridCell>().Propagate(item.GetComponent<InputTile>().compatibleRight);
                }
                break;
            case LookDirection.LEFT:
                x -= 1;
                if (!_gridCell[x, y].GetComponent<GridCell>().isDefinite)
                {
                    _gridCell[x, y].GetComponent<GridCell>().Propagate(item.GetComponent<InputTile>().compatibleLeft);
                }
                break;
        }
    }

    bool IsWaveCollapsed()
    {
        foreach(var cell in _gridCell)
        {
            if (!cell.GetComponent<GridCell>().isDefinite)
            {
                Debug.Log("Wave not Fully Collapsed");
                return false;
            }
        }
        Debug.Log("Wave Fully Collapsed");
        return true;
    }

    private List<GameObject> GetLowestEntropyCells()
    {

        List<GameObject> lowestEntropyCellsSelected = new();
        int lowestEntropy = int.MaxValue;
        int entropy;

        //Check all items
        for(int y = 0; y <  _size; y++)
        {
            for(int x = 0; x < _size; x++)
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
                      
        Debug.Log("# of lowest entropy cells: " + _lowestEntropyCells.Count.ToString());
        return lowestEntropyCellsSelected;
    }

    IEnumerator CollapseAllRandom()
    {
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
}
