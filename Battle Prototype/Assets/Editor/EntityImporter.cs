using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class EntityImporter : AssetPostprocessor
{
     private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
     {
         foreach(string asset in importedAssets)
         {
             if (asset.Substring(asset.Length - 4, 4).ToLower() == ".npc")
             {
                 string text = System.IO.File.ReadAllText(asset);
                 Debug.Log(text);
             }
         }
     }
}
