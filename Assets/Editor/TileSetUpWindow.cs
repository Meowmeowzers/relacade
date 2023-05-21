using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

// Based from Renaissance Coders tutorials from YT
namespace HelloWorld.Editor
{
	public class TileSetUpWindow : EditorWindow
	{
		//Left
		private TileInputSet selectedInputTileSet;
		private SerializedObject serializedTileSetObject;

		// Serialized Properties
		#region Serialized Properties
		private SerializedProperty allInput;
		private SerializedProperty foreground;
		private SerializedProperty background;
		private SerializedProperty four;
		private SerializedProperty filled;
		private SerializedProperty horizontal;
		private SerializedProperty vertical;
		private SerializedProperty edgeup;
		private SerializedProperty edgedown;
		private SerializedProperty edgeleft;
		private SerializedProperty edgeright;
		private SerializedProperty elbowUL;
		private SerializedProperty elbowUR;
		private SerializedProperty elbowDL;
		private SerializedProperty elbowDR;
		private SerializedProperty cornerUL;
		private SerializedProperty cornerUR;
		private SerializedProperty cornerDL;
		private SerializedProperty cornerDR;
		private SerializedProperty cornerULDR;
		private SerializedProperty cornerURDL;
		private SerializedProperty twoUL;
		private SerializedProperty twoUR;
		private SerializedProperty twoDL;
		private SerializedProperty twoDR;
		private SerializedProperty threeUL;
		private SerializedProperty threeUR;
		private SerializedProperty threeDL;
		private SerializedProperty threeDR;
		private SerializedProperty oneUL;
		private SerializedProperty oneUR;
		private SerializedProperty oneDL;
		private SerializedProperty oneDR;
		#endregion

		//Right
		private SerializedObject selectedTileConstraints;
		private SerializedProperty id;
		private SerializedProperty gameObject;
		private SerializedProperty compatibleTopList;
		private SerializedProperty compatibleBottomList;
		private SerializedProperty compatibleLeftList;
		private SerializedProperty compatibleRightList;
		Texture2D previewTexture;

		private string assetName = "New Tile Set Configuration Data";
		//private string savePath = "Assets/";

		private enum DirectionToSet
		{ 
			Foreground, Background, Filled, 
			EdgeUp, EdgeDown, EdgeLeft, EdgeRight,
			ElbowUpLeft, ElbowUpRight, ElbowDownLeft, ElbowDownRight, 
			CornerUpLeft, CornerUpRight, CornerDownLeft, CornerDownRight, CornerULDR, CornerURDL,
			FourFace, Vertical, Horizontal,
			TwoFaceUpLeft, TwoFaceUpRight, TwoFaceDownLeft, TwoFaceDownRight,
			ThreeFaceUp, ThreeFaceDown, ThreeFaceLeft, ThreeFaceRight,
			OneFaceUp, OneFaceDown, OneFaceLeft, OneFaceRight
		};

		#region Window Variables

		private readonly int headerSectionHeight = 35;
		private readonly int tileSetupSectionWidth = 300;

		private Texture2D headerBackgroundTexture;
		private Texture2D leftBackgroundTexture;
		private Texture2D rightBackgroundTexture;

		private Color headerBackgroundColor = new(30f / 255f, 30f / 255f, 30f / 255f, 0.5f);
		private Color leftBackgroundColor = new(30f / 255f, 30f / 255f, 30f / 255f, 0.5f);
		private Color rightBackgroundColor = new(0.6f, .2f, 0.7f, 0.7f);

		private Rect headerSection;
		private Rect tileSetupSection;
		private Rect tileConstraintSetupSection;

		private static readonly int windowMinWidth = 600;
		private static readonly int windowMinHeight = 350;

		private Vector2 scrollPositionLeft = Vector2.zero;
		private Vector2 scrollPositionRight = Vector2.zero;

		//private int newArraySize;
		//private int currentArraySize;
		private bool shouldClear = false;

		private int selectedOptionIndex = 0;
		private readonly string[] menuOptions = { "Full Combine", "2x2 Only", "3x3 Only" };
		GenericMenu menu;

		private bool[] foldout = { false, false, false, false, false, false, false, false ,false };

		#endregion Window Variables

		#region Menu items
		[MenuItem("Relacade/Tile Set Configuration Setup")]
		private static void StartWindow()
		{
			TileSetUpWindow window = (TileSetUpWindow)GetWindow(typeof(TileSetUpWindow));
			window.minSize = new(windowMinWidth, windowMinHeight);
			window.Show();
		}

		[MenuItem("Relacade/Create Asset/Tile Set Configuration")]
		private static void CreateTileSetConfiguration()
		{
			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Objects/EditorWave.prefab");

			if (prefab != null)
			{
				GameObject prefabInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
				prefabInstance.transform.SetParent(null); // Set parent to null to place in Hierarchy root
				prefabInstance.name = prefab.name + " (Instance)";
			}
			else
			{
				Debug.LogWarning("Prefab not found at path: " + "Assets / Objects / EditorWave.prefab");
			}
		}

		[MenuItem("Relacade/Create Object/WaveTileGrid")]
		private static void CreateWave()
		{
			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Objects/EditorWave.prefab");

			if (prefab != null)
			{
				GameObject prefabInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
				prefabInstance.name = prefab.name + " (Instance)";
			}
			else
			{
				Debug.LogWarning("Prefab not found at path: Assets/Objects/EditorWave.prefab");
			}
		}

		[MenuItem("Relacade/Create Asset/Input Tile")]
		private static void CreateInputTile()
		{
			ScriptableObject scriptableObject = ScriptableObject.CreateInstance<TileInput>();
			string savePath = EditorUtility.SaveFilePanelInProject("Save Scriptable Object", "Input Tile", "asset", "Choose a location to save the ScriptableObject.");
			if (string.IsNullOrEmpty(savePath)) return;
			AssetDatabase.CreateAsset(scriptableObject, savePath);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		[MenuItem("Relacade/Sample/Tile Set Configuration")]
		private static void CreateSampleTileSetConfiguration()
		{
			ScriptableObject loadedAsset;

			string assetPath = "Assets/Objects/SO Tiles/ITileSet.asset";
			loadedAsset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
			//loadedAsset = Resources.Load<ScriptableObject>(assetPath);

			if (loadedAsset == null)
			{
				Debug.Log("Fail to find asset");
				return;
			}

			ScriptableObject duplicatedObject = Instantiate(loadedAsset);

			string outputPath = EditorUtility.SaveFilePanel("Save Scriptable Object", "Sample Tile Set", "Sample Tile Set Config", "asset");

			if (string.IsNullOrEmpty(outputPath))
			{
				Debug.Log("Save operation cancelled");
				return;
			}

			outputPath = FileUtil.GetProjectRelativePath(outputPath);
			AssetDatabase.CreateAsset(duplicatedObject, outputPath);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
		#endregion

		private void OnEnable()
		{
			InitTextures();
			InitData();
		}

		private void OnGUI()
		{
			DrawLayouts();
			DrawHeader();
			DrawLeft();
			DrawRight();
			
			serializedTileSetObject?.ApplyModifiedProperties();
            // Null propagation?, it says it is used for simple null check
        }

        private void SerializeProperties()
		{
			serializedTileSetObject = new(selectedInputTileSet);
			serializedTileSetObject.Update();

			allInput = serializedTileSetObject.FindProperty("AllInputTiles");
			foreground = serializedTileSetObject.FindProperty("ForeGroundTiles");
			background = serializedTileSetObject.FindProperty("BackGroundTiles");
			filled = serializedTileSetObject.FindProperty("FourFaceTiles");
			four = serializedTileSetObject.FindProperty("FilledTiles");
			vertical = serializedTileSetObject.FindProperty("VerticalTiles");
			horizontal = serializedTileSetObject.FindProperty("HorizontalTiles");
			edgeup = serializedTileSetObject.FindProperty("EdgeUpTiles");
			edgedown = serializedTileSetObject.FindProperty("EdgeDownTiles");
			edgeleft = serializedTileSetObject.FindProperty("EdgeLeftTiles");
			edgeright = serializedTileSetObject.FindProperty("EdgeRightTiles");
			elbowUL = serializedTileSetObject.FindProperty("ElbowUpLeftTiles");
			elbowUR = serializedTileSetObject.FindProperty("ElbowUpRightTiles");
			elbowDL = serializedTileSetObject.FindProperty("ElbowDownLeftTiles");
			elbowDR = serializedTileSetObject.FindProperty("ElbowDownRightTiles");
			cornerUL = serializedTileSetObject.FindProperty("CornerUpLeftTiles");
			cornerUR = serializedTileSetObject.FindProperty("CornerUpRightTiles");
			cornerDL = serializedTileSetObject.FindProperty("CornerDownLeftTiles");
			cornerDR = serializedTileSetObject.FindProperty("CornerDownRightTiles");
			cornerULDR = serializedTileSetObject.FindProperty("CornerULDRTiles");
			cornerURDL = serializedTileSetObject.FindProperty("CornerURDLTiles");
			twoUL = serializedTileSetObject.FindProperty("TwoFaceUpLeftTiles");
			twoUR = serializedTileSetObject.FindProperty("TwoFaceUpRightTiles");
			twoDL = serializedTileSetObject.FindProperty("TwoFaceDownLeftTiles");
			twoDR = serializedTileSetObject.FindProperty("TwoFaceDownRightTiles");
			threeUL = serializedTileSetObject.FindProperty("ThreeFaceUpTiles");
			threeUR = serializedTileSetObject.FindProperty("ThreeFaceDownTiles");
			threeDL = serializedTileSetObject.FindProperty("ThreeFaceLeftTiles");
			threeDR = serializedTileSetObject.FindProperty("ThreeFaceRightTiles");
			oneUL = serializedTileSetObject.FindProperty("OneFaceUpTiles");
			oneUR = serializedTileSetObject.FindProperty("OneFaceDownTiles");
			oneDL = serializedTileSetObject.FindProperty("OneFaceLeftTiles");
			oneDR = serializedTileSetObject.FindProperty("OneFaceRightTiles");
		}

		private void InitTextures()
		{
			headerBackgroundTexture = new Texture2D(1, 1);
			headerBackgroundTexture.SetPixel(0, 0, headerBackgroundColor);
			headerBackgroundTexture.Apply();

			leftBackgroundTexture = new Texture2D(1, 1);
			leftBackgroundTexture.SetPixel(0, 0, leftBackgroundColor);
			leftBackgroundTexture.Apply();

			rightBackgroundTexture = new Texture2D(1, 1);
			rightBackgroundTexture.SetPixel(0, 0, rightBackgroundColor);
			rightBackgroundTexture.Apply();
		}

		private void InitData()
		{
			if(selectedInputTileSet != null)
			{
				SerializeProperties();
			}
		}

		private void DrawLayouts()
		{
			headerSection.x = 0;
			headerSection.y = 0;
			headerSection.width = Screen.width;
			headerSection.height = headerSectionHeight;
			
			tileSetupSection.x = 0;
			tileSetupSection.y = headerSectionHeight;
			tileSetupSection.width = tileSetupSectionWidth;
			tileSetupSection.height = Screen.height - headerSectionHeight;

			tileConstraintSetupSection.x = tileSetupSectionWidth;
			tileConstraintSetupSection.y = headerSectionHeight;
			tileConstraintSetupSection.width = Screen.width - tileSetupSectionWidth;
			tileConstraintSetupSection.height = Screen.height - headerSectionHeight;

			GUI.DrawTexture(headerSection, headerBackgroundTexture);
			GUI.DrawTexture(tileSetupSection, leftBackgroundTexture);
			//GUI.DrawTexture(tileConstraintSetupSection, rightBackgroundTexture);
		}

		private void DrawHeader()
		{
			GUILayout.BeginArea(headerSection);
			EditorGUILayout.Space(3);
			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			EditorGUILayout.LabelField("Tile Set Config", GUILayout.Width(100));
			selectedInputTileSet = (TileInputSet)EditorGUILayout.ObjectField(selectedInputTileSet, typeof(TileInputSet), false, GUILayout.MaxWidth(300));

			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Auto Generate", EditorStyles.toolbarButton, GUILayout.MaxWidth(200)))
			{
				if (selectedInputTileSet != null /*& serializedTileSetObject != null*/)
				{
					AutoGenerateDirect();
				}
			}
			if (GUILayout.Button("Options", EditorStyles.toolbarDropDown, GUILayout.Width(60)))
			{
				menu = new GenericMenu();

				for (int i = 0; i < menuOptions.Length; i++)
				{
					int optionIndex = i; // Store the index in a separate variable to use in the delegate

					menu.AddItem(new GUIContent(menuOptions[i]), selectedOptionIndex == i, () =>
					{
						selectedOptionIndex = optionIndex;
						Repaint();
					});
				}

				menu.ShowAsContext();
			}

			shouldClear = EditorGUILayout.ToggleLeft("Clear?", shouldClear, GUILayout.Width(60));

			if (GUILayout.Button("Clear", GUILayout.MaxWidth(200)))
			{
				if (selectedInputTileSet != null && serializedTileSetObject != null && shouldClear)
				{
					Debug.Log("Cleared");
					ClearAllInputTileConstraints();
				}
			}

			EditorGUILayout.EndHorizontal();

			GUILayout.EndArea();
		}

		private void AutoGenerateDirect()
		{
			#region Four
			for (int i = 0; i < selectedInputTileSet.ForeGroundTiles.Count; i++)
			{
				selectedInputTileSet.ForeGroundTiles[i] = MainCombine(selectedInputTileSet.ForeGroundTiles[i], selectedInputTileSet, DirectionToSet.Foreground);
			}
			for (int i = 0; i < selectedInputTileSet.BackGroundTiles.Count; i++)
			{
				selectedInputTileSet.BackGroundTiles[i] = MainCombine(selectedInputTileSet.BackGroundTiles[i], selectedInputTileSet, DirectionToSet.Background);
			}
			for (int i = 0; i < selectedInputTileSet.FourFaceTiles.Count; i++)
			{
				selectedInputTileSet.FourFaceTiles[i] = MainCombine(selectedInputTileSet.FourFaceTiles[i], selectedInputTileSet, DirectionToSet.FourFace);
			}
			for (int i = 0; i < selectedInputTileSet.FilledTiles.Count; i++)
			{
				selectedInputTileSet.FilledTiles[i] = MainCombine(selectedInputTileSet.FilledTiles[i], selectedInputTileSet, DirectionToSet.Filled);
			}
			#endregion

			#region Edge
			for (int i = 0; i < selectedInputTileSet.EdgeUpTiles.Count; i++)
			{
				selectedInputTileSet.EdgeUpTiles[i] = MainCombine(selectedInputTileSet.EdgeUpTiles[i], selectedInputTileSet, DirectionToSet.EdgeUp);
			}
			for (int i = 0; i < selectedInputTileSet.EdgeDownTiles.Count; i++)
			{
				selectedInputTileSet.EdgeDownTiles[i] = MainCombine(selectedInputTileSet.EdgeDownTiles[i], selectedInputTileSet, DirectionToSet.EdgeDown);
			}
			for (int i = 0; i < selectedInputTileSet.EdgeLeftTiles.Count; i++)
			{
				selectedInputTileSet.EdgeLeftTiles[i] = MainCombine(selectedInputTileSet.EdgeLeftTiles[i], selectedInputTileSet, DirectionToSet.EdgeLeft);
			}
			for (int i = 0; i < selectedInputTileSet.EdgeRightTiles.Count; i++)
			{
				selectedInputTileSet.EdgeRightTiles[i] = MainCombine(selectedInputTileSet.EdgeRightTiles[i], selectedInputTileSet, DirectionToSet.EdgeRight);
			}
			#endregion

			#region Elbow
			for (int i = 0; i < selectedInputTileSet.ElbowUpLeftTiles.Count; i++)
			{
				selectedInputTileSet.ElbowUpLeftTiles[i] = MainCombine(selectedInputTileSet.ElbowUpLeftTiles[i], selectedInputTileSet, DirectionToSet.ElbowUpLeft);
			}
			for (int i = 0; i < selectedInputTileSet.ElbowUpRightTiles.Count; i++)
			{
				selectedInputTileSet.ElbowUpRightTiles[i] = MainCombine(selectedInputTileSet.ElbowUpRightTiles[i], selectedInputTileSet, DirectionToSet.ElbowUpRight);
			}
			for (int i = 0; i < selectedInputTileSet.ElbowDownLeftTiles.Count; i++)
			{
				selectedInputTileSet.ElbowDownLeftTiles[i] = MainCombine(selectedInputTileSet.ElbowDownLeftTiles[i], selectedInputTileSet, DirectionToSet.ElbowDownLeft);
			}
			for (int i = 0; i < selectedInputTileSet.ElbowDownRightTiles.Count; i++)
			{
				selectedInputTileSet.ElbowDownRightTiles[i] = MainCombine(selectedInputTileSet.ElbowDownRightTiles[i], selectedInputTileSet, DirectionToSet.ElbowDownRight);
			}
			#endregion

			#region Corner
			for (int i = 0; i < selectedInputTileSet.CornerUpRightTiles.Count; i++)
			{
				selectedInputTileSet.CornerUpRightTiles[i] = MainCombine(selectedInputTileSet.CornerUpRightTiles[i], selectedInputTileSet, DirectionToSet.CornerUpRight);
			}
			for (int i = 0; i < selectedInputTileSet.CornerUpLeftTiles.Count; i++)
			{
				selectedInputTileSet.CornerUpLeftTiles[i] = MainCombine(selectedInputTileSet.CornerUpLeftTiles[i], selectedInputTileSet, DirectionToSet.CornerUpLeft);
			}
			for (int i = 0; i < selectedInputTileSet.CornerDownLeftTiles.Count; i++)
			{
				selectedInputTileSet.CornerDownLeftTiles[i] = MainCombine(selectedInputTileSet.CornerDownLeftTiles[i], selectedInputTileSet, DirectionToSet.CornerDownLeft);
			}
			for (int i = 0; i < selectedInputTileSet.CornerDownRightTiles.Count; i++)
			{
				selectedInputTileSet.CornerDownRightTiles[i] = MainCombine(selectedInputTileSet.CornerDownRightTiles[i], selectedInputTileSet, DirectionToSet.CornerDownRight);
			}
			for (int i = 0; i < selectedInputTileSet.CornerULDRTiles.Count; i++)
			{
				selectedInputTileSet.CornerULDRTiles[i] = MainCombine(selectedInputTileSet.CornerULDRTiles[i], selectedInputTileSet, DirectionToSet.CornerULDR);
			}
			for (int i = 0; i < selectedInputTileSet.CornerURDLTiles.Count; i++)
			{
				selectedInputTileSet.CornerURDLTiles[i] = MainCombine(selectedInputTileSet.CornerURDLTiles[i], selectedInputTileSet, DirectionToSet.CornerURDL);
			}
			#endregion

			#region ThreeFace
			for (int i = 0; i < selectedInputTileSet.ThreeFaceUpTiles.Count; i++)
			{
				selectedInputTileSet.ThreeFaceUpTiles[i] = MainCombine(selectedInputTileSet.ThreeFaceUpTiles[i], selectedInputTileSet, DirectionToSet.ThreeFaceUp);
			}
			for (int i = 0; i < selectedInputTileSet.ThreeFaceDownTiles.Count; i++)
			{
				selectedInputTileSet.ThreeFaceDownTiles[i] = MainCombine(selectedInputTileSet.ThreeFaceDownTiles[i], selectedInputTileSet, DirectionToSet.ThreeFaceDown);
			}
			for (int i = 0; i < selectedInputTileSet.ThreeFaceLeftTiles.Count; i++)
			{
				selectedInputTileSet.ThreeFaceLeftTiles[i] = MainCombine(selectedInputTileSet.ThreeFaceLeftTiles[i], selectedInputTileSet, DirectionToSet.ThreeFaceLeft);
			}
			for (int i = 0; i < selectedInputTileSet.ThreeFaceRightTiles.Count; i++)
			{
				selectedInputTileSet.ThreeFaceRightTiles[i] = MainCombine(selectedInputTileSet.ThreeFaceRightTiles[i], selectedInputTileSet, DirectionToSet.ThreeFaceRight);
			}
			#endregion

			#region TwoFace / Curved
			for (int i = 0; i < selectedInputTileSet.TwoFaceUpLeftTiles.Count; i++)
			{
				selectedInputTileSet.TwoFaceUpLeftTiles[i] = MainCombine(selectedInputTileSet.TwoFaceUpLeftTiles[i], selectedInputTileSet, DirectionToSet.TwoFaceUpLeft);
			}
			for (int i = 0; i < selectedInputTileSet.TwoFaceUpRightTiles.Count; i++)
			{
				selectedInputTileSet.TwoFaceUpRightTiles[i] = MainCombine(selectedInputTileSet.TwoFaceUpRightTiles[i], selectedInputTileSet, DirectionToSet.TwoFaceUpRight);
			}
			for (int i = 0; i < selectedInputTileSet.TwoFaceDownLeftTiles.Count; i++)
			{
				selectedInputTileSet.TwoFaceDownLeftTiles[i] = MainCombine(selectedInputTileSet.TwoFaceDownLeftTiles[i], selectedInputTileSet, DirectionToSet.TwoFaceDownLeft);
			}
			for (int i = 0; i < selectedInputTileSet.TwoFaceDownRightTiles.Count; i++)
			{
				selectedInputTileSet.TwoFaceDownRightTiles[i] = MainCombine(selectedInputTileSet.TwoFaceDownRightTiles[i], selectedInputTileSet, DirectionToSet.TwoFaceDownRight);
			}
			#endregion

			#region OneFace
			for (int i = 0; i < selectedInputTileSet.OneFaceUpTiles.Count; i++)
			{
				selectedInputTileSet.OneFaceUpTiles[i] = MainCombine(selectedInputTileSet.OneFaceUpTiles[i], selectedInputTileSet, DirectionToSet.OneFaceUp);
			}
			for (int i = 0; i < selectedInputTileSet.OneFaceDownTiles.Count; i++)
			{
				selectedInputTileSet.OneFaceDownTiles[i] = MainCombine(selectedInputTileSet.OneFaceDownTiles[i], selectedInputTileSet, DirectionToSet.OneFaceDown);
			}
			for (int i = 0; i < selectedInputTileSet.OneFaceLeftTiles.Count; i++)
			{
				selectedInputTileSet.OneFaceLeftTiles[i] = MainCombine(selectedInputTileSet.OneFaceLeftTiles[i], selectedInputTileSet, DirectionToSet.OneFaceLeft);
			}
			for (int i = 0; i < selectedInputTileSet.OneFaceRightTiles.Count; i++)
			{
				selectedInputTileSet.OneFaceRightTiles[i] = MainCombine(selectedInputTileSet.OneFaceRightTiles[i], selectedInputTileSet, DirectionToSet.OneFaceRight);
			}
			#endregion

			#region VH
			for (int i = 0; i < selectedInputTileSet.VerticalTiles.Count; i++)
			{
				selectedInputTileSet.VerticalTiles[i] = MainCombine(selectedInputTileSet.VerticalTiles[i], selectedInputTileSet, DirectionToSet.Vertical);
			}
			for (int i = 0; i < selectedInputTileSet.HorizontalTiles.Count; i++)
			{
				selectedInputTileSet.HorizontalTiles[i] = MainCombine(selectedInputTileSet.HorizontalTiles[i], selectedInputTileSet, DirectionToSet.Horizontal);
			}
			#endregion

			GetAllUniqueInputTiles();
			GiveUniqueIDToTiles();
		}

		private TileInput MainCombine(TileInput tileInput, TileInputSet set, DirectionToSet direction)
		{
			switch (direction)
			{
				#region Four/Full/Filled
				case DirectionToSet.Foreground:

                    #region ForeGround

                    #region 2x2
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.ForeGroundTiles,
						set.EdgeDownTiles,
						set.ElbowDownRightTiles
					);
					
					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.ForeGroundTiles,
						set.EdgeUpTiles,
						set.ElbowUpLeftTiles,
						set.ElbowUpRightTiles
					);
					
					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.ForeGroundTiles,
						set.EdgeRightTiles,
						set.ElbowUpRightTiles,
						set.ElbowDownRightTiles
					);
					
					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.ForeGroundTiles,
						set.EdgeLeftTiles,
						set.ElbowUpLeftTiles,
						set.ElbowDownLeftTiles
					);
                    #endregion
                    #region 3x3
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.FilledTiles,
						set.HorizontalTiles,
						set.OneFaceUpTiles,
						set.OneFaceLeftTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.ThreeFaceUpTiles
					);

                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.HorizontalTiles,
						set.OneFaceDownTiles,
						set.OneFaceLeftTiles,
						set.OneFaceRightTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceDownTiles
					);

                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles,
						set.OneFaceDownTiles,
						set.OneFaceLeftTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceDownLeftTiles,
						set.ThreeFaceLeftTiles
					);

                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles,
						set.OneFaceDownTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpRightTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceRightTiles
					);
                    #endregion

                    break;

					#endregion 

				case DirectionToSet.Background:

					#region Background

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.BackGroundTiles,
						set.EdgeUpTiles,
						set.CornerUpLeftTiles,
						set.CornerUpRightTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.BackGroundTiles,
						set.EdgeDownTiles,
						set.CornerDownLeftTiles,
						set.CornerDownRightTiles
					);
					
					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.BackGroundTiles,
						set.EdgeLeftTiles,
						set.CornerUpLeftTiles,
						set.CornerDownLeftTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.BackGroundTiles,
						set.EdgeRightTiles,
						set.CornerUpRightTiles,
						set.CornerDownRightTiles
					);

					#endregion

					break;

				case DirectionToSet.Filled:

                    #region Filled
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.FilledTiles,
						set.ForeGroundTiles,
						set.FourFaceTiles,
						set.VerticalTiles,
						set.HorizontalTiles,
						set.OneFaceUpTiles,
						set.OneFaceDownTiles,
						set.OneFaceLeftTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceLeftTiles,
						set.ThreeFaceRightTiles
					);

                    tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.FilledTiles,
						set.ForeGroundTiles,
						set.FourFaceTiles,
						set.VerticalTiles,
						set.HorizontalTiles,
						set.OneFaceUpTiles,
						set.OneFaceDownTiles,
						set.OneFaceLeftTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceLeftTiles,
						set.ThreeFaceRightTiles
					);

                    tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.FilledTiles,
						set.ForeGroundTiles,
						set.FourFaceTiles,
						set.VerticalTiles,
						set.HorizontalTiles,
						set.OneFaceUpTiles,
						set.OneFaceDownTiles,
						set.OneFaceLeftTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceLeftTiles,
						set.ThreeFaceRightTiles
					);

                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.FilledTiles,
						set.ForeGroundTiles,
						set.FourFaceTiles,
						set.VerticalTiles,
						set.HorizontalTiles,
						set.OneFaceUpTiles,
						set.OneFaceDownTiles,
						set.OneFaceLeftTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceLeftTiles,
						set.ThreeFaceRightTiles
					);

                    #endregion
                    break;
				
				case DirectionToSet.FourFace:

					#region FourFace
					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, set.FilledTiles,
						set.FourFaceTiles,
						set.VerticalTiles,
						set.OneFaceDownTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceLeftTiles,
						set.ThreeFaceRightTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, set.FilledTiles,
						set.FourFaceTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceLeftTiles,
						set.ThreeFaceRightTiles);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, set.FilledTiles,
						set.FourFaceTiles,
						set.HorizontalTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpRightTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceRightTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, set.FilledTiles,
						set.FourFaceTiles,
						set.HorizontalTiles,
						set.OneFaceLeftTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceDownLeftTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceLeftTiles
					);
					#endregion
					break;

				#endregion

				#region Edge
				case DirectionToSet.EdgeUp:

					#region Top
					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.ForeGroundTiles,
						set.EdgeDownTiles,
						set.ElbowDownLeftTiles,
						set.ElbowDownRightTiles
					);
					
					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.BackGroundTiles,
						set.EdgeDownTiles,
						set.CornerDownLeftTiles,
						set.CornerDownRightTiles
					);
					
					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.EdgeUpTiles,
						set.CornerUpRightTiles,
						set.ElbowUpLeftTiles,
						set.CornerURDLTiles
					);
					
					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.EdgeUpTiles,
						set.CornerUpLeftTiles,
						set.ElbowUpRightTiles,
						set.CornerULDRTiles
					);
				#endregion Top
					break;


				case DirectionToSet.EdgeDown:

					#region Bottom

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.BackGroundTiles,
						set.EdgeUpTiles,
						set.CornerUpLeftTiles,
						set.CornerUpRightTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.ForeGroundTiles,
						set.EdgeUpTiles,
						set.ElbowUpLeftTiles,
						set.ElbowUpRightTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.EdgeDownTiles,
						set.CornerDownRightTiles,
						set.ElbowDownLeftTiles,
						set.CornerULDRTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.EdgeDownTiles,
						set.CornerDownLeftTiles,
						set.ElbowDownRightTiles,
						set.CornerURDLTiles
					);

					#endregion Bottom

					break;

				case DirectionToSet.EdgeLeft:

					#region Left

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.EdgeLeftTiles,
						set.CornerDownLeftTiles,
						set.ElbowUpLeftTiles,
						set.CornerURDLTiles
					);
					
					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.EdgeLeftTiles,
						set.CornerUpLeftTiles,
						set.ElbowDownLeftTiles,
						set.CornerULDRTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.ForeGroundTiles,
						set.EdgeRightTiles,
						set.ElbowUpRightTiles,
						set.ElbowDownRightTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.BackGroundTiles,
						set.EdgeRightTiles,
						set.CornerUpRightTiles,
						set.CornerDownRightTiles
					);

					#endregion Left

					break;

				case DirectionToSet.EdgeRight:

					#region Right

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.EdgeRightTiles,
						set.CornerDownRightTiles,
						set.ElbowUpRightTiles,
						set.CornerULDRTiles
					);
					
					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.EdgeRightTiles,
						set.CornerUpRightTiles,
						set.ElbowDownRightTiles,
						set.CornerURDLTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.BackGroundTiles,
						set.EdgeLeftTiles,
						set.CornerDownLeftTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.ForeGroundTiles,
						set.EdgeLeftTiles,
						set.ElbowUpLeftTiles,
						set.ElbowDownLeftTiles
					);

					#endregion Right

					break;

				#endregion

				#region Elbow
				case DirectionToSet.ElbowUpLeft:

					#region TopLeft

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.ForeGroundTiles,
						set.EdgeDownTiles,
						set.ElbowDownLeftTiles,
						set.ElbowDownRightTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.EdgeLeftTiles,
						set.CornerUpLeftTiles,
						set.ElbowDownLeftTiles,
						set.CornerULDRTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.ForeGroundTiles,
						set.EdgeRightTiles,
						set.ElbowUpRightTiles,
						set.ElbowDownRightTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.EdgeUpTiles,
						set.CornerUpLeftTiles,
						set.ElbowUpRightTiles,
						set.CornerULDRTiles
					);

					#endregion TopLeft

					break;

				case DirectionToSet.ElbowUpRight:

					#region TopRight

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.ForeGroundTiles,
						set.EdgeDownTiles,
						set.ElbowDownLeftTiles,
						set.ElbowDownRightTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.EdgeRightTiles,
						set.CornerUpRightTiles,
						set.ElbowDownRightTiles,
						set.CornerURDLTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.EdgeUpTiles,
						set.CornerUpRightTiles,
						set.ElbowUpLeftTiles,
						set.CornerURDLTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.ForeGroundTiles,
						set.EdgeLeftTiles,
						set.ElbowUpLeftTiles,
						set.ElbowDownLeftTiles
					);

					#endregion TopRight

					break;

				case DirectionToSet.ElbowDownLeft:

					#region BottomLeft

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.EdgeLeftTiles,
						set.CornerDownLeftTiles,
						set.ElbowUpLeftTiles,
						set.CornerURDLTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.ForeGroundTiles,
						set.EdgeUpTiles,
						set.ElbowUpLeftTiles,
						set.ElbowUpRightTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.ForeGroundTiles,
						set.EdgeRightTiles,
						set.ElbowUpRightTiles,
						set.ElbowDownRightTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.EdgeDownTiles,
						set.CornerDownLeftTiles,
						set.ElbowDownRightTiles,
						set.CornerURDLTiles
					);

					#endregion BottomLeft

					break;

				case DirectionToSet.ElbowDownRight:

					#region BottomRight

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.EdgeRightTiles,
						set.CornerDownRightTiles,
						set.ElbowUpRightTiles,
						set.CornerULDRTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.ForeGroundTiles,
						set.EdgeUpTiles,
						set.ElbowUpLeftTiles,
						set.ElbowUpRightTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.EdgeDownTiles,
						set.CornerDownRightTiles,
						set.ElbowDownLeftTiles,
						set.CornerULDRTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.ForeGroundTiles,
						set.EdgeLeftTiles,
						set.ElbowUpLeftTiles,
						set.ElbowDownLeftTiles
					);

					#endregion BottomRight

					break;

				#endregion

				#region Corner
				case DirectionToSet.CornerUpLeft:

					#region ITopLeft

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.EdgeLeftTiles,
						set.ElbowUpLeftTiles,
						set.CornerDownLeftTiles,
						set.CornerURDLTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.BackGroundTiles,
						set.EdgeDownTiles,
						set.CornerDownLeftTiles,
						set.CornerDownRightTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.EdgeUpTiles,
						set.CornerUpRightTiles,
						set.ElbowUpLeftTiles,
						set.CornerURDLTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.BackGroundTiles,
						set.EdgeRightTiles,
						set.CornerUpRightTiles,
						set.CornerDownRightTiles
					);

					#endregion ITopLeft

					break;

				case DirectionToSet.CornerUpRight:

					#region ITopRight

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.EdgeRightTiles,
						set.ElbowUpRightTiles,
						set.CornerDownRightTiles,
						set.CornerULDRTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.BackGroundTiles,
						set.EdgeDownTiles,
						set.CornerDownRightTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.BackGroundTiles,
						set.EdgeLeftTiles,
						set.CornerUpLeftTiles,
						set.CornerDownLeftTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.EdgeUpTiles,
						set.CornerUpLeftTiles,
						set.ElbowUpRightTiles,
						set.CornerULDRTiles
					);

					#endregion ITopRight

					break;

				case DirectionToSet.CornerDownLeft:

					#region IBottomLeft

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.BackGroundTiles,
						set.EdgeUpTiles,
						set.CornerUpLeftTiles,
						set.CornerUpRightTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.EdgeLeftTiles,
						set.CornerUpLeftTiles,
						set.ElbowDownLeftTiles,
						set.CornerULDRTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.EdgeDownTiles,
						set.ElbowDownLeftTiles,
						set.CornerDownRightTiles,
						set.CornerULDRTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.BackGroundTiles,
						set.EdgeRightTiles,
						set.CornerUpRightTiles,
						set.CornerDownRightTiles
					);

					#endregion IBottomLeft

					break;

				case DirectionToSet.CornerDownRight:

					#region IBottomRight

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.BackGroundTiles,
						set.EdgeUpTiles,
						set.CornerUpRightTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.EdgeRightTiles,
						set.CornerUpRightTiles,
						set.ElbowDownRightTiles,
						set.CornerURDLTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.BackGroundTiles,
						set.EdgeLeftTiles,
						set.CornerUpLeftTiles,
						set.CornerDownLeftTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.EdgeDownTiles,
						set.CornerDownLeftTiles,
						set.ElbowDownRightTiles,
						set.CornerURDLTiles
					);

					#endregion IBottomRight

					break;

				case DirectionToSet.CornerULDR:
					
					#region CornerULDR
					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.EdgeLeftTiles,
						set.CornerDownLeftTiles,
						set.ElbowUpLeftTiles,
						set.CornerURDLTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.EdgeRightTiles,
						set.CornerUpLeftTiles,
						set.ElbowDownRightTiles,
						set.CornerURDLTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.EdgeUpTiles,
						set.CornerUpLeftTiles,
						set.ElbowUpLeftTiles,
						set.CornerURDLTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.EdgeDownTiles,
						set.CornerDownLeftTiles,
						set.ElbowDownRightTiles,
						set.CornerURDLTiles
					);
					#endregion

					break;
				case DirectionToSet.CornerURDL:

					#region CornerULDR
					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.EdgeRightTiles,
						set.CornerDownRightTiles,
						set.ElbowUpRightTiles,
						set.CornerULDRTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.EdgeLeftTiles,
						set.CornerUpLeftTiles,
						set.ElbowDownLeftTiles,
						set.CornerULDRTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.EdgeDownTiles,
						set.CornerDownRightTiles,
						set.ElbowDownLeftTiles,
						set.CornerULDRTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.EdgeUpTiles,
						set.CornerUpLeftTiles,
						set.CornerUpRightTiles,
						set.CornerULDRTiles
					);
					#endregion

					break;
				#endregion

				#region Three
				case DirectionToSet.ThreeFaceUp:

					#region ThreeUp

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.VerticalTiles,
						set.OneFaceDownTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceLeftTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.ForeGroundTiles,
						set.FilledTiles,
						set.HorizontalTiles,
						set.OneFaceDownTiles,
						set.OneFaceLeftTiles,
						set.OneFaceRightTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceDownTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.HorizontalTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpRightTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceRightTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.HorizontalTiles,
						set.OneFaceLeftTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceDownLeftTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceLeftTiles
					);

					#endregion

					break;
				case DirectionToSet.ThreeFaceDown:

					#region ThreeDown

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.HorizontalTiles,
						set.OneFaceUpTiles,
						set.OneFaceLeftTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.ThreeFaceUpTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.VerticalTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceLeftTiles,
						set.ThreeFaceRightTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.HorizontalTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpRightTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceRightTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.HorizontalTiles,
						set.OneFaceLeftTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceUpLeftTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceLeftTiles
					);

					#endregion

					break;
				case DirectionToSet.ThreeFaceLeft:

					#region ThreeLeft

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.VerticalTiles,
						set.OneFaceDownTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceLeftTiles,
						set.ThreeFaceRightTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceLeftTiles,
						set.ThreeFaceRightTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.HorizontalTiles,
						set.OneFaceRightTiles,
						set.TwoFaceDownRightTiles,
						set.TwoFaceUpRightTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceRightTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles,
						set.OneFaceDownTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpRightTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceRightTiles
					);

					#endregion

					break;
				case DirectionToSet.ThreeFaceRight:

					#region ThreeRight
					
					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.VerticalTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceLeftTiles,
						set.ThreeFaceRightTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceLeftTiles,
						set.ThreeFaceRightTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles,
						set.OneFaceDownTiles,
						set.OneFaceLeftTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceUpLeftTiles,
						set.ThreeFaceLeftTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.HorizontalTiles,
						set.OneFaceLeftTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceUpLeftTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceLeftTiles
					);

					#endregion

					break;
				#endregion

				#region Two
				case DirectionToSet.TwoFaceUpLeft:

					#region TwoUpLeft

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.VerticalTiles,
						set.OneFaceDownTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceLeftTiles,
						set.ThreeFaceRightTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.HorizontalTiles,
						set.OneFaceDownTiles,
						set.OneFaceLeftTiles,
						set.OneFaceRightTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceDownTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.HorizontalTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpRightTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceRightTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles,
						set.OneFaceDownTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpRightTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceRightTiles
					);

					#endregion
					break;

				case DirectionToSet.TwoFaceUpRight:

					#region TwoUpRight

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.VerticalTiles,
						set.OneFaceDownTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceLeftTiles,
						set.ThreeFaceRightTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.HorizontalTiles,
						set.OneFaceDownTiles,
						set.OneFaceLeftTiles,
						set.OneFaceRightTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceDownTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles,
						set.OneFaceDownTiles,
						set.OneFaceLeftTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceDownLeftTiles,
						set.ThreeFaceLeftTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.HorizontalTiles,
						set.OneFaceLeftTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceDownLeftTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceLeftTiles
					);

					#endregion
					break;

				case DirectionToSet.TwoFaceDownLeft:

					#region TwoDownLeft

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.OneFaceUpTiles,
						set.OneFaceLeftTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.ThreeFaceUpTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.ThreeFaceUpTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.HorizontalTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpRightTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceRightTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles,
						set.OneFaceDownTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpRightTiles,
						set.ThreeFaceRightTiles
					);

					#endregion
					break;

				case DirectionToSet.TwoFaceDownRight:

					#region TwoDownRight

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.HorizontalTiles,
						set.OneFaceUpTiles,
						set.OneFaceLeftTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.ThreeFaceUpTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceLeftTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles,
						set.OneFaceDownTiles,
						set.OneFaceLeftTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceDownLeftTiles,
						set.ThreeFaceLeftTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.HorizontalTiles,
						set.OneFaceLeftTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceDownLeftTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceLeftTiles
					);

					#endregion
					break;

				#endregion

				#region One
				case DirectionToSet.OneFaceUp:

					#region OneUp

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.VerticalTiles,
						set.OneFaceDownTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceLeftTiles,
						set.ThreeFaceRightTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.HorizontalTiles,
						set.OneFaceDownTiles,
						set.OneFaceLeftTiles,
						set.OneFaceRightTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceDownTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles,
						set.OneFaceDownTiles,
						set.OneFaceLeftTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceDownLeftTiles,
						set.ThreeFaceLeftTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles,
						set.OneFaceDownTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpRightTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceRightTiles
					);

					#endregion

					break;
				case DirectionToSet.OneFaceDown:

					#region OneDown

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.HorizontalTiles,
						set.OneFaceUpTiles,
						set.OneFaceLeftTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.ThreeFaceUpTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceLeftTiles,
						set.ThreeFaceRightTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles,
						set.OneFaceDownTiles,
						set.OneFaceLeftTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceDownLeftTiles,
						set.ThreeFaceLeftTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles,
						set.OneFaceDownTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpRightTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceRightTiles
					);

					#endregion

					break;
				case DirectionToSet.OneFaceLeft:

					#region OneLeft

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.HorizontalTiles,
						set.OneFaceUpTiles,
						set.OneFaceLeftTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.ThreeFaceUpTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.HorizontalTiles,
						set.OneFaceDownTiles,
						set.OneFaceLeftTiles,
						set.OneFaceRightTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceDownTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.HorizontalTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpRightTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceRightTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles,
						set.OneFaceDownTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpRightTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceRightTiles
					);


					#endregion

					break;
				case DirectionToSet.OneFaceRight:

					#region OneRight

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.HorizontalTiles,
						set.OneFaceUpTiles,
						set.OneFaceLeftTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.ThreeFaceUpTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.HorizontalTiles,
						set.OneFaceDownTiles,
						set.OneFaceLeftTiles,
						set.OneFaceRightTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceDownTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles,
						set.OneFaceDownTiles,
						set.OneFaceLeftTiles,
						set.TwoFaceDownLeftTiles,
						set.ThreeFaceLeftTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.HorizontalTiles,
						set.OneFaceLeftTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceDownLeftTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceLeftTiles
					);

					#endregion

					break;
				#endregion

				#region VH
				case DirectionToSet.Vertical:

                    #region Vertical
                    tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.VerticalTiles,
						set.OneFaceDownTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceLeftTiles,
						set.ThreeFaceRightTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceLeftTiles,
						set.ThreeFaceRightTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles,
						set.OneFaceDownTiles,
						set.OneFaceLeftTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceDownLeftTiles,
						set.ThreeFaceLeftTiles
					);

                    tileInput.compatibleRight = SubCombine(tileInput.compatibleRight,
                        set.FilledTiles,
                        set.ForeGroundTiles,
                        set.VerticalTiles,
                        set.OneFaceUpTiles,
                        set.OneFaceDownTiles,
                        set.OneFaceRightTiles,
                        set.TwoFaceUpRightTiles,
                        set.TwoFaceDownRightTiles,
                        set.ThreeFaceRightTiles
					);

					#endregion

					break;
				case DirectionToSet.Horizontal:

					#region Horizontal
					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.HorizontalTiles,
						set.OneFaceUpTiles,
						set.OneFaceLeftTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.ThreeFaceUpTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, 
						set.FilledTiles,
						set.ForeGroundTiles,
						set.HorizontalTiles,
						set.OneFaceDownTiles,
						set.OneFaceLeftTiles,
						set.OneFaceRightTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceDownTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.HorizontalTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpRightTiles,
						set.TwoFaceDownRightTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceRightTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, 
						set.FilledTiles,
						set.FourFaceTiles,
						set.HorizontalTiles,
						set.OneFaceLeftTiles,
						set.TwoFaceDownLeftTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceLeftTiles
					);

					#endregion

					break;
				#endregion

				default:
					break;
			}
			return tileInput;
        }

		private List<TileInput> SubCombine(List<TileInput> tileConstraintsList, params List<TileInput>[] tilesToAdd)
		{
			foreach (var set in tilesToAdd)
			{
				foreach (var item in set)
				{
					if (!tileConstraintsList.Contains(item))
					{
						tileConstraintsList.Add(item);
					}
				}

			}
			return tileConstraintsList;
		}

		private void DrawLeft()
		{
			GUILayout.BeginArea(tileSetupSection);
			scrollPositionLeft = EditorGUILayout.BeginScrollView(scrollPositionLeft, false, true);

			EditorGUILayout.BeginVertical(GUILayout.MaxWidth(280));
			ButtonCategory("All Tiles", 0);
			ShowTileSets(foldout[0], allInput);

			ButtonCategory("Filled Tiles", 1);
			ShowTileSets(foldout[1], foreground, background, four, filled);

			ButtonCategory("Edge Tiles", 2);
			ShowTileSets(foldout[2], edgeup, edgedown, edgeleft, edgeright);

			ButtonCategory("Elbow Tiles", 3);
			ShowTileSets(foldout[3], elbowUL, elbowUR, elbowDL, elbowDR);

			ButtonCategory("Corner Tiles", 4);
			ShowTileSets(foldout[4], cornerUL, cornerUR, cornerDL, cornerDR, cornerULDR, cornerURDL);

			ButtonCategory("Vertical / Horizontal Tiles", 5);
			ShowTileSets(foldout[5], vertical, horizontal);

			ButtonCategory("One Face Tiles", 6);
			ShowTileSets(foldout[6], oneUL, oneUR, oneDL, oneDR);

			ButtonCategory("Two Face / Curved Tiles", 7);
			ShowTileSets(foldout[7], twoUL, twoUR, twoDL, twoDR);

			ButtonCategory("Three Face Tiles", 8);
			ShowTileSets(foldout[8], threeUL, threeUR, threeDL, threeDR);

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndScrollView();
			EditorGUILayout.Space(20);
			GUILayout.EndArea();
		}
		private void ButtonCategory(string name, int show)
		{
			if (GUILayout.Button(name, GUILayout.Height(30)))
			{
				SerializeProperties();
				foldout[show] = !foldout[show];
			}
		}

		private void ShowTileSets(bool showTileset, params SerializedProperty[] property)
		{
			if (showTileset)
			{
				foreach(var item in property)
				{
					EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(270));

					EditorGUILayout.LabelField(item.name, EditorStyles.largeLabel, GUILayout.MaxWidth(190));

					// Array size field
					int currentArraySize = item.arraySize;
					int newArraySize = EditorGUILayout.IntField(currentArraySize, GUILayout.Width(40));
					if (newArraySize != currentArraySize)
					{
						item.arraySize = newArraySize;
					}

					// Plus button
					if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(20), GUILayout.Height(20)))
					{
						item.arraySize++;
					}

					// Minus button
					if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.Width(20)))
					{
						if (item.arraySize > 0)
						{
							item.arraySize--;
						}
					}
					GUILayout.Space(10);
					EditorGUILayout.EndHorizontal();
					
					EditorGUILayout.Space(5);

					for (int i = 0; i < item.arraySize; i++)
					{
						SerializedProperty elementProperty = item.GetArrayElementAtIndex(i);

						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField($"Item {i + 1}", EditorStyles.miniLabel,GUILayout.MaxWidth(65));
						EditorGUILayout.PropertyField(elementProperty, GUIContent.none, GUILayout.MaxWidth(175));

						if (GUILayout.Button(">", EditorStyles.toolbarButton ,GUILayout.Width(30)))
						{
							if (elementProperty.objectReferenceValue != null)
							{
								Object elementReference = elementProperty.objectReferenceValue;
								selectedTileConstraints = new(elementReference);
							}
						}
						EditorGUILayout.EndHorizontal();
					}
					EditorGUILayout.Space(5);
				}
			}
		}


		private void DrawRight()
		{
			GUILayout.BeginArea(tileConstraintSetupSection);
			scrollPositionRight = EditorGUILayout.BeginScrollView(scrollPositionRight);

			if (selectedInputTileSet == null)
			{
				EditorGUILayout.BeginVertical(GUILayout.Width(100));
				EditorGUILayout.LabelField("Load a tile set configuration");
				selectedInputTileSet = (TileInputSet)EditorGUILayout.ObjectField(selectedInputTileSet, typeof(TileInputSet), false, GUILayout.Height(30));
				GUILayout.Space(10);
				EditorGUILayout.LabelField("or");
				GUILayout.Space(10);
				EditorGUILayout.LabelField("Create a tile set configuration");
				//EditorGUILayout.LabelField("Create Tile Set Data", GUILayout.Width(60));
				assetName = EditorGUILayout.TextField(assetName, GUILayout.Width(250));

				if (GUILayout.Button("Create Tile Set Configuration", GUILayout.Height(30)))
				{
					ScriptableObject scriptableObject = ScriptableObject.CreateInstance<TileInputSet>();
					string savePath = EditorUtility.SaveFilePanelInProject("Save Scriptable Object", assetName, "asset", "Choose a location to save the ScriptableObject.");
					if (string.IsNullOrEmpty(savePath)) return;
					AssetDatabase.CreateAsset(scriptableObject, savePath);
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();

					selectedInputTileSet = scriptableObject as TileInputSet;
				}

				EditorGUILayout.EndVertical();
			}
			/*
			else if (selectedTileConstraints == null)
			{
				EditorGUILayout.LabelField("- Press the + and - buttons to add new items");
				EditorGUILayout.LabelField("- Select an item from the tile list by pressing the > button");
				EditorGUILayout.LabelField("- Make sure to fill the item with an input tile then press the > button");
				EditorGUILayout.LabelField("- Dont have input tiles? you should plan, create and setup input tiles first");
			}*/
			else
			{
				if (selectedTileConstraints != null)
				{
					selectedTileConstraints.Update();

					EditorGUILayout.Space(5);

					id = selectedTileConstraints.FindProperty("id");
					gameObject = selectedTileConstraints.FindProperty("gameObject");
					compatibleTopList = selectedTileConstraints.FindProperty("compatibleTop");
					compatibleBottomList = selectedTileConstraints.FindProperty("compatibleBottom");
					compatibleLeftList = selectedTileConstraints.FindProperty("compatibleLeft");
					compatibleRightList = selectedTileConstraints.FindProperty("compatibleRight");
					previewTexture = AssetPreview.GetAssetPreview(gameObject.objectReferenceValue);

					EditorGUILayout.BeginHorizontal();
					if (previewTexture != null)
					{
						GUILayout.Label(previewTexture, GUILayout.Width(80), GUILayout.Height(80));
					}
					EditorGUILayout.BeginVertical();
						GUILayout.Space(20);
						EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("ID", GUILayout.Width(80));
							EditorGUILayout.PropertyField(id, GUIContent.none);
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("GameObject", GUILayout.Width(80));
							EditorGUILayout.PropertyField(gameObject, GUIContent.none);
						EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();

					GUILayout.FlexibleSpace();
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.PropertyField(compatibleTopList, true);
					EditorGUILayout.PropertyField(compatibleBottomList, true);
					EditorGUILayout.PropertyField(compatibleLeftList, true);
					EditorGUILayout.PropertyField(compatibleRightList, true);

					selectedTileConstraints.ApplyModifiedProperties();
				}
			}

			EditorGUILayout.EndScrollView();
			EditorGUILayout.Space(20);
			GUILayout.EndArea();
		}

		private void GetAllUniqueInputTiles()
		{
            GetUniqueTiles(selectedInputTileSet.AllInputTiles);
            GetUniqueTiles(selectedInputTileSet.ForeGroundTiles);
            GetUniqueTiles(selectedInputTileSet.BackGroundTiles);
            GetUniqueTiles(selectedInputTileSet.FilledTiles);
            GetUniqueTiles(selectedInputTileSet.EdgeUpTiles);
            GetUniqueTiles(selectedInputTileSet.EdgeDownTiles);
            GetUniqueTiles(selectedInputTileSet.EdgeLeftTiles);
            GetUniqueTiles(selectedInputTileSet.EdgeRightTiles);
            GetUniqueTiles(selectedInputTileSet.ElbowUpLeftTiles);
            GetUniqueTiles(selectedInputTileSet.ElbowUpRightTiles);
            GetUniqueTiles(selectedInputTileSet.ElbowDownLeftTiles);
            GetUniqueTiles(selectedInputTileSet.ElbowDownRightTiles);
            GetUniqueTiles(selectedInputTileSet.CornerUpLeftTiles);
            GetUniqueTiles(selectedInputTileSet.CornerUpRightTiles);
            GetUniqueTiles(selectedInputTileSet.CornerDownLeftTiles);
            GetUniqueTiles(selectedInputTileSet.CornerDownRightTiles);
            GetUniqueTiles(selectedInputTileSet.CornerULDRTiles);
            GetUniqueTiles(selectedInputTileSet.CornerURDLTiles);
            GetUniqueTiles(selectedInputTileSet.FourFaceTiles);
            GetUniqueTiles(selectedInputTileSet.VerticalTiles);
            GetUniqueTiles(selectedInputTileSet.HorizontalTiles);
            GetUniqueTiles(selectedInputTileSet.TwoFaceUpLeftTiles);
            GetUniqueTiles(selectedInputTileSet.TwoFaceUpRightTiles);
            GetUniqueTiles(selectedInputTileSet.TwoFaceDownLeftTiles);
            GetUniqueTiles(selectedInputTileSet.TwoFaceDownRightTiles);
            GetUniqueTiles(selectedInputTileSet.ThreeFaceUpTiles);
            GetUniqueTiles(selectedInputTileSet.ThreeFaceDownTiles);
            GetUniqueTiles(selectedInputTileSet.ThreeFaceLeftTiles);
            GetUniqueTiles(selectedInputTileSet.ThreeFaceRightTiles);
            GetUniqueTiles(selectedInputTileSet.OneFaceUpTiles);
            GetUniqueTiles(selectedInputTileSet.OneFaceDownTiles);
            GetUniqueTiles(selectedInputTileSet.OneFaceLeftTiles);
            GetUniqueTiles(selectedInputTileSet.OneFaceRightTiles);
        }

		private void GetUniqueTiles(List<TileInput> list)
		{
			foreach (TileInput item in list)
			{
				if (!selectedInputTileSet.AllInputTiles.Contains(item))
				{
					selectedInputTileSet.AllInputTiles.Add(item);
				}
			}
		}

		private void GiveUniqueIDToTiles()
		{
			for(int i = 0; i < selectedInputTileSet.AllInputTiles.Count; i++)
			{
				selectedInputTileSet.AllInputTiles[i].id = i + 1;
			}
		}

		private void ClearAllInputTiles()
		{
			ClearAllInputTilesInDirection(selectedInputTileSet.AllInputTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.ForeGroundTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.BackGroundTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.FilledTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.EdgeUpTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.EdgeDownTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.EdgeLeftTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.EdgeRightTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.ElbowUpLeftTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.ElbowUpRightTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.ElbowDownLeftTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.ElbowDownRightTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.CornerUpLeftTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.CornerUpRightTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.CornerDownLeftTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.CornerDownRightTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.CornerULDRTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.CornerURDLTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.FourFaceTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.VerticalTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.HorizontalTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.TwoFaceUpLeftTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.TwoFaceUpRightTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.TwoFaceDownLeftTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.TwoFaceDownRightTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.ThreeFaceUpTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.ThreeFaceDownTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.ThreeFaceLeftTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.ThreeFaceRightTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.OneFaceUpTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.OneFaceDownTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.OneFaceLeftTiles);
			ClearAllInputTilesInDirection(selectedInputTileSet.OneFaceRightTiles);
		}

		private void ClearAllInputTilesInDirection(List<TileInput> tilesInDirectionList)
		{
			tilesInDirectionList.Clear();
		}

		private void ClearAllInputTileConstraints()
		{
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.AllInputTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.ForeGroundTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.BackGroundTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.FilledTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.EdgeUpTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.EdgeDownTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.EdgeLeftTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.EdgeRightTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.ElbowUpLeftTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.ElbowUpRightTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.ElbowDownLeftTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.ElbowDownRightTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.CornerUpLeftTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.CornerUpRightTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.CornerDownLeftTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.CornerDownRightTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.CornerULDRTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.CornerURDLTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.FourFaceTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.VerticalTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.HorizontalTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.TwoFaceUpLeftTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.TwoFaceUpRightTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.TwoFaceDownLeftTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.TwoFaceDownRightTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.ThreeFaceUpTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.ThreeFaceDownTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.ThreeFaceLeftTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.ThreeFaceRightTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.OneFaceUpTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.OneFaceDownTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.OneFaceLeftTiles);
			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.OneFaceRightTiles);
		}

		private void ClearAllInputTileConstraintsInDirection(List<TileInput> tileInDirectionList)
		{
			foreach(var item in tileInDirectionList)
			{
				item.compatibleTop.Clear();
				item.compatibleBottom.Clear();
				item.compatibleLeft.Clear();
				item.compatibleRight.Clear();
			}
		}


		private void AutoGenerateSerialized()
		{
			//TODO: Use serialized objects and properties to enable undo functionality in editor
			/*
			string[] directions = { "TopLeftTiles", "TopTiles", "TopRightTiles", "LeftTiles", "FourDirectionTiles",
			"RightTiles", "BottomLeftTiles", "BottomTiles", "BottomRightTiles"};
			*/

			//TileInputSet temp = selectedInputTileSet;

			/*
			SerializedProperty fourDirection = serializedTileSetObject.FindProperty("FourDirectionTiles");
			SerializedProperty topDirection = serializedTileSetObject.FindProperty("TopTiles");
			SerializedProperty bottomDirection = serializedTileSetObject.FindProperty("BottomTiles");
			SerializedProperty leftDirection = serializedTileSetObject.FindProperty("LeftTiles");
			SerializedProperty rightDirection = serializedTileSetObject.FindProperty("RightTiles");

			for (int i = 0; i < fourDirection.arraySize; i++)
			{
				SerializedProperty top = fourDirection.GetArrayElementAtIndex(i).FindPropertyRelative("compatibleTop");
				SerializedProperty bottom = fourDirection.GetArrayElementAtIndex(i).FindPropertyRelative("compatibleBottom");
				SerializedProperty left = fourDirection.GetArrayElementAtIndex(i).FindPropertyRelative("compatibleLeft");
				SerializedProperty right = fourDirection.GetArrayElementAtIndex(i).FindPropertyRelative("compatibleRight");

				for (int j = 0; j < top.arraySize; j++)
				{
					top.GetArrayElementAtIndex(j).objectReferenceValue = topDirection.GetArrayElementAtIndex(j).objectReferenceValue;
				}
			}
			*/
			/*
			List<TileInput> temp = selectedInputTileSet.TopLeftTiles;
			for(int i = 0; i < temp.Count; i++)
			{
				fourDirection.GetArrayElementAtIndex(i).objectReferenceValue = temp[i];
			}

			/*
			for (int i = 0; i < topDirection.arraySize; i++)
			{
				SerializedProperty listCombine;
				listCombine.InsertArrayElementAtIndex(i);
				listCombine.GetArrayElementAtIndex(i).objectReferenceValue = listCombine.GetArrayElementAtIndex(i).objectReferenceValue;
			}
			*/
		}
	}
}