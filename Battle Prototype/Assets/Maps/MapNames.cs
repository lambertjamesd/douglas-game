using UnityEngine;
using System.Collections;

public class MapPath {
	public string mapName;
	public string portalName;
}

[System.Serializable]
public class MapEntry {
	public string name;
	public Tiled2Unity.TiledMap tiled;
}

[System.Serializable]
public class MapNames {
	public MapEntry[] maps;
}
