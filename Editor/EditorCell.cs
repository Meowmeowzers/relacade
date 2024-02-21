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
		[SerializeField] private bool isDefinite = false;

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
			currentTiles.Clear();
			currentTiles.AddRange(value);
			allTiles.AddRange(value);
			propagatedTileInputs.Clear();
			isDefinite = false;
			entropy = currentTiles.Count;
		}

		public void SelectTile()
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
			isDefinite = true;
			entropy = 1;

			//Debug.Log("Selected Cell: " + xIndex + " " + yIndex
			//  + ", Selected Tile: " + selectedTile + ", ID: " + selectedTile.id);
		}

		public void SelectTile(TileInput tileInput)
		{
			selectedTileID = tileInput.id;
			selectedTile = tileInput;
			Instantiate(tileInput.gameObject, transform);
			currentTiles.Clear();
			currentTiles.Add(selectedTile);
			isDefinite = true;
			entropy = 1;
		}

		public void Propagate(List<TileInput> compatibleTiles)
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

		public bool IsCellNotConflict()
		{
			if (currentTiles.Count > 0)
				return true;
			else
				return false;
		}

		public void ResetAndInitializeCell(List<TileInput> value)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				DestroyImmediate(transform.GetChild(i).gameObject);
			}

			Initialize(value);
		}

		public bool IsNotDefiniteState()
		{
			if (!isDefinite)
				return true;
			else
				return false;
		}
	}
}