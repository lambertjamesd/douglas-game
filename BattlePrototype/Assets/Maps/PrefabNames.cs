using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class PrefabEntry {
	public string name;
	public GameObject prefab;
}

public class PrefabNames : ScriptableObject {
	public PrefabEntry[] maps;
    private Dictionary<string, PrefabEntry> mapping = null;

    private void CheckInitialized() {
        if (mapping == null) {
            mapping = new Dictionary<string, PrefabEntry>();

            foreach (PrefabEntry entry in maps) {
                mapping[entry.name] = entry;
            }
        }
    }

    public PrefabEntry GetEntry(string name) {
        this.CheckInitialized();
        if (!this.mapping.ContainsKey(name))
        {
            Debug.LogError("Cannot find name " + name);
        }
        return this.mapping[name];
    }

#if UNITY_EDITOR
    [MenuItem ("Assets/Create/PrefabNames")]
    static void CreateFont()
    {
        PrefabNames mapNames = ScriptableObject.CreateInstance<PrefabNames>();
        
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }
        
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New PrefabNames.asset");
        
        AssetDatabase.CreateAsset(mapNames, assetPathAndName);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = mapNames;
    }
#endif

}
