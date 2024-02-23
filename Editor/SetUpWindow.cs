using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HelloWorld.Editor
{
	public class SetUpWindow : EditorWindow
	{
		private InputTileSet selectedInputTileSet;
		private SerializedObject serializedTileSetObject;
		private SerializedProperty allInput;
		private SerializedProperty tileReferences;

		private SerializedObject selectedTileConstraints;
		private SerializedProperty tileName;
		private SerializedProperty gameObject;
		private SerializedProperty weight;
		private SerializedProperty compatibleTopList;
		private SerializedProperty compatibleBottomList;
		private SerializedProperty compatibleLeftList;
		private SerializedProperty compatibleRightList;
		private Texture2D previewTexture;

		private InputTile tempInputTile;
		private string assetName = "New Tile Set";
		private bool[] showCompatibleTiles = { true, true, true, true, true, true };
		private int selectedTileIndex = 0;
		private bool shouldClear = false;

		#region Window Variables

		private readonly int headerSectionHeight = 26;
		private readonly int tileSetupSectionWidth = 312;

		private Texture2D headerBackgroundTexture;
		private Texture2D leftBackgroundTexture;
		private Texture2D rightBackgroundTexture;
		private Texture2D listItemTexture;
		private Texture2D listItemLightTexture;

		private Color headerBackgroundColor = new(30f / 255f, 30f / 255f, 30f / 255f, 0.5f);
		private Color leftBackgroundColor = new(30f / 255f, 30f / 255f, 30f / 255f, 0.5f);
		private Color rightBackgroundColor = new(0.6f, .2f, 0.7f, 0.7f);
		private Color listItemColor = new(0.18f, 0.18f, 0.18f, 1f);
		private Color listItemLightColor = new(0.2f, 0.2f, 0.2f, 1f);

		private Rect headerSection;
		private Rect tileSetupSection;
		private Rect tileConstraintSetupSection;

		private Vector2 scrollPositionLeft = Vector2.zero;
		private Vector2 scrollPositionRight = Vector2.zero;

		private GenericMenu DeletesDropdown = new();

		private GUIStyle customStyle = new();

		private GUIContent weightLabel = new("Weight", "Higher weight means more chance to spawn");
		private GUIContent gameObjectLabel = new("GameObject", "GameObject to spawn");
		private GUIContent mirrorLabel = new("Mirror", "Make this tile and the selected tiles be compatible with each other");
		private GUIContent sendToLabel = new("Send", "Make the selected tiles compatible with this tile");
		private GUIContent receiveFromLabel = new("Receive", "Make this tile compatible with other tiles");
		private GUIContent compatibleTopLabel = new("Compatible Top", "Compatible tiles for adjacent top");
		private GUIContent compatibleBottomLabel = new("Compatible Bottom", "Compatible tiles for adjacent bottom");
		private GUIContent compatibleLeftLabel = new("Compatible Left", "Compatible tiles for adjacent left");
		private GUIContent compatibleRightLabel = new("Compatible Right", "Compatible tiles for adjacent right");
		private GUIContent cleanUpTileConstraintsLabel = new("Clean/Clean up all tile compatibilities", "Clean missing/corrupted constraints from selected tile");
		private GUIContent cleanUpSetLabel = new("Clean/Clean up tile set", "Clean up missing/corrupted tile from all tiles in set");
		private GUIContent reloadSetLabel = new("Clean/Reload set", "Attempt to clean the set and its tile's constraints then reload");
		private GUIContent autoSetIDLabel = new("Clean/Set IDs for all tile");
		private GUIContent deleteOneLabel = new("Delete this tile");
		private GUIContent deleteAllLabel = new("Delete all tiles");
		private GUIContent clearOneLabel = new("Clear this tile's compatibilites");
		private GUIContent clearAllLabel = new("Clear all tile compatibilities");
		private GUIContent enableClearAndDeleteLabel = new("Clear/Delete?", "Enables deleting and clearing of tiles");

		#endregion Window Variables

		private void OnEnable()
		{
			InitTextures();
			InitData();
			InitDeletesMenu();
		}

		private void OnDestroy()
		{
			GetWindow<ReceiveConstraintsWindow>().Close();
			GetWindow<SendConstraintsWindow>().Close();
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

			listItemTexture = new Texture2D(1, 1);
			listItemTexture.SetPixel(0, 0, listItemColor);
			listItemTexture.Apply();

			listItemLightTexture = new Texture2D(1, 1);
			listItemLightTexture.SetPixel(0, 0, listItemLightColor);
			listItemLightTexture.Apply();

			customStyle.normal.background = listItemTexture;
			customStyle.hover.background = listItemLightTexture;
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

		private void InitDeletesMenu()
		{
			DeletesDropdown.AddItem(cleanUpTileConstraintsLabel, false, () =>
			{
				if (selectedInputTileSet != null)
				{
					CleanUpAllTileConstraints();
				}
			});
			DeletesDropdown.AddItem(cleanUpSetLabel, false, () =>
			{
				if (selectedInputTileSet != null)
				{
					CleanUpSet();
				}
			});
			DeletesDropdown.AddItem(reloadSetLabel, false, () =>
			{
				if (selectedInputTileSet != null)
				{
					CleanUpSet();
					CleanUpAllTileConstraints();
				}
			}); 
			DeletesDropdown.AddSeparator("Clean/");
			DeletesDropdown.AddItem(autoSetIDLabel, false, () =>
			{
				if (selectedInputTileSet != null)
				{
					GiveUniqueIDToTiles();
				}
			});
			DeletesDropdown.AddSeparator("");
			DeletesDropdown.AddItem(deleteOneLabel, false, () =>
			{
				if (shouldClear && selectedTileConstraints != null)
				{
					RemoveAndDeleteTile(allInput, tileReferences, selectedTileIndex);
					GiveUniqueIDToTiles();
				}
			});
			DeletesDropdown.AddItem(clearOneLabel, false, () =>
			{
				if (shouldClear && selectedTileConstraints != null)
				{
					ClearInputTileConstraints();
				}
			});
			DeletesDropdown.AddSeparator("");
			DeletesDropdown.AddItem(deleteAllLabel, false, () =>
			{
				if (shouldClear && allInput != null)
				{
					RemoveAndDeleteAllTiles(allInput, tileReferences);
					GiveUniqueIDToTiles();
					shouldClear = false;
				}
			});
			DeletesDropdown.AddItem(clearAllLabel, false, () =>
			{
				if (shouldClear && allInput != null)
				{
					ClearAllInputTileConstraints();
					shouldClear = false;
				}
			});
		}

		private void DrawHeader()
		{
			GUILayout.BeginArea(headerSection);
			EditorGUILayout.Space(3);
			GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Width(position.width));

			EditorGUILayout.LabelField("Input Tile Set", GUILayout.MaxWidth(80));
			selectedInputTileSet = (InputTileSet)EditorGUILayout.ObjectField(selectedInputTileSet, typeof(InputTileSet), false, GUILayout.MaxWidth(200));

			GUILayout.FlexibleSpace();

			shouldClear = EditorGUILayout.ToggleLeft(enableClearAndDeleteLabel, shouldClear, GUILayout.Width(90), GUILayout.ExpandWidth(false));
			EditorGUILayout.Space();
			if (GUILayout.Button("Options", EditorStyles.toolbarDropDown))
			{
				DeletesDropdown.ShowAsContext();
			}
			EditorGUILayout.EndHorizontal();
			GUILayout.EndArea();
		}

		private void DrawLeft()
		{
			GUILayout.BeginArea(tileSetupSection);
			EditorGUILayout.BeginVertical();

			if (selectedInputTileSet != null)
			{
				SerializeProperties();
				ShowTileSets(allInput);
			}

			EditorGUILayout.EndVertical();
			GUILayout.EndArea();
		}

		private void ShowTileSets(SerializedProperty allTiles)
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
			EditorGUILayout.LabelField(allInput.arraySize.ToString() + " Tiles", EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
			
			if (GUILayout.Button("+", EditorStyles.toolbarButton))
			{
				CreateAndAddTile(allTiles);
				GiveUniqueIDToTiles();
			}
			if (GUILayout.Button("-", EditorStyles.toolbarButton))
			{
				if (allTiles.arraySize > 0)
				{
					RemoveAndDeleteLastTile(allTiles);
					GiveUniqueIDToTiles();
				}
			}
			EditorGUILayout.EndHorizontal();

			scrollPositionLeft = EditorGUILayout.BeginScrollView(scrollPositionLeft, GUILayout.ExpandWidth(false), GUILayout.Height(position.height - 50));
	   
			SerializedProperty selectedTile;
			SerializedObject selectedTileObject;
			SerializedProperty selectedTileGameObject;
			Texture2D selectedTilePreviewTexture;

			for (int i = 0; i < allTiles.arraySize; i++)
			{
				if (allTiles.GetArrayElementAtIndex(i).objectReferenceValue != null)
				{
					selectedTile = allTiles.GetArrayElementAtIndex(i);
					selectedTileObject = new(allTiles.GetArrayElementAtIndex(i).objectReferenceValue);
					selectedTileGameObject = selectedTileObject.FindProperty("gameObject");
					selectedTilePreviewTexture = AssetPreview.GetAssetPreview(selectedTileGameObject.objectReferenceValue);

					EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(150));
					GUILayout.Label(selectedTilePreviewTexture, GUILayout.Width(50), GUILayout.Height(50));

					CenterVerticalStart(50);
					EditorGUILayout.LabelField(selectedInputTileSet.allInputTiles[i].tileName, EditorStyles.boldLabel, GUILayout.ExpandWidth(false));
					CenterVerticalEnd();

					CenterVerticalStart(50);
					if (GUILayout.Button(">", EditorStyles.toolbarButton, GUILayout.Width(30)))
					{
						if (selectedTile.objectReferenceValue != null)
						{
							selectedTileConstraints = new(selectedTile.objectReferenceValue);
							selectedTileIndex = i;
						}
						else
						{
							selectedTileConstraints = null;
							selectedTileIndex = i;
						}
					}
					CenterVerticalEnd();
					EditorGUILayout.EndHorizontal();
				}
				else
				{
					EditorGUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					EditorGUILayout.LabelField("Missing Data", EditorStyles.miniLabel, GUILayout.Width(100), GUILayout.Height(50));

					if (GUILayout.Button(reloadSetLabel, EditorStyles.toolbarButton, GUILayout.Width(100), GUILayout.Height(50)))
					{
						selectedTileConstraints = null;
						CleanUpSet();
					}
					GUILayout.FlexibleSpace();
					EditorGUILayout.EndHorizontal();
				}
			}

			EditorGUILayout.EndScrollView();
			allTiles.serializedObject.ApplyModifiedProperties();
		}

		private void DrawRight()
		{
			GUILayout.BeginArea(tileConstraintSetupSection);

			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			EditorGUILayout.LabelField("Set Up", EditorStyles.boldLabel);
			GUILayout.EndHorizontal();

			if (selectedInputTileSet == null)
			{
				EditorGUILayout.BeginVertical(GUILayout.Width(100));
				EditorGUILayout.LabelField("Load an input tile set");
				selectedInputTileSet = (InputTileSet)EditorGUILayout.ObjectField(selectedInputTileSet, typeof(InputTileSet), false, GUILayout.Height(30));
				GUILayout.Space(10);
				EditorGUILayout.LabelField("or");
				GUILayout.Space(10);
				EditorGUILayout.LabelField("Create a new input tile set");
				CreateTileSetButton();
				EditorGUILayout.EndVertical();
			}
			else if (allInput.arraySize == 0)
			{
				GUILayout.Space(15);
				EditorGUILayout.HelpBox("* Press the + and - buttons to add new items  \n* Then, select an item from the tile list by pressing the > button", MessageType.Info);
			}
			else if (selectedTileConstraints == null)
			{
				GUILayout.Space(15);
				EditorGUILayout.BeginVertical();
				EditorGUILayout.HelpBox("Select an Input Tile", MessageType.Info);
				EditorGUILayout.EndVertical();
			}
			else
			{
				selectedTileConstraints?.Update();

				EditorGUILayout.Space(5);

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
					GUILayout.Label(Texture2D.blackTexture, GUILayout.Width(80), GUILayout.Height(80));
				else
					GUILayout.Label(previewTexture, GUILayout.Width(80), GUILayout.Height(80));
				

				CenterVerticalStart(80);
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Name", GUILayout.Width(80));
					EditorGUILayout.PropertyField(tileName, GUIContent.none);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(gameObjectLabel, GUILayout.Width(80));
					EditorGUILayout.PropertyField(gameObject, GUIContent.none);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(weightLabel, GUILayout.Width(80));
					EditorGUILayout.PropertyField(weight, GUIContent.none);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button(mirrorLabel, GUILayout.Height(30)))
				{
					MirrorConstraintsWindow.OpenWindow(selectedTileConstraints, allInput);
				}
				if (GUILayout.Button(sendToLabel, GUILayout.Height(30)))
				{
					SendConstraintsWindow.OpenWindow(selectedTileConstraints, allInput);
				}
				if (GUILayout.Button(receiveFromLabel, GUILayout.Height(30)))
				{
					ReceiveConstraintsWindow.OpenWindow(selectedTileConstraints, allInput);
				}
				EditorGUILayout.EndHorizontal();

				CenterVerticalEnd();

				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
				scrollPositionRight = EditorGUILayout.BeginScrollView(scrollPositionRight);
				EditorGUILayout.BeginVertical(GUILayout.MaxWidth(390));

				showCompatibleTiles[0] = EditorGUILayout.BeginFoldoutHeaderGroup(showCompatibleTiles[0], compatibleTopLabel + " - " + compatibleTopList.arraySize.ToString(), EditorStyles.foldoutHeader);
				if (showCompatibleTiles[0])
					ShowCompatibleTilesList(compatibleTopList);
				EditorGUILayout.EndFoldoutHeaderGroup();

				showCompatibleTiles[1] = EditorGUILayout.BeginFoldoutHeaderGroup(showCompatibleTiles[1], compatibleBottomLabel + " - " + compatibleBottomList.arraySize.ToString(), EditorStyles.foldoutHeader);
				if (showCompatibleTiles[1])               
					ShowCompatibleTilesList(compatibleBottomList);
				EditorGUILayout.EndFoldoutHeaderGroup();

				showCompatibleTiles[2] = EditorGUILayout.BeginFoldoutHeaderGroup(showCompatibleTiles[2], compatibleLeftLabel + " - " + compatibleLeftList.arraySize.ToString(), EditorStyles.foldoutHeader);
				if (showCompatibleTiles[2])
					ShowCompatibleTilesList(compatibleLeftList);
				EditorGUILayout.EndFoldoutHeaderGroup();

				showCompatibleTiles[3] = EditorGUILayout.BeginFoldoutHeaderGroup(showCompatibleTiles[3], compatibleRightLabel + " - " + compatibleRightList.arraySize.ToString(), EditorStyles.foldoutHeader);
				if (showCompatibleTiles[3])
					ShowCompatibleTilesList(compatibleRightList);
				EditorGUILayout.EndFoldoutHeaderGroup();

				EditorGUILayout.EndVertical();
				EditorGUILayout.EndScrollView();
				selectedTileConstraints.ApplyModifiedProperties();
			}

			GUILayout.EndArea();
		}

		private void ShowCompatibleTilesList(SerializedProperty property)
		{
			string tempString;
			for (int i = 0; i < property.arraySize; i++)
			{
				tempInputTile = property.GetArrayElementAtIndex(i).objectReferenceValue as InputTile;
				if (tempInputTile != null)
				{
					previewTexture = AssetPreview.GetAssetPreview(tempInputTile.gameObject);
					tempString = tempInputTile.tileName;
				}
				else
				{
					previewTexture = Texture2D.blackTexture;
					tempString = "Missing";
				}

				EditorGUILayout.BeginHorizontal(customStyle);
				GUILayout.Label(previewTexture, GUILayout.Height(30), GUILayout.Width(30));
				EditorGUILayout.LabelField(tempString, GUILayout.Height(30));
				EditorGUILayout.EndHorizontal();
			}
		}

		private void CreateTileSetButton()
		{
			assetName = EditorGUILayout.TextField(assetName, GUILayout.Width(250));

			if (GUILayout.Button("Create New Input Tile Set", GUILayout.Height(30)))
			{
				ScriptableObject scriptableObject = CreateInstance<InputTileSet>();
				string location = EditorUtility.SaveFilePanelInProject("Create new input tile set", assetName, "asset", "?Insert message?");

				AssetDatabase.CreateAsset(scriptableObject, location);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();

				selectedInputTileSet = scriptableObject as InputTileSet;
			}
		}

		// 
		private void CreateAndAddTile(SerializedProperty allTiles)
		{
			allTiles.arraySize++;
			tileReferences.arraySize++;

			InputTile newTileInput = CreateInstance<InputTile>();
			newTileInput.id = allInput.arraySize - 1;
			newTileInput.tileName = "New Tile" + " " + newTileInput.id;
			newTileInput.name = selectedInputTileSet.name + "_" + newTileInput.id;
			
			AssetDatabase.AddObjectToAsset(newTileInput, selectedInputTileSet);
			AssetDatabase.SaveAssets();

			allTiles.GetArrayElementAtIndex(allTiles.arraySize - 1).objectReferenceValue = newTileInput;
			tileReferences.GetArrayElementAtIndex(tileReferences.arraySize - 1).objectReferenceValue = newTileInput;
			selectedTileConstraints = null;

			CheckListForNull(allTiles);

			allTiles.serializedObject.ApplyModifiedProperties();
		}

		private void RemoveAndDeleteLastTile(SerializedProperty allTiles)
		{
			InputTile tileToDelete = (InputTile)allTiles.GetArrayElementAtIndex(allTiles.arraySize - 1).objectReferenceValue;
			allTiles.DeleteArrayElementAtIndex(allTiles.arraySize - 1);
			tileReferences.DeleteArrayElementAtIndex(tileReferences.arraySize - 1);

			AssetDatabase.RemoveObjectFromAsset(tileToDelete);
			AssetDatabase.Refresh();
			AssetDatabase.SaveAssets();

			selectedTileConstraints = null;

			CheckListForNull(allTiles);

			allTiles.serializedObject.ApplyModifiedProperties();
		}

		private void RemoveAndDeleteTile(SerializedProperty allTiles, SerializedProperty tileRef, int index)
		{
			InputTile tileToDelete = allTiles.GetArrayElementAtIndex(index).objectReferenceValue as InputTile;

			if (selectedInputTileSet.tileReferences.Contains(tileToDelete))
			{
				int tempIndex = selectedInputTileSet.tileReferences.IndexOf(tileToDelete);
				tileRef.DeleteArrayElementAtIndex(tempIndex);
			}

			allTiles.DeleteArrayElementAtIndex(index);
			AssetDatabase.RemoveObjectFromAsset(tileToDelete);

			selectedTileConstraints = null;
			CheckListForNull(allTiles);

			allTiles.serializedObject.ApplyModifiedProperties();
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		private void RemoveAndDeleteAllTiles(SerializedProperty allTiles, SerializedProperty tileReferences)
		{
			InputTile tileToDelete;

			for (int i = allInput.arraySize - 1; i >= 0; i--)
			{
				tileToDelete = allTiles.GetArrayElementAtIndex(i).objectReferenceValue as InputTile;
				allTiles.DeleteArrayElementAtIndex(i);
				AssetDatabase.RemoveObjectFromAsset(tileToDelete);
			}

			selectedTileConstraints = null;
			CleanUpSet();

			allTiles.serializedObject.ApplyModifiedProperties();
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		private bool CheckListForNull(SerializedProperty allTiles)
		{
			bool result = false;
			for (int i = allTiles.arraySize - 1; i >= 0; i--)
			{
				if (allTiles.GetArrayElementAtIndex(i).objectReferenceValue == null)
				{
					allTiles.DeleteArrayElementAtIndex(i);
					result = true;
				}
			}
			allTiles.serializedObject.ApplyModifiedProperties();
			return result;
		}

		private void ClearAllInputTileConstraints()
		{
			SerializedObject temp = new(selectedInputTileSet);

			ClearAllInputTileConstraintsInDirection(selectedInputTileSet.allInputTiles);

			temp.ApplyModifiedProperties();
			temp.Update();
		}

		private void ClearAllInputTileConstraintsInDirection(params List<InputTile>[] tileInDirectionList)
		{
			foreach (var set in tileInDirectionList)
			{
				foreach (var item in set)
				{
					if (item == null) continue;
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

		private void ClearInputTileConstraints()
		{
			InputTile item = selectedTileConstraints.targetObject as InputTile;
			if (item == null) return;
			else
			{
				SerializedObject temp = new(item);
				temp.FindProperty("compatibleTop").ClearArray();
				temp.FindProperty("compatibleBottom").ClearArray();
				temp.FindProperty("compatibleLeft").ClearArray();
				temp.FindProperty("compatibleRight").ClearArray();
				temp.ApplyModifiedProperties();
			}
		}

		private void GiveUniqueIDToTiles()
		{
			//serializeproperty.intValue doesnt work
			//Converted the serialized allTiles into a list, directly modified the list
			//IDK what happened it did not modified the serialized allTiles directly but it still works
			//Could it be that object reference value passes the reference?
			//im dumb lol
			//If automatic handling of multi - object editing, undo, and Prefab overrides is not needed,
			//the script variables can be modified directly by the editor without using the SerializedObject and SerializedProperty system
			//revise next time
			serializedTileSetObject.Update();

			SerializedProperty listProperty = serializedTileSetObject.FindProperty("allInputTiles");
			if (listProperty != null && listProperty.isArray)
			{
				List<InputTile> tileList = GetListFromSerializedProperty(listProperty);
				if (tileList != null)
				{
					for (int i = 0; i < tileList.Count; i++)
					{
						InputTile tile = tileList[i];
						if (tile != null)
						{
							tile.id = i;
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
					Debug.Log("Unable to retrieve List<TileInput> from serialized allTiles");
				}
			}
			else
			{
				Debug.Log("List<TileInput> allTiles is null or not a list");
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

			allInput = serializedTileSetObject.FindProperty("allInputTiles");
			tileReferences = serializedTileSetObject.FindProperty("tileReferences");

			serializedTileSetObject.Update();
		}

		private void CleanUpSet()
		{
			InputTile tempRef;

			CheckListForNull(allInput);

			for (int i = tileReferences.arraySize - 1; i >= 0; i--)
			{
				tempRef = tileReferences.GetArrayElementAtIndex(i).objectReferenceValue as InputTile;
				if (!selectedInputTileSet.allInputTiles.Contains(tempRef))
				{
					tileReferences.DeleteArrayElementAtIndex(i);
					AssetDatabase.RemoveObjectFromAsset(tempRef);
				}
			}
			serializedTileSetObject.ApplyModifiedProperties();
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		private void CleanUpAllTileConstraints()
		{
			SerializedObject selectedTileObject;

			for (int i = allInput.arraySize - 1;i >= 0; i--)
			{
				selectedTileObject = new(allInput.GetArrayElementAtIndex(i).objectReferenceValue);
				CleanUpTileConstraints(selectedTileObject);
			}
			AssetDatabase.Refresh();
		}
		private void CleanUpTileConstraints(SerializedObject selectedTileObject)
		{
			InputTile tempRef;
			SerializedProperty top = selectedTileObject.FindProperty("compatibleTop");
			SerializedProperty bottom = selectedTileObject.FindProperty("compatibleBottom");
			SerializedProperty left = selectedTileObject.FindProperty("compatibleLeft");
			SerializedProperty right = selectedTileObject.FindProperty("compatibleRight");

			for (int i = top.arraySize - 1; i >= 0; i--)
			{
				tempRef = top.GetArrayElementAtIndex(i).objectReferenceValue as InputTile;
				if (!selectedInputTileSet.allInputTiles.Contains(tempRef))
				{
					top.DeleteArrayElementAtIndex(i);
				}
			}
			for (int i = bottom.arraySize - 1; i >= 0; i--)
			{
				tempRef = bottom.GetArrayElementAtIndex(i).objectReferenceValue as InputTile;
				if (!selectedInputTileSet.allInputTiles.Contains(tempRef))
				{
					bottom.DeleteArrayElementAtIndex(i);
				}
			}
			for (int i = left.arraySize - 1; i >= 0; i--)
			{
				tempRef = left.GetArrayElementAtIndex(i).objectReferenceValue as InputTile;
				if (!selectedInputTileSet.allInputTiles.Contains(tempRef))
				{
					left.DeleteArrayElementAtIndex(i);
				}
			}
			for (int i = right.arraySize - 1; i >= 0; i--)
			{
				tempRef = right.GetArrayElementAtIndex(i).objectReferenceValue as InputTile;
				if (!selectedInputTileSet.allInputTiles.Contains(tempRef))
				{
					right.DeleteArrayElementAtIndex(i);
				}
			}
			selectedTileObject.ApplyModifiedProperties();
			AssetDatabase.Refresh();
		}

		// GUI
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

		// Helper
		private List<InputTile> GetListFromSerializedProperty(SerializedProperty property)
		{
			List<InputTile> list = new();
			for (int i = 0; i < property.arraySize; i++)
			{
				SerializedProperty elementProperty = property.GetArrayElementAtIndex(i);
				InputTile element = elementProperty.objectReferenceValue as InputTile;
				list.Add(element);
			}
			return list;
		}
	}
}