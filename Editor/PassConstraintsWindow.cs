using UnityEditor;
using UnityEngine;

namespace HelloWorld.Editor
{
    public class PassConstraintsWindow : EditorWindow
    {
        private static SerializedProperty set;
        private static SerializedObject tile;
        private Vector2 scrollPosition;

        private static bool[][] togglesArray;

        private string itemName = "";
        private static Texture2D tilePreview;
        private static Texture2D listItemTexture;
        private static Texture2D listItemLightTexture;
        private static GUIStyle listItemStyle = new();
        private static GUIStyle previewStyle = new();
        private static Color listItemColor = new(0.18f, 0.18f, 0.18f, 1f);
        private static Color listItemLightColor = new(0.2f, 0.2f, 0.2f, 1f);
        private GUIContent isMutualLabel = new("Mutual", "Not functional yet");
        private bool showHelp = false;
        private bool isMutual = false;

        static Texture2D previewTexture;
        static Texture2D constraintTexture;

        public static void OpenWindow(SerializedObject newTile, SerializedProperty newSet)
        {
            PassConstraintsWindow window = (PassConstraintsWindow)GetWindow(typeof(PassConstraintsWindow));
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
            previewStyle.padding = new();
            previewStyle.margin = new();

        }

        private void OnGUI()
        {
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
            isMutual = EditorGUILayout.ToggleLeft(isMutualLabel, isMutual, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(60));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Help", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) showHelp = !showHelp;
            EditorGUILayout.EndHorizontal();
            if (showHelp)
                EditorGUILayout.HelpBox("Mark the checkboxes of the input tiles you want to propagate this tile to as constraint", MessageType.Info);
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

                    itemName = tileInput != null ? tileInput.tileName : "No tile";

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
                        if (item.compatibleTop.Contains(tileInput))
                            togglesArray[i][0] = true;

                        if (item.compatibleBottom.Contains(tileInput))
                            togglesArray[i][1] = true;

                        if (item.compatibleLeft.Contains(tileInput))
                            togglesArray[i][2] = true;

                        if (item.compatibleRight.Contains(tileInput))
                            togglesArray[i][3] = true;
                    }
                }
            }
        }

        private void ApplyModifications()
        {
            if (tile != null)
            {
                TileInput tileInput = tile.targetObject as TileInput;

                for (int i = 0; i < set.arraySize; i++)
                {
                    SerializedProperty itemProperty = set.GetArrayElementAtIndex(i);
                    TileInput item = itemProperty.objectReferenceValue as TileInput;

                    if (item != null)
                    {
                        SerializedObject itemSerializedObject = new(item);
                        for (int j = 0; j < 4; j++)
                        {
                            UpdateTile(item, tileInput, togglesArray[i][j], j);
                        }

                        // Apply modifications to the item
                        itemSerializedObject.ApplyModifiedProperties();
                        itemSerializedObject.UpdateIfRequiredOrScript();

                        EditorUtility.SetDirty(itemSerializedObject.targetObject); // Mark the item as dirty for serialization
                    }
                }
            }

            // Making sure it persists
            tile.ApplyModifiedProperties();
            tile.UpdateIfRequiredOrScript();

            EditorUtility.SetDirty(tile.targetObject);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void UpdateTile(TileInput tileToModify, TileInput tile, bool value, int index)
        {// Improve this
            switch (index)
            {
                case 0:
                    if (value)
                    {
                        if (!tileToModify.compatibleTop.Contains(tile))
                            tileToModify.compatibleTop.Add(tile);
                    }
                    else
                    {
                        if (tileToModify.compatibleTop.Contains(tile))
                            tileToModify.compatibleTop.Remove(tile);
                    }
                    break;

                case 1:
                    if (value)
                    {
                        if (!tileToModify.compatibleBottom.Contains(tile))
                            tileToModify.compatibleBottom.Add(tile);
                    }
                    else
                    {
                        if (tileToModify.compatibleBottom.Contains(tile))
                            tileToModify.compatibleBottom.Remove(tile);
                    }
                    break;

                case 2:
                    if (value)
                    {
                        if (!tileToModify.compatibleLeft.Contains(tile))
                            tileToModify.compatibleLeft.Add(tile);
                    }
                    else
                    {
                        if (tileToModify.compatibleLeft.Contains(tile))
                            tileToModify.compatibleLeft.Remove(tile);
                    }
                    break;

                case 3:
                    if (value)
                    {
                        if (!tileToModify.compatibleRight.Contains(tile))
                            tileToModify.compatibleRight.Add(tile);
                    }
                    else
                    {
                        if (tileToModify.compatibleRight.Contains(tile))
                            tileToModify.compatibleRight.Remove(tile);
                    }
                    break;
            }
        }
    }
}