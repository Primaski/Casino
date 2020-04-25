using System;
using System.Collections.Generic;
using System.Text;
using static Casino.Core.Defs;
using Casino.Core.Util;
using Casino.Core.Error;
using System.Linq;

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
            do {
                Console.WriteLine("Press any key to start the round.");
                Console.ReadKey();
                while (table.NewDealIsPossible()) {
                    NewDeal();
                }
                table.ScoreRound(roundNumber);
                Console.WriteLine(table.PrintRoundStats(roundNumber));
                table.NewRound();
            } while (!GameIsOver());
            Console.WriteLine("The game is now over.");

        }

        private static void NewDeal() {
            table.DealCards();
            while(!(table.p1.HasEmptyHand() && table.p2.HasEmptyHand())) {
                AskPlayerForInput();
            }
        }

        private static void AskPlayerForInput() {
            if (DEBUG_MODE) PrintGameStats();
            Console.WriteLine("It's your turn, " + table.GetActivePlayer().Name + "!");
            Move move = null;
            string cmd = Console.ReadLine();
            move = GetPlayerMove(cmd);
            if (move != null) table.MakeMove(move);
        }

        private static bool GameIsOver() {
            return !table.NewDealIsPossible() && table.RoundHasBeenScored && (table.PlayerHasWon() != Players.NONE);
        }

        private static bool IsValidGame(Player[] players) {
            //add more in the future
            return (players.Length == 2);
        }


        private static Move GetPlayerMove(string cmd) {
            Move move = null;
            try {
                if(DEBUG_MODE && cmd == "stats") {
                    PrintGameStats(); return null;
                } 
                move = table.GetMove(cmd);
            } catch (CardNotPresentException cnp) {
                if (DEBUG_MODE) Console.WriteLine(cnp.ToString());
                switch (cnp.Location) {
                    case CardLocations.PlayerOneHand:
                    case CardLocations.PlayerTwoHand:
                    Console.WriteLine("Sorry, you don't have a " + PrintCard(cnp.Card) + "! Please try again.");
                    return null;
                    case CardLocations.Table:
                    if (cnp.Card == 0 || !IsACard(cnp.Card)) {
                        Console.WriteLine("Sorry, one of the cards you specified on the table does not exist. " +
                            "Remember to call Builds by their Build Name! Please try again.");
                        return null;
                    }
                    Console.WriteLine("Sorry, there's no " + PrintCard(cnp.Card) + " on the table! Please try again.");
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
                    ((ac.Location == CardLocations.Table) ? "on the table" : "in your hand") + " is ambiguous. Try again, this time, specify the suit!");
                return null;
            } catch (IllegalPickupException ip) {
                if (DEBUG_MODE) Console.WriteLine(ip.ToString());
                if (!CardHasAValue(ip.PickupCard) || ip.BuildValue == 0) {
                    Console.WriteLine("There is no way to pick up that build value with the card you played! Please try again.");
                    return null;
                }
                Console.WriteLine("There is no way to pick up a build with a value of " + ip.BuildValue + " with your " + GetCardValue(ip.PickupCard)
                    + ". Please try again!");
                return null;
            } catch (InvalidBuildException ib) {
                if (DEBUG_MODE) Console.WriteLine(ib.ToString());
                if(!(ib.Build?.Any()) ?? false || ib.BuildValue == 0) {
                    Console.WriteLine("It is impossible to obtain that build! Please try again.");
                    return null;
                }
                Console.WriteLine("It is impossible to build up to " + ib.BuildValue + " using " + PrintCards(ib.Build) + ". Please try again!");
            } catch (Exception e) {
                Console.WriteLine("Something unexpected went wrong. Here's what we know:\n\n" + e);
                return null;
            }

            return move;
        }

        private static void PrintGameStats() {
            Console.WriteLine("Creating stats...");
            IEnumerable<Tuple<string, short, string, byte>> stats = new[] {
                Tuple.Create(table.p1.Name,table.p1.CountCardsInDeck,PrintCardsShorthand(table.p1.Hand),table.p1.Score),
                Tuple.Create(table.p2.Name,table.p2.CountCardsInDeck,PrintCardsShorthand(table.p2.Hand),table.p2.Score),
                Tuple.Create("Table",table.CountCardsInDeck,PrintCardsShorthand(table.CardsOnTable),(byte)0),
            };
            Console.WriteLine(stats.ToStringTable(new[] { "Field", "Deck #", "In hand/On table", "Score" },
                a => a.Item1, a => a.Item2, a => a.Item3, a => a.Item4));
            Console.WriteLine("Would you like more detailed stats? (y/n)");
            char ans = Console.ReadKey().KeyChar;
            Console.WriteLine();
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
