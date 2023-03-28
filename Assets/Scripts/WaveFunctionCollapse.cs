using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaveFunctionCollapse : MonoBehaviour
{

    public Tilemap tilemap;
    public List<TileBase> tileset;

    private int[,] wave;
    private bool[,] consistent;
    private int tileSize;
    
    void Start()
    {
        tileSize = tilemap.cellBounds.size.x;
        wave = new int[tileSize, tileSize];
        consistent = new bool[tileSize, tileSize];
        InitializeWave();
        CollapseWave();
        RenderWave();
    }

    void InitializeWave()
    {
        for (int y = 0; y < tileSize; y++)
        {
            for (int x = 0; x < tileSize; x++)
            {
                wave[x, y] = tileset.Count - 1;
                consistent[x, y] = true;
            }
        }
    }

    void CollapseWave()
    {
        while (!IsWaveCollapsed())
        {
            int[] waveCoordinates = GetLowestEntropyTile();
            int x = waveCoordinates[0];
            int y = waveCoordinates[1];
            int tileIndex = GetRandomCompatibleTile(x, y);
            if (tileIndex == -1)
            {
                consistent[x, y] = false;
                wave[x, y] = tileset.Count - 1;
            }
            else
            {
                wave[x, y] = tileIndex;
                for (int neighborY = y - 1; neighborY <= y + 1; neighborY++)
                {
                    for (int neighborX = x - 1; neighborX <= x + 1; neighborX++)
                    {
                        if (neighborX == x && neighborY == y) continue;
                        if (neighborX < 0 || neighborX >= tileSize || neighborY < 0 || neighborY >= tileSize) continue;
                        consistent[neighborX, neighborY] = false;
                    }
                }
            }
        }
    }

    bool IsWaveCollapsed()
    {
        for (int y = 0; y < tileSize; y++)
        {
            for (int x = 0; x < tileSize; x++)
            {
                if (!consistent[x, y]) return false;
            }
        }
        return true;
    }

    int[] GetLowestEntropyTile()
    {
        int lowestEntropy = int.MaxValue;
        int[] lowestEntropyTile = new int[2];
        for (int y = 0; y < tileSize; y++)
        {
            for (int x = 0; x < tileSize; x++)
            {
                if (consistent[x, y])
                {
                    int entropy = GetTileEntropy(x, y);
                    if (entropy < lowestEntropy)
                    {
                        lowestEntropy = entropy;
                        lowestEntropyTile[0] = x;
                        lowestEntropyTile[1] = y;
                    }
                }
            }
        }
        return lowestEntropyTile;
    }

    int GetTileEntropy(int x, int y)
    {
        int entropy = 0;
        for (int i = 0; i < tileset.Count; i++)
        {
            if (IsTileCompatible(i, x, y))
            {
                entropy++;
            }
        }
        return entropy;
    }

    int GetRandomCompatibleTile(int x, int y)
    {
        List<int> compatibleTiles = new();
        for (int i = 0; i < tileset.Count; i++)
        {
            if (IsTileCompatible(i, x, y))
            {
                compatibleTiles.Add(i);
            }
        }
        if (compatibleTiles.Count == 0) return -1;
        return compatibleTiles[Random.Range(0, compatibleTiles.Count)];
    }

    bool IsTileCompatible(int tileIndex, int x, int y)
    {
        TileBase tile = tileset[tileIndex];
        if (!tilemap.HasTile(new Vector3Int(x, y, 0))) return false;
        TileBase tileAbove = tilemap.GetTile(new Vector3Int(x, y + 1, 0));
        TileBase tileBelow = tilemap.GetTile(new Vector3Int(x, y - 1, 0));
        TileBase tileLeft = tilemap.GetTile(new Vector3Int(x - 1, y, 0));
        TileBase tileRight = tilemap.GetTile(new Vector3Int(x + 1, y, 0));
        if (tileAbove != null && !tileAbove.name.Equals(tile.name)) return false;
        if (tileBelow != null && !tileBelow.name.Equals(tile.name)) return false;
        if (tileLeft != null && !tileLeft.name.Equals(tile.name)) return false;
        if (tileRight != null && !tileRight.name.Equals(tile.name)) return false;
        return true;
    }

    void RenderWave()
    {
        for (int y = 0; y < tileSize; y++)
        {
            for (int x = 0; x < tileSize; x++)
            {
                TileBase tile = tileset[wave[x, y]];
                tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }
}
