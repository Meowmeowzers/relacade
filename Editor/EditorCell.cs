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

		public List<InputTile> currentTiles = new();
		public List<InputTile> propagatedTiles = new();
		public List<InputTile> allTiles = new();

		public InputTile selectedTile;
		public InputTile fixedTile;

		private float totalWeight = 0f;
		private float cumulutativeWeight = 0f;
		private float randomChoice = 0f;
		
		public void Initialize(List<InputTile> value)
		{
			allTiles.Clear();
			allTiles.AddRange(value);
			currentTiles.Clear();
			currentTiles.AddRange(value);
			propagatedTiles.Clear();
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

		public void ChangeTile(InputTile tile){
			RemoveTile();
			Instantiate(tile.gameObject, transform.position, Quaternion.identity, transform);
		}

		public void RemoveTile(){
			for (int i = transform.childCount - 1; i >= 0; i--)
			{
				DestroyImmediate(transform.GetChild(i).gameObject);
			}
		}
		public void PropagateWith(List<InputTile> compatibleTiles)
		{
			propagatedTiles.Clear();
			foreach (var item in compatibleTiles)
			{
				foreach (var itemid in currentTiles)
				{
					if (itemid.id == item.id)
					{
						propagatedTiles.Add(item);
					}
				}
			}
			currentTiles.Clear();
			currentTiles.AddRange(propagatedTiles);
			entropy = currentTiles.Count;
		}

		public void ResetAndInitializeCell(List<InputTile> value)
		{
			RemoveTile();
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