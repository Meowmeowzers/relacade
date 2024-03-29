using UnityEditor;
using UnityEngine;

namespace HelloWorld.Editor
{
	[CustomEditor(typeof(EditorCell))]
	public class EditorCellInspector : UnityEditor.Editor
	{
		private EditorCell inspected;
		private SerializedProperty xIndex;
		private SerializedProperty yIndex;
		private SerializedProperty entropy;
		private SerializedProperty selectedID;
		private SerializedProperty fixedTile;
		private InputTile selectedTileObject;
		private bool isControlShown = true;
		private bool isInfoShown = false;
		private bool isSelectShown = false;

		private Texture2D tilePreview;
		private Texture2D tempPreview;
		private Vector2 scroll = Vector2.zero;

		private void OnEnable()
		{
			inspected = target as EditorCell;
			xIndex = serializedObject.FindProperty("xIndex");
			yIndex = serializedObject.FindProperty("yIndex");
			entropy = serializedObject.FindProperty("entropy");
			selectedID = serializedObject.FindProperty("selectedTileID");
			fixedTile = serializedObject.FindProperty("fixedTile");
			selectedTileObject = inspected.selectedTile;
			serializedObject.Update();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			// Preview
			StartHorizontalCentered();
			if (selectedTileObject == null && !inspected.IsFixed())
			{
				GUILayout.Label("No Selected Tile", GUILayout.Width(100), GUILayout.Height(100));
			}
			else
			{
				if (inspected.IsFixed())
					tilePreview = AssetPreview.GetAssetPreview(inspected.fixedTile.gameObject);
				else
					tilePreview = AssetPreview.GetAssetPreview(selectedTileObject.gameObject);
				GUILayout.Label(tilePreview, GUILayout.Width(100), GUILayout.Height(100));
			}
			EndHorizontalCentered();

			// Tile name
			StartHorizontalCentered();
			if (selectedTileObject != null)
				GUILayout.Label(selectedTileObject.tileName, EditorStyles.boldLabel, GUILayout.ExpandWidth(false));

			EndHorizontalCentered();

			GUILayout.Space(5);

			// Main info
			EditorGUI.BeginDisabledGroup(true);

			StartHorizontalCentered();
			EditorGUILayout.LabelField("X", EditorStyles.whiteLabel, GUILayout.Width(20));
			EditorGUILayout.PropertyField(xIndex, GUIContent.none);
			EditorGUILayout.LabelField("Y", EditorStyles.whiteLabel, GUILayout.Width(20));
			EditorGUILayout.PropertyField(yIndex, GUIContent.none);
			EndHorizontalCentered();
			EditorGUILayout.ToggleLeft("Fixed Constraint", fixedTile.objectReferenceValue != null);

			EditorGUI.EndDisabledGroup();

			// More info
			isInfoShown = EditorGUILayout.Foldout(isInfoShown, "Info", true, EditorStyles.foldout);
			if (isInfoShown)
			{
				EditorGUI.indentLevel++;

				EditorGUI.BeginDisabledGroup(true);

				EditorGUILayout.BeginVertical();
				EditorGUILayout.PropertyField(entropy);
				EditorGUILayout.PropertyField(selectedID);
				EditorGUILayout.Toggle("Definite", !inspected.IsNotDefiniteState());
				//EditorGUILayout.PropertyField(entropy);
				EditorGUILayout.EndVertical();

				EditorGUI.EndDisabledGroup();

				EditorGUI.indentLevel--;
			}

			// Select tile controls
			isSelectShown = EditorGUILayout.Foldout(isSelectShown, "Select Tile", true, EditorStyles.foldout);
			if (isSelectShown)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Change current tile");
				EditorGUILayout.EndHorizontal();
				if (GUILayout.Button("Clear"))
				{
					inspected.RemoveTile();
				}

				scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(300));
				for (int i = 0; i < inspected.allTiles.Count; i++)
				{
					ChangeTileButton(inspected.allTiles[i]);
				}
				EditorGUILayout.EndScrollView();
			}

			// Fixed Tile controls
			isControlShown = EditorGUILayout.Foldout(isControlShown, "Fixed Tile", true, EditorStyles.foldout);
			if (isControlShown)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Fixed Tile", GUILayout.Width(70));
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.PropertyField(fixedTile, GUIContent.none);
				EditorGUI.EndDisabledGroup();
				if (GUILayout.Button("Clear"))
				{
					fixedTile.objectReferenceValue = null;
				}
				EditorGUILayout.EndHorizontal();

				scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(300));
				for (int i = 0; i < inspected.allTiles.Count; i++)
				{
					FixedTileButton(inspected.allTiles[i]);
				}
				EditorGUILayout.EndScrollView();
			}

			serializedObject.ApplyModifiedProperties();
		}

		private void FixedTileButton(InputTile tileInput)
		{
			tempPreview = AssetPreview.GetAssetPreview(tileInput.gameObject);

			EditorGUILayout.BeginHorizontal(GUILayout.Height(50));
			GUILayout.Label(tempPreview, GUILayout.Height(50), GUILayout.Width(50));
			if (GUILayout.Button(tileInput.tileName, GUILayout.Height(50)))
			{
				fixedTile.objectReferenceValue = tileInput;
			}
			EditorGUILayout.EndHorizontal();
		}

		private void ChangeTileButton(InputTile tileInput)
		{
			tempPreview = AssetPreview.GetAssetPreview(tileInput.gameObject);

			EditorGUILayout.BeginHorizontal(GUILayout.Height(50));
			GUILayout.Label(tempPreview, GUILayout.Height(50), GUILayout.Width(50));
			if (GUILayout.Button(tileInput.tileName, GUILayout.Height(50)))
			{
				inspected.ChangeTile(tileInput);
			}
			EditorGUILayout.EndHorizontal();
		}

		private static void StartHorizontalCentered()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
		}

		private static void EndHorizontalCentered()
		{
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
	}
}