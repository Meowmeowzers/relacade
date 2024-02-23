using UnityEditor;
using UnityEngine;

namespace HelloWorld.Editor
{
	public class MenuItems : EditorWindow
	{
		[MenuItem("Relacade/TileSet Configure")]
		private static void StartTestWindow()
		{
			NewSetUpWindow window = (NewSetUpWindow)GetWindow(typeof(NewSetUpWindow));
			window.titleContent = new("Configure tile set");
			window.minSize = new(690, 350);
			window.Show();
		}

		[MenuItem("Relacade/Create Tile Set", priority = 1)]
		private static void CreateTileSetConfiguration()
		{
			ScriptableObject scriptableObject = CreateInstance<TileInputSet>();
			string savePath = EditorUtility.SaveFilePanelInProject("Save tile set", "InputTileSet", "asset", "Choose a location to save the Tile Set asset.");
			if (string.IsNullOrEmpty(savePath)) return;
			AssetDatabase.CreateAsset(scriptableObject, savePath);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		[MenuItem("Relacade/Create WaveGrid Object", priority = 1)]
		private static void CreateWave()
		{
			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.gatozhanya.relacade/Objects/EditorWave.prefab");

			if (prefab != null)
			{
				GameObject prefabInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
				prefabInstance.name = prefab.name + " (Instance)";
			}
			else
			{
				Debug.LogWarning("Corrupt package....");
			}
		}

		//[MenuItem("Relacade/Create/Sample/Tile Set Configuration 3x3", priority = 3)]
		//private static void CreateSampleTileSetConfiguration()
		//{
		//    ScriptableObject loadedAsset;

		//    string assetPath = "Packages/com.gatozhanya.relacade/Objects/Sample/Sample 2.asset";
		//    loadedAsset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);

		//    if (loadedAsset == null)
		//    {
		//        Debug.Log("Fail to find asset");
		//        return;
		//    }

		//    ScriptableObject duplicatedObject = Instantiate(loadedAsset);

		//    string outputPath = EditorUtility.SaveFilePanel("Save Scriptable Object", "Sample Tile Set", "Sample Tile Set Config", "asset");

		//    if (string.IsNullOrEmpty(outputPath))
		//    {
		//        Debug.Log("Save operation cancelled");
		//        return;
		//    }

		//    outputPath = FileUtil.GetProjectRelativePath(outputPath);
		//    AssetDatabase.CreateAsset(duplicatedObject, outputPath);
		//    AssetDatabase.SaveAssets();
		//    AssetDatabase.Refresh();
		//}

		//[MenuItem("Relacade/Create/Sample/Tile Set Configuration 2x2", priority = 3)]
		//private static void CreateSampleTileSetConfiguration2()
		//{
		//    string assetPath = "Packages/com.gatozhanya.relacade/Objects/Sample/Sample 1.asset";
		//    ScriptableObject loadedAsset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);

		//    if (loadedAsset == null)
		//    {
		//        Debug.Log("Fail to find asset");
		//        return;
		//    }

		//    ScriptableObject duplicatedObject = Instantiate(loadedAsset);

		//    string outputPath = EditorUtility.SaveFilePanel("Save Scriptable Object", "Sample Tile Set", "Sample Tile Set Config", "asset");

		//    if (string.IsNullOrEmpty(outputPath))
		//    {
		//        Debug.Log("Save operation cancelled");
		//        return;
		//    }

		//    outputPath = FileUtil.GetProjectRelativePath(outputPath);
		//    AssetDatabase.CreateAsset(duplicatedObject, outputPath);
		//    AssetDatabase.SaveAssets();
		//    AssetDatabase.Refresh();
		//}
	}
}