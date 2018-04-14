using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum Suite
{
    Blossoms,
    Stars,
    Hearts,
    Skulls,
    Count,
}

public enum CardType
{
    _1,
    _2,
    _3,
    _4,
    _5,
    _6,
    _7,
    _8,
    _9,
    _10,
    Count,
}

public class CardEquality : IEqualityComparer<Card>
{
    public int GetHashCode(Card card)
    {
        return card.suite.GetHashCode() ^ card.type.GetHashCode();
    }

    public bool Equals(Card a, Card b)
    {
        return a.suite == b.suite && a.type == b.type;
    }
}

public class Card
{
    public Sprite sprite;
    public Suite suite;
    public CardType type;

    public static string ToString(Card card)
    {
        if (card == null)
        {
            return null;
        }
        else
        {
            return card.suite + " " + card.type;
        }
    }

    public Card(Sprite sprite, Suite suite, CardType type)
    {
        this.sprite = sprite;
        this.suite = suite;
        this.type = type;
    }

    public string ShortName()
    {
        return type.ToString().Substring(1) + suite.ToString().Substring(0, 1);
    }

    public int PointValue()
    {
        return (int)type + 1;
    }

    public static CardType PointsToType(int points)
    {
        if (points >= 1 && points <= 10)
        {
            return (CardType)(points - 1);
        }
        else
        {
            throw new System.Exception("No such card");
        }
    }

    public static int MIN_CARD_SCORE = 1;
    public static int MAX_CARD_SCORE = 10;
}

public class DeckSkin : ScriptableObject {
    public Texture2D image;
    public Vector2 size;
    public Sprite back;

    public Sprite[] CreateSprites()
    {
        Sprite[] cards = new Sprite[(int)Suite.Count * (int)CardType.Count];

        int index = 0;
        for (int suite = 0; suite < (int)Suite.Count; ++suite)
        {
            for (int type = 0; type < (int)CardType.Count; ++type)
            {
                cards[index] = Sprite.Create(image, new Rect(new Vector2(type * size.x, suite * size.y), size), new Vector2(0.5f, 0.5f));
                ++index;
            }
        }

        return cards;
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Deck")]
    static void CreateDeck()
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
        Sprite[] cardSprites = skin == null ? null : skin.CreateSprites();

        for (int suite = 0; suite < (int)Suite.Count; ++suite)
        {
            for (int type = 0; type < (int)CardType.Count; ++type)
            {
                cards.Add(new Card(skin != null ? cardSprites[type + suite * (int)CardType.Count] : null, (Suite)suite, (CardType)type));
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

    public void Remove(IEnumerable<Card> toRemove)
    {
        cards.RemoveAll(card => toRemove.Contains(card, new CardEquality()));
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