using System.Collections.Generic;
using System.IO;
using UnityEngine;

//Reference: Velvery yt
namespace HelloWorld
{
    public class SaveWaveData : MonoBehaviour
    {
        public string fileName = "";
        public TileGrid tileGrid;
        public List<CellData> cells = new();
        private DataTileGrid tileGridData;
        public int size;
        public float tilesize;

        public void SaveData()
        {
            GetCellData();
            GetTileGridData();
            size = tileGrid.size;
            tilesize = tileGrid.tileSize;
            //DataHandler.SavetoJSON(tileGridData, fileName);
            string data = JsonUtility.ToJson(tileGridData);
            FileStream filestream = new(Application.dataPath + "/" + fileName, FileMode.Create);

            using StreamWriter writer = new(filestream);
            writer.Write(data);
        }

        public void GetTileGridData()
        {
            //tileGridData.Clear();
            tileGridData = new(tileGrid.size, tileGrid.tileSize, tileGrid.inputTiles, cells);
        }

        public void GetCellData()
        {
            cells.Clear();
            foreach (var item in tileGrid.gridCell)
            {
                GridCell cell = item.GetComponent<GridCell>();
                int newXIndex = cell.xIndex;
                int newYIndex = cell.yIndex;
                int newSelectedTileID = cell.selectedTileID;
                CellData cellData = new(newXIndex, newYIndex, newSelectedTileID);
                cells.Add(cellData);
            }
        }
    }
}