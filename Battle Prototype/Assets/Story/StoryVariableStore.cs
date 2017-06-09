﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class StoryVariableStore : VariableStore {
    public string prefix = "";

    public override int GetInt(string name)
    {
        object result = StoryManager.GetSingleton().GetStory().variablesState[prefix + name];
        if (result == null)
        {
            return 0;
        }

        return (int)result;
    }

    public override void SetInt(string name, int value)
    {
        StoryManager.GetSingleton().GetStory().variablesState[prefix + name] = value;
    }
}
