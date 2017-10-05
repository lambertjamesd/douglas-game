using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Tiled2Unity.CustomTiledImporter]
public class TiledObjectImporter : Tiled2Unity.ICustomTiledImporter {
    private MapNames maps;

    private MapNames GetMapNames()
    {
        if (maps == null)
        {
            maps = UnityEditor.AssetDatabase.LoadAssetAtPath<MapNames>("Assets/Maps/WorldMaps.asset");
        }

        return maps;
    }

    private void SaveMapNames()
    {
        UnityEditor.AssetDatabase.SaveAssets();
    }

	private static string[] sideAttributes = new string[]{
		"TopMap",
		"RightMap",
		"BottomMap",
		"LeftMap",
        "DeadMap",
        "LivingMap"
	};

	private static PrefabNames prefabMapping = null;

	private static PrefabNames getPrefabMapping() {
        if (prefabMapping == null)
        {
            prefabMapping = UnityEditor.AssetDatabase.LoadAssetAtPath<PrefabNames>("Assets/Maps/PrefabNames.asset");
		}

		return prefabMapping;
	}

	private static string PrefabNamesPath = "PrefabNames";
    private string mapName = null;

	public void HandleCustomProperties(GameObject gameObject,
	                                   IDictionary<string, string> keyValuePairs)
	{
        if (keyValuePairs.ContainsKey("PortalTo"))
        {
			MapPortal portal = gameObject.AddComponent<MapPortal>();
			string[] parts = keyValuePairs["PortalTo"].Split(new char[]{':'}, 2);
			portal.target = new MapPath(parts[0], parts.Length > 1 ? parts[1] : "default");

			gameObject.GetComponent<Collider2D>().isTrigger = true;
		}
		
		if (keyValuePairs.ContainsKey("PortalFrom"))
        {
			StartingPoint portal = gameObject.AddComponent<StartingPoint>();
			portal.locationName = keyValuePairs["PortalFrom"];
            Collider2D collider = gameObject.GetComponent<Collider2D>();

            gameObject.transform.position = collider.bounds.center;
            GameObject.DestroyImmediate(collider);
        }

		if (keyValuePairs.ContainsKey("ObjectName"))
        {
			PrefabSpawner spawner = gameObject.AddComponent<PrefabSpawner>();
			PrefabNames names = getPrefabMapping();
			if (names != null)
            {
				spawner.toSpawn = names.GetEntry(keyValuePairs["ObjectName"]).prefab;
				spawner.condition = keyValuePairs.ContainsKey("When") ? keyValuePairs["When"] : "";
			}

			BoxCollider2D collider = gameObject.GetComponent<BoxCollider2D>();

            if (collider != null)
            {
			    gameObject.transform.position = gameObject.transform.position + new Vector3(collider.offset.x, collider.offset.y, 0.0f);
			    Object.DestroyImmediate(collider);
            }

            Tiled2Unity.TileObject tileObject = gameObject.GetComponent<Tiled2Unity.TileObject>();

            if (tileObject != null)
            {
                gameObject.transform.position += new Vector3(tileObject.TileWidth * 0.5f, tileObject.TileHeight * 0.5f, 0.0f);
                Object.DestroyImmediate(tileObject);
            }

            while (gameObject.transform.childCount > 0)
            {
                Object.DestroyImmediate(gameObject.transform.GetChild(0).gameObject);
            }
		}

		if (sideAttributes.Any((key) => keyValuePairs.ContainsKey(key))) {
			MapAttachements attatchments = gameObject.AddComponent<MapAttachements>();
			attatchments.attachments = sideAttributes.Select((name) => {
				if (keyValuePairs.ContainsKey(name)) {
					return keyValuePairs[name];
				} else {
					return null;
				}
			}).ToArray();
		}

        Tiled2Unity.TiledMap map = gameObject.GetComponent<Tiled2Unity.TiledMap>();

        if (map != null)
        {
            if (keyValuePairs.ContainsKey("Name"))
            {
                mapName = keyValuePairs["Name"];
            }
            else
            {
                mapName = null;
            }
            Pathfinding pathfinding = gameObject.AddComponent<Pathfinding>();
            pathfinding.width = map.NumTilesWide;
            pathfinding.height = map.NumTilesHigh;
            pathfinding.tileSize = new Vector2(map.TileWidth, map.TileHeight) * map.ExportScale;
            pathfinding.layers = new List<PathfindingLayer>{
                new PathfindingLayer(PathingTypes.Walking, (1 << LayerMask.NameToLayer("L1: Barrier")) | (1 << LayerMask.NameToLayer("L1: ProjectilePassible")) | (1 << LayerMask.NameToLayer("L1: Water"))),
                new PathfindingLayer(PathingTypes.Flying, 1 << LayerMask.NameToLayer("L1: Barrier")),
            };
        }
	}
	
	public void CustomizePrefab(GameObject prefab)
	{

    }

    public void HandleFinalPrefab(UnityEngine.Object prefab)
    {
        if (mapName != null && prefab is GameObject)
        {
            var mapNames = GetMapNames();
            mapNames.AddEntry(new MapEntry(mapName, ((GameObject)prefab).GetComponent<Tiled2Unity.TiledMap>()));
            EditorUtility.SetDirty(mapNames);
            SaveMapNames();
        }
    }
}
