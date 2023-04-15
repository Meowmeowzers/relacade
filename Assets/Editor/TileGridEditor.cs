using HelloWorld;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

[CustomEditor(typeof(TileGrid))]
public class TileGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        
        TileGrid tileGrid = (TileGrid)target;

        tileGrid.gridCellObject = (GameObject) EditorGUILayout.ObjectField(tileGrid.gridCellObject, typeof(GameObject), false);
        tileGrid.size = EditorGUILayout.IntField("Grid Size", 2);
        tileGrid.tileSize = EditorGUILayout.FloatField("Cell Size", 1f);
        tileGrid.startDelay = EditorGUILayout.FloatField("Start Delay", 1f);
        tileGrid.setDelay = EditorGUILayout.FloatField("Set Delay", 0f);
        if (GUILayout.Button("Generate"))
        {
            Debug.Log("Button Pressed...");
            tileGrid.InitializeWave();
        }

    }
}
