using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class ScriptInteraction : MonoBehaviour {
    
    public TextAsset scriptSource;
    public TextDialog textPrefab;
    private TextDialog prefabInstance;
    private Story story;

	// Use this for initialization
	void Start () {
        if (scriptSource != null)
        {
            story = new Story(scriptSource.text);
        } 
	}

    public IEnumerator interact()
    {
        if (story != null)
        {
            prefabInstance = Instantiate<TextDialog>(textPrefab);

            if (!story.canContinue)
            {
                story.ChoosePathString("start", new string[] { });
            }

            while (story.canContinue)
            {
                string[] parts = prefabInstance.SplitSections(story.Continue());

                foreach (string part in parts)
                {
                    prefabInstance.text.text = part;

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

            Destroy(prefabInstance.gameObject);
        }
        else
        {
            yield return null;
        }
    }
}
