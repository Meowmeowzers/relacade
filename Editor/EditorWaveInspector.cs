using System;
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
		private SerializedProperty serializedInputTiles;
		private SerializedProperty serializedFixedTiles;
		private SerializedProperty serializedHasFixed;

		private GUIContent gridSizeXLabel = new("Grid Size X", "Set the width of the tile row");
		private GUIContent gridSizeYLabel = new("Grid Size Y", "Set the height of the tile row");
		private GUIContent cellSizeLabel = new("Cell Size", "Set the size of the row cells");
		private GUIContent tileSetLabel = new("Tile Set");
		private GUIContent enableFixedTilesLabel = new("Enable fixed tiles");

		private bool shouldFinalize = false;
		private bool isdead = false; // Used to suppress null reference error on destroy

		[Range(2, 80)] private int tempX = 8;
		[Range(2, 80)] private int tempY = 8;
		[Min(1)] private float tempCellSize = 1f;
		private InputTileSet tempSet;

		EditorCell tempCell;
		Texture2D tempPreview;
		
		private void OnEnable()
		{
			tileGrid = target as EditorWave;

			serializedSizeX = serializedObject.FindProperty("gridSizeX");
			serializedSizeY = serializedObject.FindProperty("gridSizeY");
			serializedTileSize = serializedObject.FindProperty("tileSize");
			serializedTileSet = serializedObject.FindProperty("inputTileSet");
			serializedInputTiles = serializedObject.FindProperty("inputTiles");
			serializedFixedTiles = serializedObject.FindProperty("fixedTiles");
			serializedHasFixed = serializedObject.FindProperty("hasFixed");

			serializedInputTiles.isExpanded = false;
			serializedObject.Update();

			if (tileGrid.IsInitialized())
			{
				tempSet = serializedTileSet.objectReferenceValue as InputTileSet;
				tempX = serializedSizeX.intValue;
				tempY = serializedSizeY.intValue;
				tempCellSize = serializedTileSize.floatValue;
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.BeginVertical();

			if (tileGrid.IsDone())
			{
				EditorGUI.BeginDisabledGroup(serializedHasFixed.boolValue);

				tempSet = EditorGUILayout.ObjectField(tileSetLabel, tempSet, typeof(InputTileSet), false) as InputTileSet;
				tempX = EditorGUILayout.IntSlider(gridSizeXLabel, tempX, 2, 80);
				tempY = EditorGUILayout.IntSlider(gridSizeYLabel, tempY, 2, 80);
				tempCellSize = EditorGUILayout.FloatField(cellSizeLabel, tempCellSize);

				EditorGUI.EndDisabledGroup();

				EditorGUILayout.PropertyField(serializedHasFixed, enableFixedTilesLabel);

				EditorGUILayout.BeginHorizontal();
				EditorGUI.BeginDisabledGroup(!tileGrid.IsInitialized());
				if (GUILayout.Button("Generate", GUILayout.Height(25)))
				{
					if (tileGrid.IsDone() && tileGrid.inputTileSet != null)
					{
						tileGrid.ResetCells();
						if (serializedHasFixed.boolValue)
							tileGrid.StartCollapseFixed();
						tileGrid.StartCollapse();
						tempX = serializedSizeX.intValue;
						tempY = serializedSizeY.intValue;
						tempCellSize = serializedTileSize.floatValue;
						tempSet = serializedTileSet.objectReferenceValue as InputTileSet;
					}
				}
				if (GUILayout.Button("Clear", GUILayout.Height(25)))
				{
					tileGrid.ResetCells();
					if (serializedHasFixed.boolValue)
						tileGrid.StartCollapseFixed();
					tempX = serializedSizeX.intValue;
					tempY = serializedSizeY.intValue;
					tempCellSize = serializedTileSize.floatValue;
					tempSet = serializedTileSet.objectReferenceValue as InputTileSet;
				}
				EditorGUI.EndDisabledGroup();
				if (tileGrid.IsInitialized())
				{
					EditorGUI.BeginDisabledGroup(tileGrid.CheckIfSameSize(tempX, tempY, tempCellSize, tempSet));
					if (GUILayout.Button("Initialize", GUILayout.Height(25)))
					{
						Reload2();
						tileGrid.SetFields(tempX, tempY, tempCellSize, tempSet);
						tileGrid.InitializeWave();
					}
					EditorGUI.EndDisabledGroup();
				}
				else
				{
					if (GUILayout.Button("Initialize", GUILayout.Height(25)))
					{
						Reload2();
						tileGrid.SetFields(tempX, tempY, tempCellSize, tempSet);
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
				EditorGUILayout.BeginVertical(GUILayout.Height(145));
				EditorGUILayout.HelpBox("\n\n\n\n    Generating...\n\n\n\n", MessageType.Info);
				if (GUILayout.Button("Stop", GUILayout.Height(25)))
				{
					tileGrid.StopCollapse();
				}
				EditorGUILayout.EndVertical();
			}

			EditorGUILayout.EndVertical();

			if (!isdead)
			{
				serializedObject.ApplyModifiedProperties();
			}
		}

		private void Reload2()
		{
			if (tempSet != null)
			{
				InputTile tile;

				serializedInputTiles.ClearArray();

				for (int i = 0; i < tempSet.allInputTiles.Count; i++)
				{
					tile = tempSet.allInputTiles[i];
					serializedInputTiles.InsertArrayElementAtIndex(i);
					serializedInputTiles.GetArrayElementAtIndex(i).objectReferenceValue = tile;
				}
				serializedObject.ApplyModifiedProperties();
			}
		}

		private void FixedTileList(SerializedProperty tileProperty)
		{
			for(int i = 0; i < tileProperty.arraySize; i++) 
			{
				tempCell = tileProperty.GetArrayElementAtIndex(i).objectReferenceValue as EditorCell;
				if (tempCell == null)
					Debug.Log(tempCell.selectedTile.gameObject);
				if (tempCell.fixedTile.gameObject != null)
					tempPreview = AssetPreview.GetAssetPreview(tempCell.fixedTile.gameObject);
				else
					tempPreview = Texture2D.blackTexture;

				EditorGUILayout.BeginHorizontal(GUILayout.Height(50));
					EditorGUILayout.BeginVertical();
						GUILayout.FlexibleSpace();
						EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("X", GUILayout.Width(30));
							EditorGUILayout.IntField(tempCell.xIndex);
							EditorGUILayout.LabelField("Y", GUILayout.Width(30));
							EditorGUILayout.IntField(tempCell.yIndex);
						EditorGUILayout.EndHorizontal();
					GUILayout.FlexibleSpace();
					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical();
							GUILayout.Label(tempPreview, GUILayout.Width(50), GUILayout.Height(50));
					EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
			}
			tileProperty.serializedObject.ApplyModifiedProperties();
		}
	}
}