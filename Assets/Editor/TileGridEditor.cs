using HelloWorld;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CustomEditor(typeof(EditorTileGrid))]
public class TileGridEditor : Editor
{
    private EditorTileGrid tileGrid;
    private bool showDefaultInspector = false;
    private bool enableDefaultInspector = false;
    
    public override void OnInspectorGUI()
    {
        tileGrid = (EditorTileGrid) target;
        tileGrid.gridCellObject = (GameObject) EditorGUILayout.ObjectField("Cell Object", tileGrid.gridCellObject, typeof(GameObject), true);
        tileGrid.size = EditorGUILayout.IntSlider("Grid size", tileGrid.size, 2, 50);
        tileGrid.tileSize = EditorGUILayout.FloatField("Cell Size", tileGrid.tileSize);
        //tileGrid.inputTiles = EditorGUILayout.ObjectField(tileGrid.inputTiles, typeof(GameObject), false);
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

        if(showDefaultInspector = EditorGUILayout.Foldout(showDefaultInspector, "Show default Inspector"))
        {
        enableDefaultInspector = EditorGUILayout.BeginToggleGroup("Enable", enableDefaultInspector);
        base.OnInspectorGUI();
        EditorGUILayout.EndToggleGroup();

        }
    }
}
