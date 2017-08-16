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
    public Button bidButton;
    public NumberSpinner spinner;
    public List<Button> cardSelection;
    public Button playAgain;

    public Text playerMoney;
    public Text oponentMoney;

    private int moneyInPot = 0;

    // Use this for initialization
    void Start () {
        deck = new Deck(deckSkin);

        players = new PlayerLogic[]
        {
            new HumanPlayerLogic(0, playerHand, foldButton, bidButton, spinner, cardSelection, playerMoney),
            new DumbAIPlayer(1, oponentHand, oponentMoney)
        };

        foreach (PlayerLogic logic in players)
        {
            logic.hand.UseBack(deckSkin.back);
        }
	}

    public void StartHand()
    {
        StartCoroutine(PlayHand());
    }

    public IEnumerator PlayHand()
    {
        playAgain.gameObject.SetActive(false);

        for (int i = 0; i < players.Length; ++i)
        {
            deck.Discard(players[i].hand.TakeCards());
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
            int currentBid = 0;

            List<TurnChoice> choices = new List<TurnChoice>();

            for (int i = 0; i < players.Length; ++i)
            {
                if (stillIn[i])
                {
                    int playerIndex = (currentTurn + i) % players.Length;

                    PlayerLogic player = players[playerIndex];
                    player.StartTurn(playerHands, currentBid, 0);

                    while (player.TakingTurn())
                    {
                        yield return null;
                    }

                    TurnChoice choice = player.TakeTurn();

                    currentBid = System.Math.Max(choice.bid, currentBid);

                    if (choice.card == null)
                    {
                        stillIn[i] = false;
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
                if (choices[i].card != null)
                {
                    moneyInPot += choices[i].bid;
                    players[playerIndex].ExecuteTurn(choices[i]);
                    ++stillInCount;
                }
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
                player.AdjustMoney(moneyInPot / winningPlayerCount + (player == players[currentTurn] ? (moneyInPot % winningPlayerCount) : 0));
            }
        }

        moneyInPot = 0;

        playAgain.gameObject.SetActive(true);
    }
}
