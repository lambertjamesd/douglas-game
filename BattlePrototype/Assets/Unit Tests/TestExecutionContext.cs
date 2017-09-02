using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestExecutionContext : ExecutionContext
{
    public GameState gameState;
    public List<string> outputText = new List<string>();

    public TestExecutionContext(GameState gameState)
    {
        this.gameState = gameState;
    }

    public IEnumerator emitText(string text)
    {
        outputText.Add(text);
        return null;
    }

    public GameState getGameState()
    {
        return gameState;
    }

    public void setBoolean(string name, bool value)
    {
        gameState.setBoolean(name, value);
    }

    public void setNumber(string name, double value)
    {
        gameState.setNumber(name, value);
    }

    public void setString(string name, string value)
    {
        gameState.setString(name, value);
    }
}
