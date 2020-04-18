using System;
using System.Collections.Generic;
using System.Text;
using static Casino.Core.Defs;
using Casino.Core.Util;

namespace Casino.Core {
    public static class Game {
        private static Table table = null;
        private static byte roundNumber = 1;


        public static void NewGame(Player[] players) {
            Deck deck = new Deck();
            try {
                if (!IsValidGame(players)) {
                    throw new Exception(Errorstr.InvalidGameSetup());
                }
            } catch {
                throw;
            }
            table = new Table(deck, players[0], players[1]);
            PlayGame();

            return;
        }

        private static bool IsValidGame(Player[] players) {
            //add more in the future
            return (players.Length == 2);
        }
        private static void PlayGame() {
            Tuple<List<byte>, List<byte>> dealtCards = table.DealCards();
            table.p1.ReceiveCards(dealtCards.Item1);
            table.p2.ReceiveCards(dealtCards.Item2);
            //PrintGameStats();
            while (true) {
                Console.WriteLine("test: ");
                string test = Console.ReadLine();
                table.IsValidMove("throw " + test);
            }
            throw new NotImplementedException();
        }

        private static void PrintGameStats(bool detailed = false) {
            Console.WriteLine("Creating stats...");
            IEnumerable<Tuple<string, short, string, byte>> stats = new[] {
                Tuple.Create(table.p1.Name,table.p1.CountCardsInDeck,PrintCardsShorthand(table.p1.Hand),table.p1.Score),
                Tuple.Create(table.p2.Name,table.p2.CountCardsInDeck,PrintCardsShorthand(table.p2.Hand),table.p2.Score),
                Tuple.Create("Table",table.CountCardsInDeck,PrintCardsShorthand(table.CardsOnTable),(byte)0),
            };
            Console.WriteLine(stats.ToStringTable(new[] { "Field", "Deck #", "In hand/On table", "Score" },
                a => a.Item1, a => a.Item2, a => a.Item3, a => a.Item4));
            Console.WriteLine("Would you like more detailed stats? (y/n)");
            char ans = Console.ReadLine()?[0] ?? 'n';
            if(ans == 'y') {
                Console.WriteLine("Deck info:");
                Console.WriteLine("Player 1 deck:\n" + PrintCards(table.p1.LocalDeck.GetDeck()) + "\n");
                Console.WriteLine("Player 2 deck:\n" + PrintCards(table.p2.LocalDeck.GetDeck()) + "\n");
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
