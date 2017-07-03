using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Ink.Runtime;

public class TextDialog : MonoBehaviour {
    public Text text;
    public TextOption optionPrefab;
    private List<TextOption> existingOptions;
    private int selection = 0;
    
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
}
