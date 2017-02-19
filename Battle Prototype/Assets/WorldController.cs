using UnityEngine;
using System.Collections;

public class WorldController : MonoBehaviour {
	private Bounds worldBounds = new Bounds(Vector3.zero, Vector3.zero);
	public int PixelsPerUnit = 32;
	public MapNames mapNames;
	public MapPath startingLocation = new MapPath("default", "default");
	public GameObject player;
	public FollowCamera followCamera;

	Vector3 LerpMap(Tiled2Unity.TiledMap from, Vector2 lerp) {
		Vector3 localPosition = new Vector3(
			from.NumTilesWide * from.TileWidth / PixelsPerUnit * lerp.x, 
			-from.NumTilesHigh * from.TileHeight / PixelsPerUnit * lerp.y,
			0.0f
		);

		return from.transform.TransformPoint(localPosition);
	}

	public void AttachTilemap(Tiled2Unity.TiledMap from, Tiled2Unity.TiledMap tilemap, Vector3 direction) {

	}

	public void Reset() {
		worldBounds = new Bounds(Vector3.zero, Vector3.zero);
	}

	public Tiled2Unity.TiledMap SpawnTilemap(Tiled2Unity.TiledMap from, Vector3 origin) {
		Tiled2Unity.TiledMap result = Instantiate(from);
		result.transform.position = origin;

		var min = LerpMap(result, Vector2.zero);
		var max = LerpMap(result, Vector2.one);

		worldBounds.min = Vector3.Min(worldBounds.min, Vector3.Min(min, max));
		worldBounds.max = Vector3.Max(worldBounds.max, Vector3.Max(min, max));

		if (followCamera != null) {
			followCamera.bounds = worldBounds;
		}

		return result;
	}

	public void Start() {
		if (followCamera != null) {
			followCamera.target = player.transform;
		}

		if (startingLocation != null) {
			Goto(startingLocation);
		}
	}

	public void Goto(MapPath location) {
		var mapEntry = mapNames.GetEntry(location.mapName);
		var map = SpawnTilemap(mapEntry.tiled, Vector3.zero);

		if (player != null) {
			var staringPoints = GetComponentsInChildren<StartingPoint>();

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
