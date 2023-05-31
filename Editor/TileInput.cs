using System;
using System.Collections.Generic;
using UnityEngine;

namespace HelloWorld.Editor
{
    [CreateAssetMenu(fileName = "New Input Tile Data", menuName = "Relacade/Input Tile Data")]
    [Serializable]
    public class TileInput : ScriptableObject
    {
        [SerializeField] public GameObject gameObject;
        [SerializeField] public int id;

        [SerializeField] public List<TileInput> compatibleTop = new();
        [SerializeField] public List<TileInput> compatibleBottom = new();
        [SerializeField] public List<TileInput> compatibleLeft = new();
        [SerializeField] public List<TileInput> compatibleRight = new();
    }
}