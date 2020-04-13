using System;
using System.Collections.Generic;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core {
    public class Game {
        public Game(Player[] players, Deck deck) {
            try {
                if (players?[0].name != "" && players?[1].name != "" &&
                    players.Length == 2 && deck.CardDeck?.Length == DECK_SIZE) {
                    Console.WriteLine("Good to go! Welcome, " + players[0].name + " and " + players[1].name + "!");
                } else {
                    throw new Exception("Invalid params.");
                }
            } catch(Exception e) {
                Console.WriteLine(e.ToString());
            }
            return;
        }
    }
}
