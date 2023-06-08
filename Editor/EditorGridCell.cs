using System.Collections.Generic;
using UnityEngine;

namespace HelloWorld.Editor
{
    public class EditorGridCell : MonoBehaviour
    {
        public int xIndex;
        public int yIndex;
        public int entropy;
        public int selectedTileID;
        private bool isDefinite = false;

        public List<TileInput> tileInputs;
        public List<TileInput> propagatedTileInputs = new();

        public TileInput selectedTileInput;

        //New script
        public void Initialize(List<TileInput> value)
        {
            tileInputs.Clear();
            tileInputs.AddRange(value);
            propagatedTileInputs.Clear();
            isDefinite = false;
            entropy = tileInputs.Count;
        }

        public void SelectTile()
        {
            selectedTileInput = tileInputs[Random.Range(0, tileInputs.Count)];
            if(selectedTileInput.gameObject != null )
            {
                Instantiate(selectedTileInput.gameObject, transform);
            }

            tileInputs.Clear();
            tileInputs.Add(selectedTileInput);

            selectedTileID = selectedTileInput.id;
            isDefinite = true;
            entropy = 1;

            //Debug.Log("Selected Cell: " + xIndex + " " + yIndex
            //  + ", Selected Tile: " + selectedTileInput + ", ID: " + selectedTileInput.id);
        }

        public void SelectTile(TileInput tileInput)
        {
            selectedTileID = tileInput.id;
            selectedTileInput = tileInput;
            Instantiate(tileInput.gameObject, transform);
            tileInputs.Clear();
            tileInputs.Add(selectedTileInput);
            isDefinite = true;
            entropy = 1;
        }

        public void Propagate(List<TileInput> compatibleTiles)
        {
            propagatedTileInputs.Clear();
            foreach (var item in compatibleTiles)
            {
                foreach(var itemid in tileInputs)
                {
                    if(itemid.id == item.id)
                    {
                        propagatedTileInputs.Add(item);
                    }
                }
            }
            tileInputs.Clear();
            tileInputs.AddRange(propagatedTileInputs);
            entropy = tileInputs.Count;
        }

        public bool IsCellNotConflict()
        {
            if (tileInputs.Count > 0)
                return true;
            else
                return false;
        }

        //NewScript end

        public void ResetAndInitializeCell(List<TileInput> value)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            Initialize(value);
        }

        public bool IsNotDefiniteState()
        {
            if (!isDefinite)
                return true;
            else
                return false;
        }
    }
}