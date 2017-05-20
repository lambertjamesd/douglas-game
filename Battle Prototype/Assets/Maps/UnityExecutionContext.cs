using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityExecutionContext : MonoBehaviour, ExecutionContext
{
    private GameState gameState = new GameState();
    public CustomFontRenderer fontRenderer;
    public Transform textLocation;

    public IEnumerator emitText(string text)
    {
        while (!Input.GetButtonDown("Submit"))
        {
            fontRenderer.DrawMultilineText(textLocation.position, 10.0f, text);
            yield return null;
        }
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
