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

    public int GetInt(string name)
    {
        var result = currentStory.variablesState[name];

        if (result != null)
        {
            return (int)result;
        }
        else
        {
            return 0;
        }
    }

    public void SetInt(string name, int value)
    {
        currentStory.variablesState[name] = value;
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

        currentStory.BindExternalFunction<string, bool>("useUnscaledTime", (objectName, value) =>
        {
            currentBindings.UseUnscaledTime(objectName, value);
            return true;
        });

        currentStory.BindExternalFunction<float>("setTimeScale", (value) =>
        {
            TimePause.SetUniformScale(value);
            return true;
        });

        currentStory.BindExternalFunction<bool>("setTextBoxVisible", (value) =>
        {
            currentBindings.SetTextBoxVisible(value);
            return true;
        });

        currentStory.BindExternalFunction("getPlayerX", () =>
        {
            GameObject gameObject = GameObject.FindWithTag("Player");
            if (gameObject != null)
            {
                return gameObject.transform.position.x;
            }
            else
            {
                return 0.0f;
            }
        });

        currentStory.BindExternalFunction("getPlayerY", () =>
        {
            GameObject gameObject = GameObject.FindWithTag("Player");
            if (gameObject != null)
            {
                return gameObject.transform.position.y;
            }
            else
            {
                return 0.0f;
            }
        });

        currentStory.BindExternalFunction<string>("getCharacterX", (character) =>
        {
            GameObject gameObject = GameObject.FindWithTag(character);
            if (gameObject != null)
            {
                return gameObject.transform.position.x;
            }
            else
            {
                return 0.0f;
            }
        });

        currentStory.BindExternalFunction<string>("getCharacterY", (character) =>
        {
            GameObject gameObject = GameObject.FindWithTag(character);
            if (gameObject != null)
            {
                return gameObject.transform.position.y;
            }
            else
            {
                return 0.0f;
            }
        });

        currentStory.BindExternalFunction<string, string, string>("playCards", (playerName, returnTo, returnKnot) =>
        {
            CardGameInitializer.PlayCards(playerName, returnTo, returnKnot);
            return true;
        });

        currentStory.BindExternalFunction<string>("showStore", (storeName) =>
        {
            currentBindings.ShowStore(storeName);
            return true;
        });

        currentStory.BindExternalFunction<int, string>("useSpinner", (digits, name) =>
        {
            currentBindings.UseSpinner(digits, name);
            return true;
        });
    }
}
