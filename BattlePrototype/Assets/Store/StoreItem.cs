using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class StoreItem
{
    public string name;
    public string displayName;
    public int cost;
    [TextArea(3, 5)]
    public string description;
    public Sprite visual;
    public string condition;
    public bool multiple;
    public int quantity;
    public string savedName;

    private Condition parsedCondition;

    public void Buy()
    {
        Ink.Runtime.Story story = StoryManager.GetSingleton().GetStory();

        story.variablesState["player_money"] = (int)story.variablesState["player_money"] - cost;
        story.variablesState[savedName] = (int)story.variablesState[savedName] + (multiple ? quantity : 1);
    }

    public bool isEnabled()
    {
        Ink.Runtime.Story story = StoryManager.GetSingleton().GetStory();

        var storyValue = story.variablesState[savedName];
        if (!multiple && storyValue != null && (int)storyValue > 0)
        {
            return false;
        }

        if (parsedCondition == null && condition != null && condition != "")
        {
            parsedCondition = ConditionParser.parseCondition(condition);
        }

        if (cost > (int)story.variablesState["player_money"])
        {
            return false;
        }

        if (parsedCondition == null)
        {
            return true;
        }
        else
        {
            return parsedCondition.evaluate(new GameState(story));
        }
    }
}

[System.Serializable]
public class StoreInventory : ScriptableObject
{
    public string name;
    public StoreItem[] inventory;

#if UNITY_EDITOR
    [MenuItem("Assets/Create/StoreInventory")]
    static void CreateFont()
    {
        StoreInventory gunStats = ScriptableObject.CreateInstance<StoreInventory>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {

        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New Store.asset");

        AssetDatabase.CreateAsset(gunStats, assetPathAndName);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = gunStats;
    }
#endif
}