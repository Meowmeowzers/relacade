using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloWorld
{
    [CreateAssetMenu(fileName ="New Input Tile Data", menuName="Relacade/Input Tile Data")]
    [Serializable]
    public class TileInput : ScriptableObject
    {
        public GameObject gameObject;
        public int id;
        
        public List<TileInput> compatibleTop = new();
        public List<TileInput> compatibleBottom = new();
        public List<TileInput> compatibleLeft = new();
        public List<TileInput> compatibleRight = new();
    }
}
