using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTests : MonoBehaviour {
    private bool passed = true;

    void Start () {
        TestScripts();
        if (passed)
        {
            Debug.Log("All tests passed");
        }
	}

    private void TestScript(GameScript script, GameState startingState, List<string> expectedText)
    {
        TestExecutionContext context = new TestExecutionContext(startingState);
        var result = script.run(context);
        while (result != null && result.MoveNext());

        if (context.outputText.Count != expectedText.Count)
        {
            Debug.LogError("Expected " + expectedText.Count + " text outputs, got " + context.outputText.Count);
            passed = false;
            return;
        }

        for (int i = 0; i < context.outputText.Count; ++i)
        {
            if (context.outputText[i] != expectedText[i])
            {
                Debug.LogError("Expected text at " + i + " to be " + expectedText[i] + ", got " + context.outputText[i]);
                passed = false;
                return;
            }
        }
    }

    void TestScripts() {
        GameScript test = GameScriptParser.parse(
@"Text line
(if boolean)
    True Condition
(else)
    False Condition
"
);
        TestScript(test, new GameState(new Ink.Runtime.Story("{}")), new List<string> { "Text line", "False Condition" });

        TestScript(test, new GameState(new Ink.Runtime.Story("{}")), new List<string> { "Text line", "True Condition" });
    }
}
