ROOT_DIR = $(dir $(shell cd))

TILEMAPS := $(subst $(ROOT_DIR),..\,$(shell dir /B /S ..\Artwork\Tiles\Tilemaps\*.tmx))
SPIRIT_TILEMAPS := $(TILEMAPS:.tmx=.tmx.spirit)

SPIRIT_TILESETS := ..\Artwork\Tiles\Tilesets\DessertSpirit.png \
	..\Artwork\Tiles\Tilesets\Spirit_Interior.png

.PHONY: tilemaps

tilemaps: tilesets $(SPIRIT_TILEMAPS)

tilesets: $(SPIRIT_TILESETS)

..\Artwork\Tiles\Tilesets\DessertSpirit.png: ..\Artwork\Tiles\Tilesets\Dessert.png ..\Artwork\Tiles\Tilesets\palletes\DessertPalleteSwap.txt 
	ImagePalleteSwap.exe $< $(word 2,$^) $@
	
..\Artwork\Tiles\Tilesets\Spirit_Interior.png: ..\Artwork\Tiles\Tilesets\Interior.png ..\Artwork\Tiles\Tilesets\palletes\DessertPalleteSwap.txt 
	ImagePalleteSwap.exe $< $(word 2,$^) $@
	
%.tmx.spirit:%.tmx
	SpiritWorldGenerator.exe $< -o $@ -m tileset_mapping.list
	Tiled2UnityLite.exe --scale=0.03125 $< ..\BattlePrototype\Assets\Tiled2Unity
	Tiled2UnityLite.exe --scale=0.03125 $@ ..\BattlePrototype\Assets\Tiled2Unity