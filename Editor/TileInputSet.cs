using System;
using System.Collections.Generic;
using UnityEngine;

namespace HelloWorld.Editor
{
    [CreateAssetMenu(fileName = "New Input Tile Set", menuName = "Relacade/Input Tile Set")]
    [Serializable]
    public class TileInputSet : ScriptableObject
    {
        public List<TileInput> AllInputTiles = new();
    }
}