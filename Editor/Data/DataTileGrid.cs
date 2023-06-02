using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DataTileGrid
{
    [SerializeField] public int size = 8;
    [SerializeField] public float tileSize = 1;
    [SerializeField] public List<GameObject> inputTiles;
    [SerializeField] public List<CellData> cellData;

    public DataTileGrid(int size, float tileSize, List<GameObject> inputTiles, List<CellData> cellData)
    {
        this.size = size;
        this.tileSize = tileSize;
        this.inputTiles = inputTiles;
        this.cellData = cellData;
    }
}