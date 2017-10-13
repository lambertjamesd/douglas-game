ImagePalleteSwap.exe ..\Artwork\Tiles\Tilesets\Dessert.png ..\Artwork\Tiles\Tilesets\palletes\DessertPalleteSwap.txt ..\Artwork\Tiles\Tilesets\DessertSpirit.png
ImagePalleteSwap.exe ..\Artwork\Tiles\Tilesets\Interior.png ..\Artwork\Tiles\Tilesets\palletes\DessertPalleteSwap.txt ..\Artwork\Tiles\Tilesets\Spirit_Interior.png

for /f "tokens=*" %%a in (included_tilesets.list) do (
	SpiritWorldGenerator.exe ..\Artwork\Tiles\Tilemaps\%%a.tmx -o ..\Artwork\Tiles\Tilemaps\%%a_Spirit.tmx -m tileset_mapping.list
	Tiled2UnityLite.exe --scale=0.03125 ..\Artwork\Tiles\Tilemaps\%%a.tmx ..\BattlePrototype\Assets\Tiled2Unity
	Tiled2UnityLite.exe --scale=0.03125 ..\Artwork\Tiles\Tilemaps\%%a_Spirit.tmx ..\BattlePrototype\Assets\Tiled2Unity
)