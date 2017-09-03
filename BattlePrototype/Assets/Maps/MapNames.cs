using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class MapPath {
    public static MapPath WithString(string parts)
    {
        var partSplit = parts.Split(new char[] { ':' }, 2);
        return new MapPath(partSplit[0], partSplit.Length > 1 ? partSplit[1] : "default");
    }

    public MapPath(string mapName, string portalName) {
        this.mapName = mapName;
        this.portalName = portalName;
    }

	public string mapName;
	public string portalName;
}

[System.Serializable]
public class MapEntry {
    public MapEntry(string name, Tiled2Unity.TiledMap tiled)
    {
        this.name = name;
        this.tiled = tiled;
    }

	public string name;
	public Tiled2Unity.TiledMap tiled;
}

public class MapNames : ScriptableObject {
	public List<MapEntry> maps = new List<MapEntry>();
    private Dictionary<string, MapEntry> mapping = null;

    private void CheckInitialized() {
        if (mapping == null) {
            mapping = new Dictionary<string, MapEntry>();

            foreach (MapEntry entry in maps) {
                mapping[entry.name] = entry;
            }
        }
    }

    public MapEntry GetEntry(string name) {
        this.CheckInitialized();

        if (this.mapping.ContainsKey(name))
        {
            return this.mapping[name];
        }
        else
        {
            string message = "Entry named '" + name + "' does not exits";
            Debug.LogError(message, this);
            throw new System.Exception("message");
        }
    }

    public void AddEntry(MapEntry entry)
    {
        Debug.Log(entry.name, this);
        maps.RemoveAll((otherEntry) => otherEntry.name == entry.name);
        maps.Add(entry);
    }

#if UNITY_EDITOR
    [MenuItem ("Assets/Create/MapNames")]
    static void CreateFont()
    {
        MapNames mapNames = ScriptableObject.CreateInstance<MapNames>();
        
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }
        
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New MapNames.asset");
        
        AssetDatabase.CreateAsset(mapNames, assetPathAndName);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = mapNames;
    }
#endif

}
