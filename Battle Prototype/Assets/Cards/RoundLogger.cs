using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class RoundLogger {
    public static void LogRound(int round, int player, IEnumerable<Card> playedCards, int bid, IEnumerable<Card> unusedCards, float timeSpent, int moneyHad)
    {
        using (var file = File.AppendText("round_history.csv"))
        {
            file.WriteLine(round + "," + player + "," + CardListToString(playedCards) + "," + bid + "," + CardListToString(unusedCards) + "," + timeSpent + "," + moneyHad);
        }
    }

    public static string CardListToString(IEnumerable<Card> playedCards)
    {
        return System.String.Join("+", playedCards.Select(card => card.ShortName()).ToArray());
    }
}
