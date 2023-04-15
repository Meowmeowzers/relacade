using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HelloWorld
{
    public class LoadWaveData : MonoBehaviour
    {
        public string fileName = "";
        public TileGrid tileGrid;
        private DataTileGrid tileGridData;

        private void Start()
        {
            tileGrid = FindAnyObjectByType<TileGrid>();
        }

        public void LoadWaveDataFromFile()
        {
            //AssetDatabase.Refresh();
            string path = Application.dataPath + "/" + fileName;
            Debug.Log(path);

            StreamReader reader = new(path);
            string content = reader.ReadToEnd();
            reader.Close();

            tileGridData = JsonUtility.FromJson<DataTileGrid>(content);
            Debug.Log(tileGridData.size + " " + tileGridData.tileSize);

            int newSize = tileGridData.size;
            float newTileSize = tileGridData.tileSize;
            List<CellData> newCellData = tileGridData.cellData;

            tileGrid.ResetWave(newSize, newTileSize, newCellData);
        }

        public void GetCellData()
        {
            //int xMax = tileGrid.size;
            int yMax = tileGrid.size;
            //int allCount = xMax * yMax;
            int x = 0;
            int y = 0;
            int count = 1;

            foreach (var item in tileGrid.gridCell)
            {
                if (y >= yMax)
                {
                    x++;
                    y = 0;
                    count = 1;
                }

                GridCell cell = item.GetComponent<GridCell>();
                int newXIndex = cell.xIndex;
                int newYIndex = cell.yIndex;
                int newSelectedTileID = cell.selectedTileID;
                CellData cellData = new(newXIndex, newYIndex, newSelectedTileID);
                //cells.Add(cellData);
                Debug.Log(x + " " + y);
                y++;
                count++;
            }
        }
    }
}