using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Tiled2Unity.CustomTiledImporter]
public class TiledObjectImporter : Tiled2Unity.ICustomTiledImporter {
	private static string[] sideAttributes = new string[]{
		"TopMap",
		"RightMap",
		"BottomMap",
		"LeftMap"
	};

	public void HandleCustomProperties(GameObject gameObject,
	                                   IDictionary<string, string> keyValuePairs)
	{
		if (keyValuePairs.ContainsKey("PortalTo")) {
			MapPortal portal = gameObject.AddComponent<MapPortal>();
			string[] parts = keyValuePairs["PortalTo"].Split(new char[]{':'}, 2);
			portal.target = new MapPath(parts[0], parts.Length > 1 ? parts[1] : "default");

			gameObject.GetComponent<Collider2D>().isTrigger = true;
		}
		
		if (keyValuePairs.ContainsKey("PortalFrom")) {
			StartingPoint portal = gameObject.AddComponent<StartingPoint>();
			portal.locationName = keyValuePairs["PortalFrom"];
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
	}
	
	public void CustomizePrefab(GameObject prefab)
	{

	}
}
