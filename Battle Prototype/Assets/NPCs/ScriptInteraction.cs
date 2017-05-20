using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptInteraction : MonoBehaviour {

    public UnityExecutionContext executionContext;
    public TextAsset scriptSource;
    private GameScript script;

	// Use this for initialization
	void Start () {
        executionContext = executionContext ?? gameObject.GetComponentInParent<UnityExecutionContext>();

        if (scriptSource != null)
        {
            script = GameScriptParser.parse(scriptSource.text);
        } 
	}

    public IEnumerator interact()
    {
        if (script != null)
        {
            var result = script.run(executionContext);

            while (result != null && result.MoveNext())
            {
                yield return null;
            }
        }
        else
        {
            yield return null;
        }
    }
}
