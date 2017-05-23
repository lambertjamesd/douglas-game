using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class ScriptInteraction : MonoBehaviour {
    
    public string storyEntryPoint;
    public TextDialog textPrefab;
    private TextDialog prefabInstance;

    public IEnumerator interact()
    {
        if (storyEntryPoint != null)
        {
            prefabInstance = Instantiate<TextDialog>(textPrefab);

            Story story = StoryManager.GetSingleton().GetStory();

            story.ChoosePathString(storyEntryPoint, new string[]{});

            do
            {
                string storyText = story.Continue();
                List<Choice> choices = story.currentChoices;
                string[] parts = prefabInstance.SplitSections(storyText, choices.Count);

                Debug.Log(storyText);

                prefabInstance.Reset();

                foreach (string part in parts)
                {
                    prefabInstance.text.text = part;

                    if (choices.Count > 0 && part != parts[parts.Length -1])
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
        }
        else
        {
            yield return null;
        }
    }
}
