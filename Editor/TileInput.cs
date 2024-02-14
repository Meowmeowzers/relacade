using System;
using System.Collections.Generic;
using UnityEngine;

namespace HelloWorld.Editor
{
    [CreateAssetMenu(fileName = "New Input Tile", menuName = "Relacade/Input Tile")]
    [Serializable]
    public class TileInput : ScriptableObject
    {
        public GameObject gameObject;
        public int id;
        public float weight = 1f;

        public List<TileInput> compatibleTop = new();
        public List<TileInput> compatibleBottom = new();
        public List<TileInput> compatibleLeft = new();
        public List<TileInput> compatibleRight = new();
    }
}