using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryLoader : MonoBehaviour {

    public TextAsset story;

	void Start () {
        StoryManager.GetSingleton().SetStory(story);
	}
}
