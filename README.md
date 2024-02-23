# Relacade / Hello World
A custom package for Unity Engine to procedurally generate 2D tile based game levels using own simple implementation of WaveFunctionCollapse algorithm.

## How to install
1. Copy this link.
	- https://github.com/Meowmeowzers/Relacade.git
2. In Unity package manager, press the + button near the top left of the package manager window.
3. Choose the "Add package from git URL..." option.
4. Paste the copied link.
5. Wait for the installation to finish.

## Important components
1. Input Tile Set
	- An asset that contains a set of input tiles.
2. WaveGrid
	- A game object that serves as a canvas for the level generation process.
3. Tile Set Configure Window
	- A dedicated window for setting up input tiles.

## How to use (General)
1. Design and create your tile-based level gameobjects
2. Open TileSetUpWindow
3. Create InputTileSet asset
4. Create and setup InputTiles
5. Set up compatible tiles for an InputTile
6. Create a WaveGrid object in scene
7. Plug in your InputTileSet to the WaveTileGrid
8. Generate

## How to use fixed tiles
1. Initialize a WaveGrid object in scene
2. Choose a cell by either locating it nested within the WaveGrid object or by accessing the scene's gizmos and clicking on the cell gizmo
3. Select a tile from the GridCell inspector
4. In WaveGrid, check the "Enable fixed tiles" toggle
5. Generate
	- Using the clear button clears the grid and shows fixed tiles

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
- Please dont modify the input tile set outside of the TileSet Configure Window. It may cause issues.
- InputTileSet asset may randomly bloat. Use the clean/reload option in TileSet Configure Window to attempt to fix it.

## Notes
- A simple implementation. Simple tiled model. No rotations. No backtracking. Simple entropy
- Doesn't use Unity tilemap.

## Related Links:
- [WFC(mxgmn)](https://github.com/mxgmn/WaveFunctionCollapse)
- [Unity Scripting Documentation](https://docs.unity3d.com/2022.2/Documentation/ScriptReference/index.html)