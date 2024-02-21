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
		private SerializedProperty serializedHasFixed;

		private GUIContent gridSizeXLabel = new("Grid Size X", "Set the width of the tile grid");
		private GUIContent gridSizeYLabel = new("Grid Size Y", "Set the height of the tile grid");
		private GUIContent cellSizeLabel = new("Cell Size", "Set the size of the grid cells");
		private GUIContent tileSetLabel = new("Tile Set");
		private GUIContent enableFixedTilesLabel = new("Fixed Tiles", "Enable fixed tiles");

		private bool shouldFinalize = false;
		private bool isdead = false; // Used to suppress null reference error on destroy
		private bool isMoreShown = false;

		[Range(2, 80)] private int tempX = 1;
		[Range(2, 80)] private int tempY = 1;
		[Min(1)] private float tempCellSize = 1f;

		private void OnEnable()
		{
			tileGrid = target as EditorWave;

			serializedSizeX = serializedObject.FindProperty("tileSizeX");
			serializedSizeY = serializedObject.FindProperty("tileSizeY");
			serializedTileSize = serializedObject.FindProperty("tileSize");
			serializedTileSet = serializedObject.FindProperty("tileInputSet");
			serializedTileInputs = serializedObject.FindProperty("allTileInputs");
			serializedHasFixed = serializedObject.FindProperty("hasFixed");

			serializedTileInputs.isExpanded = false;
			serializedObject.Update();

			tempX = serializedSizeX.intValue;
			tempY = serializedSizeY.intValue;
			tempCellSize = serializedTileSize.floatValue;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.BeginVertical();

			if (tileGrid.IsDone())
			{
				EditorGUILayout.PropertyField(serializedTileSet, tileSetLabel);

				EditorGUI.BeginDisabledGroup(serializedHasFixed.boolValue);

				tempX = EditorGUILayout.IntSlider(gridSizeXLabel, tempX, 2, 80);
				tempY = EditorGUILayout.IntSlider(gridSizeYLabel, tempY, 2, 80);
				tempCellSize = EditorGUILayout.FloatField(cellSizeLabel, tempCellSize);
				tileGrid.CheckIfSameSize(tempX, tempY, tempCellSize);

				EditorGUI.EndDisabledGroup();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(serializedHasFixed, enableFixedTilesLabel);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				if (tileGrid.IsInitialized())
				{
					if (GUILayout.Button("Generate", GUILayout.Height(25)))
					{
						if (tileGrid.IsDone())
						{
							tileGrid.ResetCells();
							if (serializedHasFixed.boolValue)
								tileGrid.StartCollapseFixedTiles();
							tileGrid.StartCollapse();
						}
					}
					if (GUILayout.Button("Clear", GUILayout.Height(25)))
					{
						tileGrid.ResetCells();
						if (serializedHasFixed.boolValue)
							tileGrid.StartCollapseFixedTiles();
					}
				}
				else
				{
					if (GUILayout.Button("Initialize", GUILayout.Height(25)))
					{
						Reload();
						tileGrid.SetSize(tempX, tempY, tempCellSize);
						tileGrid.InitializeWave();
					}
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

		private void Reload()
		{
			if (serializedTileSet.objectReferenceValue != null)
			{
				SerializedObject tileSetObject = new(serializedTileSet.objectReferenceValue);
				SerializedProperty allTiles = tileSetObject.FindProperty("allInputTiles");
				SerializedProperty tile;

				serializedTileInputs.ClearArray();

				for (int i = 0; i < allTiles.arraySize; i++)
				{
					tile = allTiles.GetArrayElementAtIndex(i);
					serializedTileInputs.InsertArrayElementAtIndex(i);
					serializedTileInputs.GetArrayElementAtIndex(i).objectReferenceValue = tile.objectReferenceValue;
				}
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}