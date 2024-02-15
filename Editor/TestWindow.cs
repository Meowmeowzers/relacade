using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HelloWorld.Editor
{
    public class TestWindow : EditorWindow
    {
        //Left
        private TileInputSet selectedInputTileSet;

        private SerializedObject serializedTileSetObject;

        // Serialized Properties

        #region Serialized Properties

        private SerializedProperty allInput;

        #endregion Serialized Properties

        //Right
        private SerializedObject selectedTileConstraints;

        private SerializedProperty tileName;
        private SerializedProperty id;
        private SerializedProperty gameObject;
        private SerializedProperty weight;
        private SerializedProperty compatibleTopList;
        private SerializedProperty compatibleBottomList;
        private SerializedProperty compatibleLeftList;
        private SerializedProperty compatibleRightList;
        private Texture2D previewTexture;

        private TileInput tempInputTile;
        private string assetName = "New Tile Set Configuration Data";
        private string inputTileName = "New Input Tile";
        private string inputTileSetLocation = "";
        private string inpuTileSetGuid = "";
        private string guidFolderPath = "";
        private string tempString = "";

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

        private Vector2 scrollPositionLeft = Vector2.zero;
        private Vector2 scrollPositionRight = Vector2.zero;

        private bool shouldClear = false;
        private int tempIndex = 0;

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

        private void DrawHeader()
        {
            GUILayout.BeginArea(headerSection);
            EditorGUILayout.Space(3);
            GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(position.width));

            EditorGUILayout.LabelField("Tile Set Config", GUILayout.Width(100), GUILayout.MaxWidth(100));
            selectedInputTileSet = (TileInputSet)EditorGUILayout.ObjectField(selectedInputTileSet, typeof(TileInputSet), false, GUILayout.MaxWidth(200));

            if (GUILayout.Button("Auto Set ID", EditorStyles.toolbarButton, GUILayout.MaxWidth(200)))
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
                }
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
                inputTileSetLocation = GetContainingFolder();
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
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(pic);
                EditorGUILayout.LabelField("All Input Tiles", EditorStyles.boldLabel, GUILayout.MaxWidth(170));

                // Array size field
                int currentArraySize = property.arraySize;
                int newArraySize = EditorGUILayout.IntField(currentArraySize, GUILayout.Width(40));
                if (newArraySize != currentArraySize)
                {
                    property.arraySize = newArraySize;
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(5);

                for (int i = 0; i < property.arraySize; i++)
                {
                    if (property.GetArrayElementAtIndex(i).objectReferenceValue != null)
                    {
                        SerializedProperty elementProperty = property.GetArrayElementAtIndex(i);
                        SerializedObject gameObjectProperty = new(property.GetArrayElementAtIndex(i).objectReferenceValue);
                        SerializedProperty texture = gameObjectProperty.FindProperty("gameObject");
                        Texture2D previewTexture = AssetPreview.GetAssetPreview(texture.objectReferenceValue);

                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Label(previewTexture, GUILayout.Width(50), GUILayout.Height(50));

                        CenterVerticalStart(50);
                        EditorGUILayout.LabelField(selectedInputTileSet.AllInputTiles[i].tileName, EditorStyles.boldLabel);
                        CenterVerticalEnd();

                        CenterVerticalStart(50);
                        if (GUILayout.Button(">", EditorStyles.toolbarButton, GUILayout.Width(30)))
                        {
                            if (elementProperty.objectReferenceValue != null)
                            {
                                UnityEngine.Object elementReference = elementProperty.objectReferenceValue;
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
                        SerializedProperty elementProperty = property.GetArrayElementAtIndex(i);

                        EditorGUILayout.BeginHorizontal();

                        CenterVerticalStart(50);
                        EditorGUILayout.LabelField("Empty", EditorStyles.miniLabel, GUILayout.MaxWidth(50));
                        CenterVerticalEnd();

                        CenterVerticalStart(50);
                        EditorGUILayout.PropertyField(elementProperty, GUIContent.none, GUILayout.MaxWidth(175));
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
                    property.arraySize++;
                    CreateAndAddTile(property.GetArrayElementAtIndex(property.arraySize - 1));
                    CheckListForNull(property);
                    property.serializedObject.ApplyModifiedProperties();
                }
                if (GUILayout.Button("-", GUILayout.Width(35), GUILayout.Height(35)))
                {
                    if (property.arraySize > 0)
                    {
                        RemoveAndDeleteTile(property.GetArrayElementAtIndex(property.arraySize - 1));
                        property.arraySize--;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
                if (GUILayout.Button("Reload", GUILayout.Width(65), GUILayout.Height(35)))
                {
                    if (property.arraySize > 0)
                    {
                        CheckListForNull(property);
                    }
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(5);
                property.serializedObject.ApplyModifiedProperties();
                
            }
        }

        private void CreateAndAddTile(SerializedProperty serializedProperty)
        {
            selectedTileConstraints = null;
            string location = GetAssetPathWithUniqueName(inputTileSetLocation + "/" +
                System.IO.Path.GetFileNameWithoutExtension(inputTileSetLocation) + ".asset");

            TileInput newTileInput = CreateInstance<TileInput>();
            newTileInput.tileName = System.IO.Path.GetFileNameWithoutExtension(location);
            newTileInput.id = allInput.arraySize - 1;

            AssetDatabase.CreateAsset(newTileInput, location);
            AssetDatabase.SaveAssets();

            serializedProperty.objectReferenceValue = newTileInput;
        }

        private void RemoveAndDeleteTile(SerializedProperty serializedProperty)
        {
            selectedTileConstraints = null;
            tempString = AssetDatabase.GetAssetPath(serializedProperty.objectReferenceValue);
            if (AssetDatabase.LoadAssetAtPath<Object>(tempString) != null)
            {
                AssetDatabase.DeleteAsset(tempString);
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }
        }

        private bool CheckListForNull(SerializedProperty serializedProperty)
        {
            bool result = false;
            for (int i = serializedProperty.arraySize - 1; i >= 0; i--)
            {
                if (serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    serializedProperty.DeleteArrayElementAtIndex(i);
                    result = true;
                }
            }
            return result;
        }
        private string GetContainingFolder()
        {
            inpuTileSetGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(selectedInputTileSet));
            guidFolderPath = AssetDatabase.GUIDToAssetPath(inpuTileSetGuid);

            return System.IO.Path.GetDirectoryName(guidFolderPath);
        }

        private string GetAssetPathWithUniqueName(string originalPath)
        {
            int suffix = 1;
            string newPath = originalPath;
            while (AssetDatabase.LoadAssetAtPath<Object>(newPath) != null)
            {
                newPath = originalPath.Replace(".asset", "_" + suffix + ".asset");
                suffix++;
            }
            return newPath;
        }

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

                CreateTileSetConfigButton();

                EditorGUILayout.EndVertical();
            }
            else if (allInput.arraySize == 0)
            {
                EditorGUILayout.LabelField("* Press the + and - buttons to add new items");
                EditorGUILayout.LabelField("* Then, select an item from the tile list by pressing the > button");
            }
            else if (selectedTileConstraints == null)
            {
                GUILayout.Space(15);
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Select an Input Tile");
                EditorGUILayout.EndVertical();
            }
            else
            {
                if(selectedTileConstraints != null) selectedTileConstraints.Update();

                EditorGUILayout.Space(5);

                id = selectedTileConstraints.FindProperty("id");
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
                EditorGUILayout.LabelField("ID", GUILayout.Width(80));
                EditorGUILayout.PropertyField(id, GUIContent.none);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("GameObject", GUILayout.Width(80));
                EditorGUILayout.PropertyField(gameObject, GUIContent.none);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Weight", GUILayout.Width(80));
                EditorGUILayout.PropertyField(weight, GUIContent.none);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Send Tile", GUILayout.Height(30)))
                {
                    PassConstraintsWindow.OpenWindow(selectedTileConstraints, allInput);
                }
                if (GUILayout.Button("Receive Constraints", GUILayout.Height(30)))
                {
                    ReceiveConstraintsWindow.OpenWindow(selectedTileConstraints, allInput);
                }
                EditorGUILayout.EndHorizontal();
                CenterVerticalEnd();

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

            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space(20);
            GUILayout.EndArea();
        }

        private void CreateTileSetConfigButton()
        {
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
        }

        private void SetLoadedInputTileButton()
        {
            if (GUILayout.Button("Set Input Tile Value", GUILayout.Height(30)))
            {
                if (tempInputTile != null)
                {
                    allInput.GetArrayElementAtIndex(tempIndex).objectReferenceValue = tempInputTile;
                    UnityEngine.Object elementReference = allInput.GetArrayElementAtIndex(tempIndex).objectReferenceValue;
                    selectedTileConstraints = new(elementReference);
                    tempInputTile = null;
                }
            }
        }

        private void SetCreatedInputTileButton()
        {
            if (GUILayout.Button("Create InputTile", GUILayout.Height(30)))
            {
                ScriptableObject scriptableObject = CreateInstance<TileInput>();
                //string savePath = EditorUtility.SaveFilePanelInProject("Save Scriptable Object", inputTileName, "asset", "Choose a location to save the Input Tile.", inputTileSetLocation);
                //if (string.IsNullOrEmpty(savePath)) return;

                AssetDatabase.CreateAsset(scriptableObject, inputTileSetLocation + "/" + inputTileName + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                allInput.GetArrayElementAtIndex(tempIndex).objectReferenceValue = scriptableObject as TileInput;
                UnityEngine.Object elementReference = allInput.GetArrayElementAtIndex(tempIndex).objectReferenceValue;
                selectedTileConstraints = new(elementReference);
                tempInputTile = null;
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

        private void OnDestroy()
        {
            GetWindow<ReceiveConstraintsWindow>().Close();
            GetWindow<PassConstraintsWindow>().Close();
        }
    }
}