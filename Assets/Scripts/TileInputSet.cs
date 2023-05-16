using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HelloWorld
{
    [CreateAssetMenu(fileName = "New Input Tile Set Data", menuName = "Relacade/Input Tile Set Data")]
    public class TileInputSet : ScriptableObject
    {
        [SerializeField] public List<TileInput> AllInputTiles = new();

        [SerializeField] public List<TileInput> FourDirectionTiles = new();
        [SerializeField] public List<TileInput> TopTiles = new();
        [SerializeField] public List<TileInput> TopLeftTiles = new();
        [SerializeField] public List<TileInput> TopRightTiles = new();
        [SerializeField] public List<TileInput> LeftTiles = new();
        [SerializeField] public List<TileInput> RightTiles = new();
        [SerializeField] public List<TileInput> BottomTiles = new();
        [SerializeField] public List<TileInput> BottomLeftTiles = new();
        [SerializeField] public List<TileInput> BottomRightTiles = new();
        [SerializeField] public List<TileInput> IFourDirectionTiles = new();
        [SerializeField] public List<TileInput> ITopLeftTiles = new();
        [SerializeField] public List<TileInput> ITopRightTiles = new();
        [SerializeField] public List<TileInput> IBottomLeftTiles = new();
        [SerializeField] public List<TileInput> IBottomRightTiles = new();
    }
}
