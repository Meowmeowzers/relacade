using System.Collections.Generic;
using UnityEngine;

namespace HelloWorld
{

    //Future: make entropy 1 the definite state
    //TODO: Dont use tilegrid variable. Pass values from parent instead. IDK to reduce coupling??
    public class EditorGridCell : MonoBehaviour
    {
        public int xIndex;
        public int yIndex;
        public int entropy;
        public int selectedTileID;
        public bool isDefinite = false;
        public List<GameObject> inputTiles;

        public List<TileInput> tileInputs;
        public List<TileInput> propagatedTileInputs;
        private TileInput selectedTileInput;

        [SerializeField] private EditorTileGrid tileGrid;
        private List<GameObject> propagatedCells = new();
        private GameObject select;

        public void Initialize()
        {
            tileGrid = GetComponentInParent<EditorTileGrid>();
            entropy = tileGrid.inputTiles.Count;
            inputTiles.Clear();
            inputTiles.AddRange(tileGrid.inputTiles);
            propagatedCells.Clear();
            isDefinite = false;
        }
        //New script
        public void Initialize(int tileCount, List<TileInput> value)
        {
            entropy = tileCount;
            tileInputs.Clear();
            tileInputs.AddRange(value);
            propagatedTileInputs.Clear();
            isDefinite = false;
        }

        public void SelectTile(int removeNextTime)
        {
            //int select = inputTiles[Random.Range(0, inputTiles.Count)].GetComponent<InputTile>().id;
            selectedTileInput = Instantiate(tileInputs[Random.Range(0, tileInputs.Count)], transform);

            tileInputs.Clear();
            tileInputs.Add(selectedTileInput);

            selectedTileID = selectedTileInput.id;
            isDefinite = true;
            entropy = 1;

            Debug.Log("Selected Cell: " + xIndex + " " + yIndex
                + ", Selected Tile: " + select + ", ID: " + selectedTileInput.id);
        }
        public void NewSelectTile(TileInput tileInput)
        {
            selectedTileID = tileInput.id;
            selectedTileInput = tileInput;
            Instantiate(tileInput.gameObject, transform);
            tileInputs.Clear();
            tileInputs.Add(selectedTileInput);
            isDefinite = true;
            entropy = 1;
        }

        public void NewPropagate(List<TileInput> compatibleTiles)
        {
            propagatedTileInputs.Clear();
            foreach (var item in compatibleTiles)
            {
                if (tileInputs.Contains(item))
                {
                    propagatedTileInputs.Add(item);
                }
            }
            tileInputs.Clear();
            tileInputs.AddRange(propagatedTileInputs);
            entropy = tileInputs.Count;
        }

        public bool NewIsCellNotConflict()
        {
            if (tileInputs.Count > 0)
                return true;
            else
                return false;
        }
        //NewScript end

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
            Initialize();
            DestroyImmediate(GetComponentInChildren<Transform>().gameObject);
        }

        public bool IsDefiniteState()
        {
            if (entropy == 1)
                return true;
            else
                return false;
        }
    }
}