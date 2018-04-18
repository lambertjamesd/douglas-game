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

    public Text debugLog;

    public int buyInPrice = 10;

    public CardGameVariables defaultPlayer;
    public CardGameVariables[] cardPlayers;

    public TextAsset defaultAIData;

    public static int MAX_BID_SCALAR = 3;

    private Dictionary<string, shootout.AllTurnProbabilities> loadedAI = new Dictionary<string, shootout.AllTurnProbabilities>();

    private int moneyInPot = 0;

    static float CARD_ANIMATION_TIME = 0.5f;

    public void DebugLog(string message)
    {
        //if (debugLog.text != null)
        //{
        //    debugLog.text = debugLog.text + "\n" + message;
        //}
        //else
        //{
        //    debugLog.text = message;
        //}
    }

    PlayerLogic BuildPlayerLogic(PlayerHand hand, Text money, CardGameVariables variables, string savedName, TextAsset fallback)
    {
        switch (variables.aiType)
        {
            case CardAIType.NeverFold:
            default:
                shootout.AllTurnProbabilities turnProbs;

                if (loadedAI.ContainsKey(savedName))
                {
                    turnProbs = loadedAI[savedName];
                }
                else
                {
                    turnProbs = new shootout.AllTurnProbabilities();
                    loadedAI[savedName] = turnProbs;
                    string filename = Path.Combine(Application.persistentDataPath, savedName + ".ai");

                    DebugLog("Loading AI from " + filename);


                    if (File.Exists(filename))
                    {
                        DebugLog("Loading from file");
                        using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open)))
                        {
                            turnProbs.Read(reader);
                        }
                    }
                    else
                    {
                        DebugLog("Loading for prefab");
                        using (BinaryReader reader = new BinaryReader(new MemoryStream(fallback.bytes)))
                        {
                            turnProbs.Read(reader);
                        }
                    }
                }

                DebugLog("Loaded");


                return new CalculatedAI(1, hand, money, shootout.CardGameAI.TurnProbability(turnProbs));
        }
    }

    public void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            SaveAI();
        }
    }

    public void OnDestroy()
    {
        SaveAI();
    }

    private void SaveAI()
    {
        DebugLog("Saving AI");
        foreach (string savedName in loadedAI.Keys)
        {
            string filename = Path.Combine(Application.persistentDataPath, savedName + ".ai");

            DebugLog(filename);

            using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.OpenOrCreate)))
            {
                loadedAI[savedName].Write(writer);
            }
        }
        DebugLog("AI Saved");
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
        CardGameVariables oponent = GetOponent(CardGameInitializer.playerName);

        deck = new Deck(oponent.deckSkin ?? deckSkin);

        players = new PlayerLogic[]
        {
            //BuildPlayerLogic(playerHand, playerMoney, oponent, "SmartAI", defaultAIData),
            new HumanPlayerLogic(0, playerHand, foldButton, guiTransform, bidPreview, multiplier, multiplierShow, multiplierImages, cardSelection, playerMoney),
            BuildPlayerLogic(oponentHand, oponentMoney, oponent, "SmartAI", defaultAIData),
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
        yield return TweenHelper.LerpPosition(from, to, CARD_ANIMATION_TIME, (pos) => text.transform.position = pos);
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

        List<shootout.TurnResult> allRoundChoices = new List<shootout.TurnResult>();

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
                        choices.Add(new shootout.TurnResult(moneyInPot / 2, 1, player.hand.GetHand().Where(card => card != null).Take(1).ToArray()[0]));
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

            allRoundChoices.AddRange(choices);

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
            PlayerHand thisHand = player.hand;
            PlayerHand theirHand = players[1 - player.Index].hand;

            player.Learn(allRoundChoices, startingTurn == player.Index, thisHand.GetMaxScore(), theirHand.GetMaxScore(), thisHand.ShowedDouble(), theirHand.ShowedDouble());
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

        StartHand();
    }
}
