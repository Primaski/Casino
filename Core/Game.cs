using System;
using System.Collections.Generic;
using System.Text;
using static Casino.Core.Defs;
using Casino.Core.Util;
using Casino.Core.Error;

namespace Casino.Core {
    public static class Game {
        private static Table table = null;
        private static byte roundNumber = 1;


        public static void NewGame(string[] playerNames) {
            if (playerNames == null || playerNames.Length != 2) {
                Console.WriteLine("Invalid number of players.");
                return;
            }
            Deck deck = new Deck();
            Player p1 = new Player(playerNames[0], Players.One);
            Player p2 = new Player(playerNames[1], Players.Two);
            table = new Table(deck, p1, p2);
            while (table.Deck.CardCount != 0) {
                NewRound();
            }
            Console.WriteLine("Summing up points...");
            throw new NotImplementedException();
        }

        private static bool IsValidGame(Player[] players) {
            //add more in the future
            return (players.Length == 2);
        }
        private static void NewRound() {
            Tuple<List<byte>, List<byte>> dealtCards = table.DealCards();
            table.p1.ReceiveCards(dealtCards.Item1);
            table.p2.ReceiveCards(dealtCards.Item2);
            while (table.p1.CountCardsInHand != 0 || table.p2.CountCardsInHand != 0) {
                if(DEBUG_MODE) PrintGameStats();
                Console.WriteLine("It's your turn, " + table.GetActivePlayer().Name + "!");
                Move move = null;
                while (move == null) {
                    string cmd = Console.ReadLine();
                    move = GetPlayerMove(cmd);
                }
                if(DEBUG_MODE) Console.WriteLine("You will throw away your " + PrintCard(move.CardPlayed) + "!");
                table.MakeMove(move);
            }
        }

        private static Move GetPlayerMove(string cmd) {
            Move move = null;
            try {
                move = table.GetMove(cmd);
            } catch (CardNotPresentException cnp) {
                if (DEBUG_MODE) Console.WriteLine(cnp.ToString());
                switch (cnp.Location) {
                    case CardLocations.PlayerOneHand:
                    case CardLocations.PlayerTwoHand:
                    Console.WriteLine("Sorry, you don't have that card in your hand! Please try again.");
                    return null;
                    case CardLocations.Table:
                    if (cnp.Card == 0 || !IsACard(cnp.Card)) {
                        Console.WriteLine("Sorry, one of the cards you specified on the table does not exist. " +
                            "Remember to call Builds by their Build Name! Please try again.");
                        return null;
                    }
                    Console.WriteLine("Sorry, you don't have a " + PrintCard(cnp.Card) + "! Please try again.");
                    return null;
                    default:
                    Console.WriteLine("Sorry, you specified a card that doesn't exist in your hand or on the table. Please try again.");
                    return null;
                }
            } catch (UnparseableCardException uc) {
                if (DEBUG_MODE) Console.WriteLine(uc.ToString());
                if (uc.Card == 0 || !IsACard(uc.Card)) {
                    Console.WriteLine("Sorry, one of the cards provided was not readable. Please try again.");
                    return null;
                }
                Console.WriteLine("Sorry, that is not a valid card (card: " + uc.Card.ToString() + "! Please try again.");
                return null;
            } catch (UnparseableMoveException um) {
                if (DEBUG_MODE) Console.WriteLine(um.ToString());
                Console.WriteLine("Sorry, your move was not of the right format! Please try again!");
                return null;
            } catch (AmbiguousCardException ac) {
                if (DEBUG_MODE) Console.WriteLine(ac.ToString());
                if (!CardHasAValue(ac.Card) || ac.Location == CardLocations.UNKNOWN) {
                    Console.WriteLine("Sorry, one of the cards you specified could refer to multiple cards. Try again, this time, specify the suit!");
                    return null;
                }
                Console.WriteLine("Sorry, the " + GetCardValue(ac.Card) + " card " + 
                    ((ac.Location == CardLocations.Table) ? "on the table" : "in your hand" ) + " is ambiguous. Try again, this time, specify the suit!");
                return null;
            } catch (Exception e) {
                Console.WriteLine("Something unexpected went wrong. Here's what we know:\n\n" + e);
                return null;
            }

            return move;
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
