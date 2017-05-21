using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextDialog : MonoBehaviour {
    public Text text;
    
    public string[] SplitSections(string input)
    {
        List<string> result = new List<string>();
        TextGenerator textGenerator = new TextGenerator();
        int i = 100;
        input = input.TrimEnd(null);
        while (input.Length > 0 && --i > 0)
        {
            textGenerator.Populate(input, text.GetGenerationSettings(text.rectTransform.rect.size));
            result.Add(input.Substring(0, textGenerator.characterCountVisible));
            input = input.Substring(textGenerator.characterCountVisible);
        }

        return result.ToArray();
    }
}
