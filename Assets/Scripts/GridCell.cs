using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    private TileGrid tileGrid;
    [SerializeField] private GameObject[] inputTiles;
    public int entropy;
    public bool isDefinite = false;

    void Start()
    {
        tileGrid = GetComponentInParent<TileGrid>();
        //Debug.Log(tileGrid + " " + entropy);
        entropy = tileGrid.inputTiles.Length;
        inputTiles = tileGrid.inputTiles;
    }

    public void SelectTile()
    {
        Instantiate(inputTiles[Random.Range(0, inputTiles.Length)], transform);
        isDefinite = true;
    }
}
