using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Tiled2Unity.CustomTiledImporter]
public class TiledObjectImporter : Tiled2Unity.ICustomTiledImporter {
	public void HandleCustomProperties(GameObject gameObject,
	                                   IDictionary<string, string> keyValuePairs)
	{
		Debug.Log(gameObject);
		Debug.Log("Handle custom properties from Tiled map");
	}
	
	public void CustomizePrefab(GameObject prefab)
	{
		Debug.Log(prefab);
		Debug.Log("Customize prefab");
	}
}
