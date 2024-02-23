using System;
using System.Collections.Generic;
using UnityEngine;

namespace HelloWorld.Editor
{
	[Serializable]
	public class TileInputSet : ScriptableObject
	{
		[HideInInspector]
		public List<TileInput> allInputTiles = new();
		[HideInInspector]
		public List<TileInput> tileReferences = new();
	}
}