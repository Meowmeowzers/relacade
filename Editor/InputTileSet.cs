using System;
using System.Collections.Generic;
using UnityEngine;

namespace HelloWorld.Editor
{
	[Serializable]
	public class InputTileSet : ScriptableObject
	{
		[HideInInspector]
		public List<InputTile> allInputTiles = new();
		[HideInInspector]
		public List<InputTile> tileReferences = new();
	}
}