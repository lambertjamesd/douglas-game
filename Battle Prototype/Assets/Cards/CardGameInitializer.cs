using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardAIType
{
    NeverFold,
}

[System.Serializable]
public struct CardGameVariables
{
    public string playerName;
    public CardAIType aiType;
    public string oponentMoneyStore;
    public DeckSkin deckSkin;
}

public class CardGameInitializer
{
    public static string playerName;
    public static MapPath returnPoint;
    public static string returnKnot;
    private static bool pendingGame = false;

    public static void PlayCards(string playerName, string returnPoint, string returnKnot)
    {
        CardGameInitializer.playerName = playerName;
        CardGameInitializer.returnPoint = MapPath.WithString(returnPoint);
        CardGameInitializer.returnKnot = returnKnot;
        Time.timeScale = 1.0f;
        pendingGame = true;
    }

    public static void Commit()
    {
        if (pendingGame)
        {
            pendingGame = false;
            UnityEngine.SceneManagement.SceneManager.LoadScene((int)GameScenes.CardGame);
        }
    }
}
