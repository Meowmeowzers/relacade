using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HelloWorld.Editor
{
    public class NewSetUpWindow : EditorWindow
    {
        private TileInputSet selectedInputTileSet;
        private SerializedObject serializedTileSetObject;
        private SerializedProperty allInput;

        //Right
        private SerializedObject selectedTileConstraints;

        private SerializedProperty tileName;
        private SerializedProperty gameObject;
        private SerializedProperty weight;
        private SerializedProperty compatibleTopList;
        private SerializedProperty compatibleBottomList;
        private SerializedProperty compatibleLeftList;
        private SerializedProperty compatibleRightList;
        private Texture2D previewTexture;

        private TileInput tempInputTile;
        private string assetName = "New Tile Set";

        private bool[] showCompatibleTiles = { true, true, true, true, true, true };
        private int tempIndex = 0;
        private GUIStyle customStyle = new();

        private GenericMenu DeletesDropdown = new GenericMenu();
        private bool shouldClear = false;

        #region Window Variables

        private readonly int headerSectionHeight = 30;
        private readonly int tileSetupSectionWidth = 312;

        private Texture2D headerBackgroundTexture;
        private Texture2D leftBackgroundTexture;
        private Texture2D rightBackgroundTexture;
        private Texture2D listItemTexture;
        private Texture2D listItemLightTexture;

        private Color headerBackgroundColor = new(30f / 255f, 30f / 255f, 30f / 255f, 0.5f);
        private Color leftBackgroundColor = new(30f / 255f, 30f / 255f, 30f / 255f, 0.5f);
        private Color rightBackgroundColor = new(0.6f, .2f, 0.7f, 0.7f);
        private Color listItemColor = new(0.18f, 0.18f, 0.18f, 1f);
        private Color listItemLightColor = new(0.2f, 0.2f, 0.2f, 1f);

        private Rect headerSection;
        private Rect tileSetupSection;
        private Rect tileConstraintSetupSection;

        private Vector2 scrollPositionLeft = Vector2.zero;
        private Vector2 scrollPositionRight = Vector2.zero;

        private GUIContent weightLabel = new("Weight", "Higher weight means more chance to spawn");
        private GUIContent gameObjectLabel = new("GameObject", "GameObject to spawn");
        private GUIContent sendToLabel = new("Send To", "Send this tile as constraints for other tiles");
        private GUIContent receiveFromLabel = new("Receive From", "Receive constraints from other tile");
        private GUIContent compatibleTopLabel = new("Compatible Top", "Compatible tiles for adjacent top");
        private GUIContent compatibleBottomLabel = new("Compatible Bottom", "Compatible tiles for adjacent bottom");
        private GUIContent compatibleLeftLabel = new("Compatible Left", "Compatible tiles for adjacent left");
        private GUIContent compatibleRightLabel = new("Compatible Right", "Compatible tiles for adjacent right");

        #endregion Window Variables

        private void OnEnable()
        {
            InitTextures();
            InitData();
            InitDeletesMenu();
        }

        private void OnDestroy()
        {
            GetWindow<ReceiveConstraintsWindow>().Close();
            GetWindow<PassConstraintsWindow>().Close();
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

            listItemTexture = new Texture2D(1, 1);
            listItemTexture.SetPixel(0, 0, listItemColor);
            listItemTexture.Apply();

            listItemLightTexture = new Texture2D(1, 1);
            listItemLightTexture.SetPixel(0, 0, listItemLightColor);
            listItemLightTexture.Apply();

            customStyle.normal.background = listItemTexture;
            customStyle.hover.background = listItemLightTexture;
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
        }

        private void InitDeletesMenu()
        {
            DeletesDropdown.AddItem(new GUIContent("Delete this tile"), false, () =>
            {
                if (shouldClear && selectedTileConstraints != null)
                {
                    RemoveAndDeleteTile(allInput, tempIndex);
                    GiveUniqueIDToTiles();
                    selectedTileConstraints = null;
                    shouldClear = false;
                }
            });
            DeletesDropdown.AddItem(new GUIContent("Clear tile constraints"), false, () =>
            {
                if (shouldClear && selectedTileConstraints != null)
                {
                    ClearInputTileConstraints();
                    selectedTileConstraints = null;
                    shouldClear = false;
                }
            });
            DeletesDropdown.AddSeparator("/");
            DeletesDropdown.AddItem(new GUIContent("Delete all tiles"), false, () =>
            {
                if (shouldClear && allInput != null)
                {
                    RemoveAndDeleteAllTiles(allInput);
                    GiveUniqueIDToTiles();
                    shouldClear = false;
                }
            });
            DeletesDropdown.AddItem(new GUIContent("Clear all tile constraints"), false, () =>
            {
                if (shouldClear && allInput != null)
                {
                    ClearAllInputTileConstraints();
                    shouldClear = false;
                }
            });
        }

        private void DrawHeader()
        {
            GUILayout.BeginArea(headerSection);
            EditorGUILayout.Space(3);
            GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(position.width));

            EditorGUILayout.LabelField("Tile Set Config", GUILayout.Width(100), GUILayout.MaxWidth(100));
            selectedInputTileSet = (TileInputSet)EditorGUILayout.ObjectField(selectedInputTileSet, typeof(TileInputSet), false, GUILayout.MaxWidth(200));

            if (GUILayout.Button("Auto Set ID", EditorStyles.toolbarButton, GUILayout.MaxWidth(100), GUILayout.ExpandWidth(false)))
            {
                if (selectedInputTileSet != null)
                {
                    GiveUniqueIDToTiles();
                    UpdateSerializedProperties();
                }
            }
            GUILayout.FlexibleSpace();
            shouldClear = EditorGUILayout.Toggle("", shouldClear, GUILayout.Width(15));

            if (GUILayout.Button("Options", EditorStyles.toolbarDropDown))
            {
                DeletesDropdown.ShowAsContext();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DrawLeft()
        {
            GUILayout.BeginArea(tileSetupSection);
            scrollPositionLeft = EditorGUILayout.BeginScrollView(scrollPositionLeft, GUILayout.ExpandWidth(true), GUILayout.Height(position.height));

            EditorGUILayout.BeginVertical();

            if (selectedInputTileSet != null)
            {
                SerializeProperties();
                ShowTileSets(allInput);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(100);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space(20);
            GUILayout.EndArea();
        }

        private void ShowTileSets(SerializedProperty property)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(property.arraySize.ToString() + " Input Tile/s", EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            SerializedProperty elementProperty;
            SerializedObject gameObjectProperty;
            SerializedProperty texture;
            Texture2D previewTexture;

            for (int i = 0; i < property.arraySize; i++)
            {
                if (property.GetArrayElementAtIndex(i).objectReferenceValue != null)
                {
                    elementProperty = property.GetArrayElementAtIndex(i);
                    gameObjectProperty = new(property.GetArrayElementAtIndex(i).objectReferenceValue);
                    texture = gameObjectProperty.FindProperty("gameObject");
                    previewTexture = AssetPreview.GetAssetPreview(texture.objectReferenceValue);

                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(150));
                    GUILayout.Label(previewTexture, GUILayout.Width(50), GUILayout.Height(50));

                    CenterVerticalStart(50);
                    EditorGUILayout.LabelField(selectedInputTileSet.AllInputTiles[i].tileName, EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
                    CenterVerticalEnd();

                    CenterVerticalStart(50);
                    if (GUILayout.Button(">", EditorStyles.toolbarButton, GUILayout.Width(30)))
                    {
                        if (elementProperty.objectReferenceValue != null)
                        {
                            Object elementReference = elementProperty.objectReferenceValue;
                            selectedTileConstraints = new(elementReference);
                            tempIndex = i;
                        }
                        else
                        {
                            selectedTileConstraints = null;
                            tempIndex = i;
                        }
                    }
                    CenterVerticalEnd();
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();

                    CenterVerticalStart(50);
                    EditorGUILayout.LabelField("Empty", EditorStyles.miniLabel, GUILayout.MaxWidth(50));
                    CenterVerticalEnd();

                    CenterVerticalStart(50);
                    if (GUILayout.Button(">", EditorStyles.toolbarButton, GUILayout.Width(30)))
                    {
                        selectedTileConstraints = null;
                        tempIndex = i;
                    }
                    CenterVerticalEnd();

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("+", GUILayout.Width(35), GUILayout.Height(35)))
            {
                CreateAndAddTile(property);
                GiveUniqueIDToTiles();
            }
            if (GUILayout.Button("-", GUILayout.Width(35), GUILayout.Height(35)))
            {
                if (property.arraySize > 0)
                {
                    RemoveAndDeleteLastTile(property);
                    GiveUniqueIDToTiles();
                }
            }
            if (GUILayout.Button("Reload", GUILayout.Width(65), GUILayout.Height(35)))
            {
                if (property.arraySize > 0)
                {
                    CheckListForNull(property);
                    GiveUniqueIDToTiles();
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            property.serializedObject.ApplyModifiedProperties();
        }

        private void DrawRight()
        {
            GUILayout.BeginArea(tileConstraintSetupSection);
            scrollPositionRight = EditorGUILayout.BeginScrollView(scrollPositionRight);

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField("Tile Set Up", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            if (selectedInputTileSet == null)
            {
                EditorGUILayout.BeginVertical(GUILayout.Width(100));
                EditorGUILayout.LabelField("Load an input tile set");
                selectedInputTileSet = (TileInputSet)EditorGUILayout.ObjectField(selectedInputTileSet, typeof(TileInputSet), false, GUILayout.Height(30));
                GUILayout.Space(10);
                EditorGUILayout.LabelField("or");
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Create a new input tile set");
                CreateTileSetButton();
                EditorGUILayout.EndVertical();
            }
            else if (allInput.arraySize == 0)
            {
                GUILayout.Space(15);
                EditorGUILayout.HelpBox("* Press the + and - buttons to add new items  \n* Then, select an item from the tile list by pressing the > button", MessageType.Info);
            }
            else if (selectedTileConstraints == null)
            {
                GUILayout.Space(15);
                EditorGUILayout.BeginVertical();
                EditorGUILayout.HelpBox("Select an Input Tile", MessageType.Info);
                EditorGUILayout.EndVertical();
            }
            else
            {
                if (selectedTileConstraints != null) selectedTileConstraints.Update();

                EditorGUILayout.Space(5);

                tileName = selectedTileConstraints.FindProperty("tileName");
                gameObject = selectedTileConstraints.FindProperty("gameObject");
                weight = selectedTileConstraints.FindProperty("weight");
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

                CenterVerticalStart(80);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Name", GUILayout.Width(80));
                EditorGUILayout.PropertyField(tileName, GUIContent.none);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(gameObjectLabel, GUILayout.Width(80));
                EditorGUILayout.PropertyField(gameObject, GUIContent.none);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(weightLabel, GUILayout.Width(80));
                EditorGUILayout.PropertyField(weight, GUIContent.none);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(sendToLabel, GUILayout.Height(30)))
                {
                    PassConstraintsWindow.OpenWindow(selectedTileConstraints, allInput);
                }
                if (GUILayout.Button(receiveFromLabel, GUILayout.Height(30)))
                {
                    ReceiveConstraintsWindow.OpenWindow(selectedTileConstraints, allInput);
                }
                EditorGUILayout.EndHorizontal();

                CenterVerticalEnd();

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical(GUILayout.MaxWidth(390));

                showCompatibleTiles[0] = EditorGUILayout.BeginFoldoutHeaderGroup(showCompatibleTiles[0], compatibleTopLabel, EditorStyles.foldoutHeader);
                if (showCompatibleTiles[0])
                {
                    ShowCompatibleTilesList(compatibleTopList);
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                showCompatibleTiles[1] = EditorGUILayout.BeginFoldoutHeaderGroup(showCompatibleTiles[1], compatibleBottomLabel, EditorStyles.foldoutHeader);
                if (showCompatibleTiles[1])
                {
                    ShowCompatibleTilesList(compatibleBottomList);
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                showCompatibleTiles[2] = EditorGUILayout.BeginFoldoutHeaderGroup(showCompatibleTiles[2], compatibleLeftLabel, EditorStyles.foldoutHeader);
                if (showCompatibleTiles[2])
                {
                    ShowCompatibleTilesList(compatibleLeftList);
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                showCompatibleTiles[3] = EditorGUILayout.BeginFoldoutHeaderGroup(showCompatibleTiles[3], compatibleRightLabel, EditorStyles.foldoutHeader);
                if (showCompatibleTiles[3])
                {
                    ShowCompatibleTilesList(compatibleRightList);
                }
                EditorGUILayout.EndFoldoutHeaderGroup();

                EditorGUILayout.EndVertical();

                selectedTileConstraints.ApplyModifiedProperties();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space(20);
            GUILayout.EndArea();
        }

        private void ShowCompatibleTilesList(SerializedProperty property)
        {
            for (int i = 0; i < property.arraySize; i++)
            {
                tempInputTile = property.GetArrayElementAtIndex(i).objectReferenceValue as TileInput;
                previewTexture = AssetPreview.GetAssetPreview(tempInputTile.gameObject);
                EditorGUILayout.BeginHorizontal(customStyle);
                GUILayout.Label(previewTexture, GUILayout.Height(30), GUILayout.Width(30));
                EditorGUILayout.LabelField(tempInputTile.tileName, GUILayout.Height(30));
                EditorGUILayout.EndHorizontal();
            }
        }

        private void CreateTileSetButton()
        {
            assetName = EditorGUILayout.TextField(assetName, GUILayout.Width(250));

            if (GUILayout.Button("Create New Input Tile Set", GUILayout.Height(30)))
            {
                ScriptableObject scriptableObject = CreateInstance<TileInputSet>();
                string location = EditorUtility.SaveFilePanelInProject("Create new input tile set", assetName, "asset", "?Insert message?");

                AssetDatabase.CreateAsset(scriptableObject, location);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                selectedInputTileSet = scriptableObject as TileInputSet;
            }
        }

        // Methods
        private void CreateAndAddTile(SerializedProperty property)
        {
            property.arraySize++;

            TileInput newTileInput = CreateInstance<TileInput>();
            newTileInput.tileName = "New Input Tile";
            newTileInput.id = allInput.arraySize - 1;
            newTileInput.name = selectedInputTileSet.name + newTileInput.id;

            AssetDatabase.AddObjectToAsset(newTileInput, selectedInputTileSet);
            AssetDatabase.SaveAssets();

            property.GetArrayElementAtIndex(property.arraySize - 1).objectReferenceValue = newTileInput;
            selectedTileConstraints = null;

            CheckListForNull(property);

            property.serializedObject.ApplyModifiedProperties();
        }

        private void RemoveAndDeleteLastTile(SerializedProperty property)
        {
            TileInput tileToDelete = (TileInput)property.GetArrayElementAtIndex(property.arraySize - 1).objectReferenceValue;
            property.DeleteArrayElementAtIndex(property.arraySize - 1);

            AssetDatabase.RemoveObjectFromAsset(tileToDelete);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            selectedTileConstraints = null;

            CheckListForNull(property);

            property.serializedObject.ApplyModifiedProperties();
        }

        private void RemoveAndDeleteTile(SerializedProperty property, int index)
        {
            TileInput tileToDelete = (TileInput)property.GetArrayElementAtIndex(index).objectReferenceValue;
            property.DeleteArrayElementAtIndex(index);

            AssetDatabase.RemoveObjectFromAsset(tileToDelete);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            selectedTileConstraints = null;

            CheckListForNull(property);

            property.serializedObject.ApplyModifiedProperties();
        }

        private void RemoveAndDeleteAllTiles(SerializedProperty tiles)
        {
            TileInput tileToDelete;

            for (int i = allInput.arraySize - 1; i >= 0; i--)
            {
                tileToDelete = tiles.GetArrayElementAtIndex(i).objectReferenceValue as TileInput;
                tiles.DeleteArrayElementAtIndex(i);
                AssetDatabase.RemoveObjectFromAsset(tileToDelete);
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            selectedTileConstraints = null;
            CheckListForNull(tiles);
            tiles.serializedObject.ApplyModifiedProperties();
        }

        private bool CheckListForNull(SerializedProperty property)
        {
            bool result = false;
            for (int i = property.arraySize - 1; i >= 0; i--)
            {
                if (property.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    property.DeleteArrayElementAtIndex(i);
                    result = true;
                }
            }
            property.serializedObject.ApplyModifiedProperties();
            return result;
        }

        private void ClearAllInputTileConstraints()
        {
            SerializedObject temp = new(selectedInputTileSet);

            ClearAllInputTileConstraintsInDirection(selectedInputTileSet.AllInputTiles);

            temp.ApplyModifiedProperties();
            temp.Update();
        }

        private void ClearAllInputTileConstraintsInDirection(params List<TileInput>[] tileInDirectionList)
        {
            foreach (var set in tileInDirectionList)
            {
                foreach (var item in set)
                {
                    if (item == null) continue;
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

        private void ClearInputTileConstraints()
        {
            TileInput item = selectedTileConstraints.targetObject as TileInput;
            if (item == null) return;
            else
            {
                SerializedObject temp = new(item);
                temp.FindProperty("compatibleTop").ClearArray();
                temp.FindProperty("compatibleBottom").ClearArray();
                temp.FindProperty("compatibleLeft").ClearArray();
                temp.FindProperty("compatibleRight").ClearArray();
                temp.ApplyModifiedProperties();
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
                            tile.id = i;
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

        private void UpdateSerializedProperties()
        {
            allInput.serializedObject.ApplyModifiedProperties();

            serializedTileSetObject.ApplyModifiedProperties();
        }

        private void SerializeProperties()
        {
            serializedTileSetObject = new(selectedInputTileSet);

            allInput = serializedTileSetObject.FindProperty("AllInputTiles");

            serializedTileSetObject.Update();
        }

        // GUI
        private static void CenterVerticalStart(int height)
        {
            EditorGUILayout.BeginVertical(GUILayout.Height(height));
            GUILayout.FlexibleSpace();
        }

        private static void CenterVerticalEnd()
        {
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }

        // Helper
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
    }
}