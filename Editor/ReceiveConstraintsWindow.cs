using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

// I think this can now be combined with the other one
// Having mutual option makes them both the same, revise
namespace HelloWorld.Editor
{
	public class ReceiveConstraintsWindow : EditorWindow
	{
		private static SerializedProperty set;
		private static SerializedObject tile;
		private Vector2 scrollPosition;

		private static bool[][] togglesArray;
		private bool isMirrored = false;
		private bool showHelp = false;

		private static Texture2D tilePreview;
		private static Texture2D listItemTexture;
		private static Texture2D listItemLightTexture;
		private static GUIStyle listItemStyle = new();
		private static GUIStyle previewStyle = new();
		private static Color listItemColor = new(0.18f, 0.18f, 0.18f, 1f);
		private static Color listItemLightColor = new(0.2f, 0.2f, 0.2f, 1f);
		private static Color lightGrayColor = new(0.5f, 0.5f, 0.5f, 1f);
		private GUIContent isMirroredLabel = new("Mirror", "Apply mirrored modifications to the selected tiles");
		private string tempItemName = "";
		private static int tempIndex = -1;

		private Rect leftSection;
		private Rect rightSection;
		private readonly int leftSectionWidth = 300;

		static Texture2D previewTexture;
		static Texture2D constraintTexture;
		static Texture2D grayTexture;

		private static Color normalColor = new(1f, 1f, 1f, 1f);
		private static Color fadeColor = new(1f, 1f, 1f, 0.3f);
		private static Color tempColor;

		public static void OpenWindow(SerializedObject newTile, SerializedProperty newSet)
		{
			ReceiveConstraintsWindow window = (ReceiveConstraintsWindow)GetWindow(typeof(ReceiveConstraintsWindow));
			window.titleContent = new GUIContent("Receive Constraints");
			window.minSize = new(650, 320);
			window.maxSize = new(650, 1080);
			tile = newTile;
			set = newSet;
			tempIndex = -1;
			previewTexture = null;
			constraintTexture = null;
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

			grayTexture = new Texture2D(1, 1);
			grayTexture.SetPixel(0 ,0 , lightGrayColor);
			grayTexture.Apply();

			listItemStyle.normal.background = listItemTexture;
			listItemStyle.hover.background = listItemLightTexture;

			previewStyle.normal.background = listItemLightTexture;
			previewStyle.hover.background = grayTexture;
			previewStyle.active.background = listItemTexture;
			previewStyle.padding = new(1, 1, 1, 1);
		}
		private void DrawLayouts()
		{
			leftSection.x = 0;
			leftSection.width = leftSectionWidth;
			leftSection.height = position.height;

			rightSection.x = leftSectionWidth;
			rightSection.width = position.width - leftSectionWidth;
			rightSection.height = position.height;

			GUI.DrawTexture(leftSection, listItemTexture);
			GUI.DrawTexture(rightSection, listItemLightTexture);
		}

		private void OnGUI()
		{
			DrawLayouts();
			DrawLeft();
			DrawRight();
		}

		private void DrawLeft()
		{
			GUILayout.BeginArea(leftSection);

			GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
			isMirrored = EditorGUILayout.ToggleLeft(isMirroredLabel, isMirrored, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(60));
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Help", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) showHelp = !showHelp;
			EditorGUILayout.EndHorizontal();
			if (showHelp){
				EditorGUILayout.HelpBox(
					"Check the tiles you want to mark this tile as compatible with." +
					"\n\nClick the preview image on the left side of a row to preview the tile side by side",
					MessageType.Info);
			}
			else
				EditorGUILayout.Space(10);

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
                        
						StartCenteredHorizontal();
                        PreviewButton(tempIndex, 0, constraintTexture);
                        EndCenteredHorizontal();

                        StartCenteredHorizontal();
                        PreviewButton(tempIndex, 2, constraintTexture);
                        if (GUILayout.Button(previewTexture, previewStyle, GUILayout.Width(70), GUILayout.Height(70))) { }
                        PreviewButton(tempIndex, 3, constraintTexture);
                        EndCenteredHorizontal();

                        StartCenteredHorizontal();
                        PreviewButton(tempIndex, 1, constraintTexture);
						EndCenteredHorizontal();

                        EditorGUILayout.EndVertical();
                    }
                }
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(20);
			if (GUILayout.Button("Apply", GUILayout.Height(40)))
			{
				ApplyModifications();
				Close();
			}

			GUILayout.EndArea();
		}

        private static void PreviewButton(int i, int j, Texture2D texture)
        {
			if(i > -1 && togglesArray[i][j]) GUI.color = Color.white;
			else GUI.color = fadeColor;

			if (GUILayout.Button(texture, previewStyle, GUILayout.Width(70), GUILayout.Height(70)))
			{
				if (i > -1) togglesArray[i][j] = !togglesArray[i][j];
			}
			GUI.color = Color.white;
        }

        private void DrawRight()
		{
			GUILayout.BeginArea(rightSection);

			EditorGUILayout.BeginHorizontal(GUILayout.Width(330));
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

					tempItemName = tileInput != null ? tileInput.tileName : "No tile";

					if (GUILayout.Button(tilePreview, GUILayout.Width(40), GUILayout.Height(40)))
					{
						tempIndex = i;
						if (tileInput != null)
							constraintTexture = AssetPreview.GetAssetPreview(tileInput.gameObject);
						else
							constraintTexture = Texture2D.blackTexture;
					}
					EditorGUILayout.LabelField(tempItemName, GUILayout.Width(90), GUILayout.Height(30), GUILayout.ExpandWidth(false));

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

			GUILayout.EndArea();
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
						togglesArray[i][0] = tileInput.compatibleTop.Contains(item);
						togglesArray[i][1] = tileInput.compatibleBottom.Contains(item);
						togglesArray[i][2] = tileInput.compatibleLeft.Contains(item);
						togglesArray[i][3] = tileInput.compatibleRight.Contains(item);
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

		private void UpdateTile(InputTile targetTile, InputTile tileToReceiveFrom, bool isCompatible, int index)
		{// Improve this?
			switch (index)
			{
				case 0:
					if (isCompatible)
					{
						if (!targetTile.compatibleTop.Contains(tileToReceiveFrom))
							targetTile.compatibleTop.Add(tileToReceiveFrom);
						if (isMirrored && !tileToReceiveFrom.compatibleBottom.Contains(targetTile))
                            tileToReceiveFrom.compatibleBottom.Add(targetTile);
					}
					else
					{
						if (targetTile.compatibleTop.Contains(tileToReceiveFrom))
							targetTile.compatibleTop.Remove(tileToReceiveFrom);
						if (isMirrored && tileToReceiveFrom.compatibleBottom.Contains(targetTile))
                            tileToReceiveFrom.compatibleBottom.Remove(targetTile);
					}
					break;

				case 1:
					if (isCompatible)
					{
						if (!targetTile.compatibleBottom.Contains(tileToReceiveFrom))
							targetTile.compatibleBottom.Add(tileToReceiveFrom);
						if (isMirrored && !tileToReceiveFrom.compatibleTop.Contains(targetTile))
                            tileToReceiveFrom.compatibleTop.Add(targetTile);
					}
					else
					{
						if (targetTile.compatibleBottom.Contains(tileToReceiveFrom))
							targetTile.compatibleBottom.Remove(tileToReceiveFrom);
						if (isMirrored && tileToReceiveFrom.compatibleTop.Contains(targetTile))
                            tileToReceiveFrom.compatibleTop.Remove(targetTile);
					}
					break;

				case 2:
					if (isCompatible)
					{
						if (!targetTile.compatibleLeft.Contains(tileToReceiveFrom))
							targetTile.compatibleLeft.Add(tileToReceiveFrom);
						if (isMirrored && !tileToReceiveFrom.compatibleRight.Contains(targetTile))
                            tileToReceiveFrom.compatibleRight.Add(targetTile);
					}
					else
					{
						if (targetTile.compatibleLeft.Contains(tileToReceiveFrom))
							targetTile.compatibleLeft.Remove(tileToReceiveFrom);
						if (isMirrored && tileToReceiveFrom.compatibleRight.Contains(targetTile))
                            tileToReceiveFrom.compatibleRight.Remove(targetTile);
					}
					break;

				case 3:
					if (isCompatible)
					{
						if (!targetTile.compatibleRight.Contains(tileToReceiveFrom))
							targetTile.compatibleRight.Add(tileToReceiveFrom);
						if (isMirrored && !tileToReceiveFrom.compatibleLeft.Contains(targetTile))
                            tileToReceiveFrom.compatibleLeft.Add(targetTile);
					}
					else
					{
						if (targetTile.compatibleRight.Contains(tileToReceiveFrom))
							targetTile.compatibleRight.Remove(tileToReceiveFrom);
						if (isMirrored && tileToReceiveFrom.compatibleLeft.Contains(targetTile))
                            tileToReceiveFrom.compatibleLeft.Remove(targetTile);
					}
					break;            
			}
		}

        private static void EndCenteredHorizontal()
        {
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private static void StartCenteredHorizontal()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
        }
	}
}