using HelloWorld;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EditorTileGrid))]
public class TileGridInspector : Editor
{
    private EditorTileGrid tileGrid;
    private TextAsset importedData;
    private string fileName = "";
    private bool showDefaultInspector = false;
    private bool enableDefaultInspector = false;

    public List<CellData> cells = new();
    private DataTileGrid tileGridData;

    public int size;
    public float tilesize;

    public TileInputSet tileInputSet;
    public SerializedObject serializedTileInputSet;

    private void OnEnable()
    {
        tileGrid = (EditorTileGrid)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        tileGrid.gridCellObject = (GameObject)EditorGUILayout.ObjectField("Cell Object", tileGrid.gridCellObject, typeof(GameObject), false);
        //EditorGUILayout.ObjectField("TileGrid", tileGrid, typeof(TileGridInspector), false);
        tileGrid.size = EditorGUILayout.IntSlider("Grid size", tileGrid.size, 2, 50);
        tileGrid.tileSize = EditorGUILayout.FloatField("Cell Size", tileGrid.tileSize);

        tileInputSet = (TileInputSet)EditorGUILayout.ObjectField("Tile Input Set", tileInputSet, typeof(TileInputSet), false);
        if(tileInputSet != null)
        {
            serializedTileInputSet = new SerializedObject(tileInputSet);
        }

        GUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate"))
            {
                tileGrid.ClearCells();
                tileGrid.gridCell = new GameObject[tileGrid.size, tileGrid.size];
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

        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("inputTiles"), new("Input Tiles"), true);
        if (showDefaultInspector = EditorGUILayout.Foldout(showDefaultInspector, "Show default Inspector"))
        {
            enableDefaultInspector = EditorGUILayout.BeginToggleGroup("Enable", enableDefaultInspector);
            //base.OnInspectorGUI();
            DrawDefaultInspector();
            EditorGUILayout.EndToggleGroup();
        }
        if (GUILayout.Button("Test"))
        {
            EditorCoroutineUtility.StartCoroutine(Test(), this);
            Debug.Log("asdf");
        }

        serializedObject.ApplyModifiedProperties();
       
    }

    public IEnumerator Test()
    {
        int y = 0;
        while (y != 10)
        {
            Debug.Log(y);
            yield return new EditorWaitForSeconds(1f);
            y++;
        }
    }

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
        tileGridData = new(tileGrid.size, tileGrid.tileSize, tileGrid.inputTiles, cells);
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

        tileGridData = JsonUtility.FromJson<DataTileGrid>(content);
        //Debug.Log(tileGridData.size + " " + tileGridData.tileSize);

        int newSize = tileGridData.size;
        float newTileSize = tileGridData.tileSize;
        List<CellData> newCellData = tileGridData.cellData;

        tileGrid.ResetWave(newSize, newTileSize, newCellData);
    }

    public void LoadWaveDataFromFile()
    {
        tileGridData = JsonUtility.FromJson<DataTileGrid>(importedData.ToString());

        int newSize = tileGridData.size;
        float newTileSize = tileGridData.tileSize;
        List<CellData> newCellData = tileGridData.cellData;

        tileGrid.ResetWave(newSize, newTileSize, newCellData);
    }
}