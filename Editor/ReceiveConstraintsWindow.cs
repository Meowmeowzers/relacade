using HelloWorld.Editor;
using UnityEditor;
using UnityEngine;

public class ReceiveConstraintsWindow : EditorWindow
{
    private static SerializedProperty set;
    private static SerializedObject tile;
    private Vector2 scrollPosition;

    private static bool[][] togglesArray;

    public static void OpenWindow(SerializedObject newTile, SerializedProperty newSet)
    {
        ReceiveConstraintsWindow window = (ReceiveConstraintsWindow)GetWindow(typeof(ReceiveConstraintsWindow));
        window.titleContent = new GUIContent("Add Constraints Window");
        window.minSize = new Vector2(300, 320);
        window.maxSize = new Vector2(400, 600);
        tile = newTile;
        set = newSet;

        InitializeTogglesArray();

        window.Show();
    }

    private static void InitializeTogglesArray()
    {
        int setSize = set.arraySize;
        togglesArray = new bool[setSize][];
        for (int i = 0; i < setSize; i++)
        {
            togglesArray[i] = new bool[4];

            SerializedProperty itemProperty = set.GetArrayElementAtIndex(i);
            TileInput item = itemProperty.objectReferenceValue as TileInput;

            if (item != null)
            {
                togglesArray[i][0] = IsItemContained(tile.FindProperty("compatibleTop"), item);
                togglesArray[i][1] = IsItemContained(tile.FindProperty("compatibleBottom"), item);
                togglesArray[i][2] = IsItemContained(tile.FindProperty("compatibleLeft"), item);
                togglesArray[i][3] = IsItemContained(tile.FindProperty("compatibleRight"), item);
            }
        }
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(false));
        EditorGUILayout.LabelField("- Mark the checkboxes you want to");
        EditorGUILayout.LabelField("  add as a constraint for this tile");
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (tile != null)
        {
            GameObject tileGameObject = tile.FindProperty("gameObject").objectReferenceValue as GameObject;

            if (tileGameObject != null)
            {
                Texture2D previewTexture = AssetPreview.GetAssetPreview(tileGameObject);

                if (previewTexture != null)
                {
                    GUILayout.Label(previewTexture, GUILayout.Width(60), GUILayout.Height(60));
                }
            }
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(GUILayout.Width(position.width));
        EditorGUILayout.LabelField(GUIContent.none, GUILayout.Width(80));
        EditorGUILayout.LabelField("Top", EditorStyles.boldLabel, GUILayout.Width(35));
        EditorGUILayout.LabelField("Bottom", EditorStyles.boldLabel, GUILayout.Width(55));
        EditorGUILayout.LabelField("Left", EditorStyles.boldLabel, GUILayout.Width(40));
        EditorGUILayout.LabelField("Right", EditorStyles.boldLabel, GUILayout.Width(35));
        EditorGUILayout.EndHorizontal();

        if (tile != null && set != null)
        {
            for (int i = 0; i < set.arraySize; i++)
            {
                SerializedProperty itemProperty = set.GetArrayElementAtIndex(i);
                TileInput item = itemProperty.objectReferenceValue as TileInput;

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(item != null ? item.name : "Unnamed", GUILayout.Width(85));

                for (int j = 0; j < 4; j++)
                {
                    SerializedProperty compatibilityProperty = GetCompatibilityProperty(j);

                    if (compatibilityProperty != null && item != null)
                    {
                        bool compatible = togglesArray[i][j];
                        compatible = EditorGUILayout.Toggle(compatible, GUILayout.Width(20));
                        togglesArray[i][j] = compatible;

                        UpdateCompatibility(compatibilityProperty, item, compatible);
                    }

                    GUILayout.Space(25);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.Space();

        EditorGUILayout.EndScrollView();
        if (GUILayout.Button("Apply"))
        {
            ApplyModifications();
            Close();
        }
    }

    private SerializedProperty GetCompatibilityProperty(int index)
    {
        SerializedProperty property = null;

        switch (index)
        {
            case 0:
                property = tile.FindProperty("compatibleTop");
                break;

            case 1:
                property = tile.FindProperty("compatibleBottom");
                break;

            case 2:
                property = tile.FindProperty("compatibleLeft");
                break;

            case 3:
                property = tile.FindProperty("compatibleRight");
                break;
        }

        return property;
    }

    private void UpdateCompatibility(SerializedProperty property, TileInput item, bool compatible)
    {
        if (property != null && item != null)
        {
            if (compatible && !IsItemContained(property, item))
            {
                property.arraySize++;
                SerializedProperty elementProperty = property.GetArrayElementAtIndex(property.arraySize - 1);
                elementProperty.objectReferenceValue = item;
            }
            else if (!compatible && IsItemContained(property, item))
            {
                for (int i = 0; i < property.arraySize; i++)
                {
                    SerializedProperty elementProperty = property.GetArrayElementAtIndex(i);
                    if (elementProperty.objectReferenceValue == item)
                    {
                        property.DeleteArrayElementAtIndex(i);
                        break;
                    }
                }
            }
        }
    }

    private static bool IsItemContained(SerializedProperty property, TileInput item)
    {
        if (property != null && item != null)
        {
            for (int i = 0; i < property.arraySize; i++)
            {
                SerializedProperty elementProperty = property.GetArrayElementAtIndex(i);
                if (elementProperty.objectReferenceValue == item)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void ApplyModifications()
    {
        if (tile != null)
        {
            tile.ApplyModifiedProperties();

            // Marked as dirty to ensure the changes persist on restart
            EditorUtility.SetDirty(tile.targetObject);
        }
    }
}