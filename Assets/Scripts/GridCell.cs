using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//TODO::make entropy the definite state
public class GridCell : MonoBehaviour
{
    public int yIndex;
    public int xIndex;
    public int entropy;
    public int selectedTileID;
    public bool isDefinite = false;

    public List<GameObject> inputTiles;
    private List<GameObject> propagatedCells = new();

    private TileGrid tileGrid;

    void Start()
    {
        tileGrid = GetComponentInParent<TileGrid>();
        entropy = tileGrid.inputTiles.Count;
        inputTiles.AddRange(tileGrid.inputTiles);
    }

    public void SelectTile()
    {
        Debug.Log("Selected Cell: " + xIndex + " " + yIndex);
        foreach(var item in inputTiles)
        {
            Debug.Log("Selected Cell " + xIndex + " " + yIndex + " item: " + item);
        }
        //int select = inputTiles[Random.Range(0, inputTiles.Count)].GetComponent<InputTile>().id;
        GameObject select = Instantiate(inputTiles[Random.Range(0, inputTiles.Count)], transform);
        Debug.Log(select);
        inputTiles.Clear();
        inputTiles.Add(select);
        selectedTileID = select.GetComponent<InputTile>().id;
        isDefinite = true;
        entropy = 1;
        
    }

    public void Propagate(List<GameObject> compatibleTop)
    {
        //inputTiles.Clear();
        propagatedCells.Clear();
        foreach(var item in compatibleTop)
        {
            if(inputTiles.Contains(item))
            {
                propagatedCells.Add(item);
            }
        }
        inputTiles.Clear();
        inputTiles.AddRange(propagatedCells);
        entropy = inputTiles.Count;
    }
}
