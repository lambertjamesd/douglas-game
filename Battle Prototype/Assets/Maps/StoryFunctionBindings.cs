using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class StoryFunctionBindings : MonoBehaviour
{
    public WorldController world;
    public TextDialog textPrefab;
    private TextDialog prefabInstance;

    public void BindToStory(Story story)
    {
        story.BindExternalFunction<string, float, float>("createObject", (objectName, x, y) =>
        {
            PrefabEntry entry = world.prefabNames.GetEntry(objectName);

            if (entry != null)
            {
                GameObject.Instantiate(entry.prefab, new Vector3(x, y, 0.0f), Quaternion.identity, world.GetCurrentMap().transform);
                return true;
            }
            else
            {
                Debug.LogError("Could not find prefab named " + objectName);
                return false;
            }
        });
        
        story.BindExternalFunction<float>("setTimeScale", (value) =>
        {
            Time.timeScale = value;
            return true;
        });

        story.BindExternalFunction<bool>("setTextBoxVisible", (value) =>
        {
            if (prefabInstance != null)
            {
                prefabInstance.gameObject.SetActive(value);
            }
            return true;
        });

        story.BindExternalFunction("getPlayerX", () =>
        {
            return GameObject.FindWithTag("Player").transform.position.x;
        });

        story.BindExternalFunction("getPlayerY", () =>
        {
            return GameObject.FindWithTag("Player").transform.position.y;
        });
    }

    public static StoryFunctionBindings GetBindings()
    {
        return GameObject.FindWithTag("GameController").GetComponent<StoryFunctionBindings>();
    }

    public IEnumerator interact(string storyEntryPoint)
    {
        Time.timeScale = 0.0f;

        prefabInstance = Instantiate<TextDialog>(textPrefab);

        Story story = StoryManager.GetSingleton().GetStory();

        story.ChoosePathString(storyEntryPoint, new string[] { });

        do
        {
            string storyText = story.Continue();
            List<Choice> choices = story.currentChoices;
            string[] parts = prefabInstance.SplitSections(storyText, choices.Count);

            prefabInstance.Reset();

            foreach (string part in parts)
            {
                prefabInstance.text.text = part;

                if (choices.Count > 0 && part != parts[parts.Length - 1] || choices.Count == 0)
                {
                    while (!Input.GetButtonDown("Submit"))
                    {
                        yield return null;
                    }

                    while (!Input.GetButtonUp("Submit"))
                    {
                        yield return null;
                    }
                }
            }

            if (choices.Count > 0)
            {
                prefabInstance.ShowOptions(choices);

                int currentSelection = 0;

                while (!Input.GetButtonDown("Submit"))
                {
                    if (Input.GetButtonDown("Next") && currentSelection < choices.Count - 1)
                    {
                        currentSelection++;
                    }

                    if (Input.GetButtonDown("Previous") && currentSelection > 0)
                    {
                        currentSelection--;
                    }

                    prefabInstance.Select(currentSelection);


                    yield return null;
                }

                if (currentSelection < choices.Count)
                {
                    story.ChooseChoiceIndex(choices[currentSelection].index);
                    if (story.canContinue)
                    {
                        story.Continue();
                    }
                }

                while (!Input.GetButtonUp("Submit"))
                {
                    yield return null;
                }
            }
        }
        while (story.canContinue);

        Destroy(prefabInstance.gameObject);

        Time.timeScale = 1.0f;
    }
}
