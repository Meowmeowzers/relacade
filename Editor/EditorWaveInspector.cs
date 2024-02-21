using UnityEditor;
using UnityEngine;

namespace HelloWorld.Editor
{
	[CustomEditor(typeof(EditorWave))]
	public class EditorWaveInspector : UnityEditor.Editor
	{
		private EditorWave tileGrid;

		private SerializedProperty serializedSizeX;
		private SerializedProperty serializedSizeY;
		private SerializedProperty serializedTileSize;
		private SerializedProperty serializedTileSet;
		private SerializedProperty serializedTileInputs;

		private GUIContent gridSizeXLabel = new("Grid Size X", "Set the width of the tile grid");
		private GUIContent gridSizeYLabel = new("Grid Size Y", "Set the height of the tile grid");
		private GUIContent cellSizeLabel = new("Cell Size", "Set the size of the grid cells");
		private GUIContent tileSetLabel = new("Tile Set");

		private bool shouldFinalize = false;
		private bool isdead = false; // Used to suppress null reference error on destroy
		private bool isMoreShown = false;

		private void OnEnable()
		{
			tileGrid = target as EditorWave;

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
				EditorGUILayout.PropertyField(serializedTileSet, tileSetLabel);
				EditorGUILayout.PropertyField(serializedSizeX, gridSizeXLabel);
				EditorGUILayout.PropertyField(serializedSizeY, gridSizeYLabel);
				EditorGUILayout.PropertyField(serializedTileSize, cellSizeLabel);

				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Initialize", GUILayout.Height(25)))
				{
					Reload();
					tileGrid.EnsureGridCellObject(AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.gatozhanya.relacade/Objects/EditorGridCell.prefab"));
					if (tileGrid.IsDone())
					{
						tileGrid.RemoveGridCells();
						tileGrid.InitializeWave();
					}
				}
				if (GUILayout.Button("Generate", GUILayout.Height(25)))
				{
					Reload();
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
						isdead = true;
						tileGrid.FinalizeGrid();
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				EditorGUILayout.BeginVertical();
				EditorGUILayout.HelpBox("\nGenerating...\n\nDrag mouse here after generation to show back controls\n", MessageType.Info);
				if (GUILayout.Button("Stop", GUILayout.Height(25)))
				{
					tileGrid.Stop();
				}
				EditorGUILayout.EndVertical();

			}




			isMoreShown = EditorGUILayout.Foldout(isMoreShown, "More", true, EditorStyles.foldout);
			if (isMoreShown)
			{
				EditorGUI.indentLevel++;

				EditorGUI.BeginDisabledGroup(true);

				EditorGUILayout.BeginVertical();
				GUILayout.Label("More info");
				//EditorGUILayout.PropertyField(entropy);
				EditorGUILayout.EndVertical();

				EditorGUI.EndDisabledGroup();

				EditorGUI.indentLevel--;
			}

			EditorGUILayout.EndVertical();

			if (!isdead)
			{
				serializedObject.ApplyModifiedProperties();
			}
		}

		// Auto reload before generating
		private void Reload()
		{
			if (serializedTileSet.objectReferenceValue != null)
			{
				SerializedObject scriptableListObject = new(serializedTileSet.objectReferenceValue);

				SerializedProperty scriptableObjectListProperty = scriptableListObject.FindProperty("allInputTiles");

				serializedTileInputs.ClearArray();
				for (int i = 0; i < scriptableObjectListProperty.arraySize; i++)
				{
					SerializedProperty elementProperty = scriptableObjectListProperty.GetArrayElementAtIndex(i);
					serializedTileInputs.InsertArrayElementAtIndex(i);
					serializedTileInputs.GetArrayElementAtIndex(i).objectReferenceValue = elementProperty.objectReferenceValue;
				}
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}