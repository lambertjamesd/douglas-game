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
    public Button stopPlaying;

    public Text playerMoney;
    public Text oponentMoney;
    public Text potLabel;

    public Text positivePrefab;
    public Text negativePrefab;

    public int buyInPrice = 10;

    public CardGameVariables defaultPlayer;
    public CardGameVariables[] cardPlayers;

    private int moneyInPot = 0;

    PlayerLogic BuildPlayerLogic(CardGameVariables variables)
    {
        switch (variables.aiType)
        {
            case CardAIType.NeverFold:
            default:
                return new DumbCardAIPlayer(1, oponentHand, oponentMoney);
        }
    }

    public CardGameVariables GetOponent(string name)
    {
        foreach (CardGameVariables variables in cardPlayers)
        {
            if (variables.playerName == name)
            {
                return variables;
            }
        }

        return defaultPlayer;
    }

    // Use this for initialization
    void Start ()
    {
        CardGameVariables oponent = GetOponent(CardGameInitializer.playerName);

        deck = new Deck(oponent.deckSkin ?? deckSkin);

        players = new PlayerLogic[]
        {
            new HumanPlayerLogic(0, playerHand, foldButton, guiTransform, spinner, cardSelection, playerMoney),
            BuildPlayerLogic(oponent)
        };

        var story = StoryManager.GetSingleton().GetStory();

        if (story != null)
        {
            players[0].AdjustMoney((int)(story.variablesState["player_money"] ?? 1000) - players[0].money);

            if (oponent.oponentMoneyStore != null)
            {
                players[1].AdjustMoney((int)story.variablesState[oponent.oponentMoneyStore] - players[1].money);
            }
        }

        foreach (PlayerLogic logic in players)
        {
            logic.hand.UseBack(oponent.deckSkin == null ? deckSkin.back : oponent.deckSkin.back);
        }

        StartHand();

    }

    public void EndGame()
    {
        CardGameVariables oponent = GetOponent(CardGameInitializer.playerName);
        var story = StoryManager.GetSingleton();
        story.SetInt("player_money", players[0].money);
        story.SetInt(oponent.oponentMoneyStore, players[1].money);
        WorldInitializer.LoadWorld(CardGameInitializer.returnPoint ?? new MapPath("default", "default"), CardGameInitializer.returnKnot);
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
        stopPlaying.gameObject.SetActive(false);

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

        int stillInCount = players.Length;

        for (int i = 0; i < players.Length; ++i)
        {
            stillIn[i] = true;
            players[i].hand.GiveHand(deck.Deal(5));
        }

        for (int round = 0; stillInCount > 1 && round < 3; ++round)
        {
            var playerHands = players.Select(player => player.hand.GetPlayedCards()).ToList();
            int currentBid = -1;

            List<TurnChoice> choices = new List<TurnChoice>();
            List<float> choiceInSeconds = new List<float>();

            for (int i = 0; i < players.Length; ++i)
            {
                int playerIndex = (currentTurn + i) % players.Length;

                if (stillIn[playerIndex])
                {
                    PlayerLogic player = players[playerIndex];

                    if (stillInCount == 1)
                    {
                        choices.Add(new TurnChoice(moneyInPot / 2, player.hand.GetHand().Where(card => card != null).Take(1).ToArray()[0]));
                        choiceInSeconds.Add(0.0f);
                    }
                    else
                    {
                        player.StartTurn(playerHands, currentBid, 0);

                        float startTime = Time.time;

                        yield return player.StartTurn(playerHands, currentBid, moneyInPot);

                        choiceInSeconds.Add(Time.time - startTime);

                        TurnChoice choice = player.TurnResult();

                        currentBid = System.Math.Max(choice.bid, currentBid);

                        if (choice.card == null)
                        {
                            stillIn[playerIndex] = false;
                        }
                        else
                        {
                            player.AdjustMoney(-choice.bid);
                            yield return AnimateMoney(-choice.bid, player.moneyLabel.transform.position, potLabel.transform.position);
                            moneyInPot += choice.bid;
                            potLabel.text = moneyInPot.ToString();
                        }

                        if (choice.IsFold())
                        {
                            --stillInCount;
                        }
                        choices.Add(choice);
                    }
                }
                else
                {
                    choices.Add(TurnChoice.Fold());
                    --stillInCount;
                    choiceInSeconds.Add(0.0f);
                }
            }

            for (int i = 0; i < players.Length; ++i)
            {
                int playerIndex = (currentTurn + i) % players.Length;
                PlayerLogic player = players[playerIndex];
                TurnChoice choice = choices[i];
                RoundLogger.LogRound(
                    round, 
                    playerIndex, 
                    choice.PlayedCards(), 
                    choice.bid, 
                    player.hand.UnplayedCards(), 
                    choiceInSeconds[i], 
                    player.money + choice.bid
                );

                if (!stillIn[playerIndex] && stillInCount == 1)
                {
                    break;
                }
                if (choices[i].card != null)
                {
                    if (choices[i].extraCard != null)
                    {
                        yield return player.hand.PutCardOnTable(choice.extraCard);
                    }
                    players[playerIndex].ExecuteTurn(choice);
                }
            }

            currentTurn = (currentTurn + 1) % players.Length;
        }

        var playersStillIn = players.Where(player => stillIn[player.Index]);

        foreach (PlayerLogic player in players)
        {
            player.hand.RevealHand();
        }
        
        int maxScore = playersStillIn.Aggregate(0, (sum, player) => System.Math.Max(sum, player.hand.GetScore()));
        var winningPlayers = playersStillIn.Where(player => maxScore == player.hand.GetScore());
        int winningCount = winningPlayers.Count();
        foreach (PlayerLogic player in winningPlayers)
        {
            int moneyEarned = moneyInPot / winningCount + (player == players[currentTurn] ? (moneyInPot % winningCount) : 0);
            moneyInPot -= moneyEarned;
            potLabel.text = moneyInPot.ToString();
            yield return AnimateMoney(moneyEarned, potLabel.transform.position, player.moneyLabel.transform.position);
            player.AdjustMoney(moneyEarned);
        }

        playAgain.gameObject.SetActive(true);
        stopPlaying.gameObject.SetActive(true);
    }
}
