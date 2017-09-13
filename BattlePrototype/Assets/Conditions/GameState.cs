using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;

public class GameState {
    private Story story;

    public GameState(Story story)
    {
        this.story = story;
    }

    public void setBoolean(string name, bool boolValue)
    {
    }

    public bool getBoolean(string name)
    {
        object result = story.variablesState[name];
        if (result != null)
        {
            return (int)result != 0;
        }
        else
        {
            return false;
        }
    }

    public void setNumber(string name, double numberValue)
    {
    }

    public double getNumber(string name)
    {
        object result = story.variablesState[name];
        if (result != null)
        {
            return (double)result;
        }
        else
        {
            return 0.0;
        }
    }

    public void setString(string name, string stringValue)
    {
    }

    public string getString(string name)
    {
        object result = story.variablesState[name];
        if (result != null)
        {
            return result.ToString();
        }
        else
        {
            return "";
        }
    }
}