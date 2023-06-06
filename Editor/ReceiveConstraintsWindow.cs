using HelloWorld.Editor;
using UnityEditor;
using UnityEngine;

public class ReceiveConstraintsWindow: EditorWindow
{
    private static SerializedProperty set;
    private static SerializedObject tile;
    private Vector2 scrollPosition;

    public static void OpenWindow(SerializedObject newTile, SerializedProperty newSet)
    {
        ReceiveConstraintsWindow window = (ReceiveConstraintsWindow)GetWindow(typeof(ReceiveConstraintsWindow));
        window.titleContent = new GUIContent("Add Constraints Window");
        window.minSize = new(300, 320);
        window.maxSize = new(300, 320);
        tile = newTile;
        set = newSet;
        window.Show();
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

                SerializedProperty compatibleTopProperty = tile.FindProperty("compatibleTop");
                bool compatibleTop = IsItemContained(compatibleTopProperty, item);
                compatibleTop = EditorGUILayout.Toggle(compatibleTop, GUILayout.Width(20));
                UpdateCompatibility(compatibleTopProperty, item, compatibleTop);

                GUILayout.Space(25);

                SerializedProperty compatibleBottomProperty = tile.FindProperty("compatibleBottom");
                bool compatibleBottom = IsItemContained(compatibleBottomProperty, item);
                compatibleBottom = EditorGUILayout.Toggle(compatibleBottom, GUILayout.Width(20));
                UpdateCompatibility(compatibleBottomProperty, item, compatibleBottom);

                GUILayout.Space(25);

                SerializedProperty compatibleLeftProperty = tile.FindProperty("compatibleLeft");
                bool compatibleLeft = IsItemContained(compatibleLeftProperty, item);
                compatibleLeft = EditorGUILayout.Toggle(compatibleLeft, GUILayout.Width(20));
                UpdateCompatibility(compatibleLeftProperty, item, compatibleLeft);

                GUILayout.Space(25);

                SerializedProperty compatibleRightProperty = tile.FindProperty("compatibleRight");
                bool compatibleRight = IsItemContained(compatibleRightProperty, item);
                compatibleRight = EditorGUILayout.Toggle(compatibleRight, GUILayout.Width(20));
                UpdateCompatibility(compatibleRightProperty, item, compatibleRight);

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

    private bool IsItemContained(SerializedProperty property, TileInput item)
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

    private void ApplyModifications()
    {
        if (tile != null)
        {
            tile.ApplyModifiedProperties();

            // Mark the modified tile as dirty to ensure the changes persist on restart
            EditorUtility.SetDirty(tile.targetObject);
        }
    }


}