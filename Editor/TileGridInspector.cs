using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HelloWorld.Editor
{
	[CustomEditor(typeof(EditorTileGrid))]
	public class TileGridInspector : UnityEditor.Editor
	{
		private EditorTileGrid tileGrid;

		private SerializedProperty serializedSize;
		private SerializedProperty serializedTileSize;
		private SerializedProperty serializedTileSet;
		private SerializedProperty serializedTileInputs;
		private SerializedProperty serializedGridCellObject;

		private readonly List<CellData> cells = new();
		private DataTileGrid tileGridData;

		private bool shouldFinalize = false;
		private bool isdead = false; // Used to suppress null reference error on destroy

		private void OnEnable()
		{
			tileGrid = (EditorTileGrid)target;

			serializedSize = serializedObject.FindProperty("size");
			serializedTileSize = serializedObject.FindProperty("tileSize");
			serializedTileSet = serializedObject.FindProperty("tileInputSet");
			serializedTileInputs = serializedObject.FindProperty("allTileInputs");
			serializedGridCellObject = serializedObject.FindProperty("gridCellObject");

			serializedTileInputs.isExpanded = false;
		}

		public override void OnInspectorGUI()
		{
			if (EditorUtility.InstanceIDToObject(tileGrid.GetInstanceID()) == null)
			{
				Debug.LogWarning("Target object has been destroyed.");
				return;
			}

			serializedObject.Update();

			EditorGUILayout.PropertyField(serializedGridCellObject);
			EditorGUILayout.PropertyField(serializedTileSet);
			EditorGUILayout.PropertyField(serializedSize);
			EditorGUILayout.PropertyField(serializedTileSize);

			if (GUILayout.Button("Reload Tile set", GUILayout.Height(20)))
			{
                Undo.RecordObject(target, "Reload TileSet");
                tileGrid.ReLoadTileInputsFromSet();
			}

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Generate", GUILayout.Height(25)))
			{
				tileGrid.ClearCells();
				tileGrid.InitializeGridCells();
				tileGrid.InitializeWave();
			}
			if (GUILayout.Button("Clear", GUILayout.Height(25)))
			{
				tileGrid.Stop();
				tileGrid.ClearCells();
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			shouldFinalize = EditorGUILayout.ToggleLeft("Finalize?", shouldFinalize, GUILayout.Width(70));

			if (shouldFinalize)
			{
				if (GUILayout.Button("Finalize", GUILayout.Height(20)))
				{
					//Fix/suppress errors
					Debug.Log("Finalize game objects");
					isdead = true;
					tileGrid.FinalizeGrid();
				}
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Separator();

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Load File"))
			{
				Load();
			}
			if (GUILayout.Button("Save"))
			{
				SaveData();
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.PropertyField(serializedTileInputs);

			if (!isdead)
			{
				serializedObject?.ApplyModifiedProperties();
			}
			//EditorUtility.SetDirty(target);
		}

		private void Load()
		{
			string filePath = EditorUtility.OpenFilePanel("Select File", "", "json");
			if (!string.IsNullOrEmpty(filePath))
			{
				Debug.Log(filePath);

				StreamReader reader = new(filePath);
				string content = reader.ReadToEnd();
				reader.Close();

				tileGridData = JsonUtility.FromJson<DataTileGrid>(content);
				//Debug.Log(tileGridData.size + " " + tileGridData.tileSize);

				int newSize = tileGridData.size;
				float newTileSize = tileGridData.tileSize;
				List<CellData> newCellData = tileGridData.cellData;

				tileGrid.ResetWave(newSize, newTileSize, newCellData);
			}
		}

		public void SaveData()
		{
			string filePath = EditorUtility.SaveFilePanelInProject("Save level data", "LevelData", "json", "Choose a location to save the generated level data.");
			if (!string.IsNullOrEmpty(filePath))
			{
				GetCellData();
				GetTileGridData();

				string data = JsonUtility.ToJson(tileGridData);
				FileStream filestream = new(filePath, FileMode.Create);

				using StreamWriter writer = new(filestream);
				writer.Write(data);

				Debug.Log("Grid data saved...");

				//if (string.IsNullOrEmpty(filePath)) return;
				//AssetDatabase.CreateAsset(scriptableObject, filePath);
				//AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
		}

		public void GetTileGridData()
		{
			tileGridData = new(tileGrid.size, tileGrid.tileSize, tileGrid.allTileInputs, cells);
		}

		public void GetCellData()
		{
			cells.Clear();
			Debug.Log(tileGrid.gridCell);
			int newXIndex;
			int newYIndex;
			int newSelectedTileID;
			foreach (var item in tileGrid.gridCell)
			{
				EditorGridCell cell = item.GetComponent<EditorGridCell>();
				Debug.Log(cell);
				newXIndex = cell.xIndex;
				newYIndex = cell.yIndex;
				newSelectedTileID = cell.selectedTileID;
				CellData cellData = new(newXIndex, newYIndex, newSelectedTileID);
				cells.Add(cellData);
			}
		}
	}
}