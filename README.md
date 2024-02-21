# Relacade / Hello World
A prototype program to procedurally generate tile based game levels in Unity Engine using own implementation of WaveFunctionCollapse algorithm.

## How to install
1. Copy this link.
	- https://github.com/Meowmeowzers/Relacade.git
2. In Unity package manager, press the + button near the top right of the package manager window.
Choose the "Add package from git URL..." option.
3. Paste the copied link.
4. Wait for the installation to finish.

## Important components
1. Input Tile Set
	- An asset that contains a set of input tiles.
2. Wave Tile Grid
	- A game object that serves as a canvas for the level generation process.
3. Tile Set Up Window
	- A dedicated window for setting up input tiles. Provide an input tile set.

## How to use
1. Design and create your tile-based level gameobjects
2. Open TileSetUpWindow
3. Create InputTileSet asset
4. Create and setup InputTiles
5. Set up compatible tiles for an InputTile
6. Create a WaveTileGrid object in scene
7. Plug in your InputTileSet to the WaveTileGrid
8. Generate

## Update
- 2024/2 
	- Now using weighted random choice instead of randomly choosing tiles.
	- Can set fixed tiles.
	- No need to create input tile assets.
	- UI improvements
	- Can now modify height and width of tile grid.
	- Remove features that are useless or broken.
		- input tile generator
		- save/load
- 2023
	- Initial

## Issues
- UI problems

## Notes
- A simple implementation. Simple tiled model. No rotations. No backtracking. Simple entropy
- Doesn't use Unity tilemap.

## Related Sources:
- [WFC(mxgmn)](https://github.com/mxgmn/WaveFunctionCollapse)
- [Unity Scripting Documentation](https://docs.unity3d.com/2022.2/Documentation/ScriptReference/index.html)