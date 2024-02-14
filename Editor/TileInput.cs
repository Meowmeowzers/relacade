using System;
using System.Collections.Generic;
using UnityEngine;

namespace HelloWorld.Editor
{
    [CreateAssetMenu(fileName = "New Input Tile", menuName = "Relacade/Input Tile")]
    [Serializable]
    public class TileInput : ScriptableObject
    {
        [SerializeField] public GameObject gameObject;
        [SerializeField] public int id;
        [SerializeField] public float weight = 1f;

        [SerializeField] public List<TileInput> compatibleTop = new();
        [SerializeField] public List<TileInput> compatibleBottom = new();
        [SerializeField] public List<TileInput> compatibleLeft = new();
        [SerializeField] public List<TileInput> compatibleRight = new();
    }
}