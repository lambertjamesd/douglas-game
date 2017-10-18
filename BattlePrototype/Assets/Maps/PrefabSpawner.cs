using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrefabSpawner : MonoBehaviour {
    public string condition = "";
    public GameObject toSpawn;
	public IDictionary<string, string> properties = new Dictionary<string, string>();
	public Vector3 size = Vector3.zero;

    public void Start() {
        if (condition == "" || ConditionParser.parseCondition(condition).evaluate(new GameState(StoryManager.GetSingleton().GetStory()))) {
			GameObject instance = Instantiate(toSpawn, transform.position + transform.TransformDirection(size), transform.rotation) as GameObject;
            instance.transform.parent = transform.parent;

			if (properties.Count > 0)
			{
				PrefabProperties prefabProperties = instance.GetComponent<PrefabProperties>();

				if (prefabProperties == null)
				{
					prefabProperties = instance.AddComponent<PrefabProperties>();
				}

				prefabProperties.properties = properties;
			}
        }

    }
}
