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

public class WorldController : MonoBehaviour {
	private Bounds worldBounds = new Bounds(Vector3.zero, Vector3.zero);
	private MapAttachements currentAttachement = null;
	public int PixelsPerUnit = 32;
	public MapNames mapNames;
    public PrefabNames prefabNames;
    public TextAsset story;
	public MapPath startingLocation = new MapPath("default", "default");
	public GameObject player;
	public FollowCamera followCamera;
	public List<Tiled2Unity.TiledMap> currentMaps = new List<Tiled2Unity.TiledMap>();
    public StoryFunctionBindings storyFunctionBindings;

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

	public void AttachTilemap(Tiled2Unity.TiledMap from, Tiled2Unity.TiledMap tilemap, Vector3 direction) {
	
	}

	public void Reset() {
		foreach (Tiled2Unity.TiledMap map in currentMaps) {
			Destroy(map.gameObject);
		}
		currentMaps = new List<Tiled2Unity.TiledMap>();
	}

    public Tiled2Unity.TiledMap GetCurrentMap()
    {
        if (currentMaps.Count > 0)
        {
            return currentMaps[0];
        }
        else
        {
            return null;
        }
    }

	public Tiled2Unity.TiledMap SpawnTilemap(Tiled2Unity.TiledMap from, Vector3 origin) {
		Tiled2Unity.TiledMap result = Instantiate(from);
		currentMaps.Add(result);
		result.transform.position = origin;
		result.transform.parent = transform;

		var min = LerpMap(result, Vector2.zero);
		var max = LerpMap(result, Vector2.one);

		worldBounds.min = Vector3.Min(min, max);
		worldBounds.max = Vector3.Max(min, max);

		if (followCamera != null) {
			followCamera.bounds = worldBounds;
		}

		currentAttachement = result.gameObject.GetComponent<MapAttachements>();

        Projectile.projectileParent = result.transform;

        return result;
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
					Reset();
					var map = mapNames.GetEntry(currentAttachement.attachments[i]);
					Vector2 mapSize = MapSize(map.tiled);
					SpawnTilemap(map.tiled,
						new Vector3(
							Mathf.Lerp(worldBounds.min.x, worldBounds.max.x, currentBoundsAnchor[i].x) + mapSize.x * newBoundAnchor[i].x,
							Mathf.Lerp(worldBounds.min.y, worldBounds.max.y, currentBoundsAnchor[i].y) + mapSize.y * newBoundAnchor[i].y,
							0.0f
						)
					);
					break;
				}
			}
		}
	}

    public void SwitchTo(MapDirections direction)
    {
        int directionAsInt = (int)direction;
        if (currentAttachement != null && directionAsInt < currentAttachement.attachments.Length && currentAttachement.attachments[directionAsInt] != null)
        {
            var map = mapNames.GetEntry(currentAttachement.attachments[directionAsInt]);
            Vector3 position = new Vector3(
                worldBounds.min.x,
                worldBounds.max.y,
                0.0f
            );
            Reset();
            SpawnTilemap(map.tiled, position);
        }
    }

	public void Goto(MapPath location) {
		Reset();
		var mapEntry = mapNames.GetEntry(location.mapName);
		var map = SpawnTilemap(mapEntry.tiled, Vector3.zero);

        if (player != null) {
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
