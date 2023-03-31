using System.Collections.Generic;
using UnityEngine;

//Future: make entropy 1 the definite state
public class GridCell : MonoBehaviour
{
    public int xIndex;
    public int yIndex;
    public int entropy;
    public int selectedTileID;
    public bool isDefinite = false;
    public List<GameObject> inputTiles;

    private TileGrid tileGrid;
    private List<GameObject> propagatedCells = new();
    private GameObject select;

    private void Start()
    {
        tileGrid = GetComponentInParent<TileGrid>();
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

    public int CheckEntropy()
    {
        return inputTiles.Count;
    }

    public void ResetCell()
    {
        entropy = tileGrid.inputTiles.Count;
        inputTiles.Clear();
        propagatedCells.Clear();
        inputTiles.AddRange(tileGrid.inputTiles);
        isDefinite = false;
        Destroy(GetComponentInChildren<Transform>().gameObject);
        
    }
}