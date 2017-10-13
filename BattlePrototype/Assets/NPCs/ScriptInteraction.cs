using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class ScriptInteraction : MonoBehaviour {
    
    public string storyEntryPoint;

	public void Start()
	{
		PrefabProperties properties = GetComponent<PrefabProperties>();

		if (properties != null && properties.properties.ContainsKey("Knot"))
		{
			storyEntryPoint = properties.properties["Knot"];
		}
	}

    public virtual IEnumerator interact()
    {
        if (storyEntryPoint != null)
        {
            var other = StoryFunctionBindings.GetBindings().interact(storyEntryPoint);

            while (other.MoveNext())
            {
                yield return other.Current;
            }
        }
        else
        {
            yield return null;
        }
    }
}
