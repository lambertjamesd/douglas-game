using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldInitializer {
    public static MapPath location;
    public static string storyKnot;

    public static void LoadWorld(MapPath location, string storyKnot)
    {
        WorldInitializer.location = location;
        WorldInitializer.storyKnot = storyKnot;
        UnityEngine.SceneManagement.SceneManager.LoadScene((int)GameScenes.WorldSpawner);
    }
}
