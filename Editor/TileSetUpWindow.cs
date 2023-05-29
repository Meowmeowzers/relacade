using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

        private void SerializeProperties()
        {
            serializedTileSetObject = new(selectedInputTileSet);

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

            serializedTileSetObject.Update();
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

            if (GUILayout.Button("Auto Set ID", EditorStyles.toolbarButton, GUILayout.MaxWidth(200)))
            {
                if (selectedInputTileSet != null /*& serializedTileSetObject != null*/)
                {
                    ClearAllInputTileConstraints();
                    GiveUniqueIDToTiles();
                    UpdateSerializedProperties();
                }
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

            if (selectedInputTileSet != null)
            {
                SerializeProperties();
                ShowTileSets(true, allInput, textureAllInput);
            }

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
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(250));
                GUILayout.Label(pic);
                EditorGUILayout.LabelField("All Input Tiles", EditorStyles.largeLabel, GUILayout.MaxWidth(170));

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
                    if (property.GetArrayElementAtIndex(i).objectReferenceValue != null)
                    {
                        SerializedProperty elementProperty = property.GetArrayElementAtIndex(i);
                        SerializedObject gameObjectProperty = new(property.GetArrayElementAtIndex(i).objectReferenceValue);
                        SerializedProperty texture = gameObjectProperty.FindProperty("gameObject");

                        EditorGUILayout.BeginHorizontal();
                        Texture2D previewTexture = AssetPreview.GetAssetPreview(texture.objectReferenceValue);
                        GUILayout.Label(previewTexture, GUILayout.Width(50), GUILayout.Height(50));

                        EditorGUILayout.LabelField($"Item {i + 1}", EditorStyles.miniLabel, GUILayout.MaxWidth(60));
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
                    else
                    {
                        SerializedProperty elementProperty = property.GetArrayElementAtIndex(i);
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label(Texture2D.blackTexture, GUILayout.Width(50), GUILayout.Height(50));
                        EditorGUILayout.LabelField($"Item {i + 1}", EditorStyles.miniLabel, GUILayout.MaxWidth(60));
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
                }

                EditorGUILayout.Space(5);
                property.serializedObject.ApplyModifiedProperties();
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
                        //continue if item is null
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