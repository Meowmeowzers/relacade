/*
using UnityEditor;
using UnityEngine;

Test Script????
public class MenuBarWindow : EditorWindow
{
    private Texture2D image1;
    private Texture2D image2;
    private int selectedOptionIndex = 0;
    private string[] menuOptions = { "Option 1", "Option 2", "Option 3" };
    private Object targetObject;

    private bool showFoldout = false;

    [MenuItem("Window/Menu Bar Window")]
    public static void ShowWindow()
    {
        MenuBarWindow window = GetWindow<MenuBarWindow>();
        window.titleContent = new GUIContent("Menu Bar");
        window.Show();
    }

    private void OnEnable()
    {
        image1 = EditorGUIUtility.IconContent("TerrainInspector.TerrainToolSplat").image as Texture2D;
        image2 = EditorGUIUtility.IconContent("TerrainInspector.TerrainToolPlants").image as Texture2D;
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(100));

        GUILayout.Label("Label:", GUILayout.Width(50f));

        // Display the selected option index
        GUILayout.Label($"Selected Option: {selectedOptionIndex}");

        GUILayout.Space(10f); // Add some space between the label and the object field

        targetObject = EditorGUILayout.ObjectField(targetObject, typeof(Object), true, GUILayout.Width(150f));

        GUILayout.FlexibleSpace(); // Add flexible space to push the button to the right

        if (GUILayout.Button("Options", EditorStyles.toolbarDropDown))
        {
            GenericMenu menu = new GenericMenu();

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

        GUILayout.EndHorizontal();

        GUILayout.Space(10f);

        if (GUILayout.Button("Press Me"))
        {
            showFoldout = !showFoldout;
        }

        if (showFoldout)
        {
            GUILayout.Space(10f);

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField("fffffffffffffff");

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(image1, GUILayout.Width(40), GUILayout.Height(40));
        GUILayout.Label(image2, GUILayout.Width(40), GUILayout.Height(40));

        if (GUILayout.Button("My Button"))
        {
            // Button click action
        }

        EditorGUILayout.EndHorizontal();
    }
}
    */