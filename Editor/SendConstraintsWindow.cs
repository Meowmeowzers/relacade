using UnityEditor;
using UnityEngine;

// I think this can now be combined with the other one
// Having mutual option makes them both the same, revise
namespace HelloWorld.Editor
{
	public class SendConstraintsWindow : EditorWindow
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
			SendConstraintsWindow window = (SendConstraintsWindow)GetWindow(typeof(SendConstraintsWindow));
			window.titleContent = new GUIContent("Send Constraints");
			window.minSize = new(330, 500);
			window.maxSize = new(330, 1080);
			set = newSet;
			tile = newTile;
			CheckExistingTiles();
			InitWindow();
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

			previewTexture = new Texture2D(1, 1);
			previewTexture.SetPixel(0, 0, listItemLightColor);
			previewTexture.Apply();

			constraintTexture = new Texture2D(1, 1);
			constraintTexture.SetPixel(0, 0, Color.black);
			constraintTexture.Apply();

			listItemStyle.normal.background = listItemTexture;
			listItemStyle.hover.background = listItemLightTexture;

			previewStyle.padding = new(0,0,0,0); //unused i think
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
                    "Check the tiles you want to mark as compatible with this tile." +
					"\n\nClick the preview image on the left side of a row to preview the tile side by side",
					MessageType.Info);
			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(320));

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
							GUILayout.Label(previewTexture, previewStyle, GUILayout.Width(60), GUILayout.Height(60));
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
							GUILayout.Label(previewTexture, previewStyle, GUILayout.Width(60), GUILayout.Height(60));
							GUILayout.Label(constraintTexture, previewStyle, GUILayout.Width(60), GUILayout.Height(60));
							GUILayout.Label(previewTexture, previewStyle, GUILayout.Width(60), GUILayout.Height(60));
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
							GUILayout.Label(previewTexture, previewStyle, GUILayout.Width(60), GUILayout.Height(60));
						GUILayout.FlexibleSpace();
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.EndVertical();
					}
				}
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();

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
					TileInput tileInput = itemProperty.objectReferenceValue as TileInput;

					if (tileInput != null)
						tilePreview = AssetPreview.GetAssetPreview(tileInput.gameObject);
					else
						tilePreview = Texture2D.blackTexture;

					itemName = tileInput != null ? tileInput.tileName : "No tileToSend";

					EditorGUILayout.BeginHorizontal(listItemStyle);
					if (GUILayout.Button(tilePreview, GUILayout.Width(40), GUILayout.Height(40)))
					{
						if(tileInput != null)
							constraintTexture = AssetPreview.GetAssetPreview(tileInput.gameObject);
						else
							constraintTexture = Texture2D.blackTexture;
					}
					EditorGUILayout.LabelField(itemName, GUILayout.Width(90), GUILayout.Height(40), GUILayout.ExpandWidth(false));

					if (togglesArray[i] == null)
						togglesArray[i] = new bool[4];

					for (int j = 0; j < 4; j++)
					{
						if (j != 0)
							GUILayout.Space(25);
						togglesArray[i][j] = EditorGUILayout.Toggle(togglesArray[i][j], GUILayout.Width(20), GUILayout.Height(40));
					}

					EditorGUILayout.EndHorizontal();
				}
			}

			GUILayout.EndVertical();
			EditorGUILayout.EndScrollView();
			EditorGUILayout.Space();

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
				TileInput tileInput = tile.targetObject as TileInput;

				togglesArray = new bool[set.arraySize][];

				for (int i = 0; i < set.arraySize; i++)
				{
					SerializedProperty itemProperty = set.GetArrayElementAtIndex(i);
					TileInput item = itemProperty.objectReferenceValue as TileInput;

					togglesArray[i] = new bool[4];

					if (item != null)
					{
						togglesArray[i][0] = item.compatibleTop.Contains(tileInput);
						togglesArray[i][1] = item.compatibleBottom.Contains(tileInput);
						togglesArray[i][2] = item.compatibleLeft.Contains(tileInput);
						togglesArray[i][3] = item.compatibleRight.Contains(tileInput);
					}
				}
			}
		}

		private void ApplyModifications()
		{
			SerializedProperty selectedTileProperty;
			TileInput selectedTile;

			if (tile != null)
			{
				TileInput tileToSend = tile.targetObject as TileInput;

				for (int i = 0; i < set.arraySize; i++)
				{
					selectedTileProperty = set.GetArrayElementAtIndex(i);
					selectedTile = selectedTileProperty.objectReferenceValue as TileInput;

					if (selectedTile == null) continue;
					
					for (int j = 0; j < 4; j++)
					{
						UpdateTile(selectedTile, tileToSend, togglesArray[i][j], j);
					}

					selectedTileProperty.serializedObject.ApplyModifiedProperties();
				}
			}

			tile.ApplyModifiedProperties();
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		private void UpdateTile(TileInput tileToModify, TileInput tileToSend, bool isCompatible, int index)
		{// Improve this?
			switch (index)
			{
				case 0:
					if (isCompatible)
					{
						if (!tileToModify.compatibleTop.Contains(tileToSend))
							tileToModify.compatibleTop.Add(tileToSend);
					}
					else
					{
						if (tileToModify.compatibleTop.Contains(tileToSend))
							tileToModify.compatibleTop.Remove(tileToSend);
					}
					break;

				case 1:
					if (isCompatible)
					{
						if (!tileToModify.compatibleBottom.Contains(tileToSend))
							tileToModify.compatibleBottom.Add(tileToSend);
					}
					else
					{
						if (tileToModify.compatibleBottom.Contains(tileToSend))
							tileToModify.compatibleBottom.Remove(tileToSend);
					}
					break;

				case 2:
					if (isCompatible)
					{
						if (!tileToModify.compatibleLeft.Contains(tileToSend))
							tileToModify.compatibleLeft.Add(tileToSend);
					}
					else
					{
						if (tileToModify.compatibleLeft.Contains(tileToSend))
							tileToModify.compatibleLeft.Remove(tileToSend);
					}
					break;

				case 3:
					if (isCompatible)
					{
						if (!tileToModify.compatibleRight.Contains(tileToSend))
							tileToModify.compatibleRight.Add(tileToSend);
					}
					else
					{
						if (tileToModify.compatibleRight.Contains(tileToSend))
							tileToModify.compatibleRight.Remove(tileToSend);
					}
					break;
			}
		}
	}
}