using UnityEditor;
using UnityEngine;

namespace HelloWorld.Editor
{
    public class MenuItems : EditorWindow
    {
        [MenuItem("Relacade/Configure Input TileSet")]
        private static void StartTestWindow()
        {
            NewSetUpWindow window = (NewSetUpWindow)GetWindow(typeof(NewSetUpWindow));
            window.titleContent = new("Configure tile set");
            window.minSize = new(690, 350);
            window.Show();
        }

        [MenuItem("Relacade/Create Input Tile Set", priority = 1)]
        private static void CreateTileSetConfiguration()
        {
            ScriptableObject scriptableObject = CreateInstance<TileInputSet>();
            string savePath = EditorUtility.SaveFilePanelInProject("Save tile input set", "Input Tile Set", "asset", "Choose a location to save the Tile Set Configuration.");
            if (string.IsNullOrEmpty(savePath)) return;
            AssetDatabase.CreateAsset(scriptableObject, savePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        //[MenuItem("Relacade/Create/Input Tile", priority = 1)]
        //private static void CreateInputTile()
        //{
        //    ScriptableObject scriptableObject = CreateInstance<TileInput>();
        //    string savePath = EditorUtility.SaveFilePanelInProject("Save Scriptable Object", "Input Tile", "asset", "Choose a location to save the ScriptableObject.");
        //    if (string.IsNullOrEmpty(savePath)) return;
        //    AssetDatabase.CreateAsset(scriptableObject, savePath);
        //    AssetDatabase.SaveAssets();
        //    AssetDatabase.Refresh();
        //}

        [MenuItem("Relacade/Create WaveTileGrid Object", priority = 1)]
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
                Debug.LogWarning("Prefab not found....");
            }
        }

        //[MenuItem("Relacade/Create/Sample/Tile Set Configuration 3x3", priority = 3)]
        //private static void CreateSampleTileSetConfiguration()
        //{
        //    ScriptableObject loadedAsset;

        //    string assetPath = "Packages/com.gatozhanya.relacade/Objects/3x3 Input Tiles and Set/3x3 Sample tile set config.asset";
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
        //    string assetPath = "Packages/com.gatozhanya.relacade/Objects/2x2 Input Tiles and Set/2x2 Sample Tile Set Config.asset";
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