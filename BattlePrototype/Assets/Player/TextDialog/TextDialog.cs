using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Ink.Runtime;

public struct NumberSpinnerParams
{
    public string variableName;
    public int digitCount;

    public NumberSpinnerParams(string variableName, int digitCount)
    {
        this.variableName = variableName;
        this.digitCount = digitCount;
    }
}

public class TextDialog : MonoBehaviour {
    public Text text;
    public TextOption optionPrefab;
    public KeyboardNumberSpinner spinnerPrefab;
    public RectTransform spinnerPosition;
    private List<TextOption> existingOptions;
    private int selection = 0;
    private KeyboardNumberSpinner spinnerInstance;
    private NumberSpinnerParams? spinnerParams = null;

    public void Start()
    {

    }

    public void Reset()
    {
        if (existingOptions != null)
        {
            foreach (TextOption option in existingOptions)
            {
                Destroy(option.gameObject);
            }
        }
        existingOptions = null;
        selection = 0;
    }

    public string[] SplitSections(string input, int optionCount)
    {
        input = input.TrimEnd(null);

        TextGenerator textGenerator = new TextGenerator();
        
        string lastLine = null;

        if (optionCount > 0)
        {
            string reverseString = new string(input.ToCharArray().Reverse().ToArray());
            Vector2 smallSize = text.rectTransform.rect.size - new Vector2(0.0f, optionPrefab.text.rectTransform.rect.size.y * optionCount);
            textGenerator.Populate(reverseString, text.GetGenerationSettings(smallSize));

            lastLine = input.Substring(input.Length - textGenerator.characterCountVisible);
            input = input.Substring(0, input.Length - textGenerator.characterCountVisible);
        }


        List<string> result = new List<string>();
        int i = 100;
        while (input.Length > 0 && --i > 0)
        {
            textGenerator.Populate(input, text.GetGenerationSettings(text.rectTransform.rect.size));
            result.Add(input.Substring(0, textGenerator.characterCountVisible));
            input = input.Substring(textGenerator.characterCountVisible);
        }

        if (lastLine != null)
        {
            result.Add(lastLine);
        }

        return result.ToArray();
    }

    public void ShowNumberSpinner(NumberSpinnerParams spinParams)
    {
        spinnerParams = spinParams;
        spinnerInstance = Instantiate(spinnerPrefab);
        spinnerInstance.rectTransform.SetParent(spinnerPosition.parent, false);
        spinnerInstance.rectTransform.position = spinnerPosition.position;
        spinnerInstance.SetDigitCount(spinnerParams.GetValueOrDefault().digitCount);
        spinnerInstance.SetValue((int)StoryManager.GetSingleton().GetStory().variablesState[spinnerParams.GetValueOrDefault().variableName]);
    }

    public void ShowOptions(List<Choice> choices)
    {
        int index = 0;
        existingOptions = new List<TextOption>();
        foreach (Choice choice in choices)
        {
            TextOption option = Instantiate<TextOption>(optionPrefab);

            float offset = (choices.Count - index) * option.text.rectTransform.rect.size.y;

            option.text.rectTransform.parent = text.rectTransform.parent;
            option.text.rectTransform.position = new Vector2(text.rectTransform.position.x, text.rectTransform.rect.yMax + offset);

            option.UseChoice(choice);
            option.SetSelected(index == 0);
            existingOptions.Add(option);

            ++index;
        }
    }

    public void Select(int index)
    {
        if (existingOptions != null)
        {
            if (selection < existingOptions.Count)
            {
                existingOptions[selection].SetSelected(false);
            }

            if (index < existingOptions.Count)
            {
                existingOptions[index].SetSelected(true);
            }

            selection = index;
        }
    }

    public bool HasSpinner()
    {
        return spinnerParams != null && spinnerInstance != null;
    }

    public void ResetSpinner()
    {
        if (HasSpinner())
        {
            StoryManager.GetSingleton().GetStory().variablesState[spinnerParams.GetValueOrDefault().variableName] = spinnerInstance.GetValue();
            Destroy(spinnerInstance.gameObject);
            spinnerParams = null;
        }
    }
}
