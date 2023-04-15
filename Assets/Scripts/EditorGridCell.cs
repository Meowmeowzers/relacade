using System.Collections.Generic;
using UnityEngine;

//Future: make entropy 1 the definite state
namespace HelloWorld
{
    public class EditorGridCell : MonoBehaviour
    {
        public int xIndex;
        public int yIndex;
        public int entropy;
        public int selectedTileID;
        public bool isDefinite = false;
        public List<GameObject> inputTiles;

        [SerializeField] private EditorTileGrid tileGrid;
        private List<GameObject> propagatedCells = new();
        private GameObject select;


        public void Initialize()
        {
            tileGrid = GetComponentInParent<EditorTileGrid>();
            entropy = tileGrid.inputTiles.Count;
            inputTiles.AddRange(tileGrid.inputTiles);
        }

        public void SelectTile()
        {
            //int select = inputTiles[Random.Range(0, inputTiles.Count)].GetComponent<InputTile>().id;
            select = Instantiate(inputTiles[Random.Range(0, inputTiles.Count)], transform);

            inputTiles.Clear();
            inputTiles.Add(select);
            selectedTileID = select.GetComponent<InputTile>().id;
            isDefinite = true;
            entropy = 1;

            Debug.Log("Selected Cell: " + xIndex + " " + yIndex
                + ", Selected Tile: " + select + ", ID: " + select.GetComponent<InputTile>().id);
        }

        public void SelectTile(GameObject gameObject)
        {
            selectedTileID = gameObject.GetComponent<InputTile>().id;

            select = Instantiate(gameObject, transform);
            inputTiles.Clear();
            inputTiles.Add(select);
            isDefinite = true;
            entropy = 1;
        }

        public void Propagate(List<GameObject> compatibleTop)
        {
            propagatedCells.Clear();
            foreach (var item in compatibleTop)
            {
                if (inputTiles.Contains(item))
                {
                    propagatedCells.Add(item);
                }
            }
            inputTiles.Clear();
            inputTiles.AddRange(propagatedCells);
            entropy = inputTiles.Count;
        }

        public bool IsCellNotConflict()
        {
            if (inputTiles.Count > 0)
                return true;
            else
                return false;
        }

        public void ResetCell()
        {
            entropy = tileGrid.inputTiles.Count;
            inputTiles.Clear();
            propagatedCells.Clear();
            inputTiles.AddRange(tileGrid.inputTiles);
            isDefinite = false;
            DestroyImmediate(GetComponentInChildren<Transform>().gameObject);
        }
    }
}