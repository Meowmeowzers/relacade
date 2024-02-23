using UnityEditor;
using UnityEngine;

namespace HelloWorld.Editor
{
	public class MenuItems : EditorWindow
	{
		[MenuItem("Relacade/TileSet Configure")]
		private static void StartTestWindow()
		{
			SetUpWindow window = (SetUpWindow)GetWindow(typeof(SetUpWindow));
			window.titleContent = new("Configure tile set");
			window.minSize = new(690, 350);
			window.Show();
		}

		[MenuItem("Relacade/Create WaveGrid", priority = 1)]
		private static void CreateWave()
		{
			GameObject gameObject = new("WaveGrid");
			gameObject.AddComponent<EditorWave>();
		}
	}
}