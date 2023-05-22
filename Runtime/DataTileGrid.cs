using System;
using System.Collections.Generic;
using UnityEngine;

namespace HelloWorld
{
    [Serializable]
    public class DataTileGrid
    {
        public int size = 8;
        public float tileSize = 1;
        public List<TileInput> inputTiles;
        public List<CellData> cellData;

        public DataTileGrid(int size, float tileSize, List<TileInput> inputTiles, List<CellData> cellData)
        {
            this.size = size;
            this.tileSize = tileSize;
            this.inputTiles = inputTiles;
            this.cellData = cellData;
        }
    }
}
