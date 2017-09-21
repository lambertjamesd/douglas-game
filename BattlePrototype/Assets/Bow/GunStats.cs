using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum AmmoType
{
    Colt45,
    _44Magnum,
    ShotgunShell,
}

public class GunStats : ScriptableObject
{
    public string gunName;
    public float speed;
    public int capacity;
    public float shotDelay;
    public float reloadDelay;
    public float reloadBulletDuration;
    public ReloadAnimation reloadAnimation;
    public Sprite smallIcon;
    public Sprite fullSize;
    public Projectile round;
    public AmmoType type = AmmoType.Colt45;
    public int shellSplitCount = 1;
    public float spread;
    public int animationIndex;

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Gun")]
    static void CreateFont()
    {
        GunStats gunStats = ScriptableObject.CreateInstance<GunStats>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {

        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New Gun.asset");

        AssetDatabase.CreateAsset(gunStats, assetPathAndName);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = gunStats;
    }
#endif

    public int GetShotsLeft(VariableStore store)
    {
        return store.GetInt(gunName + "_shots");
    }

    public void SetShotsLeft(VariableStore store, int value)
    {
        store.SetInt(gunName + "_shots", value);
    }
}