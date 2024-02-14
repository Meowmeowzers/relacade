# Relacade / Hello World
A prototype program to procedurally generate tile based game levels in Unity Engine using my own implementation of WaveFunctionCollapse algorithm.

## How to install
1. Copy this link.
	- https://github.com/Meowmeowzers/Relacade.git
2. In Unity package manager, press the + button near the top right of the package manager window.
Choose the "Add package from git URL..." option.
3. Paste the copied link.
4. Wait for the installation to finish.

## Important components
1. Input Tile
	- Contains data for your tile.
2. Input Tile Set
	- An asset that contains a set of input tiles. Provides convenience.
3. Wave Tile Grid
	- A game object that serves as a canvas for the level generation process. Only runs in Editor runtime.
4. Tile Set Up Window
	- A dedicated window for setting up input tiles. Provide an input tile set.

## How to use
1. Design and create your tile-based level gameobjects
2. Open TileSetUpWindow
3. Create InputTileSet asset
4. Create and setup InputTiles
5. Add your InputTiles to the InputTileSet
6. Set up compatible tiles for an InputTile
7. Create a WaveTileGrid object in scene
8. Plug in your InputTileSet to the WaveTileGrid
9. Generate

## Update
- 2024/2 
	- Now uses weighted random choice instead of randomly choosing tiles. This gives more control to the final output.
	- Can now modify height and width of tile grid.
- 2023
	- Initial

## Issues
- UI problems(Ease of access, janky, flow, serialization, data persistence, inconsistency)

## Notes
- A very simple implementation. Simple tiled model. No rotations. No backtracking
- Doesn't use Unity tilemap.

## Related Sources:
- [WFC(mxgmn)](https://github.com/mxgmn/WaveFunctionCollapse)
- [Unity Scripting Documentation](https://docs.unity3d.com/2022.2/Documentation/ScriptReference/index.html)