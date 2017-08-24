using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class StoryManager {
    private Story currentStory;
    private TextAsset currentAsset;

    public StoryFunctionBindings currentBindings;

    public void SetStory(TextAsset withAsset)
    {
        if (currentAsset != withAsset)
        {
            currentStory = new Story(withAsset.text);
            currentAsset = withAsset;
            BindToStory();
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

    public void BindToStory()
    {
        currentStory.BindExternalFunction<string, float, float>("createObject", (objectName, x, y) =>
        {
            return currentBindings.CreateObject(objectName, x, y);
        });

        currentStory.BindExternalFunction<string, float, float>("lookAt", (objectName, x, y) =>
        {
            currentBindings.LookAt(objectName, x, y);
        });

        currentStory.BindExternalFunction<float, string>("setTimeout", (value, knot) =>
        {
            currentBindings.SetTimeout(value, knot);
            return true;
        });

        currentStory.BindExternalFunction<float>("setTimeScale", (value) =>
        {
            Time.timeScale = value;
            return true;
        });

        currentStory.BindExternalFunction<bool>("setTextBoxVisible", (value) =>
        {
            currentBindings.SetTextBoxVisible(value);
            return true;
        });

        currentStory.BindExternalFunction("getPlayerX", () =>
        {
            return GameObject.FindWithTag("Player").transform.position.x;
        });

        currentStory.BindExternalFunction("getPlayerY", () =>
        {
            return GameObject.FindWithTag("Player").transform.position.y;
        });

        currentStory.BindExternalFunction<string, string, string>("playCards", (playerName, returnTo, returnKnot) =>
        {
            CardGameInitializer.PlayCards(playerName, returnTo, returnKnot);
            return true;
        });
    }
}
