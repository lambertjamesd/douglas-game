using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;

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
    public RadioButtons multiplier;
    public Image multiplierShow;
    public Sprite[] multiplierImages;
    public Text bidPreview;
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

    public static int MAX_BID_SCALAR = 3;

    private int moneyInPot = 0;

    PlayerLogic BuildPlayerLogic(CardGameVariables variables)
    {
        switch (variables.aiType)
        {
            case CardAIType.NeverFold:
            default:
                shootout.AllTurnProbabilities turnProbs = new shootout.AllTurnProbabilities();

                using (BinaryReader reader = new BinaryReader(File.Open("AllTurnsProbability.dat", FileMode.Open)))
                {
                    turnProbs.Read(reader);
                }

                return new CalculatedAI(1, oponentHand, oponentMoney, shootout.CardGameAI.TurnProbability(turnProbs));
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
   
    
    void Start ()
    {
        System.IO.File.WriteAllText("OnePlayedMatch.csv", CardProbability.PointProbabilityTable(1, true, 64));
        //System.IO.File.WriteAllText("TwoPlayedMatch.csv", CardProbability.PointProbabilityTable(2, true, 2048 * 32));
        //System.IO.File.WriteAllText("TwoPlayedNoMatch.csv", CardProbability.PointProbabilityTable(2, false, 2048 * 32));
        Debug.Log("Write to " + System.IO.Directory.GetCurrentDirectory());

        CardGameVariables oponent = GetOponent(CardGameInitializer.playerName);

        deck = new Deck(oponent.deckSkin ?? deckSkin);

        players = new PlayerLogic[]
        {
            new HumanPlayerLogic(0, playerHand, foldButton, guiTransform, bidPreview, multiplier, multiplierShow, multiplierImages, cardSelection, playerMoney),
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

        int startingTurn = currentTurn;

        for (int i = 0; i < players.Length; ++i)
        {
            stillIn[i] = true;
            players[i].hand.GiveHand(deck.Deal(5));
        }

        for (int round = 0; stillInCount > 1 && round < 3; ++round)
        {
            var playerHands = players.Select(player => player.hand.GetPlayedCards()).ToList();
            int currentBid = -1;
            int currentBidScalar = 0;

            List<shootout.TurnResult> choices = new List<shootout.TurnResult>();
            List<float> choiceInSeconds = new List<float>();

            int minBid = moneyInPot / 2;

            for (int i = 0; i < players.Length; ++i)
            {
                int playerIndex = (currentTurn + i) % players.Length;

                if (stillIn[playerIndex])
                {
                    PlayerLogic player = players[playerIndex];

                    if (stillInCount == 1)
                    {
                        choices.Add(new shootout.TurnResult(moneyInPot / 2, player.hand.GetHand().Where(card => card != null).Take(1).ToArray()[0]));
                        choiceInSeconds.Add(0.0f);
                    }
                    else
                    {
                        float startTime = Time.time;
                        
                        yield return player.StartTurn(playerHands, currentBid, currentBidScalar, moneyInPot, round);

                        choiceInSeconds.Add(Time.time - startTime);

                        shootout.TurnResult choice = player.TurnResult();

                        Debug.Log("Turn " + playerIndex + " " + Card.ToString(choice.chosenCard) + " extra " + Card.ToString(choice.fourthCard));
                        
                        currentBid = System.Math.Max(choice.bid, currentBid);
                        currentBidScalar = Mathf.Max(1, currentBid / minBid);

                        multiplierShow.sprite = multiplierImages[currentBidScalar - 1];

                        if (choice.IsFold())
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
                    choices.Add(shootout.TurnResult.Fold());
                    --stillInCount;
                    choiceInSeconds.Add(0.0f);
                }
            }

            for (int i = 0; i < players.Length; ++i)
            {
                int playerIndex = (currentTurn + i) % players.Length;
                PlayerLogic player = players[playerIndex];
                shootout.TurnResult choice = choices[i];
                /*RoundLogger.LogRound(
                    round, 
                    playerIndex, 
                    choice.PlayedCards(), 
                    choice.bid, 
                    player.hand.UnplayedCards(), 
                    choiceInSeconds[i], 
                    player.money + choice.bid
                );*/

                if (!stillIn[playerIndex] && stillInCount == 1)
                {
                    break;
                }
                if (choices[i].chosenCard != null)
                {
                    if (choices[i].fourthCard != null)
                    {
                        yield return player.hand.PutCardOnTable(choice.fourthCard);
                    }
                    players[playerIndex].ExecuteTurn(choice);
                }
            }

            currentTurn = (currentTurn + 1) % players.Length;
        }

        currentTurn = (startingTurn + 1) % players.Length;

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
