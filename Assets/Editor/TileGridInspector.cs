using HelloWorld;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EditorTileGrid))]
public class TileGridInspector : Editor
{
    private EditorTileGrid tileGrid;
    private SerializedObject serializedTileGrid;

    private TextAsset importedData;
    private string fileName = "";
    private bool showDefaultInspector = false;
    private bool enableDefaultInspector = false;

    public List<CellData> cells = new();
    private HelloWorld.DataTileGrid tileGridData;

    public int size;
    public float tilesize;

    public TileInputSet tileInputSet;
    public SerializedObject serializedTileInputSet;

    private SerializedProperty tileInputSetSerialized;
    private SerializedProperty serializedTileInputs;
    private SerializedProperty sizeSerialized;
    private SerializedProperty tileSizeSerialized;

    private void OnEnable()
    {
        tileGrid = (EditorTileGrid)target;
        serializedTileGrid = new(tileGrid);

        tileInputSetSerialized = serializedTileGrid.FindProperty("tileInputSet");
        serializedTileInputSet.FindProperty("tileInputSet");
        serializedTileInputs.FindPropertyRelative("tileInputs");
        sizeSerialized.FindPropertyRelative("size");
        tileSizeSerialized = serializedTileGrid.FindProperty("tileSize");
    }

    
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
    public void SaveData()
    {
        GetCellData();
        GetTileGridData();
        size = tileGrid.size;
        tilesize = tileGrid.tileSize;

        string data = JsonUtility.ToJson(tileGridData);
        FileStream filestream = new(Application.dataPath + "/" + fileName + ".json", FileMode.Create);

        using StreamWriter writer = new(filestream);
        writer.Write(data);

        Debug.Log("Grid data saved...");
        AssetDatabase.Refresh();
    }

    public void GetTileGridData()
    {
        tileGridData = new(tileGrid.size, tileGrid.tileSize, tileGrid.tileInputs, cells);
    }

    public void GetCellData()
    {
        cells.Clear();
        Debug.Log(tileGrid.gridCell);
        foreach (var item in tileGrid.gridCell)
        {
            EditorGridCell cell = item.GetComponent<EditorGridCell>();
            Debug.Log(cell);
            int newXIndex = cell.xIndex;
            int newYIndex = cell.yIndex;
            int newSelectedTileID = cell.selectedTileID;
            CellData cellData = new(newXIndex, newYIndex, newSelectedTileID);
            cells.Add(cellData);
        }
    }

    public void LoadWaveDataFromPath()
    {
        //AssetDatabase.Refresh();
        string path = Application.dataPath + "/" + fileName + ".json";
        Debug.Log(path);

        StreamReader reader = new(path);
        string content = reader.ReadToEnd();
        reader.Close();

        tileGridData = JsonUtility.FromJson<HelloWorld.DataTileGrid>(content);
        //Debug.Log(tileGridData.size + " " + tileGridData.tileSize);

        int newSize = tileGridData.size;
        float newTileSize = tileGridData.tileSize;
        List<CellData> newCellData = tileGridData.cellData;

        tileGrid.ResetWave(newSize, newTileSize, newCellData);
    }

    public void LoadWaveDataFromFile()
    {
        tileGridData = JsonUtility.FromJson<HelloWorld.DataTileGrid>(importedData.ToString());

        int newSize = tileGridData.size;
        float newTileSize = tileGridData.tileSize;
        List<CellData> newCellData = tileGridData.cellData;

        tileGrid.ResetWave(newSize, newTileSize, newCellData);
    }
}