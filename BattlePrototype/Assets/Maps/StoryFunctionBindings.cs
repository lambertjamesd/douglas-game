using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class StoryFunctionBindings : MonoBehaviour
{
    public WorldController world;
    public TextDialog textPrefab;
    private TextDialog prefabInstance;
    public StoreGUI storePrefab;
    private StoreGUI storePrefabInstance;
    private Dictionary<string, GameObject> namedObjects = new Dictionary<string, GameObject>();
    private float delayTime = 0.0f;
    private string timeoutKnot = null;
    private NumberSpinnerParams? spinnerParams;

    private GameObject GetNamedObject(string name)
    {
        if (namedObjects.ContainsKey(name))
        {
            return namedObjects[name];
        }
        else
        {
            return GameObject.FindWithTag(name);
        }
    }

    public static StoryFunctionBindings GetBindings()
    {
        return GameObject.FindWithTag("GameController").GetComponent<StoryFunctionBindings>();
    }

    public void SetGUIVisible(string name, bool value)
    {
        GameObject gui = GameObject.FindWithTag("PlayerGUI");

        if (gui != null)
        {
            PlayerHUD hud = gui.GetComponent<PlayerHUD>();

            if (hud != null)
            {
                hud.SetGUIVisible(name, value);
            }
        }
    }

    public bool CreateObject(string objectName, float x, float y)
    {
        var splitName = objectName.Split(new char[] { ':' });

        PrefabEntry entry = world.prefabNames.GetEntry(splitName[0]);

        if (entry != null)
        {
            GameObject result = GameObject.Instantiate(entry.prefab, new Vector3(x, y, 0.0f), Quaternion.identity, world.GetCurrentMap().transform);
            namedObjects[splitName.Length > 1 ? splitName[1] : splitName[0]] = result;

            return true;
        }
        else
        {
            Debug.LogError("Could not find prefab named " + objectName);
            return false;
        }
    }

    public void LookAt(string objectName, float x, float y)
    {
        var toLook = GetNamedObject(objectName);

        if (toLook != null)
        {
            DefaultMovement movement = toLook.GetComponent<DefaultMovement>();

            movement.SetDirection(new Vector3(x, y, 0.0f) - toLook.transform.position);
        }
    }

    public void UseUnscaledTime(string objectName, bool value)
    {
        var toSet = GetNamedObject(objectName);

        Debug.Log(objectName + ", " + value + ", " + (toSet == null ? "null" : toSet.ToString()));

        if (toSet != null)
        {
            AnimationController controller = toSet.GetComponent<AnimationController>();
            if (controller != null)
            {
                controller.SetUseUnscaledTime(value);
            }
        }
    }

    public void SetTimeout(float value, string knot)
    {
        delayTime += value;
        timeoutKnot = knot;
    }

    public void SetTextBoxVisible(bool value)
    {
        if (prefabInstance != null)
        {
            prefabInstance.gameObject.SetActive(value);
        }
    }

    public void ShowStore(string name)
    {
        if (storePrefabInstance != null)
        {
            storePrefabInstance.ExitStore();
        }

        storePrefabInstance = Instantiate(storePrefab);
        storePrefabInstance.UseStore(name);
    }

    public void UseSpinner(int digits, string name)
    {
        if (textPrefab != null)
        {
            spinnerParams = new NumberSpinnerParams(name, digits);
        }
    }

    public IEnumerator interact(string storyEntryPoint)
    {
        TimePause.ScaleTime(0.0f);
        Time.timeScale = 0.0f;

        prefabInstance = Instantiate<TextDialog>(textPrefab);

        Story story = StoryManager.GetSingleton().GetStory();

        story.ChoosePathString(storyEntryPoint, new string[] { });

        do
        {
            string storyText = story.Continue();
            List<Choice> choices = story.currentChoices;
            string[] parts = prefabInstance.SplitSections(storyText, choices.Count + (spinnerParams != null ? 1 : 0));

            if (delayTime > 0.0f && timeoutKnot != null)
            {
                yield return new WaitForSecondsRealtime(delayTime);
                delayTime = 0.0f;
            }

            prefabInstance.Reset();

            if (storyText.Trim() == "[...]")
            {
                continue;
            }

            foreach (string part in parts)
            {
                prefabInstance.text.text = part;

                if (part == parts[parts.Length - 1] && spinnerParams != null)
                {
                    prefabInstance.ShowNumberSpinner(spinnerParams.GetValueOrDefault());
                    spinnerParams = null;
                }

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

            prefabInstance.ResetSpinner();
        }
        while (story.canContinue);

        Destroy(prefabInstance.gameObject);

        TimePause.UnscaleTime(0.0f);
        Time.timeScale = 1.0f;

        if (timeoutKnot != null)
        {
            string toKnot = timeoutKnot;
            timeoutKnot = null;
            yield return interact(toKnot);
        }

        CardGameInitializer.Commit();
    }
}
