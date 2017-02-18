using UnityEngine;
using System.Collections;

public class WorldController : MonoBehaviour {
	private Bounds worldBounds = new Bounds(Vector3.zero, Vector3.zero);
	public int PixelsPerUnit = 32;
	public MapNames mapNames;

	Vector3 LerpMap(Tiled2Unity.TiledMap from, Vector2 lerp) {
		Vector3 localPosition = new Vector3(
			from.NumTilesWide * from.TileWidth / PixelsPerUnit * lerp.x, 
			from.NumTilesHigh * from.TileHeight / PixelsPerUnit * lerp.y,
			0.0f
		);

		return from.transform.TransformPoint(localPosition);
	}

	public void AttachTilemap(Tiled2Unity.TiledMap from, Tiled2Unity.TiledMap tilemap, Vector3 direction) {

	}

	public void Reset() {
		worldBounds = new Bounds(Vector3.zero, Vector3.zero);
	}

	public void SpawnTilemap(Tiled2Unity.TiledMap from, Vector3 origin) {
		Tiled2Unity.TiledMap result = Instantiate(from);
		result.transform.position = origin;
		worldBounds.min = Vector3.Min(worldBounds.min, LerpMap(result, Vector2.zero));
		worldBounds.max = Vector3.Max(worldBounds.max, LerpMap(result, Vector2.one));
	}

	public void Start() {

	}
}
