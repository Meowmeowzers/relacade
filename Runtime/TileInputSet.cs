using System;
using System.Collections.Generic;
using UnityEngine;

namespace HelloWorld
{
    [CreateAssetMenu(fileName = "New Input Tile Set Data", menuName = "Relacade/Input Tile Set Data")]
    [Serializable]
    public class TileInputSet : ScriptableObject
    {
        [SerializeField] public List<TileInput> AllInputTiles = new();
        [SerializeField] public List<TileInput> UniqueInputTiles = new();

        [SerializeField] public List<TileInput> ForeGroundTiles = new();
        [SerializeField] public List<TileInput> BackGroundTiles = new();
        [SerializeField] public List<TileInput> FilledTiles = new();

        // 2x2 - Road/Field tile set
        [SerializeField] public List<TileInput> EdgeUpTiles = new();
        [SerializeField] public List<TileInput> EdgeDownTiles = new();
        [SerializeField] public List<TileInput> EdgeLeftTiles = new();
        [SerializeField] public List<TileInput> EdgeRightTiles = new();

        [SerializeField] public List<TileInput> ElbowUpLeftTiles = new();
        [SerializeField] public List<TileInput> ElbowUpRightTiles = new();
        [SerializeField] public List<TileInput> ElbowDownLeftTiles = new();
        [SerializeField] public List<TileInput> ElbowDownRightTiles = new();

        [SerializeField] public List<TileInput> CornerUpLeftTiles = new();
        [SerializeField] public List<TileInput> CornerUpRightTiles = new();
        [SerializeField] public List<TileInput> CornerDownLeftTiles = new();
        [SerializeField] public List<TileInput> CornerDownRightTiles = new();

        [SerializeField] public List<TileInput> CornerULDRTiles = new();
        [SerializeField] public List<TileInput> CornerURDLTiles = new();

        // 3x3 - Directions, Road like tile set
        [SerializeField] public List<TileInput> FourFaceTiles = new();

        [SerializeField] public List<TileInput> VerticalTiles = new();
        [SerializeField] public List<TileInput> HorizontalTiles = new();

        [SerializeField] public List<TileInput> TwoFaceUpLeftTiles = new();
        [SerializeField] public List<TileInput> TwoFaceUpRightTiles = new();
        [SerializeField] public List<TileInput> TwoFaceDownLeftTiles = new();
        [SerializeField] public List<TileInput> TwoFaceDownRightTiles = new();

        [SerializeField] public List<TileInput> ThreeFaceUpTiles = new();
        [SerializeField] public List<TileInput> ThreeFaceDownTiles = new();
        [SerializeField] public List<TileInput> ThreeFaceLeftTiles = new();
        [SerializeField] public List<TileInput> ThreeFaceRightTiles = new();

        [SerializeField] public List<TileInput> OneFaceUpTiles = new();
        [SerializeField] public List<TileInput> OneFaceDownTiles = new();
        [SerializeField] public List<TileInput> OneFaceLeftTiles = new();
        [SerializeField] public List<TileInput> OneFaceRightTiles = new();

        

    }
}
