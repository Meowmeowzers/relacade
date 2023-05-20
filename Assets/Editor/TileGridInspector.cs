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

        //private TextAsset importedData;
		//private bool showSaveLoadSection = false;
        //private bool showDefaultInspector = false;
        //private bool enableDefaultInspector = false;
        private bool shouldFinalize = false;

        private void OnEnable()
		{
			tileGrid = (EditorTileGrid)target;

			serializedSize = serializedObject.FindProperty("size");
			serializedTileSize = serializedObject.FindProperty("tileSize");
			serializedTileSet = serializedObject.FindProperty("tileInputSet");
			serializedTileInputs = serializedObject.FindProperty("tileInputs");
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

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Generate", GUILayout.Height(25)))
			{
				tileGrid.ClearCells();
                tileGrid.InitializeGridCells();
                tileGrid.InitializeWave();
			}
			if (GUILayout.Button("Clear", GUILayout.Height(25)))
			{
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
					tileGrid.FinalizeGrid();
					tileGrid = null;
				}
			}

			EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            //importedData = (TextAsset)EditorGUILayout.ObjectField("Data file", importedData, typeof(TextAsset), true);
			
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
			
			//if(EditorUtility.InstanceIDToObject(tileGrid.GetInstanceID()) == null)
			//{
				serializedObject.ApplyModifiedProperties();
			//}
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
			tileGridData = new(tileGrid.size, tileGrid.tileSize, tileGrid.tileInputs, cells);
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

/*
if (showDefaultInspector = EditorGUILayout.Foldout(showDefaultInspector, "Show default Inspector"))
{
enableDefaultInspector = EditorGUILayout.BeginToggleGroup("Enable", enableDefaultInspector);
//base.OnInspectorGUI();
DrawDefaultInspector();
EditorGUILayout.EndToggleGroup();
}
*/

/*
EditorGUILayout.LabelField("File Name", GUILayout.Width(60));
        fileName = EditorGUILayout.TextField(fileName);
        if (GUILayout.Button("Save"))
        {
            Debug.Log("TODO::::SaveData()");
            //SaveData();
        }
*/
/*
public override void OnInspectorGUI()
{
    serializedObject.Update();
    serializedTileGrid.Update();
    if(serializedTileInputSet != null)
    {
        serializedTileInputSet.Update();
    }

    tileGrid.gridCellObject = (GameObject)EditorGUILayout.ObjectField("Cell Object", tileGrid.gridCellObject, typeof(GameObject), false);
    //EditorGUILayout.ObjectField("TileGrid", tileGrid, typeof(TileGridInspector), false);
    EditorGUILayout.PropertyField(sizeSerialized);
    EditorGUILayout.PropertyField(tileSizeSerialized);

    tileInputSet = (TileInputSet)EditorGUILayout.ObjectField("Tile Input Set", tileInputSet, typeof(TileInputSet), false);
    if(tileInputSet != null)
    {
        serializedTileInputSet = new SerializedObject(tileInputSet);
    }

    GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate"))
        {
            tileGrid.ClearCells();
            tileGrid.gridCell = new EditorGridCell[tileGrid.size, tileGrid.size];
            tileGrid.InitializeWave();
        }
        if (GUILayout.Button("Clear"))
        {
            tileGrid.ClearCells();
        }
    GUILayout.EndHorizontal();

    EditorGUILayout.Separator();

    fileName = EditorGUILayout.TextField("Data path", fileName);
    importedData = (TextAsset)EditorGUILayout.ObjectField("Data file", importedData, typeof(Object), true);

    if (GUILayout.Button("Save"))
    {
        SaveData();
    }

    GUILayout.BeginHorizontal();
        if (GUILayout.Button("Load from file"))
        {
            LoadWaveDataFromFile();
        }
        if (GUILayout.Button("Load from path"))
        {
            LoadWaveDataFromPath();
        }
    GUILayout.EndHorizontal();


    EditorGUILayout.PropertyField(serializedObject.FindProperty("tileInputs"), new("Input Tiles SO"), true);
    if(tileInputSet != null)
    {
        if (GUILayout.Button("Load from Input Tile Set SO"))
        {
            tileGrid.GetInputTilesFromSet();
        }
        serializedTileInputs.objectReferenceValue = tileInputSet;
        EditorGUILayout.PropertyField(serializedTileInputSet.FindProperty("AllInputTiles"), new("Input Tiles"), true);
    }


    if (showDefaultInspector = EditorGUILayout.Foldout(showDefaultInspector, "Show default Inspector"))
    {
        enableDefaultInspector = EditorGUILayout.BeginToggleGroup("Enable", enableDefaultInspector);
        //base.OnInspectorGUI();
        DrawDefaultInspector();
        EditorGUILayout.EndToggleGroup();
    }

    serializedObject.ApplyModifiedProperties();
    serializedTileGrid.ApplyModifiedProperties();
    if(serializedTileInputSet != null)
    {
        serializedTileInputSet.ApplyModifiedProperties();
    }
}
*/