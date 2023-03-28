using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public GameObject cell;
    [SerializeField] private GameObject[,] _gridCell;
    [SerializeField] private int size;
    [SerializeField] private float tileSize;
    [SerializeField] public GameObject[] inputTiles;

    [SerializeField] List<GameObject> lowestEntropyTile;

    void Start()
    {
        _gridCell = new GameObject[size, size];
        InitializeWave();
        
    }

    void InitializeWave()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Vector3 pos = new((tileSize * i) - (size * tileSize/2), (tileSize * j) - (size * tileSize / 2));

                _gridCell[i,j] = Instantiate(cell, pos, Quaternion.identity, this.transform);
                _gridCell[i,j].transform.parent = this.transform;

                //Debug.Log("Spawn " + i + " " + j);
            }
        }
        //StartCoroutine(CollapseAllRandom());
        StartCoroutine(CollapseWave());
    }

    IEnumerator CollapseWave()
    {
        yield return null;
        while (!IsCellCollapsed())
        {
            GameObject[] lowestEntropyCells = GetLowestEntropy();
            GameObject selectRandomTile = lowestEntropyCells[UnityEngine.Random.Range(0, lowestEntropyCells.Length)];

            selectRandomTile.GetComponent<GridCell>().SelectTile();
            
            yield return null;
        }
    }

    bool IsCellCollapsed()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (!_gridCell[i, j].GetComponent<GridCell>().isDefinite)
                {
                    Debug.Log("False");
                    return false;
                }
            }
        }
        Debug.Log("True");
        return true;
    }

    private GameObject[] GetLowestEntropy()
    {
        int lowestEntropy = int.MaxValue;
        int entropy;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (_gridCell[i, j].GetComponent<GridCell>().isDefinite == true) continue;

                entropy = _gridCell[i, j].GetComponent<GridCell>().entropy;

                if (entropy < lowestEntropy)
                {
                    lowestEntropy = entropy;
                    lowestEntropyTile.Add(_gridCell[i, j]);
                }
            }
        }
        return lowestEntropyTile.ToArray();
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