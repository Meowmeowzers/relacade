using UnityEditor;
using UnityEngine;

namespace HelloWorld.Editor
{
    public class ReceiveConstraintsWindow : EditorWindow
    {
        private static SerializedProperty set;
        private static SerializedObject tile;
        private Vector2 scrollPosition;

        private string itemName = "";
        private static bool[][] togglesArray;
        private bool showHelp = false;

        private static Texture2D tilePreview;
        private static Texture2D listItemTexture;
        private static Texture2D listItemLightTexture;
        private static GUIStyle listItemStyle = new();
        private static GUIStyle previewStyle = new();
        private static Color listItemColor = new(0.18f, 0.18f, 0.18f, 1f);
        private static Color listItemLightColor = new(0.2f, 0.2f, 0.2f, 1f);

        private SerializedProperty compatibilityProperty;

        static Texture2D previewTexture;
        static Texture2D constraintTexture;

        public static void OpenWindow(SerializedObject newTile, SerializedProperty newSet)
        {
            ReceiveConstraintsWindow window = (ReceiveConstraintsWindow)GetWindow(typeof(ReceiveConstraintsWindow));
            window.titleContent = new GUIContent("Receive Constraints");
            window.minSize = new(330, 350);
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

            previewStyle.padding = new();
            previewStyle.margin = new();
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Help", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) showHelp = !showHelp;
            EditorGUILayout.EndHorizontal();
            if (showHelp)
                EditorGUILayout.HelpBox("Mark the checkboxes of the input tiles you want to make as a constraint for this tile", MessageType.Info);
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
            EditorGUILayout.LabelField(GUIContent.none, EditorStyles.boldLabel, GUILayout.Width(120));
            EditorGUILayout.LabelField("Top", EditorStyles.boldLabel, GUILayout.Width(35));
            EditorGUILayout.LabelField("Bottom", EditorStyles.boldLabel, GUILayout.Width(55));
            EditorGUILayout.LabelField("Left", EditorStyles.boldLabel, GUILayout.Width(40));
            EditorGUILayout.LabelField("Right", EditorStyles.boldLabel, GUILayout.Width(35));
            EditorGUILayout.EndHorizontal();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));

            if (tile != null && set != null)
            {
                for (int i = 0; i < set.arraySize; i++)
                {
                    SerializedProperty itemProperty = set.GetArrayElementAtIndex(i);
                    TileInput tileInput = itemProperty.objectReferenceValue as TileInput;

                    EditorGUILayout.BeginHorizontal(listItemStyle);

                    if (tileInput.gameObject != null)
                        tilePreview = AssetPreview.GetAssetPreview(tileInput.gameObject);

                    itemName = tileInput != null ? tileInput.tileName : "No tile";

                    if (GUILayout.Button(tilePreview, GUILayout.Width(40), GUILayout.Height(40)))
                    {
                        constraintTexture = AssetPreview.GetAssetPreview(tileInput.gameObject);
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
            int setSize = set.arraySize;
            togglesArray = new bool[setSize][];
            for (int i = 0; i < setSize; i++)
            {
                SerializedProperty itemProperty = set.GetArrayElementAtIndex(i);
                TileInput item = itemProperty.objectReferenceValue as TileInput;

                togglesArray[i] = new bool[4];

                if (item != null)
                {
                    togglesArray[i][0] = IsItemContained(tile.FindProperty("compatibleTop"), item);
                    togglesArray[i][1] = IsItemContained(tile.FindProperty("compatibleBottom"), item);
                    togglesArray[i][2] = IsItemContained(tile.FindProperty("compatibleLeft"), item);
                    togglesArray[i][3] = IsItemContained(tile.FindProperty("compatibleRight"), item);
                }
            }
        }

        private void UpdateCompatibility(SerializedProperty property, TileInput item, bool compatible)
        {
            SerializedProperty elementProperty;

            if (property != null && item != null)
            {
                if (compatible && !IsItemContained(property, item))
                {
                    property.arraySize++;
                    elementProperty = property.GetArrayElementAtIndex(property.arraySize - 1);
                    elementProperty.objectReferenceValue = item;
                }
                else if (!compatible && IsItemContained(property, item))
                {
                    for (int i = 0; i < property.arraySize; i++)
                    {
                        elementProperty = property.GetArrayElementAtIndex(i);
                        if (elementProperty.objectReferenceValue == item)
                        {
                            property.DeleteArrayElementAtIndex(i);
                            break;
                        }
                    }
                }
            }
        }

        private void ApplyModifications()
        {
            if (tile != null)
            {
                for (int i = 0; i < set.arraySize; i++)
                {
                    SerializedProperty itemProperty = set.GetArrayElementAtIndex(i);
                    TileInput tileInput = itemProperty.objectReferenceValue as TileInput;

                    for (int j = 0; j < 4; j++)
                    {
                        compatibilityProperty = GetCompatibilityProperty(j);
                        UpdateCompatibility(compatibilityProperty, tileInput, togglesArray[i][j]);
                    }
                }
                // Making sure it saves
                tile.ApplyModifiedProperties();
                tile.UpdateIfRequiredOrScript();

                EditorUtility.SetDirty(tile.targetObject);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private static bool IsItemContained(SerializedProperty property, TileInput item)
        {
            SerializedProperty elementProperty;

            if (property != null && item != null)
            {
                for (int i = 0; i < property.arraySize; i++)
                {
                    elementProperty = property.GetArrayElementAtIndex(i);
                    if (elementProperty.objectReferenceValue == item) return true;
                }
            }
            return false;
        }

        private SerializedProperty GetCompatibilityProperty(int index)
        {
            switch (index)
            {
                case 0:
                    return tile.FindProperty("compatibleTop");

                case 1:
                    return tile.FindProperty("compatibleBottom");

                case 2:
                    return tile.FindProperty("compatibleLeft");

                case 3:
                    return tile.FindProperty("compatibleRight");

                default:
                    return null;
            }
        }
    }
}