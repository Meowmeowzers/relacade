using UnityEditor;
using UnityEngine;

namespace HelloWorld.Editor
{
	[CustomEditor(typeof(EditorWave))]
	public class TileGridInspector : UnityEditor.Editor
	{
		private EditorWave tileGrid;

		private SerializedProperty serializedSizeX;
		private SerializedProperty serializedSizeY;
		private SerializedProperty serializedTileSize;
		private SerializedProperty serializedTileSet;
		private SerializedProperty serializedTileInputs;

		private bool shouldFinalize = false;
		private bool isdead = false; // Used to suppress null reference error on destroy

		private void OnEnable()
		{
			tileGrid = (EditorWave)target;

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
			EditorGUILayout.EndVertical();
			//EditorGUILayout.PropertyField(serializedTileInputs);

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

				SerializedProperty scriptableObjectListProperty = scriptableListObject.FindProperty("allInputTiles");

				serializedTileInputs.ClearArray();
				for (int i = 0; i < scriptableObjectListProperty.arraySize; i++)
				{
					SerializedProperty elementProperty = scriptableObjectListProperty.GetArrayElementAtIndex(i);
					serializedTileInputs.InsertArrayElementAtIndex(i);
					serializedTileInputs.GetArrayElementAtIndex(i).objectReferenceValue = elementProperty.objectReferenceValue;
				}
			}
		}
	}
}