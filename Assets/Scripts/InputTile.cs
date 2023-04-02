using System.Collections.Generic;
using UnityEngine;

public class InputTile : MonoBehaviour
{
    public int id;
    public List<GameObject> compatibleTop = new();
    public List<GameObject> compatibleBottom = new();
    public List<GameObject> compatibleLeft = new();
    public List<GameObject> compatibleRight = new();
}