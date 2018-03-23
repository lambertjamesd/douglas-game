using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MapDirections
{
    Top,
    Right,
    Bottom,
    Left,
    Dead,
    Living,
}

public class MapPair
{
    public string normalizedName;
    public Tiled2Unity.TiledMap livingMap;
    public Tiled2Unity.TiledMap deadMap;

    public MapPair(string normalizedName)
    {
        this.normalizedName = normalizedName;
    }
}

public class WorldController : MonoBehaviour
{
	private Bounds worldBounds = new Bounds(Vector3.zero, Vector3.zero);
	private MapAttachements currentAttachement = null;
	public int PixelsPerUnit = 32;
	public MapNames mapNames;
    public PrefabNames prefabNames;
    public TextAsset story;
	public MapPath startingLocation = new MapPath("default", "default");
	public GameObject player;
	public FollowCamera followCamera;
	private MapPair currentMap = null;
    public StoryFunctionBindings storyFunctionBindings;
    public bool isDead = false;

    private static Vector3 UnusedOffset = new Vector3(65536.0f, 0.0f, 0.0f);

    private static Vector2[] currentBoundsAnchor = new Vector2[]{
		new Vector2(0.0f, 1.0f),
		new Vector2(1.0f, 1.0f),
		new Vector2(0.0f, 0.0f),
		new Vector2(0.0f, 1.0f),
        new Vector2(0.0f, 0.0f),
        new Vector2(0.0f, 0.0f),
	};

	private static Vector2[] newBoundAnchor = new Vector2[]{
		new Vector2(0.0f, 1.0f),
		new Vector2(0.0f, 0.0f),
		new Vector2(0.0f, 0.0f),
		new Vector2(-1.0f, 0.0f),
        new Vector2(0.0f, 0.0f),
        new Vector2(0.0f, 0.0f),
	};

	private static Vector2[] mapDirection = new Vector2[]{
		new Vector2(0.0f, 1.0f),
		new Vector2(1.0f, 0.0f),
		new Vector2(0.0f, -1.0f),
		new Vector2(-1.0f, 0.0f),
        new Vector2(0.0f, 0.0f),
        new Vector2(0.0f, 0.0f),
	};

	Vector2 MapSize(Tiled2Unity.TiledMap from) {
		return new Vector2(
			from.NumTilesWide * from.TileWidth / PixelsPerUnit, 
			from.NumTilesHigh * from.TileHeight / PixelsPerUnit
		);
	}

	Vector3 LerpMap(Tiled2Unity.TiledMap from, Vector2 lerp) {
		Vector2 mapSize = MapSize(from);

		Vector3 localPosition = new Vector3(
			mapSize.x * lerp.x, 
			-mapSize.y * lerp.y,
			0.0f
		);

		return from.transform.TransformPoint(localPosition);
	}

	public void AttachTilemap(Tiled2Unity.TiledMap from, Tiled2Unity.TiledMap tilemap, Vector3 direction)
    {
	
	}

	public void Reset() {
		if (currentMap != null) {
			Destroy(currentMap.deadMap.gameObject);
            Destroy(currentMap.livingMap.gameObject);
        }
		currentMap = null;
	}

    public MapPair GetCurrentMap()
    {
        return currentMap;
    }

	private Tiled2Unity.TiledMap SpawnTilemap(Tiled2Unity.TiledMap from, Vector3 origin)
    {
		Tiled2Unity.TiledMap result = Instantiate(from);

		result.transform.position = origin;
		result.transform.parent = transform;

        return result;
	}

    private void SetActiveMap(Tiled2Unity.TiledMap map)
    {
        Projectile.projectileParent = map.transform;
        currentAttachement = map.gameObject.GetComponent<MapAttachements>();
    }

    private MapPair SpawnMapPair(string mapName, Vector3 origin, Vector2 sizeLerp)
    {
        var mapEntry = mapNames.GetEntry(mapName);
        var map = SpawnTilemap(mapEntry.tiled, Vector3.zero);
        var mapSize = MapSize(map);
        origin += new Vector3(sizeLerp.x * mapSize.x, sizeLerp.y * mapSize.y);
        isDead = IsSpirit(mapName);
        var mapPair = new MapPair(NormalizeName(mapName));

        isDead = IsSpirit(mapName);

        if (isDead)
        {
            var otherEntry = mapNames.GetEntry(NormalizeName(mapName));

            mapPair.deadMap = map;
            mapPair.livingMap = SpawnTilemap(otherEntry.tiled, UnusedOffset);
        }
        else
        {
            var otherEntry = mapNames.GetEntry(SpiritName(mapName));

            mapPair.deadMap = SpawnTilemap(otherEntry.tiled, UnusedOffset);
            mapPair.livingMap = map;
        }

        SetActiveMap(map);
        currentMap = mapPair;

        var min = LerpMap(map, Vector2.zero);
        var max = LerpMap(map, Vector2.one);

        worldBounds.min = Vector3.Min(min, max);
        worldBounds.max = Vector3.Max(min, max);

        if (followCamera != null)
        {
            followCamera.bounds = worldBounds;
        }

        return mapPair;
    }

	public void Start() {
		if (followCamera != null && followCamera.target == null) {
			followCamera.target = player.transform;
		}

        if (story != null)
        {
            StoryManager.GetSingleton().SetStory(story);
            StoryManager.GetSingleton().currentBindings = storyFunctionBindings;
        }

        if (WorldInitializer.location != null)
        {
            Goto(WorldInitializer.location);
        }
        else if (startingLocation != null)
        {
			Goto(startingLocation);
		}
	}



	public void Update()
    {
        if (WorldInitializer.storyKnot != null && WorldInitializer.storyKnot.Length > 0)
        {
            StartCoroutine(StoryFunctionBindings.GetBindings().interact(WorldInitializer.storyKnot));
            WorldInitializer.storyKnot = null;
        }

        if (currentAttachement != null && player != null)
        {
			Vector3 playerPos = player.transform.position;
			Vector2 minOffset = playerPos - worldBounds.min;
			Vector2 maxOffset = playerPos - worldBounds.max;

			for (int i = 0; i < mapDirection.Length; ++i)
            {
				if (currentAttachement.attachments[i] != null && currentAttachement.attachments[i] != "" && Vector2.Dot(minOffset, mapDirection[i]) > 0 && Vector2.Dot(maxOffset, mapDirection[i]) > 0)
                {
                    string nextMap = currentAttachement.attachments[i];
                    Reset();
                    SpawnMapPair(nextMap,
						new Vector3(
							Mathf.Lerp(worldBounds.min.x, worldBounds.max.x, currentBoundsAnchor[i].x),
							Mathf.Lerp(worldBounds.min.y, worldBounds.max.y, currentBoundsAnchor[i].y),
							0.0f
						), 
                        newBoundAnchor[i]
					);
					break;
				}
			}
		}
	}

    public void SwitchTo(MapDirections direction)
    {
        if (direction == MapDirections.Living && isDead || direction == MapDirections.Dead && !isDead)
        {
            if (isDead)
            {
                currentMap.deadMap.transform.localPosition += UnusedOffset;
                currentMap.livingMap.transform.localPosition -= UnusedOffset;
                SetActiveMap(currentMap.livingMap);
            }
            else
            {
                currentMap.deadMap.transform.localPosition -= UnusedOffset;
                currentMap.livingMap.transform.localPosition += UnusedOffset;
                SetActiveMap(currentMap.deadMap);
            }
            isDead = !isDead;
        }
    }

    private static bool IsSpirit(string name)
    {
        return name.StartsWith("Spirit_");
    }

    private static string NormalizeName(string name)
    {
        if (IsSpirit(name))
        {
            return name.Substring("Spirit_".Length);
        }
        else
        {
            return name;
        }
    }

    public static string SpiritName(string name)
    {
        return "Spirit_" + name;
    }

	public void Goto(MapPath location) {
		Reset();
        var mapPair = SpawnMapPair(location.mapName, Vector3.zero, Vector2.zero);

        if (player != null) {
            var map = isDead ? mapPair.deadMap : mapPair.livingMap;

			var staringPoints = map.GetComponentsInChildren<StartingPoint>();

			StartingPoint start = null;

			foreach (var check in staringPoints) {
				if (check.locationName == location.portalName) {
					start = check;
					break;
				}
			}

			player.transform.position = start == null ? LerpMap(map, new Vector2(0.5f, 0.5f)) : start.transform.position;
		}
	}
}
