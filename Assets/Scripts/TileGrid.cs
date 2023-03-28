using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public GameObject gridCellObject;
    [SerializeField] private GameObject[,] _gridCell;
    [SerializeField] private int _size;
    [SerializeField] private float _tileSize;
    public GameObject[] inputTiles;

    [SerializeField] List<GameObject> lowestEntropyCellsSelected;

    GameObject[] _lowestEntropyCells;
    GameObject _selectedRandomCell;

    void Start()
    {
        _gridCell = new GameObject[_size, _size];
        InitializeWave();
    }

    void InitializeWave()
    {
        Vector3 pos;
        for (int i = 0; i < _size; i++)
        {
            for (int j = 0; j < _size; j++)
            {
                pos = new((_tileSize * i) - ((_size * _tileSize / 2) - .5f), (_tileSize * j) - ((_size * _tileSize / 2) - .5f));

                _gridCell[i,j] = Instantiate(gridCellObject, pos, Quaternion.identity, transform);
                _gridCell[i,j].transform.parent = transform;

                //Debug.Log("Initialized Cell at [ " + i + " , " + j + " ]");
            }
        }
        //StartCoroutine(CollapseAllRandom());
        StartCoroutine(CollapseWave());
    }

    IEnumerator CollapseWave()
    {
        yield return new WaitForSeconds(1f);
        while (!IsWaveCollapsed())
        {
            _lowestEntropyCells = GetLowestEntropyCells();
            _selectedRandomCell = _lowestEntropyCells[UnityEngine.Random.Range(0, _lowestEntropyCells.Length - 1)];
            _selectedRandomCell.GetComponent<GridCell>().SelectTile();

            yield return new WaitForSeconds(1f);
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

    private GameObject[] GetLowestEntropyCells()
    {
        //TODO:
        //Change to get index number of cell with lowest entropy

        lowestEntropyCellsSelected.Clear();
        int lowestEntropy = int.MaxValue;
        int entropy;

        //Check all items
        foreach(var cell in _gridCell)
        {
            if (cell.GetComponent<GridCell>().isDefinite == false)
            {
                entropy = cell.GetComponent<GridCell>().entropy;

                if (entropy <= lowestEntropy)
                {
                    lowestEntropy = entropy;
                    lowestEntropyCellsSelected.Add(cell);
                }
            }
            else continue;            
        }

        //Double check selected items
        foreach(var item in lowestEntropyCellsSelected)
        {
            entropy = item.GetComponent<GridCell>().entropy;

            if (entropy > lowestEntropy)
            {
                lowestEntropyCellsSelected.Remove(item);
            }
        }

        return lowestEntropyCellsSelected.ToArray();
    }
}

/*
 * 
 *  IEnumerator CollapseAllRandom()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("Start Collapse " + IsCellCollapsed());
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                _gridCell[i, j].GetComponent<GridCell>().SelectTile();
                yield return null;
            }
            yield return null;
        }
    }
*/