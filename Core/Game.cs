using System;
using System.Collections.Generic;
using System.Text;
using static Casino.Core.Defs;
using Casino.Core.Util;

namespace Casino.Core {
    public static class Game {

        private static Player p1 = null;
        private static Player p2 = null;
        private static Table table = null;
        private static byte roundNumber = 1;


        public static void NewGame(Player[] players) {
            Deck deck = new Deck();
            table = new Table(deck);

            try {
                if (!IsValidGame(players)) {
                    throw new Exception(ErrorMessage.InvalidGameSetup());
                }
            } catch {
                throw;
            }

            p1 = players[0];
            p2 = players[1];

            PlayGame();

            return;
        }

        private static bool IsValidGame(Player[] players) {
            //add more in the future
            return (players.Length == 2);
        }
        private static void PlayGame() {
            Tuple<List<byte>, List<byte>> dealtCards = table.DealCards();
            p1.ReceiveCards(dealtCards.Item1);
            p2.ReceiveCards(dealtCards.Item2);

            PrintGameStats(true); //debug
            throw new NotImplementedException();
        }

        private static void PrintGameStats(bool detailed = false) {
            Console.WriteLine("Creating stats...");
            IEnumerable<Tuple<string, short, string, byte>> stats = new[] {
                Tuple.Create(p1.Name,p1.CountCardsInDeck,PrintCardsShorthand(p1.Hand),p1.Score),
                Tuple.Create(p2.Name,p2.CountCardsInDeck,PrintCardsShorthand(p2.Hand),p2.Score),
                Tuple.Create("Table",table.CountCardsInDeck,PrintCardsShorthand(table.CardsOnTable),(byte)0),
            };
            Console.WriteLine(stats.ToStringTable(new[] { "Field", "Deck #", "In hand/On table", "Score" },
                a => a.Item1, a => a.Item2, a => a.Item3, a => a.Item4));
            Console.WriteLine("Would you like more detailed stats? (y/n)");
            char ans = Console.ReadLine()?[0] ?? 'n';
            if(ans == 'y') {
                Console.WriteLine("Deck info:");
                Console.WriteLine("Player 1 deck:\n" + PrintCards(p1.LocalDeck.GetDeck()) + "\n");
                Console.WriteLine("Player 2 deck:\n" + PrintCards(p2.LocalDeck.GetDeck()) + "\n");
                Console.WriteLine("Table deck:\n" + PrintCards(table.Deck.GetDeck()) + "\n");
                Console.WriteLine("Table deck (ordered):");
                table.Deck.PrintOrderedDeck();
                Console.WriteLine("=========");
                Console.WriteLine("Turn number: " + table.TurnNumber + "\n" +
                    "Round number: " + roundNumber + "\n" +
                    "Decks used: " + NUMBER_OF_DECKS + "\n" +
                    "Cards per player / table: " + CARDS_PER_PLAYER + " / " + INITIAL_CARDS_ON_TABLE + "\n"
                );
                Console.WriteLine("Press any key to finish stats.");
                Console.ReadKey();
            }
            return;
        }
    }
}
