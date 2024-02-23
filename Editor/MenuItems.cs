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

		[MenuItem("Relacade/Create WaveGrid", priority = 1)]
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
	}
}