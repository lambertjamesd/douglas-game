using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class StoryManager {
    private Story currentStory;
    private TextAsset currentAsset;

    public void SetStory(TextAsset withAsset)
    {
        if (currentAsset != withAsset)
        {
            currentStory = new Story(withAsset.text);
            currentAsset = withAsset;
        }
    }

    public Story GetStory()
    {
        return currentStory;
    }

    private static StoryManager singleton = null;

    public static StoryManager GetSingleton()
    {
        if (singleton == null)
        {
            singleton = new StoryManager();
        }

        return singleton;
    }
}
