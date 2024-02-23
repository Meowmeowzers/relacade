using System;
using System.Collections.Generic;
using UnityEngine;

namespace HelloWorld.Editor
{
	[Serializable]
	public class InputTile : ScriptableObject
	{
		public string tileName = "";
		public GameObject gameObject;
		public int id = 0;
		public float weight = 1f;

		public List<InputTile> compatibleTop = new();
		public List<InputTile> compatibleBottom = new();
		public List<InputTile> compatibleLeft = new();
		public List<InputTile> compatibleRight = new();
	}
}