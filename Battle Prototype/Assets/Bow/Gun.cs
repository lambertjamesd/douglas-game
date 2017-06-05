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
}

public class GunStats : ScriptableObject
{
    public string gunName;
    public float speed;
    public int capacity;
    public float reloadDelay;
    public float reloadBulletDuration;
    public ReloadAnimation reloadAnimation;
    public Projectile round;
    public AmmoType type = AmmoType.Colt45;


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
}

public class Gun : State
{
    public State nextState;
    public Bow bow;
    public GunStats gunStats;
    public int shotsLeft;

    public override IState UpdateState(float deltaTime)
    {
        if (shotsLeft > 0)
        {
            bow.LoadProjectile(gunStats.round);
            bow.Fire(gunStats.speed);
            --shotsLeft;
        }
        return nextState;
    }
}
