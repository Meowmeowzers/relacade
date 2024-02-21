using System;
using System.Collections.Generic;
using UnityEngine;

namespace HelloWorld.Editor
{
	[Serializable]
	public class TileInput : ScriptableObject
	{
		public string tileName = "";
		public GameObject gameObject;
		public int id = 0;
		public float weight = 1f;

		public List<TileInput> compatibleTop = new();
		public List<TileInput> compatibleBottom = new();
		public List<TileInput> compatibleLeft = new();
		public List<TileInput> compatibleRight = new();
	}
}