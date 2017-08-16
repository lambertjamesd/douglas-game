using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum Suite
{
    Clubs,
    Spades,
    Hearts,
    Diamonds,
    Count,
}

public enum CardType
{
    _A,
    _2,
    _3,
    _4,
    _5,
    _6,
    _7,
    _8,
    _9,
    _10,
    _J,
    _Q,
    _K,
    Count,
}

public class Card
{
    public Sprite sprite;
    public Suite suite;
    public CardType type;

    public Card(Sprite sprite, Suite suite, CardType type)
    {
        this.sprite = sprite;
        this.suite = suite;
        this.type = type;
    }

    public int PointValue()
    {
        if (type == CardType._A)
        {
            return 14;
        }
        else
        {
            return (int)type + 1;
        }
    }
}

public class DeckSkin : ScriptableObject {
    public Sprite[] cards;
    public Sprite back;

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Deck")]
    static void CreateFont()
    {
        DeckSkin deck = ScriptableObject.CreateInstance<DeckSkin>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {

        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New Deck.asset");

        AssetDatabase.CreateAsset(deck, assetPathAndName);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = deck;
    }
#endif
}

public class Deck
{
    public List<Card> cards = new List<Card>();
    public List<Card> discard = new List<Card>();

    public Deck(DeckSkin skin)
    {
        for (int suite = 0; suite < (int)Suite.Count; ++suite)
        {
            for (int type = 0; type < (int)CardType.Count; ++type)
            {
                cards.Add(new Card(skin.cards[type + suite * (int)CardType.Count], (Suite)suite, (CardType)type));
            }
        }
    }

    public void Shuffle()
    {
        List<Card> result = new List<Card>();

        cards.AddRange(discard);
        discard.Clear();

        while (cards.Count > 0)
        {
            int index = Random.Range(0, cards.Count);
            result.Add(cards[index]);
            cards.RemoveAt(index);
        }

        cards = result;
    }

    public List<Card> Deal(int count)
    {
        var result = cards.GetRange(0, count);
        cards.RemoveRange(0, count);
        return result;
    }

    public void Discard(IEnumerable<Card> cards)
    {
        discard.AddRange(cards);
    }
}