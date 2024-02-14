using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Based from Renaissance Coders tutorials from YT
namespace HelloWorld.Editor
{
	public class AutoTileSetUpWindow : EditorWindow
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
		private SerializedProperty threeU;
		private SerializedProperty threeD;
		private SerializedProperty threeL;
		private SerializedProperty threeR;
		private SerializedProperty oneU;
		private SerializedProperty oneD;
		private SerializedProperty oneL;
		private SerializedProperty oneR;

		#endregion Serialized Properties

		//Right
		private SerializedObject selectedTileConstraints;

		private SerializedProperty id;
		private SerializedProperty gameObject;
		private SerializedProperty compatibleTopList;
		private SerializedProperty compatibleBottomList;
		private SerializedProperty compatibleLeftList;
		private SerializedProperty compatibleRightList;
		private Texture2D previewTexture;

		private string assetName = "New Tile Set Configuration Data";

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
		private Texture2D textureAllInput;
		private Texture2D textureForeground;
		private Texture2D textureBackground;
		private Texture2D textureFour;
		private Texture2D textureFilled;
		private Texture2D textureHorizontal;
		private Texture2D textureVertical;
		private Texture2D textureEdgeup;
		private Texture2D textureEdgedown;
		private Texture2D textureEdgeleft;
		private Texture2D textureEdgeright;
		private Texture2D textureElbowUL;
		private Texture2D textureElbowUR;
		private Texture2D textureElbowDL;
		private Texture2D textureElbowDR;
		private Texture2D textureCornerUL;
		private Texture2D textureCornerUR;
		private Texture2D textureCornerDL;
		private Texture2D textureCornerDR;
		private Texture2D textureCornerULDR;
		private Texture2D textureCornerURDL;
		private Texture2D textureTwoUL;
		private Texture2D textureTwoUR;
		private Texture2D textureTwoDL;
		private Texture2D textureTwoDR;
		private Texture2D textureThreeU;
		private Texture2D textureThreeD;
		private Texture2D textureThreeL;
		private Texture2D textureThreeR;
		private Texture2D textureOneU;
		private Texture2D textureOneD;
		private Texture2D textureOneL;
		private Texture2D textureOneR;

		private Color headerBackgroundColor = new(30f / 255f, 30f / 255f, 30f / 255f, 0.5f);
		private Color leftBackgroundColor = new(30f / 255f, 30f / 255f, 30f / 255f, 0.5f);
		private Color rightBackgroundColor = new(0.6f, .2f, 0.7f, 0.7f);

		private Rect headerSection;
		private Rect tileSetupSection;
		private Rect tileConstraintSetupSection;

		//private static readonly int windowMinWidth = 600;
		//private static readonly int windowMinHeight = 350;

		private Vector2 scrollPositionLeft = Vector2.zero;
		private Vector2 scrollPositionRight = Vector2.zero;

		//private int newArraySize;
		//private int currentArraySize;
		private bool shouldClear = false;

		private int selectedOptionIndex = 0;
		private readonly string[] menuOptions = { "Full Combine", "2x2 Only (TODO)", "3x3 Only (TODO)", "3x3 - No 1D together" };
		private GenericMenu menu;

		private bool[] foldout = { false, false, false, false, false, false, false, false, false };

		#endregion Window Variables

		private void OnEnable()
		{
			InitTextures();
			InitData();
		}

		private void OnGUI()
		{
			if (selectedInputTileSet != null)
			{
				SerializeProperties();
			}

			DrawLayouts();
			DrawHeader();
			DrawLeft();
			DrawRight();

			if (serializedTileSetObject != null)
			{
				UpdateSerializedProperties();
			}
			// Null propagation?, it says it is used for simple null check
		}

		private void SerializeProperties()
		{
			serializedTileSetObject = new(selectedInputTileSet);
			serializedTileSetObject.Update();

			allInput = serializedTileSetObject.FindProperty("AllInputTiles");
			foreground = serializedTileSetObject.FindProperty("ForeGroundTiles");
			background = serializedTileSetObject.FindProperty("BackGroundTiles");
			filled = serializedTileSetObject.FindProperty("FilledTiles");
			four = serializedTileSetObject.FindProperty("FourFaceTiles");
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
			threeU = serializedTileSetObject.FindProperty("ThreeFaceUpTiles");
			threeD = serializedTileSetObject.FindProperty("ThreeFaceDownTiles");
			threeL = serializedTileSetObject.FindProperty("ThreeFaceLeftTiles");
			threeR = serializedTileSetObject.FindProperty("ThreeFaceRightTiles");
			oneU = serializedTileSetObject.FindProperty("OneFaceUpTiles");
			oneD = serializedTileSetObject.FindProperty("OneFaceDownTiles");
			oneL = serializedTileSetObject.FindProperty("OneFaceLeftTiles");
			oneR = serializedTileSetObject.FindProperty("OneFaceRightTiles");
		}

		private void UpdateSerializedProperties()
		{
			allInput.serializedObject.ApplyModifiedProperties();
			foreground.serializedObject.ApplyModifiedProperties();
			background.serializedObject.ApplyModifiedProperties();
			filled.serializedObject.ApplyModifiedProperties();
			four.serializedObject.ApplyModifiedProperties();
			vertical.serializedObject.ApplyModifiedProperties();
			horizontal.serializedObject.ApplyModifiedProperties();
			edgeup.serializedObject.ApplyModifiedProperties();
			edgedown.serializedObject.ApplyModifiedProperties();
			edgeleft.serializedObject.ApplyModifiedProperties();
			edgeright.serializedObject.ApplyModifiedProperties();
			elbowUL.serializedObject.ApplyModifiedProperties();
			elbowUR.serializedObject.ApplyModifiedProperties();
			elbowDL.serializedObject.ApplyModifiedProperties();
			elbowDR.serializedObject.ApplyModifiedProperties();
			cornerUL.serializedObject.ApplyModifiedProperties();
			cornerUR.serializedObject.ApplyModifiedProperties();
			cornerDL.serializedObject.ApplyModifiedProperties();
			cornerDR.serializedObject.ApplyModifiedProperties();
			cornerULDR.serializedObject.ApplyModifiedProperties();
			cornerURDL.serializedObject.ApplyModifiedProperties();
			twoUL.serializedObject.ApplyModifiedProperties();
			twoUR.serializedObject.ApplyModifiedProperties();
			twoDL.serializedObject.ApplyModifiedProperties();
			twoDR.serializedObject.ApplyModifiedProperties();
			threeU.serializedObject.ApplyModifiedProperties();
			threeD.serializedObject.ApplyModifiedProperties();
			threeL.serializedObject.ApplyModifiedProperties();
			threeR.serializedObject.ApplyModifiedProperties();
			oneU.serializedObject.ApplyModifiedProperties();
			oneD.serializedObject.ApplyModifiedProperties();
			oneL.serializedObject.ApplyModifiedProperties();
			oneR.serializedObject.ApplyModifiedProperties();

			serializedTileSetObject.ApplyModifiedProperties();
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

			textureAllInput = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00011.png");
			textureForeground = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00011.png");
			textureBackground = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00017.png");
			textureFour = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00012.png");
			textureFilled = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00032.png");
			textureHorizontal = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-0006.png");
			textureVertical = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-0005.png");
			textureEdgeup = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00018.png");
			textureEdgedown = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00019.png");
			textureEdgeleft = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00020.png");
			textureEdgeright = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00021.png");
			textureElbowUL = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00022.png");
			textureElbowUR = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00023.png");
			textureElbowDL = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00025.png");
			textureElbowDR = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00024.png");
			textureCornerUL = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00028.png");
			textureCornerUR = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00029.png");
			textureCornerDL = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00027.png");
			textureCornerDR = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00026.png");
			textureCornerULDR = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00031.png");
			textureCornerURDL = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00030.png");
			textureTwoUL = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-0008.png");
			textureTwoUR = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-0007.png");
			textureTwoDL = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-0009.png");
			textureTwoDR = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00010.png");
			textureThreeU = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00013.png");
			textureThreeD = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00014.png");
			textureThreeL = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00015.png");
			textureThreeR = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-00016.png");
			textureOneU = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-0001.png");
			textureOneD = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-0002.png");
			textureOneL = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-0003.png");
			textureOneR = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.gatozhanya.relacade/Objects/Icons/Sprite-0004.png");
		}

		private void InitData()
		{
			if (selectedInputTileSet != null)
			{
				SerializeProperties();
			}
		}

		private void DrawLayouts()
		{
			headerSection.x = 0;
			headerSection.y = 0;
			headerSection.width = position.width;
			headerSection.height = headerSectionHeight;

			tileSetupSection.x = 0;
			tileSetupSection.y = headerSectionHeight;
			tileSetupSection.width = tileSetupSectionWidth;
			tileSetupSection.height = position.height - headerSectionHeight;

			tileConstraintSetupSection.x = tileSetupSectionWidth;
			tileConstraintSetupSection.y = headerSectionHeight;
			tileConstraintSetupSection.width = position.width - tileSetupSectionWidth;
			tileConstraintSetupSection.height = position.height - headerSectionHeight;

			GUI.DrawTexture(headerSection, headerBackgroundTexture);
			GUI.DrawTexture(tileSetupSection, leftBackgroundTexture);
			//GUI.DrawTexture(tileConstraintSetupSection, rightBackgroundTexture);
		}

		private void DrawHeader()
		{
			GUILayout.BeginArea(headerSection);
			EditorGUILayout.Space(3);
			GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(position.width));

			EditorGUILayout.LabelField("Tile Set Config", GUILayout.Width(100), GUILayout.MaxWidth(100));
			selectedInputTileSet = (TileInputSet)EditorGUILayout.ObjectField(selectedInputTileSet, typeof(TileInputSet), false, GUILayout.MaxWidth(300));

			if (GUILayout.Button("Auto Generate", EditorStyles.toolbarButton, GUILayout.MaxWidth(200)))
			{
				if (selectedInputTileSet != null /*& serializedTileSetObject != null*/)
				{
					ClearAllInputTileConstraints();
					AutoGenerateSerialized();
					UpdateSerializedProperties();
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
			if (GUILayout.Button("Auto Set ID", EditorStyles.toolbarButton, GUILayout.MaxWidth(80)))
			{
				if (selectedInputTileSet != null /*& serializedTileSetObject != null*/)
				{
					GiveUniqueIDToTiles();
					UpdateSerializedProperties();
				}
			}

			shouldClear = EditorGUILayout.ToggleLeft("Clear?", shouldClear, GUILayout.Width(60));
			if (GUILayout.Button("Clear", GUILayout.MaxWidth(100)))
			{
				if (selectedInputTileSet != null && serializedTileSetObject != null && shouldClear)
				{
					Debug.Log("Constraints Cleared");
					ClearAllInputTileConstraints();
					UpdateSerializedProperties();
				}
			}

			EditorGUILayout.EndHorizontal();

			GUILayout.EndArea();
		}

		private void AutoGenerateSerialized()
		{
			for (int i = 0; i < foreground.arraySize; i++)
			{
				SerializedProperty temp = foreground.GetArrayElementAtIndex(i);

				foreground.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.Foreground);
				foreground.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < background.arraySize; i++)
			{
				SerializedProperty temp = background.GetArrayElementAtIndex(i);

				background.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.Background);
				background.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < four.arraySize; i++)
			{
				SerializedProperty temp = four.GetArrayElementAtIndex(i);

				four.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.FourFace);
				four.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < filled.arraySize; i++)
			{
				SerializedProperty temp = filled.GetArrayElementAtIndex(i);

				filled.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.Filled);
				filled.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}

			for (int i = 0; i < edgeup.arraySize; i++)
			{
				SerializedProperty temp = edgeup.GetArrayElementAtIndex(i);

				edgeup.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.EdgeUp);
				edgeup.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < edgedown.arraySize; i++)
			{
				SerializedProperty temp = edgedown.GetArrayElementAtIndex(i);

				edgedown.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.EdgeDown);
				edgedown.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < edgeleft.arraySize; i++)
			{
				SerializedProperty temp = edgeleft.GetArrayElementAtIndex(i);

				edgeleft.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.EdgeLeft);
				edgeleft.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < edgeright.arraySize; i++)
			{
				SerializedProperty temp = edgeright.GetArrayElementAtIndex(i);

				edgeright.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.EdgeRight);
				edgeright.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}

			for (int i = 0; i < elbowUL.arraySize; i++)
			{
				SerializedProperty temp = elbowUL.GetArrayElementAtIndex(i);

				elbowUL.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.ElbowUpLeft);
				elbowUL.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < elbowUR.arraySize; i++)
			{
				SerializedProperty temp = elbowUR.GetArrayElementAtIndex(i);

				elbowUR.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.ElbowUpRight);
				elbowUR.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < elbowDL.arraySize; i++)
			{
				SerializedProperty temp = elbowDL.GetArrayElementAtIndex(i);

				elbowDL.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.ElbowDownLeft);
				elbowDL.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < elbowDR.arraySize; i++)
			{
				SerializedProperty temp = elbowDR.GetArrayElementAtIndex(i);

				elbowDR.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.ElbowDownRight);
				elbowDR.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}

			for (int i = 0; i < cornerUL.arraySize; i++)
			{
				SerializedProperty temp = cornerUL.GetArrayElementAtIndex(i);

				cornerUL.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.CornerUpLeft);
				cornerUL.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < cornerUR.arraySize; i++)
			{
				SerializedProperty temp = cornerUR.GetArrayElementAtIndex(i);

				cornerUR.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.CornerUpRight);
				cornerUR.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < cornerDL.arraySize; i++)
			{
				SerializedProperty temp = cornerDL.GetArrayElementAtIndex(i);

				cornerDL.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.CornerDownLeft);
				cornerDL.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < cornerDR.arraySize; i++)
			{
				SerializedProperty temp = cornerDR.GetArrayElementAtIndex(i);

				cornerDR.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.CornerDownRight);
				cornerDR.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < cornerULDR.arraySize; i++)
			{
				SerializedProperty temp = cornerULDR.GetArrayElementAtIndex(i);

				cornerULDR.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.CornerULDR);
				cornerULDR.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < cornerURDL.arraySize; i++)
			{
				SerializedProperty temp = cornerURDL.GetArrayElementAtIndex(i);

				cornerURDL.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.CornerURDL);
				cornerURDL.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}

			for (int i = 0; i < oneU.arraySize; i++)
			{
				SerializedProperty temp = oneU.GetArrayElementAtIndex(i);

				oneU.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.OneFaceUp);
				oneU.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < oneD.arraySize; i++)
			{
				SerializedProperty temp = oneD.GetArrayElementAtIndex(i);

				oneD.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.OneFaceDown);
				oneD.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < oneL.arraySize; i++)
			{
				SerializedProperty temp = oneL.GetArrayElementAtIndex(i);

				oneL.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.OneFaceLeft);
				oneL.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < oneR.arraySize; i++)
			{
				SerializedProperty temp = oneR.GetArrayElementAtIndex(i);

				oneR.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.OneFaceRight);
				oneR.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}

			for (int i = 0; i < twoUL.arraySize; i++)
			{
				SerializedProperty temp = twoUL.GetArrayElementAtIndex(i);

				twoUL.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.TwoFaceUpLeft);
				twoUL.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < twoUR.arraySize; i++)
			{
				SerializedProperty temp = twoUR.GetArrayElementAtIndex(i);

				twoUR.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.TwoFaceUpRight);
				twoUR.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < twoDL.arraySize; i++)
			{
				SerializedProperty temp = twoDL.GetArrayElementAtIndex(i);

				twoDL.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.TwoFaceDownLeft);
				twoDL.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < twoDR.arraySize; i++)
			{
				SerializedProperty temp = twoDR.GetArrayElementAtIndex(i);

				twoDR.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.TwoFaceDownRight);
				twoDR.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}

			for (int i = 0; i < threeU.arraySize; i++)
			{
				SerializedProperty temp = threeU.GetArrayElementAtIndex(i);

				threeU.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.ThreeFaceUp);
				threeU.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < threeD.arraySize; i++)
			{
				SerializedProperty temp = threeD.GetArrayElementAtIndex(i);

				threeD.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.ThreeFaceDown);
				threeD.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < threeL.arraySize; i++)
			{
				SerializedProperty temp = threeL.GetArrayElementAtIndex(i);

				threeL.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.ThreeFaceLeft);
				threeL.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < threeR.arraySize; i++)
			{
				SerializedProperty temp = threeR.GetArrayElementAtIndex(i);

				threeR.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.ThreeFaceRight);
				threeR.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}

			for (int i = 0; i < vertical.arraySize; i++)
			{
				SerializedProperty temp = vertical.GetArrayElementAtIndex(i);

				vertical.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.Vertical);
				vertical.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}
			for (int i = 0; i < horizontal.arraySize; i++)
			{
				SerializedProperty temp = horizontal.GetArrayElementAtIndex(i);

				horizontal.GetArrayElementAtIndex(i).objectReferenceValue = MainCombine((TileInput)temp.objectReferenceValue, selectedInputTileSet, DirectionToSet.Horizontal);
				horizontal.GetArrayElementAtIndex(i).serializedObject.ApplyModifiedProperties();
			}

			GetAllUniqueInputTiles();
			GiveUniqueIDToTiles();
		}

		private TileInput MainCombine(TileInput tileInput, TileInputSet set, DirectionToSet direction)
		{
			if (tileInput == null) return null;

			SerializedObject temp = new(tileInput);
			SerializedProperty top = temp.FindProperty("compatibleTop");
			SerializedProperty bottom = temp.FindProperty("compatibleBottom");
			SerializedProperty left = temp.FindProperty("compatibleLeft");
			SerializedProperty right = temp.FindProperty("compatibleRight");

			switch (direction)
			{
				#region Four/Full/Filled

				case DirectionToSet.Foreground:

					#region ForeGround

					#region 2x2

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.ForeGroundTiles,
						set.EdgeDownTiles,
						set.ElbowDownRightTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.ForeGroundTiles,
						set.EdgeUpTiles,
						set.ElbowUpLeftTiles,
						set.ElbowUpRightTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.ForeGroundTiles,
						set.EdgeRightTiles,
						set.ElbowUpRightTiles,
						set.ElbowDownRightTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.ForeGroundTiles,
						set.EdgeLeftTiles,
						set.ElbowUpLeftTiles,
						set.ElbowDownLeftTiles
					);

					#endregion 2x2

					#region 3x3

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.FilledTiles,
						set.HorizontalTiles,
						set.OneFaceUpTiles,
						set.OneFaceLeftTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.ThreeFaceUpTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
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

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
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

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
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

					#endregion 3x3

					break;

				#endregion ForeGround

				case DirectionToSet.Background:

					#region Background

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.BackGroundTiles,
						set.EdgeUpTiles,
						set.CornerUpLeftTiles,
						set.CornerUpRightTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.BackGroundTiles,
						set.EdgeDownTiles,
						set.CornerDownLeftTiles,
						set.CornerDownRightTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.BackGroundTiles,
						set.EdgeLeftTiles,
						set.CornerUpLeftTiles,
						set.CornerDownLeftTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.BackGroundTiles,
						set.EdgeRightTiles,
						set.CornerUpRightTiles,
						set.CornerDownRightTiles
					);

					#endregion Background

					break;

				case DirectionToSet.Filled:

					#region Filled

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top, set.FilledTiles,
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

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom, set.FilledTiles,
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

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left, set.FilledTiles,
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

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right, set.FilledTiles,
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

					#endregion Filled

					break;

				case DirectionToSet.FourFace:

					#region FourFace

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.FilledTiles,
						set.VerticalTiles,
						set.OneFaceDownTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.FilledTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.FilledTiles,
						set.HorizontalTiles,
						set.OneFaceRightTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.FilledTiles,
						set.HorizontalTiles,
						set.OneFaceLeftTiles
					);

					#endregion FourFace

					break;

				#endregion Four/Full/Filled

				#region Edge

				case DirectionToSet.EdgeUp:

					#region Top

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.ForeGroundTiles,
						set.EdgeDownTiles,
						set.ElbowDownLeftTiles,
						set.ElbowDownRightTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.BackGroundTiles,
						set.EdgeDownTiles,
						set.CornerDownLeftTiles,
						set.CornerDownRightTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.EdgeUpTiles,
						set.CornerUpRightTiles,
						set.ElbowUpLeftTiles,
						set.CornerURDLTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.EdgeUpTiles,
						set.CornerUpLeftTiles,
						set.ElbowUpRightTiles,
						set.CornerULDRTiles
					);

					#endregion Top

					break;

				case DirectionToSet.EdgeDown:

					#region Bottom

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.BackGroundTiles,
						set.EdgeUpTiles,
						set.CornerUpLeftTiles,
						set.CornerUpRightTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.ForeGroundTiles,
						set.EdgeUpTiles,
						set.ElbowUpLeftTiles,
						set.ElbowUpRightTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.EdgeDownTiles,
						set.CornerDownRightTiles,
						set.ElbowDownLeftTiles,
						set.CornerULDRTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.EdgeDownTiles,
						set.CornerDownLeftTiles,
						set.ElbowDownRightTiles,
						set.CornerURDLTiles
					);

					#endregion Bottom

					break;

				case DirectionToSet.EdgeLeft:

					#region Left

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.EdgeLeftTiles,
						set.CornerDownLeftTiles,
						set.ElbowUpLeftTiles,
						set.CornerURDLTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.EdgeLeftTiles,
						set.CornerUpLeftTiles,
						set.ElbowDownLeftTiles,
						set.CornerULDRTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.ForeGroundTiles,
						set.EdgeRightTiles,
						set.ElbowUpRightTiles,
						set.ElbowDownRightTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.BackGroundTiles,
						set.EdgeRightTiles,
						set.CornerUpRightTiles,
						set.CornerDownRightTiles
					);

					#endregion Left

					break;

				case DirectionToSet.EdgeRight:

					#region Right

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.EdgeRightTiles,
						set.CornerDownRightTiles,
						set.ElbowUpRightTiles,
						set.CornerULDRTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.EdgeRightTiles,
						set.CornerUpRightTiles,
						set.ElbowDownRightTiles,
						set.CornerURDLTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.BackGroundTiles,
						set.EdgeLeftTiles,
						set.CornerDownLeftTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.ForeGroundTiles,
						set.EdgeLeftTiles,
						set.ElbowUpLeftTiles,
						set.ElbowDownLeftTiles
					);

					#endregion Right

					break;

				#endregion Edge

				#region Elbow

				case DirectionToSet.ElbowUpLeft:

					#region ElbowUL

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.ForeGroundTiles,
						set.EdgeDownTiles,
						set.ElbowDownLeftTiles,
						set.ElbowDownRightTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.EdgeLeftTiles,
						set.CornerUpLeftTiles,
						set.ElbowDownLeftTiles,
						set.CornerULDRTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.ForeGroundTiles,
						set.EdgeRightTiles,
						set.ElbowUpRightTiles,
						set.ElbowDownRightTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.EdgeUpTiles,
						set.CornerUpLeftTiles,
						set.ElbowUpRightTiles,
						set.CornerULDRTiles
					);

					#endregion ElbowUL

					break;

				case DirectionToSet.ElbowUpRight:

					#region ElbowUR

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.ForeGroundTiles,
						set.EdgeDownTiles,
						set.ElbowDownLeftTiles,
						set.ElbowDownRightTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.EdgeRightTiles,
						set.CornerUpRightTiles,
						set.ElbowDownRightTiles,
						set.CornerURDLTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.EdgeUpTiles,
						set.CornerUpRightTiles,
						set.ElbowUpLeftTiles,
						set.CornerURDLTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.ForeGroundTiles,
						set.EdgeLeftTiles,
						set.ElbowUpLeftTiles,
						set.ElbowDownLeftTiles
					);

					#endregion ElbowUR

					break;

				case DirectionToSet.ElbowDownLeft:

					#region ElbowDL

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.EdgeLeftTiles,
						set.CornerDownLeftTiles,
						set.ElbowUpLeftTiles,
						set.CornerURDLTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.ForeGroundTiles,
						set.EdgeUpTiles,
						set.ElbowUpLeftTiles,
						set.ElbowUpRightTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.ForeGroundTiles,
						set.EdgeRightTiles,
						set.ElbowUpRightTiles,
						set.ElbowDownRightTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.EdgeDownTiles,
						set.CornerDownLeftTiles,
						set.ElbowDownRightTiles,
						set.CornerURDLTiles
					);

					#endregion ElbowDL

					break;

				case DirectionToSet.ElbowDownRight:

					#region BottomRight

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.EdgeRightTiles,
						set.CornerDownRightTiles,
						set.ElbowUpRightTiles,
						set.CornerULDRTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.ForeGroundTiles,
						set.EdgeUpTiles,
						set.ElbowUpLeftTiles,
						set.ElbowUpRightTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.EdgeDownTiles,
						set.CornerDownRightTiles,
						set.ElbowDownLeftTiles,
						set.CornerULDRTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.ForeGroundTiles,
						set.EdgeLeftTiles,
						set.ElbowUpLeftTiles,
						set.ElbowDownLeftTiles
					);

					#endregion BottomRight

					break;

				#endregion Elbow

				#region Corner

				case DirectionToSet.CornerUpLeft:

					#region ITopLeft

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.EdgeLeftTiles,
						set.ElbowUpLeftTiles,
						set.CornerDownLeftTiles,
						set.CornerURDLTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.BackGroundTiles,
						set.EdgeDownTiles,
						set.CornerDownLeftTiles,
						set.CornerDownRightTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.EdgeUpTiles,
						set.CornerUpRightTiles,
						set.ElbowUpLeftTiles,
						set.CornerURDLTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.BackGroundTiles,
						set.EdgeRightTiles,
						set.CornerUpRightTiles,
						set.CornerDownRightTiles
					);

					#endregion ITopLeft

					break;

				case DirectionToSet.CornerUpRight:

					#region ITopRight

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.EdgeRightTiles,
						set.ElbowUpRightTiles,
						set.CornerDownRightTiles,
						set.CornerULDRTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.BackGroundTiles,
						set.EdgeDownTiles,
						set.CornerDownRightTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.BackGroundTiles,
						set.EdgeLeftTiles,
						set.CornerUpLeftTiles,
						set.CornerDownLeftTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.EdgeUpTiles,
						set.CornerUpLeftTiles,
						set.ElbowUpRightTiles,
						set.CornerULDRTiles
					);

					#endregion ITopRight

					break;

				case DirectionToSet.CornerDownLeft:

					#region IBottomLeft

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.BackGroundTiles,
						set.EdgeUpTiles,
						set.CornerUpLeftTiles,
						set.CornerUpRightTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.EdgeLeftTiles,
						set.CornerUpLeftTiles,
						set.ElbowDownLeftTiles,
						set.CornerULDRTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.EdgeDownTiles,
						set.ElbowDownLeftTiles,
						set.CornerDownRightTiles,
						set.CornerULDRTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.BackGroundTiles,
						set.EdgeRightTiles,
						set.CornerUpRightTiles,
						set.CornerDownRightTiles
					);

					#endregion IBottomLeft

					break;

				case DirectionToSet.CornerDownRight:

					#region IBottomRight

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.BackGroundTiles,
						set.EdgeUpTiles,
						set.CornerUpRightTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.EdgeRightTiles,
						set.CornerUpRightTiles,
						set.ElbowDownRightTiles,
						set.CornerURDLTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.BackGroundTiles,
						set.EdgeLeftTiles,
						set.CornerUpLeftTiles,
						set.CornerDownLeftTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.EdgeDownTiles,
						set.CornerDownLeftTiles,
						set.ElbowDownRightTiles,
						set.CornerURDLTiles
					);

					#endregion IBottomRight

					break;

				case DirectionToSet.CornerULDR:

					#region CornerULDR

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.EdgeLeftTiles,
						set.CornerDownLeftTiles,
						set.ElbowUpLeftTiles,
						set.CornerURDLTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.EdgeRightTiles,
						set.CornerUpLeftTiles,
						set.ElbowDownRightTiles,
						set.CornerURDLTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.EdgeUpTiles,
						set.CornerUpLeftTiles,
						set.ElbowUpLeftTiles,
						set.CornerURDLTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.EdgeDownTiles,
						set.CornerDownLeftTiles,
						set.ElbowDownRightTiles,
						set.CornerURDLTiles
					);

					#endregion CornerULDR

					break;

				case DirectionToSet.CornerURDL:

					#region CornerULDR

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.EdgeRightTiles,
						set.CornerDownRightTiles,
						set.ElbowUpRightTiles,
						set.CornerULDRTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.EdgeLeftTiles,
						set.CornerUpLeftTiles,
						set.ElbowDownLeftTiles,
						set.CornerULDRTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.EdgeDownTiles,
						set.CornerDownRightTiles,
						set.ElbowDownLeftTiles,
						set.CornerULDRTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.EdgeUpTiles,
						set.CornerUpLeftTiles,
						set.CornerUpRightTiles,
						set.CornerULDRTiles
					);

					#endregion CornerULDR

					break;

				#endregion Corner

				#region Three

				case DirectionToSet.ThreeFaceUp:

					#region ThreeUp

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.FilledTiles,
						set.VerticalTiles,
						set.OneFaceDownTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.ForeGroundTiles,
						set.FilledTiles,
						set.HorizontalTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceDownRightTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.FilledTiles,

						set.HorizontalTiles,
						set.OneFaceRightTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.FilledTiles,

						set.HorizontalTiles,
						set.OneFaceLeftTiles
					);

					#endregion ThreeUp

					break;

				case DirectionToSet.ThreeFaceDown:

					#region ThreeDown

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.FilledTiles,
						set.ForeGroundTiles,
						set.HorizontalTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.ThreeFaceUpTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.FilledTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.FilledTiles,
						set.HorizontalTiles,
						set.OneFaceRightTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.FilledTiles,
						set.HorizontalTiles,
						set.OneFaceLeftTiles
					);

					#endregion ThreeDown

					break;

				case DirectionToSet.ThreeFaceLeft:

					#region ThreeLeft

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.FilledTiles,
						set.VerticalTiles,
						set.OneFaceDownTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.FilledTiles,

						set.VerticalTiles,
						set.OneFaceUpTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.FilledTiles,

						set.HorizontalTiles,
						set.OneFaceRightTiles,
						set.TwoFaceDownRightTiles,
						set.TwoFaceUpRightTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.FilledTiles,
						set.ForeGroundTiles,
						set.VerticalTiles
					);

					#endregion ThreeLeft

					break;

				case DirectionToSet.ThreeFaceRight:

					#region ThreeRight

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.FilledTiles,
						set.VerticalTiles,
						set.OneFaceDownTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.FilledTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.FilledTiles,
						set.ForeGroundTiles,
						set.VerticalTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceUpLeftTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.FilledTiles,
						set.HorizontalTiles,
						set.OneFaceLeftTiles
					);

					#endregion ThreeRight

					break;

				#endregion Three

				#region Two

				case DirectionToSet.TwoFaceUpLeft:

					#region TwoUpLeft

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.FilledTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceDownRightTiles,
						set.VerticalTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
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

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.FilledTiles,
						set.TwoFaceUpRightTiles,
						set.TwoFaceDownRightTiles,
						set.HorizontalTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
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

					#endregion TwoUpLeft

					break;

				case DirectionToSet.TwoFaceUpRight:

					#region TwoUpRight

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.FilledTiles,
						set.VerticalTiles,
						set.TwoFaceDownLeftTiles,
						set.TwoFaceDownRightTiles

					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
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

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
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

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.FilledTiles,
						set.HorizontalTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceDownLeftTiles

					);

					#endregion TwoUpRight

					break;

				case DirectionToSet.TwoFaceDownLeft:

					#region TwoDownLeft

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.FilledTiles,
						set.ForeGroundTiles,
						set.OneFaceUpTiles,
						set.OneFaceLeftTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.ThreeFaceUpTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.FilledTiles,
						set.VerticalTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles

					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.FilledTiles,
						set.HorizontalTiles,
						set.TwoFaceUpRightTiles,
						set.TwoFaceDownRightTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.FilledTiles,
						set.ForeGroundTiles,
						set.VerticalTiles,
						set.OneFaceUpTiles,
						set.OneFaceDownTiles,
						set.OneFaceRightTiles,
						set.TwoFaceUpRightTiles,
						set.ThreeFaceRightTiles
					);

					#endregion TwoDownLeft

					break;

				case DirectionToSet.TwoFaceDownRight:

					#region TwoDownRight

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
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

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.FilledTiles,
						set.VerticalTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles

					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
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

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.FilledTiles,
						set.HorizontalTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceDownLeftTiles

					);

					#endregion TwoDownRight

					break;

				#endregion Two

				#region One

				case DirectionToSet.OneFaceUp:

					#region OneUp

					if (selectedOptionIndex == 3)
					{
						tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
							set.FilledTiles, set.FourFaceTiles, set.VerticalTiles,
							set.TwoFaceDownLeftTiles, set.TwoFaceDownRightTiles,
							set.ThreeFaceDownTiles, set.ThreeFaceLeftTiles, set.ThreeFaceRightTiles
						);

						tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
							set.FilledTiles, set.ForeGroundTiles, set.HorizontalTiles,
							set.TwoFaceDownLeftTiles, set.TwoFaceDownRightTiles,
							set.ThreeFaceDownTiles
						);

						tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
							set.FilledTiles, set.ForeGroundTiles, set.VerticalTiles,
							set.TwoFaceUpLeftTiles, set.TwoFaceDownLeftTiles,
							set.ThreeFaceLeftTiles
						);

						tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
							set.FilledTiles, set.ForeGroundTiles, set.VerticalTiles,
							set.TwoFaceUpRightTiles, set.TwoFaceDownRightTiles,
							set.ThreeFaceRightTiles
						);
					}
					else
					{
						tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
							set.FilledTiles, set.FourFaceTiles, set.VerticalTiles,
							set.OneFaceDownTiles,
							set.TwoFaceDownLeftTiles, set.TwoFaceDownRightTiles,
							set.ThreeFaceDownTiles, set.ThreeFaceLeftTiles, set.ThreeFaceRightTiles
						);

						tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
							set.FilledTiles, set.ForeGroundTiles, set.HorizontalTiles,
							set.OneFaceDownTiles, set.OneFaceLeftTiles, set.OneFaceRightTiles,
							set.TwoFaceDownLeftTiles, set.TwoFaceDownRightTiles,
							set.ThreeFaceDownTiles
						);

						tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
							set.FilledTiles, set.ForeGroundTiles, set.VerticalTiles,
							set.OneFaceUpTiles, set.OneFaceDownTiles, set.OneFaceLeftTiles,
							set.TwoFaceUpLeftTiles, set.TwoFaceDownLeftTiles,
							set.ThreeFaceLeftTiles
						);

						tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
							set.FilledTiles, set.ForeGroundTiles, set.VerticalTiles,
							set.OneFaceUpTiles, set.OneFaceDownTiles, set.OneFaceRightTiles,
							set.TwoFaceUpRightTiles, set.TwoFaceDownRightTiles,
							set.ThreeFaceRightTiles
						);
					}

					#endregion OneUp

					break;

				case DirectionToSet.OneFaceDown:

					#region OneDown

					if (selectedOptionIndex == 3)
					{
						tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.FilledTiles,
						set.ForeGroundTiles,
						set.HorizontalTiles,
						set.TwoFaceUpLeftTiles,
						set.TwoFaceUpRightTiles,
						set.ThreeFaceUpTiles
					);

						tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
							set.FilledTiles,
							set.FourFaceTiles,
							set.VerticalTiles,
							set.TwoFaceUpLeftTiles,
							set.TwoFaceUpRightTiles,
							set.ThreeFaceUpTiles,
							set.ThreeFaceLeftTiles,
							set.ThreeFaceRightTiles
						);

						tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
							set.FilledTiles,
							set.ForeGroundTiles,
							set.VerticalTiles,
							set.TwoFaceUpLeftTiles,
							set.TwoFaceDownLeftTiles,
							set.ThreeFaceLeftTiles
						);

						tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
							set.FilledTiles,
							set.ForeGroundTiles,
							set.VerticalTiles,
							set.TwoFaceUpRightTiles,
							set.TwoFaceDownRightTiles,
							set.ThreeFaceRightTiles
						);
					}
					else
					{
						tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
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

						tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
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

						tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
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

						tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
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
					}

					#endregion OneDown

					break;

				case DirectionToSet.OneFaceLeft:

					#region OneLeft

					if (selectedOptionIndex == 3)
					{
						tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
							set.FilledTiles,
							set.ForeGroundTiles,
							set.HorizontalTiles,
							set.TwoFaceUpLeftTiles,
							set.TwoFaceUpRightTiles,
							set.ThreeFaceUpTiles
						);

						tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
							set.FilledTiles,
							set.ForeGroundTiles,
							set.HorizontalTiles,
							set.TwoFaceDownLeftTiles,
							set.TwoFaceDownRightTiles,
							set.ThreeFaceDownTiles
						);

						tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
							set.FilledTiles,
							set.FourFaceTiles,
							set.HorizontalTiles,
							set.TwoFaceUpRightTiles,
							set.TwoFaceDownRightTiles,
							set.ThreeFaceUpTiles,
							set.ThreeFaceDownTiles,
							set.ThreeFaceRightTiles
						);

						tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
							set.FilledTiles,
							set.ForeGroundTiles,
							set.VerticalTiles,
							set.TwoFaceUpRightTiles,
							set.TwoFaceDownRightTiles,
							set.ThreeFaceRightTiles
						);
					}
					else
					{
						tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
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

						tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
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

						tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
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

						tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
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
					}

					#endregion OneLeft

					break;

				case DirectionToSet.OneFaceRight:

					#region OneRight

					if (selectedOptionIndex == 3)
					{
						tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
							set.FilledTiles,
							set.ForeGroundTiles,
							set.HorizontalTiles,
							set.TwoFaceUpLeftTiles,
							set.TwoFaceUpRightTiles,
							set.ThreeFaceUpTiles
						);

						tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
							set.FilledTiles,
							set.ForeGroundTiles,
							set.HorizontalTiles,
							set.TwoFaceDownLeftTiles,
							set.TwoFaceDownRightTiles,
							set.ThreeFaceDownTiles
						);

						tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
							set.FilledTiles,
							set.ForeGroundTiles,
							set.VerticalTiles,
							set.TwoFaceDownLeftTiles,
							set.ThreeFaceLeftTiles
						);

						tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
							set.FilledTiles,
							set.FourFaceTiles,
							set.HorizontalTiles,
							set.TwoFaceUpLeftTiles,
							set.TwoFaceDownLeftTiles,
							set.ThreeFaceUpTiles,
							set.ThreeFaceDownTiles,
							set.ThreeFaceLeftTiles
						);
					}
					else
					{
						tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
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

						tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
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

						tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
							set.FilledTiles,
							set.ForeGroundTiles,
							set.VerticalTiles,
							set.OneFaceUpTiles,
							set.OneFaceDownTiles,
							set.OneFaceLeftTiles,
							set.TwoFaceDownLeftTiles,
							set.ThreeFaceLeftTiles
						);

						tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
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
					}

					#endregion OneRight

					break;

				#endregion One

				#region VH

				case DirectionToSet.Vertical:

					#region Vertical

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
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

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
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

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
						set.FilledTiles,
						set.ForeGroundTiles
					);

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.FilledTiles,
						set.ForeGroundTiles
					);

					#endregion Vertical

					break;

				case DirectionToSet.Horizontal:

					#region Horizontal

					tileInput.compatibleTop = SubCombine(tileInput.compatibleTop, top,
						set.FilledTiles,
						set.ForeGroundTiles
					);

					tileInput.compatibleBottom = SubCombine(tileInput.compatibleBottom, bottom,
						set.FilledTiles,
						set.ForeGroundTiles
					);

					tileInput.compatibleLeft = SubCombine(tileInput.compatibleLeft, left,
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

					tileInput.compatibleRight = SubCombine(tileInput.compatibleRight, right,
						set.FilledTiles,
						set.FourFaceTiles,
						set.HorizontalTiles,
						set.OneFaceLeftTiles,
						set.TwoFaceDownLeftTiles,
						set.ThreeFaceUpTiles,
						set.ThreeFaceDownTiles,
						set.ThreeFaceLeftTiles
					);

					#endregion Horizontal

					break;

				#endregion VH

				default:
					break;
			}

			top.serializedObject.ApplyModifiedProperties();
			bottom.serializedObject.ApplyModifiedProperties();
			left.serializedObject.ApplyModifiedProperties();
			right.serializedObject.ApplyModifiedProperties();
			temp.ApplyModifiedProperties();

			return tileInput;
		}

		private List<TileInput> SubCombine(List<TileInput> tileConstraintsList, SerializedProperty property, params List<TileInput>[] tilesToAdd)
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

			property.ClearArray();
			for (int i = 0; i < tileConstraintsList.Count; i++)
			{
				property.arraySize++;
				property.GetArrayElementAtIndex(i).objectReferenceValue = tileConstraintsList[i];
			}
			return tileConstraintsList;
		}

		private void DrawLeft()
		{
			GUILayout.BeginArea(tileSetupSection);
			scrollPositionLeft = EditorGUILayout.BeginScrollView(scrollPositionLeft, GUILayout.ExpandWidth(true), GUILayout.Height(position.height));

			EditorGUILayout.BeginVertical();

			ButtonCategory("All Tiles", 0);
			ShowTileSets(foldout[0], allInput, textureAllInput);

			ButtonCategory("Filled Tiles", 1);
			ShowTileSets(foldout[1], foreground, textureForeground);
			ShowTileSets(foldout[1], background, textureBackground);
			ShowTileSets(foldout[1], four, textureFour);
			ShowTileSets(foldout[1], filled, textureFilled);

			ButtonCategory("Edge Tiles", 2);
			ShowTileSets(foldout[2], edgeup, textureEdgeup);
			ShowTileSets(foldout[2], edgedown, textureEdgedown);
			ShowTileSets(foldout[2], edgeleft, textureEdgeleft);
			ShowTileSets(foldout[2], edgeright, textureEdgeright);

			ButtonCategory("Elbow Tiles", 3);
			ShowTileSets(foldout[3], elbowUL, textureElbowUL);
			ShowTileSets(foldout[3], elbowUR, textureElbowUR);
			ShowTileSets(foldout[3], elbowDL, textureElbowDL);
			ShowTileSets(foldout[3], elbowDR, textureElbowDR);

			ButtonCategory("Corner Tiles", 4);
			ShowTileSets(foldout[4], cornerUL, textureCornerUL);
			ShowTileSets(foldout[4], cornerUR, textureCornerUR);
			ShowTileSets(foldout[4], cornerDL, textureCornerDL);
			ShowTileSets(foldout[4], cornerDR, textureCornerDR);
			ShowTileSets(foldout[4], cornerULDR, textureCornerULDR);
			ShowTileSets(foldout[4], cornerURDL, textureCornerURDL);

			ButtonCategory("Vertical / Horizontal Tiles", 5);
			ShowTileSets(foldout[5], vertical, textureVertical);
			ShowTileSets(foldout[5], horizontal, textureHorizontal);

			ButtonCategory("One Face Tiles", 6);
			ShowTileSets(foldout[6], oneU, textureOneU);
			ShowTileSets(foldout[6], oneD, textureOneD);
			ShowTileSets(foldout[6], oneL, textureOneL);
			ShowTileSets(foldout[6], oneR, textureOneR);

			ButtonCategory("Two Face / Curved Tiles", 7);
			ShowTileSets(foldout[7], twoUL, textureTwoUL);
			ShowTileSets(foldout[7], twoUR, textureTwoUR);
			ShowTileSets(foldout[7], twoDL, textureTwoDL);
			ShowTileSets(foldout[7], twoDR, textureTwoDR);

			ButtonCategory("Three Face Tiles", 8);
			ShowTileSets(foldout[8], threeU, textureThreeU);
			ShowTileSets(foldout[8], threeD, textureThreeD);
			ShowTileSets(foldout[8], threeL, textureThreeL);
			ShowTileSets(foldout[8], threeR, textureThreeR);

			EditorGUILayout.EndVertical();
			EditorGUILayout.Space(100);
			EditorGUILayout.EndScrollView();
			EditorGUILayout.Space(20);
			GUILayout.EndArea();
		}

		private void ButtonCategory(string name, int show)
		{
			if (GUILayout.Button(name, GUILayout.Height(30), GUILayout.Width(281)))
			{
				if (selectedInputTileSet != null)
				{
					SerializeProperties();
					foldout[show] = !foldout[show];
				}
			}
		}

		private void ShowTileSets(bool showTileset, SerializedProperty property, Texture2D pic)
		{
			if (showTileset)
			{
				EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(270));
				GUILayout.Label(pic);
				EditorGUILayout.LabelField(property.name, EditorStyles.largeLabel, GUILayout.MaxWidth(170));

				// Array size field
				int currentArraySize = property.arraySize;
				int newArraySize = EditorGUILayout.IntField(currentArraySize, GUILayout.Width(40));
				if (newArraySize != currentArraySize)
				{
					property.arraySize = newArraySize;
				}

				// Plus button
				if (GUILayout.Button(" +", EditorStyles.miniButtonRight, GUILayout.Width(20)))
				{
					property.arraySize++;
					property.serializedObject.ApplyModifiedProperties();
				}

				// Minus button
				if (GUILayout.Button(" -", EditorStyles.miniButtonRight, GUILayout.Width(20)))
				{
					if (property.arraySize > 0)
					{
						property.arraySize--;
						property.serializedObject.ApplyModifiedProperties();
					}
				}

				GUILayout.FlexibleSpace();

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Space(5);

				for (int i = 0; i < property.arraySize; i++)
				{
					SerializedProperty elementProperty = property.GetArrayElementAtIndex(i);

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField($"Item {i + 1}", EditorStyles.miniLabel, GUILayout.MaxWidth(65));
					EditorGUILayout.PropertyField(elementProperty, GUIContent.none, GUILayout.MaxWidth(175));

					if (GUILayout.Button(">", EditorStyles.toolbarButton, GUILayout.Width(30)))
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
			else if (selectedTileConstraints == null)
			{
				EditorGUILayout.LabelField("- Click a tile category from the right panel");
				EditorGUILayout.LabelField("- Press the + and - buttons to add new items");
				EditorGUILayout.LabelField("- Select an item from the tile list by pressing the > button");
				EditorGUILayout.LabelField("- Make sure to fill the item with an input tile then press the > button");
				EditorGUILayout.LabelField("- Dont have input tiles? you should plan, create and setup input tiles first");
			}
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

					if (previewTexture == null)
					{
						GUILayout.Label(Texture2D.blackTexture, GUILayout.Width(80), GUILayout.Height(80));
					}
					else
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

					EditorGUILayout.BeginVertical(GUILayout.MaxWidth(390));
					EditorGUILayout.PropertyField(compatibleTopList, true);
					EditorGUILayout.PropertyField(compatibleBottomList, true);
					EditorGUILayout.PropertyField(compatibleLeftList, true);
					EditorGUILayout.PropertyField(compatibleRightList, true);
					EditorGUILayout.EndVertical();

					selectedTileConstraints.ApplyModifiedProperties();
				}
			}

			EditorGUILayout.EndScrollView();
			EditorGUILayout.Space(20);
			GUILayout.EndArea();
		}

		private void GetAllUniqueInputTiles()
		{
			allInput.ClearArray();
			UpdateSerializedProperties();
			GetUniqueTiles(
				selectedInputTileSet.ForeGroundTiles,
				selectedInputTileSet.BackGroundTiles,
				selectedInputTileSet.FilledTiles,
				selectedInputTileSet.EdgeUpTiles,
				selectedInputTileSet.EdgeDownTiles,
				selectedInputTileSet.EdgeLeftTiles,
				selectedInputTileSet.EdgeRightTiles,
				selectedInputTileSet.ElbowUpLeftTiles,
				selectedInputTileSet.ElbowUpRightTiles,
				selectedInputTileSet.ElbowDownLeftTiles,
				selectedInputTileSet.ElbowDownRightTiles,
				selectedInputTileSet.CornerUpLeftTiles,
				selectedInputTileSet.CornerUpRightTiles,
				selectedInputTileSet.CornerDownLeftTiles,
				selectedInputTileSet.CornerDownRightTiles,
				selectedInputTileSet.CornerULDRTiles,
				selectedInputTileSet.CornerURDLTiles,
				selectedInputTileSet.FourFaceTiles,
				selectedInputTileSet.VerticalTiles,
				selectedInputTileSet.HorizontalTiles,
				selectedInputTileSet.TwoFaceUpLeftTiles,
				selectedInputTileSet.TwoFaceUpRightTiles,
				selectedInputTileSet.TwoFaceDownLeftTiles,
				selectedInputTileSet.TwoFaceDownRightTiles,
				selectedInputTileSet.ThreeFaceUpTiles,
				selectedInputTileSet.ThreeFaceDownTiles,
				selectedInputTileSet.ThreeFaceLeftTiles,
				selectedInputTileSet.ThreeFaceRightTiles,
				selectedInputTileSet.OneFaceUpTiles,
				selectedInputTileSet.OneFaceDownTiles,
				selectedInputTileSet.OneFaceLeftTiles,
				selectedInputTileSet.OneFaceRightTiles);

			serializedTileSetObject.Update();
		}

		private void GetUniqueTiles(params List<TileInput>[] list)
		{
			int count = 0;
			foreach (var set in list)
			{
				foreach (var item in set)
				{
					serializedTileSetObject.Update();
					//Debug.Log(allInput.arraySize);
					if (!selectedInputTileSet.AllInputTiles.Contains(item))
					{
						allInput.InsertArrayElementAtIndex(count);
						allInput.GetArrayElementAtIndex(count).objectReferenceValue = item;
						count++;
						allInput.serializedObject.ApplyModifiedProperties();
					}
				}
			}
		}

		private void GiveUniqueIDToTiles()
		{
			//serializeproperty.intValue doesnt work
			//Converted the serialized property into a list, directly modified the list
			//IDK what happened it did not modified the serialized property directly but it still works
			//Could it be that object reference value passes the reference?
			serializedTileSetObject.Update();

			SerializedProperty listProperty = serializedTileSetObject.FindProperty("AllInputTiles");
			if (listProperty != null && listProperty.isArray)
			{
				List<TileInput> tileList = GetListFromSerializedProperty(listProperty);
				if (tileList != null)
				{
					for (int i = 0; i < tileList.Count; i++)
					{
						TileInput tile = tileList[i];
						if (tile != null)
						{
							tile.id = i + 1;
						}
						else
						{
							Debug.Log("TileInput element at index " + i + " is null");
						}
					}

					serializedTileSetObject.ApplyModifiedProperties();
				}
				else
				{
					Debug.Log("Unable to retrieve List<TileInput> from serialized property");
				}
			}
			else
			{
				Debug.Log("List<TileInput> property is null or not a list");
			}
		}

		// Helper method to retrieve List<T> from a serialized property
		private List<TileInput> GetListFromSerializedProperty(SerializedProperty property)
		{
			List<TileInput> list = new();

			for (int i = 0; i < property.arraySize; i++)
			{
				SerializedProperty elementProperty = property.GetArrayElementAtIndex(i);
				TileInput element = elementProperty.objectReferenceValue as TileInput;
				list.Add(element);
			}

			return list;
		}

		private void ClearAllInputTiles()
		{
			ClearAllInputTilesInDirection(
				selectedInputTileSet.AllInputTiles,
				selectedInputTileSet.ForeGroundTiles,
				selectedInputTileSet.BackGroundTiles,
				selectedInputTileSet.FilledTiles,
				selectedInputTileSet.EdgeUpTiles,
				selectedInputTileSet.EdgeDownTiles,
				selectedInputTileSet.EdgeLeftTiles,
				selectedInputTileSet.EdgeRightTiles,
				selectedInputTileSet.ElbowUpLeftTiles,
				selectedInputTileSet.ElbowUpRightTiles,
				selectedInputTileSet.ElbowDownLeftTiles,
				selectedInputTileSet.ElbowDownRightTiles,
				selectedInputTileSet.CornerUpLeftTiles,
				selectedInputTileSet.CornerUpRightTiles,
				selectedInputTileSet.CornerDownLeftTiles,
				selectedInputTileSet.CornerDownRightTiles,
				selectedInputTileSet.CornerULDRTiles,
				selectedInputTileSet.CornerURDLTiles,
				selectedInputTileSet.FourFaceTiles,
				selectedInputTileSet.VerticalTiles,
				selectedInputTileSet.HorizontalTiles,
				selectedInputTileSet.TwoFaceUpLeftTiles,
				selectedInputTileSet.TwoFaceUpRightTiles,
				selectedInputTileSet.TwoFaceDownLeftTiles,
				selectedInputTileSet.TwoFaceDownRightTiles,
				selectedInputTileSet.ThreeFaceUpTiles,
				selectedInputTileSet.ThreeFaceDownTiles,
				selectedInputTileSet.ThreeFaceLeftTiles,
				selectedInputTileSet.ThreeFaceRightTiles,
				selectedInputTileSet.OneFaceUpTiles,
				selectedInputTileSet.OneFaceDownTiles,
				selectedInputTileSet.OneFaceLeftTiles,
				selectedInputTileSet.OneFaceRightTiles
			);
		}

		private void ClearAllInputTilesInDirection(params List<TileInput>[] tilesInDirectionList)
		{
			foreach (var item in tilesInDirectionList)
			{
				item.Clear();
			}
		}

		private void ClearAllInputTileConstraints()
		{
			SerializedObject temp = new(selectedInputTileSet);

			ClearAllInputTileConstraintsInDirection(
				selectedInputTileSet.AllInputTiles,
				selectedInputTileSet.ForeGroundTiles,
				selectedInputTileSet.BackGroundTiles,
				selectedInputTileSet.FilledTiles,
				selectedInputTileSet.EdgeUpTiles,
				selectedInputTileSet.EdgeDownTiles,
				selectedInputTileSet.EdgeLeftTiles,
				selectedInputTileSet.EdgeRightTiles,
				selectedInputTileSet.ElbowUpLeftTiles,
				selectedInputTileSet.ElbowUpRightTiles,
				selectedInputTileSet.ElbowDownLeftTiles,
				selectedInputTileSet.ElbowDownRightTiles,
				selectedInputTileSet.CornerUpLeftTiles,
				selectedInputTileSet.CornerUpRightTiles,
				selectedInputTileSet.CornerDownLeftTiles,
				selectedInputTileSet.CornerDownRightTiles,
				selectedInputTileSet.CornerULDRTiles,
				selectedInputTileSet.CornerURDLTiles,
				selectedInputTileSet.FourFaceTiles,
				selectedInputTileSet.VerticalTiles,
				selectedInputTileSet.HorizontalTiles,
				selectedInputTileSet.TwoFaceUpLeftTiles,
				selectedInputTileSet.TwoFaceUpRightTiles,
				selectedInputTileSet.TwoFaceDownLeftTiles,
				selectedInputTileSet.TwoFaceDownRightTiles,
				selectedInputTileSet.ThreeFaceUpTiles,
				selectedInputTileSet.ThreeFaceDownTiles,
				selectedInputTileSet.ThreeFaceLeftTiles,
				selectedInputTileSet.ThreeFaceRightTiles,
				selectedInputTileSet.OneFaceUpTiles,
				selectedInputTileSet.OneFaceDownTiles,
				selectedInputTileSet.OneFaceLeftTiles,
				selectedInputTileSet.OneFaceRightTiles
			);

			temp.ApplyModifiedProperties();
			temp.Update();
		}

		private void ClearAllInputTileConstraintsInDirection(params List<TileInput>[] tileInDirectionList)
		{
			foreach (var set in tileInDirectionList)
			{
				foreach (var item in set)
				{
					if (item == null)
					{
						continue;
					}
					else
					{
						SerializedObject temp = new(item);
						temp.FindProperty("compatibleTop").ClearArray();
						temp.FindProperty("compatibleBottom").ClearArray();
						temp.FindProperty("compatibleLeft").ClearArray();
						temp.FindProperty("compatibleRight").ClearArray();
						temp.ApplyModifiedProperties();
						item.compatibleTop.Clear();
						item.compatibleBottom.Clear();
						item.compatibleLeft.Clear();
						item.compatibleRight.Clear();
					}
				}
			}
		}
	}
}