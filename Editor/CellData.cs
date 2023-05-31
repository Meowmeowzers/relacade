using System;

[Serializable]
public class CellData
{
    public int xIndex;
    public int yIndex;
    public int selectedTileID;

    public CellData(int xIndex, int yIndex, int selectedTileID)
    {
        this.xIndex = xIndex;
        this.yIndex = yIndex;
        this.selectedTileID = selectedTileID;
    }
}