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
		private SerializedProperty isDefinite;
		private SerializedProperty fixTile;
		private	TileInput selectedTileObject;
		private bool isControlShown = false;
		private bool isInfoShown = false;

		private Texture2D tilePreview;
		private Texture2D fixedTilePreview;

		private void OnEnable()
		{
			inspected = target as EditorCell;
			xIndex = serializedObject.FindProperty("xIndex");
			yIndex = serializedObject.FindProperty("yIndex");
			entropy = serializedObject.FindProperty("entropy");
			selectedID = serializedObject.FindProperty("selectedTileID");
			isDefinite = serializedObject.FindProperty("isDefinite");
			fixTile = serializedObject.FindProperty("fixedTile");
			selectedTileObject = inspected.selectedTile;
			serializedObject.Update();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			// Preview
			StartHorizontalCentered();
			if (selectedTileObject.gameObject != null)
			{
				if (inspected.fixedTile != null)
					fixedTilePreview = AssetPreview.GetAssetPreview(inspected.fixedTile.gameObject);
				else
					tilePreview = AssetPreview.GetAssetPreview(selectedTileObject.gameObject);

				GUILayout.Label(tilePreview, GUILayout.Width(100), GUILayout.Height(100));
			}
			EndHorizontalCentered();

			// Tile name
			StartHorizontalCentered();
			GUILayout.Label(selectedTileObject.tileName, EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
			EndHorizontalCentered();

			// Main info
			EditorGUI.BeginDisabledGroup(true);

			StartHorizontalCentered();
			EditorGUILayout.LabelField("X", EditorStyles.whiteLabel, GUILayout.Width(30));
			EditorGUILayout.PropertyField(xIndex, GUIContent.none);
			EditorGUILayout.LabelField("Y", EditorStyles.whiteLabel, GUILayout.Width(30));
			EditorGUILayout.PropertyField(yIndex, GUIContent.none);
			EndHorizontalCentered();

			EditorGUI.EndDisabledGroup();

			// More controls
			isControlShown = EditorGUILayout.Foldout(isControlShown, "Control", true, EditorStyles.foldout);
			if (isControlShown)
			{
				EditorGUI.indentLevel++;

				EditorGUI.BeginDisabledGroup(true);

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Fixed Tile", GUILayout.Width(70));
				EditorGUILayout.PropertyField(fixTile,GUIContent.none);
				EditorGUILayout.EndHorizontal();

				EditorGUI.EndDisabledGroup();

				EditorGUI.indentLevel--;
			}

			// More info
			isInfoShown = EditorGUILayout.Foldout(isInfoShown, "Info", true, EditorStyles.foldout);
			if (isInfoShown)
			{
				EditorGUI.indentLevel++;

				EditorGUI.BeginDisabledGroup(true);

				EditorGUILayout.BeginVertical();
				EditorGUILayout.PropertyField(entropy);
				EditorGUILayout.PropertyField(selectedID);
				EditorGUILayout.PropertyField(isDefinite);
				//EditorGUILayout.PropertyField(entropy);
				EditorGUILayout.EndVertical();

				EditorGUI.EndDisabledGroup();

				EditorGUI.indentLevel--;
			}

			serializedObject.ApplyModifiedProperties();
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
