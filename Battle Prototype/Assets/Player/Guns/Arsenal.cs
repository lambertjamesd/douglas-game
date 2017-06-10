using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Arsenal : ScriptableObject
{
    public GunStats[] guns;

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Arsenal")]
    static void CreateFont()
    {
        Arsenal arsenal = ScriptableObject.CreateInstance<Arsenal>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {

        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New Arsenal.asset");

        AssetDatabase.CreateAsset(arsenal, assetPathAndName);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = arsenal;
    }
#endif
}
