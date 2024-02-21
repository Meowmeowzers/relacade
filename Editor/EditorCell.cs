using System.Collections.Generic;
using UnityEngine;

namespace HelloWorld.Editor
{
	public class EditorCell : MonoBehaviour
	{
		public int xIndex;
		public int yIndex;
		public int entropy;
		public int selectedTileID;

		public List<TileInput> currentTiles;
		public List<TileInput> propagatedTileInputs = new();
		public List<TileInput> allTiles;

		public TileInput selectedTile;
		public TileInput fixedTile;

		private float totalWeight = 0f;
		private float cumulutativeWeight = 0f;
		private float randomChoice = 0f;

		public void Initialize(List<TileInput> value)
		{
			allTiles.Clear();
			allTiles.AddRange(value);
			currentTiles.Clear();
			currentTiles.AddRange(value);
			propagatedTileInputs.Clear();
			entropy = allTiles.Count;
			selectedTile = null;
			selectedTileID = -1;
			totalWeight = 0f;
			cumulutativeWeight = 0f;
			randomChoice = 0f;
		}

		public void SelectRandomTile()
		{
			totalWeight = 0f;
			cumulutativeWeight = 0f;

			foreach (var tile in currentTiles)
			{
				totalWeight += tile.weight;
			}

			randomChoice = Random.Range(0f, totalWeight);

			foreach (var tile in currentTiles)
			{
				cumulutativeWeight += tile.weight;
				if (cumulutativeWeight >= randomChoice)
				{
					selectedTile = tile;
					break;
				}
			}
			if (selectedTile.gameObject != null)
			{
				Instantiate(selectedTile.gameObject, transform);
			}

			currentTiles.Clear();
			currentTiles.Add(selectedTile);

			selectedTileID = selectedTile.id;
			entropy = 1;
		}

		public void SelectFixedTile()
		{
			if (!IsFixed())
			{
				Debug.Log("Tile not fixed");
				return;
			}
			selectedTileID = fixedTile.id;
			selectedTile = fixedTile;
			Instantiate(fixedTile.gameObject, transform);
			currentTiles.Clear();
			currentTiles.Add(selectedTile);
			entropy = 1;
		}

		public void PropagateWith(List<TileInput> compatibleTiles)
		{
			propagatedTileInputs.Clear();
			foreach (var item in compatibleTiles)
			{
				foreach (var itemid in currentTiles)
				{
					if (itemid.id == item.id)
					{
						propagatedTileInputs.Add(item);
					}
				}
			}
			currentTiles.Clear();
			currentTiles.AddRange(propagatedTileInputs);
			entropy = currentTiles.Count;
		}

		public void ResetAndInitializeCell(List<TileInput> value)
		{
			for (int i = transform.childCount - 1; i >= 0; i--)
			{
				if (i > 0)
					Debug.Log(transform.childCount);
				DestroyImmediate(transform.GetChild(i).gameObject);
			}
			Initialize(value);
		}

		public bool IsNotDefiniteState()
		{
			if (selectedTile == null)
				return true;
			else
				return false;
		}

		public bool IsFixed()
		{
			if (fixedTile != null) return true;
			else return false;
		}

		public bool IsCellNotConflict()
		{
			if (currentTiles.Count > 0)
				return true;
			else
				return false;
		}
	}
}