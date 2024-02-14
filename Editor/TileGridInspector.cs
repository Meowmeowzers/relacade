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

        private SerializedProperty serializedSizeX;
        private SerializedProperty serializedSizeY;
        private SerializedProperty serializedTileSize;
        private SerializedProperty serializedTileSet;
        private SerializedProperty serializedTileInputs;

        private bool shouldFinalize = false;
        private bool isdead = false; // Used to suppress null reference error on destroy

        //Used for save/load 
        private DataTileGrid tileGridData;
        private List<CellData> cells = new();

        private void OnEnable()
        {
            tileGrid = (EditorTileGrid)target;

            serializedSizeX = serializedObject.FindProperty("tileSizeX");
            serializedSizeY = serializedObject.FindProperty("tileSizeY");
            serializedTileSize = serializedObject.FindProperty("tileSize");
            serializedTileSet = serializedObject.FindProperty("tileInputSet");
            serializedTileInputs = serializedObject.FindProperty("allTileInputs");
            
            serializedTileInputs.isExpanded = false;
            serializedObject.Update();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.BeginVertical();

            if (tileGrid.IsDone())
            {
                EditorGUILayout.PropertyField(serializedTileSet);
                EditorGUILayout.PropertyField(serializedSizeX);
                EditorGUILayout.PropertyField(serializedSizeY);
                EditorGUILayout.PropertyField(serializedTileSize);
            }
            else
            {
                EditorGUILayout.BeginVertical(GUILayout.Height(65));
                EditorGUILayout.HelpBox("Drag mouse here after generation to show back controls", MessageType.Info);
                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("Reload Tile set", GUILayout.Height(20)))
            {
                Reload();
            }

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Generate", GUILayout.Height(25)))
            {
                tileGrid.EnsureGridCellObject(AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.gatozhanya.relacade/Objects/EditorGridCell.prefab"));
                if (tileGrid.IsDone())
                {
                    tileGrid.RemoveGridCells();
                    tileGrid.InitializeWave();
                }
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

            /*
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
            */
            EditorGUILayout.EndVertical();

            EditorGUILayout.PropertyField(serializedTileInputs);

            if (!isdead)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void Reload()
        {
            if (serializedTileSet.objectReferenceValue != null)
            {
                SerializedObject scriptableListObject = new(serializedTileSet.objectReferenceValue);

                SerializedProperty scriptableObjectListProperty = scriptableListObject.FindProperty("AllInputTiles");

                serializedTileInputs.ClearArray();
                for (int i = 0; i < scriptableObjectListProperty.arraySize; i++)
                {
                    SerializedProperty elementProperty = scriptableObjectListProperty.GetArrayElementAtIndex(i);
                    serializedTileInputs.InsertArrayElementAtIndex(i);
                    serializedTileInputs.GetArrayElementAtIndex(i).objectReferenceValue = elementProperty.objectReferenceValue;
                }
            }
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
                List<TileInput> newTileInputs = tileGridData.inputTiles;

                tileGrid.LoadWaveFromData(newSize, newTileSize, newCellData, newTileInputs);
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
            //Debug.Log(tileGrid.gridCell);
            int newXIndex;
            int newYIndex;
            int newSelectedTileID;
            foreach (var item in tileGrid.GetCells())
            {
                EditorGridCell cell = item.GetComponent<EditorGridCell>();
                //Debug.Log(cell);
                newXIndex = cell.xIndex;
                newYIndex = cell.yIndex;
                newSelectedTileID = cell.selectedTileID;
                CellData cellData = new(newXIndex, newYIndex, newSelectedTileID);
                cells.Add(cellData);
            }
        }
    }
}