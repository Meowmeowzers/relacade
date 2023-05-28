using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HelloWorld.Editor
{
    public class TileSetupWindow : EditorWindow
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

        private static readonly int windowMinWidth = 600;
        private static readonly int windowMinHeight = 350;

        private Vector2 scrollPositionLeft = Vector2.zero;
        private Vector2 scrollPositionRight = Vector2.zero;

        private bool shouldClear = false;

        private int selectedOptionIndex = 0;
        private readonly string[] menuOptions = { "Full Combine", "2x2 Only (TODO)", "3x3 Only (TODO)", "3x3 - No 1D together" };
        GenericMenu menu;

        private bool[] foldout = { false, false, false, false, false, false, false, false, false };

        #endregion Window Variables

        [MenuItem("Relacade/Manual Tile Set Configuration")]
        private static void StartWindow()
        {
            TileSetupWindow window = (TileSetupWindow)GetWindow(typeof(TileSetupWindow));
            window.minSize = new(windowMinWidth, windowMinHeight);
            window.Show();
        }

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
            EditorGUILayout.LabelField("Tile Set Config", GUILayout.Width(100), GUILayout.MaxWidth(100));
            selectedInputTileSet = (TileInputSet)EditorGUILayout.ObjectField(selectedInputTileSet, typeof(TileInputSet), false, GUILayout.MaxWidth(300));

            GUILayout.FlexibleSpace();

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

        private void DrawLeft()
        {
            GUILayout.BeginArea(tileSetupSection);
            scrollPositionLeft = EditorGUILayout.BeginScrollView(scrollPositionLeft, false, true);

            EditorGUILayout.BeginVertical();

            ShowTileSets(true, allInput, textureAllInput);

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(100);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space(20);
            GUILayout.EndArea();
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
                }

                // Minus button
                if (GUILayout.Button(" -", EditorStyles.miniButtonRight, GUILayout.Width(20)))
                {
                    if (property.arraySize > 0)
                    {
                        property.arraySize--;
                    }
                }

                GUILayout.FlexibleSpace();

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(5);

                for (int i = 0; i < property.arraySize; i++)
                {
                    SerializedProperty elementProperty = property.GetArrayElementAtIndex(i);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(elementProperty, GUIContent.none, GUILayout.MaxWidth(175), GUILayout.Height(40));

                    if (GUILayout.Button(">", GUILayout.Height(40)))
                    {
                        if (elementProperty.objectReferenceValue != null)
                        {
                            Object elementReference = elementProperty.objectReferenceValue;
                            selectedTileConstraints = new(elementReference);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space(10);
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
            //GetUniqueTiles(selectedInputTileSet.AllInputTiles);
            selectedInputTileSet.AllInputTiles.Clear();
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
            for (int i = 0; i < selectedInputTileSet.AllInputTiles.Count; i++)
            {
                if (selectedInputTileSet.AllInputTiles[i] == null) continue;
                selectedInputTileSet.AllInputTiles[i].id = i + 1;
            }
        }

        private void ClearAllInputTileConstraints()
        {
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
        }

        private void ClearAllInputTileConstraintsInDirection(params List<TileInput>[] tileInDirectionList)
        {
            foreach (var set in tileInDirectionList)
            {
                foreach (var item in set)
                {
                    if (item == null)
                    {
                        //continue if item is null
                        continue;
                    }
                    else
                    {
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