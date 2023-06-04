using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HelloWorld.Editor
{
    public class AddOrRemoveWindow : EditorWindow
    {
        private static SerializedProperty set;
        private static SerializedObject tile;
        private Vector2 scrollPosition;

        private static bool[][] togglesArray;

        public static void OpenWindow(SerializedObject newTile, SerializedProperty newSet)
        {
            AddOrRemoveWindow window = (AddOrRemoveWindow)GetWindow(typeof(AddOrRemoveWindow));
            //WindowSetup(window, newTile, newSet);
            window.titleContent = new GUIContent("Add/Remove Constraints Window");
            window.minSize = new(300, 320);
            set = newSet;
            tile = newTile;
            CheckExistingTiles();
            window.Show();

        }
        
        private void OnGUI()
        {
            EditorGUILayout.Space();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, true, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(false));

            EditorGUILayout.LabelField("- Mark the checkboxes you want to");
            EditorGUILayout.LabelField("   propagate this tile to");

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

            EditorGUILayout.Space();


            EditorGUILayout.BeginHorizontal(GUILayout.Width(position.width));
            EditorGUILayout.LabelField(GUIContent.none, EditorStyles.boldLabel, GUILayout.Width(80));
            EditorGUILayout.LabelField("Top", EditorStyles.boldLabel, GUILayout.Width(35));
            EditorGUILayout.LabelField("Bottom", EditorStyles.boldLabel, GUILayout.Width(55));
            EditorGUILayout.LabelField("Left", EditorStyles.boldLabel, GUILayout.Width(40));
            EditorGUILayout.LabelField("Right", EditorStyles.boldLabel, GUILayout.Width(35));
            EditorGUILayout.EndHorizontal();

            if (set != null)
            {
                for (int i = 0; i < set.arraySize; i++)
                {
                    SerializedProperty itemProperty = set.GetArrayElementAtIndex(i);
                    TileInput tileInput = itemProperty.objectReferenceValue as TileInput;

                    string itemName = "Unnamed";
                    if (tileInput != null)
                    {
                        itemName = tileInput.name;
                    }

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField(itemName, GUILayout.Width(85));

                    if (togglesArray[i] == null)
                        togglesArray[i] = new bool[4];

                    for (int j = 0; j < 4; j++)
                    {
                        if(j != 0)
                        GUILayout.Space(25);
                        togglesArray[i][j] = EditorGUILayout.Toggle(togglesArray[i][j], GUILayout.Width(20));
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            if (GUILayout.Button("Apply"))
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

                int setSize = set.arraySize;
                togglesArray = new bool[setSize][];

                for (int i = 0; i < set.arraySize; i++)
                {
                    SerializedProperty itemProperty = set.GetArrayElementAtIndex(i);
                    TileInput item = itemProperty.objectReferenceValue as TileInput;

                    if (item != null)
                    {
                        togglesArray[i] = new bool[4];

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
                        SerializedObject itemSerializedObject = new SerializedObject(item);

                        if (togglesArray[i][0])
                        {
                            if (!item.compatibleTop.Contains(tileInput))
                                item.compatibleTop.Add(tileInput);
                        }
                        else
                        {
                            if (item.compatibleTop.Contains(tileInput))
                                item.compatibleTop.Remove(tileInput);
                        }

                        if (togglesArray[i][1])
                        {
                            if (!item.compatibleBottom.Contains(tileInput))
                                item.compatibleBottom.Add(tileInput);
                        }
                        else
                        {
                            if (item.compatibleBottom.Contains(tileInput))
                                item.compatibleBottom.Remove(tileInput);
                        }

                        if (togglesArray[i][2])
                        {
                            if (!item.compatibleLeft.Contains(tileInput))
                                item.compatibleLeft.Add(tileInput);
                        }
                        else
                        {
                            if (item.compatibleLeft.Contains(tileInput))
                                item.compatibleLeft.Remove(tileInput);
                        }

                        if (togglesArray[i][3])
                        {
                            if (!item.compatibleRight.Contains(tileInput))
                                item.compatibleRight.Add(tileInput);
                        }
                        else
                        {
                            if (item.compatibleRight.Contains(tileInput))
                                item.compatibleRight.Remove(tileInput);
                        }

                        // Apply modifications to the item
                        itemSerializedObject.ApplyModifiedProperties();
                        itemSerializedObject.UpdateIfRequiredOrScript();

                        EditorUtility.SetDirty(itemSerializedObject.targetObject); // Mark the item as dirty for serialization
                    }
                }

                // Apply modifications to the tile
                tile.ApplyModifiedProperties();
                tile.UpdateIfRequiredOrScript();

                EditorUtility.SetDirty(tile.targetObject); // Mark the tile as dirty for serialization
            }

            AssetDatabase.SaveAssets(); // Save the modified assets
            AssetDatabase.Refresh(); // Refresh the Asset Database to reflect the changes
        }

    }
}