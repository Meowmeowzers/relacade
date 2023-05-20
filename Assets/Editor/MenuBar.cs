using UnityEditor;
using UnityEngine;

public class MenuBarWindow : EditorWindow
{
    private int selectedOptionIndex = 0;
    private string[] menuOptions = { "Option 1", "Option 2", "Option 3" };
    private Object targetObject;

    bool  showFoldout = false;

    [MenuItem("Window/Menu Bar Window")]
    public static void ShowWindow()
    {
        MenuBarWindow window = GetWindow<MenuBarWindow>();
        window.titleContent = new GUIContent("Menu Bar");
        window.Show();
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
    }
}
