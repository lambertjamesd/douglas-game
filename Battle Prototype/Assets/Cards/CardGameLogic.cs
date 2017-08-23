using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CardGameLogic : MonoBehaviour {
    public DeckSkin deckSkin;
    private Deck deck;

    public PlayerHand playerHand;
    public PlayerHand oponentHand;

    public NumberSpinner bid;

    private int currentTurn = 0;
    private PlayerLogic[] players = null;
    private bool[] stillIn;

    public Button foldButton;
    public Transform guiTransform;
    public NumberSpinner spinner;
    public List<Button> cardSelection;
    public Button playAgain;

    public Text playerMoney;
    public Text oponentMoney;
    public Text potLabel;

    public Text positivePrefab;
    public Text negativePrefab;

    public int buyInPrice = 10;

    private int moneyInPot = 0;

    // Use this for initialization
    void Start () {
        deck = new Deck(deckSkin);

        players = new PlayerLogic[]
        {
            new HumanPlayerLogic(0, playerHand, foldButton, guiTransform, spinner, cardSelection, playerMoney),
            new FoldAI(1, oponentHand, oponentMoney)
        };

        foreach (PlayerLogic logic in players)
        {
            logic.hand.UseBack(deckSkin.back);
        }
	}

    public void CalculateScore(int sampleSize)
    {
        Deck deck = new Deck(deckSkin);

        int[] pointCount = new int[54];

        for (int i = 0; i < sampleSize; ++i)
        {
            deck.Shuffle();
            var hand = deck.Deal(5);
            pointCount[CardAIBase.IdealScore(hand, new Card[] { })]++;
            deck.Discard(hand);
        }

        System.IO.File.WriteAllLines("point_probability.csv", pointCount.Select(point => point + "," + ((float)point / sampleSize).ToString()).ToArray());
    }

    public void StartHand()
    {
        StartCoroutine(PlayHand());
    }

    public IEnumerator AnimateMoney(int amount, Vector3 from, Vector3 to)
    {
        Text text = Instantiate(amount >= 0 ? positivePrefab : negativePrefab, from, Quaternion.identity, transform);
        text.text = "$" + System.Math.Abs(amount);
        yield return TweenHelper.LerpPosition(from, to, 0.5f, (pos) => text.transform.position = pos);
        yield return new WaitForSeconds(0.5f);
        Destroy(text.gameObject);
    }

    public IEnumerator PlayHand()
    {
        playAgain.gameObject.SetActive(false);

        for (int i = 0; i < players.Length; ++i)
        {
            var player = players[i];
            deck.Discard(player.hand.TakeCards());

            player.AdjustMoney(-buyInPrice);
            yield return AnimateMoney(-buyInPrice, player.moneyLabel.transform.position, potLabel.transform.position);
            moneyInPot += buyInPrice;
            potLabel.text = moneyInPot.ToString();
        }

        deck.Shuffle();

        stillIn = new bool[players.Length];

        for (int i = 0; i < players.Length; ++i)
        {
            stillIn[i] = true;
            players[i].hand.GiveHand(deck.Deal(5));
        }

        for (int round = 0; round < 3; ++round)
        {
            var playerHands = players.Select(player => player.hand.GetPlayedCards()).ToList();
            int currentBid = -1;

            List<TurnChoice> choices = new List<TurnChoice>();
            List<float> choiceInSeconds = new List<float>();

            for (int i = 0; i < players.Length; ++i)
            {
                if (stillIn[i])
                {
                    int playerIndex = (currentTurn + i) % players.Length;

                    PlayerLogic player = players[playerIndex];
                    player.StartTurn(playerHands, currentBid, 0);

                    float startTime = Time.time;

                    yield return player.StartTurn(playerHands, currentBid, moneyInPot);

                    choiceInSeconds.Add(Time.time - startTime);

                    TurnChoice choice = player.TurnResult();

                    currentBid = System.Math.Max(choice.bid, currentBid);

                    if (choice.card == null)
                    {
                        stillIn[i] = false;
                    }
                    else
                    {
                        player.AdjustMoney(-choice.bid);
                        yield return AnimateMoney(-choice.bid, player.moneyLabel.transform.position, potLabel.transform.position);
                        moneyInPot += choice.bid;
                        potLabel.text = moneyInPot.ToString();
                    }

                    choices.Add(choice);
                }
                else
                {
                    choices.Add(TurnChoice.Fold());
                }
            }

            int stillInCount = 0;

            for (int i = 0; i < players.Length; ++i)
            {
                int playerIndex = (currentTurn + i) % players.Length;
                PlayerLogic player = players[playerIndex];
                TurnChoice choice = choices[i];
                if (choices[i].card != null)
                {
                    if (choices[i].extraCard != null)
                    {
                        yield return player.hand.PutCardOnTable(choice.extraCard);
                    }
                    players[playerIndex].ExecuteTurn(choice);
                    ++stillInCount;
                }
                RoundLogger.LogRound(round, playerIndex, choice.PlayedCards(), choice.bid, player.hand.UnplayedCards(), choiceInSeconds[i], player.money + choice.bid);
            }

            if (stillInCount <= 1)
            {
                break;
            }

            currentTurn = (currentTurn + 1) % players.Length;
        }

        int maxScore = players.Aggregate(0, (sum, player) => System.Math.Max(sum, player.hand.GetScore()));
        int winningPlayerCount = players.Aggregate(0, (sum, player) => maxScore == player.hand.GetScore() ? sum + 1 : sum);
        foreach (PlayerLogic player in players)
        {
            if (maxScore == player.hand.GetScore())
            {
                int moneyEarned = moneyInPot / winningPlayerCount + (player == players[currentTurn] ? (moneyInPot % winningPlayerCount) : 0);
                moneyInPot -= moneyEarned;
                potLabel.text = moneyInPot.ToString();
                yield return AnimateMoney(moneyEarned, potLabel.transform.position, player.moneyLabel.transform.position);
                player.AdjustMoney(moneyEarned);
            }

            player.hand.RevealHand();
        }

        playAgain.gameObject.SetActive(true);
    }
}
