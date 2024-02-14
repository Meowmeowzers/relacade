using System;
using System.Collections.Generic;

namespace HelloWorld.Editor
{
    [Serializable]
    public class WaveData
    {
        public int size = 8;
        public int sizeX = 8;
        public int sizeY = 8;
        public float tileSize = 1;
        public List<TileInput> inputTiles;
        public List<CellData> cellData;

        public WaveData(int size, float tileSize, List<TileInput> inputTiles, List<CellData> cellData)
        {
            this.size = size;
            this.tileSize = tileSize;
            this.inputTiles = inputTiles;
            this.cellData = cellData;
        }

        public WaveData(int sizeX, int sizeY, float tileSize, List<TileInput> inputTiles, List<CellData> cellData)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.tileSize = tileSize;
            this.inputTiles = inputTiles;
            this.cellData = cellData;
        }
    }
}