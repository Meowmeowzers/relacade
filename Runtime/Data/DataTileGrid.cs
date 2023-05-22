using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DataTileGrid
{
    public int size = 8;
    public float tileSize = 1;
    public List<GameObject> inputTiles;
    public List<CellData> cellData;

    public DataTileGrid(int size, float tileSize, List<GameObject> inputTiles, List<CellData> cellData)
    {
        this.size = size;
        this.tileSize = tileSize;
        this.inputTiles = inputTiles;
        this.cellData = cellData;
    }
}
