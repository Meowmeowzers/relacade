using UnityEditor;
using UnityEngine;

namespace HelloWorld.Editor
{
	public class MirrorConstraintsWindow : EditorWindow
	{
		private static SerializedProperty set;
		private static SerializedObject tile;
		private Vector2 scrollPosition;

		private static bool[][] togglesArray;
		private bool showHelp = false;

		private static Texture2D tilePreview;
		private static Texture2D listItemTexture;
		private static Texture2D listItemLightTexture;
		private static GUIStyle listItemStyle = new();
		private static GUIStyle previewStyle = new();
		private static Color listItemColor = new(0.18f, 0.18f, 0.18f, 1f);
		private static Color listItemLightColor = new(0.2f, 0.2f, 0.2f, 1f);
		private string itemName = "";

		static Texture2D previewTexture;
		static Texture2D constraintTexture;

		public static void OpenWindow(SerializedObject newTile, SerializedProperty newSet)
		{
			MirrorConstraintsWindow window = (MirrorConstraintsWindow)GetWindow(typeof(MirrorConstraintsWindow));
			window.titleContent = new GUIContent("Mirror Constraints");
			window.minSize = new(330, 500);
			window.maxSize = new(330, 1080);
			tile = newTile;
			set = newSet;
			InitWindow();
			CheckExistingTiles();
			window.Show();
		}

		private static void InitWindow()
		{
			listItemTexture = new Texture2D(1, 1);
			listItemTexture.SetPixel(0, 0, listItemColor);
			listItemTexture.Apply();

			listItemLightTexture = new Texture2D(1, 1);
			listItemLightTexture.SetPixel(0, 0, listItemLightColor);
			listItemLightTexture.Apply();

			listItemStyle.normal.background = listItemTexture;
			listItemStyle.hover.background = listItemLightTexture;

			previewStyle.padding = new(0, 0, 0, 0); //unused i think
			previewStyle.margin = new(); //unused i think
		}

		private void OnGUI()
		{
			GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Help", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) showHelp = !showHelp;
			EditorGUILayout.EndHorizontal();
			if (showHelp)
				EditorGUILayout.HelpBox(
					"Check the tiles you want to be compatible with each other." +
					"\n\nClick the preview image on the left side of a row to preview the tile side by side",
					MessageType.Info);
			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(320));
			GUILayout.FlexibleSpace();

			if (tile != null)
			{
				GameObject tileGameObject = tile.FindProperty("gameObject").objectReferenceValue as GameObject;

				if (tileGameObject != null)
				{
					previewTexture = AssetPreview.GetAssetPreview(tileGameObject);

					if (previewTexture != null)
					{
						EditorGUILayout.BeginVertical();
						EditorGUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label(constraintTexture, previewStyle, GUILayout.Width(60), GUILayout.Height(60));
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label(constraintTexture, previewStyle, GUILayout.Width(60), GUILayout.Height(60));
						GUILayout.Label(previewTexture, previewStyle, GUILayout.Width(60), GUILayout.Height(60));
						GUILayout.Label(constraintTexture, previewStyle, GUILayout.Width(60), GUILayout.Height(60));
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label(constraintTexture, previewStyle, GUILayout.Width(60), GUILayout.Height(60));
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.EndVertical();
					}
				}
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.LabelField("Top", EditorStyles.boldLabel, GUILayout.Width(35));
			EditorGUILayout.LabelField("Bottom", EditorStyles.boldLabel, GUILayout.Width(55));
			EditorGUILayout.LabelField("Left", EditorStyles.boldLabel, GUILayout.Width(40));
			EditorGUILayout.LabelField("Right", EditorStyles.boldLabel, GUILayout.Width(55));
			EditorGUILayout.EndHorizontal();

			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));

			if (tile != null && set != null)
			{
				for (int i = 0; i < set.arraySize; i++)
				{
					SerializedProperty itemProperty = set.GetArrayElementAtIndex(i);
					InputTile tileInput = itemProperty.objectReferenceValue as InputTile;

					EditorGUILayout.BeginHorizontal(listItemStyle);

					if (tileInput != null)
						tilePreview = AssetPreview.GetAssetPreview(tileInput.gameObject);
					else
						tilePreview = Texture2D.blackTexture;

					itemName = tileInput != null ? tileInput.tileName : "No tile";

					if (GUILayout.Button(tilePreview, GUILayout.Width(40), GUILayout.Height(40)))
					{
						if (tileInput != null)
							constraintTexture = AssetPreview.GetAssetPreview(tileInput.gameObject);
						else
							constraintTexture = Texture2D.blackTexture;
					}
					EditorGUILayout.LabelField(itemName, GUILayout.Width(90), GUILayout.Height(30), GUILayout.ExpandWidth(false));

					for (int j = 0; j < 4; j++)
					{
						if (j != 0) GUILayout.Space(25);
						togglesArray[i][j] = EditorGUILayout.Toggle(togglesArray[i][j], GUILayout.Width(20), GUILayout.Height(30));
					}
					EditorGUILayout.EndHorizontal();
				}
			}

			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndScrollView();

			if (GUILayout.Button("Apply", GUILayout.Height(40)))
			{
				ApplyModifications();
				Close();
			}
		}

		private static void CheckExistingTiles()
		{
			if (tile != null)
			{
				InputTile tileInput = tile.targetObject as InputTile;

				togglesArray = new bool[set.arraySize][];

				for (int i = 0; i < set.arraySize; i++)
				{
					SerializedProperty selectedTile = set.GetArrayElementAtIndex(i);
					InputTile item = selectedTile.objectReferenceValue as InputTile;

					togglesArray[i] = new bool[4];

					if (item != null)
					{
						togglesArray[i][0] = item.compatibleTop.Contains(tileInput) && tileInput.compatibleBottom.Contains(item);
						togglesArray[i][1] = item.compatibleBottom.Contains(tileInput) && tileInput.compatibleTop.Contains(item);
						togglesArray[i][2] = item.compatibleLeft.Contains(tileInput) && tileInput.compatibleRight.Contains(item);
						togglesArray[i][3] = item.compatibleRight.Contains(tileInput) && tileInput.compatibleLeft.Contains(item);
					}
				}
			}
		}
		private void ApplyModifications()
		{
			SerializedProperty selectedTileToReceiveFrom;
			InputTile tileToReceiveFrom;

			if (tile != null)
			{
				InputTile targetTile = tile.targetObject as InputTile;

				for (int i = 0; i < set.arraySize; i++)
				{
					selectedTileToReceiveFrom = set.GetArrayElementAtIndex(i);
					tileToReceiveFrom = selectedTileToReceiveFrom.objectReferenceValue as InputTile;

					if (tileToReceiveFrom == null) continue;

					for (int j = 0; j < 4; j++)
					{
						UpdateTile(targetTile, tileToReceiveFrom, togglesArray[i][j], j);
					}

					selectedTileToReceiveFrom.serializedObject.ApplyModifiedProperties();
				}
			}

			tile.ApplyModifiedProperties();
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		private void UpdateTile(InputTile currentTile, InputTile otherTile, bool isCompatible, int index)
		{// Improve this?
			switch (index)
			{
				case 0:
					if (isCompatible)
					{
						if (!currentTile.compatibleTop.Contains(otherTile))
							currentTile.compatibleTop.Add(otherTile);
						if (!otherTile.compatibleBottom.Contains(currentTile))
							otherTile.compatibleBottom.Add(currentTile);
					}
					else
					{
						if (currentTile.compatibleTop.Contains(otherTile))
							currentTile.compatibleTop.Remove(otherTile);
						if (otherTile.compatibleBottom.Contains(currentTile))
							otherTile.compatibleBottom.Remove(currentTile);
					}
					break;

				case 1:
					if (isCompatible)
					{
						if (!currentTile.compatibleBottom.Contains(otherTile))
							currentTile.compatibleBottom.Add(otherTile);
						if (!otherTile.compatibleTop.Contains(currentTile))
							otherTile.compatibleTop.Add(currentTile);
					}
					else
					{
						if (currentTile.compatibleBottom.Contains(otherTile))
							currentTile.compatibleBottom.Remove(otherTile);
						if (otherTile.compatibleTop.Contains(currentTile))
							otherTile.compatibleTop.Remove(currentTile);
					}
					break;

				case 2:
					if (isCompatible)
					{
						if (!currentTile.compatibleLeft.Contains(otherTile))
							currentTile.compatibleLeft.Add(otherTile);
						if (!otherTile.compatibleRight.Contains(currentTile))
							otherTile.compatibleRight.Add(currentTile);
					}
					else
					{
						if (currentTile.compatibleLeft.Contains(otherTile))
							currentTile.compatibleLeft.Remove(otherTile);
						if (otherTile.compatibleRight.Contains(currentTile))
							otherTile.compatibleRight.Remove(currentTile);
					}
					break;

				case 3:
					if (isCompatible)
					{
						if (!currentTile.compatibleRight.Contains(otherTile))
							currentTile.compatibleRight.Add(otherTile);
						if (!otherTile.compatibleLeft.Contains(currentTile))
							otherTile.compatibleLeft.Add(currentTile);
					}
					else
					{
						if (currentTile.compatibleRight.Contains(otherTile))
							currentTile.compatibleRight.Remove(otherTile);
						if (otherTile.compatibleLeft.Contains(currentTile))
							otherTile.compatibleLeft.Remove(currentTile);
					}
					break;
			}
		}
	}
}