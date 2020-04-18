using Casino.Core.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core {
    public class Table {
        private Deck _deck = null;
        private byte _turnNo = 1;
        private bool initialDeal = true;
        private Move currentMove = null;
        public Player p1 = null;
        public Player p2 = null;
        public Players TURN = Players.NONE;

        private List<byte> _CardsOnTable = new List<byte>();
        public Deck Deck { get { return _deck; } private set { _deck = value; } }
        public short CountCardsInDeck { get { return (short)_deck.CardCount; } }
        public List<byte> CardsOnTable { get { return _CardsOnTable; } private set { _CardsOnTable = value; } }
        public short CountCardsOnTable { get { return (short)_CardsOnTable.Count; } }
        public byte TurnNumber { get { return _turnNo; } private set { _turnNo = value; } }


        public Table(Deck deck, Player p1, Player p2) {
            if((deck.GetDeck().Count - INITIAL_CARDS_ON_TABLE) % CARDS_PER_PLAYER != 0) {
                throw new Exception(Errorstr.UnevenDeck());
            }
            if(deck.GetDeck().Count != DECK_SIZE) {
                throw new Exception(
                    Errorstr.WrongCardCount("The table", DECK_SIZE, deck.GetDeck().Count));
            }
            TURN = Players.ONE;
            Deck = deck;
        }

        /// <summary>
        /// Will put new cards on table, and return player cards. (!Meant solely for use in Game.cs)
        /// </summary>
        /// <returns>T1 of tuple will be array of cards to be dealt to Player 1, and T2 to player 2.</returns>
        public Tuple<List<byte>,List<byte>> DealCards() {
            if (initialDeal) {
                List<byte> cardsReceived = Deck.DrawCards(INITIAL_CARDS_ON_TABLE);
                _CardsOnTable.AddRange(cardsReceived);
                initialDeal = false;
            }
            List<byte> p1Cards = new List<byte>();
            List<byte> p2Cards = new List<byte>();
            for(int i = 0; i < CARDS_PER_PLAYER * 2; i++) {
                if (i % 2 == 0) {
                    p1Cards.Add(Deck.DrawCards(1)[0]);
                } else {
                    p2Cards.Add(Deck.DrawCards(1)[0]);
                }
            }
            Tuple<List<byte>, List<byte>> returningCards = new Tuple<List<byte>, List<byte>>(p1Cards, p2Cards);
            return returningCards;            
        }


        public Move IsValidMove(string moveCmd) {
            this.currentMove = new Move(moveCmd);
            Logic.IsValidMove(this);
            return null;
        }

        public Player GetActivePlayer() {
            if (TURN == Players.NONE) return null;
            return (TURN == Players.ONE) ? p1 : p2;
        }

        public Players FlipActivePlayer() {
            switch (TURN) {
                case Players.NONE: return Players.NONE;
                case Players.ONE: TURN = Players.TWO; return Players.TWO;
                default: TURN = Players.ONE; return Players.ONE;
            }

        }
        public void SetActivePlayer(Players number) {
            TURN = number;
        }



        /**********************************************************************************************/
        private static class Logic {

            private static Table table = null;
            /// <summary>
            /// Diagnoses whether a move cmd is valid. No need to pass in string, since reference to Table is required.
            /// </summary>
            /// <param name="reference">Reference to the Table class that calls it.</param>
            public static bool IsValidMove (Table origin) {
                if(origin is null)
                    throw new NullReferenceException(); //shouldn't even be theoretically possible
                table = origin;
                GetMoveAction(origin.currentMove);
                return true;
            }

            private static bool GetMoveAction(Move attemptedMove) {
                string moveCmd = attemptedMove.MoveCmd;
                if (string.IsNullOrEmpty(moveCmd)) {
                    throw new UnparseableMoveException("The attempted move command cannot be null or empty.", moveCmd);
                }
                string[] cmdArgs = moveCmd.Split(' ');
                switch (cmdArgs[0]) {
                    case "t":case "throw": case "throwaway":
                        Throwaway(cmdArgs);
                        break;
                    case "p": case "pickup":
                        Pickup(cmdArgs);
                        break;
                    case "b": case "build":
                        Build(cmdArgs);
                        break;
                    case "c": case "capture":
                        Capture(cmdArgs);
                        break;
                    default:
                    throw new UnparseableMoveException("The attempted move uses an unidentified keyword", moveCmd);
                }
                return true;
                //throw new NotImplementedException();

            }

            private static void Throwaway(string[] cmdArgs) {
                if (cmdArgs.Length != 2) {
                    throw new UnparseableMoveException("Two arguments expected. First argument throwaway, second argument " +
                        "the card that is being thrown away.", string.Join(' ', cmdArgs));
                }
                string throwawayCard = cmdArgs[1];
                if (ExtractBuildNamesFromString(ref throwawayCard)?.Count() != 0) {
                    throw new UnparseableMoveException("Build names cannot be used when throwing away cards.",
                        string.Join(' ', cmdArgs));
                }
                byte card = 0;
                try {
                    var res = ExtractCardsFromString(throwawayCard);
                    if(res.Count != 1) { //expects one argument
                        throw new UnparseableMoveException();
                    } else {
                        card = res[0];
                    }
                } catch (Exception ex) {
                    if (ex is ArgumentOutOfRangeException || ex is ArgumentNullException || ex is UnparseableMoveException){
                        throw new UnparseableMoveException(Errorstr.NoMove(), string.Join(' ', cmdArgs));
                    } else {
                        throw ex;
                    }
                }

                //ensure player has a card matching this
                List<byte> playerHand = table.GetActivePlayer().Hand;
                                
            }

            private static void Pickup(string[] cmdArgs) {
                throw new NotImplementedException();
            }

            private static void Build(string[] cmdArgs) {
                throw new NotImplementedException();
            }
            private static void Capture(string[] cmdArgs) {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Assumes all BuildNames are extracted beforehand (example: alpha, beta). Run 
            /// ExtractBuildNamesFromString() if there is uncertainty.
            /// </summary>
            private static List<byte> ExtractCardsFromString(string cmd) {
                cmd.Replace("10", "T");
                string remain = cmd;
                char currChar = '\0';
                string currCard = "";
                List<byte> result = new List<byte>();
                while (remain.Length != 0) {
                    currChar = remain[0];
                    currCard += currChar;
                    if (charcardValAbbr.Contains(char.ToUpper(currChar))) {
                        remain = remain.Substring(1);
                        if (remain.Length != 0) {
                            currChar = remain[0];
                            if (cardSuitAbbrAscii.Contains(char.ToLower(currChar))) {
                                remain = remain.Substring(1);
                                currCard += currChar;
                            }
                        }
                        result.Add(ExtractCardFromString(currCard));
                    } else {
                        throw new UnparseableMoveException(Errorstr.NoMove(), cmd);
                    }
                }
                return result;
            }

            private static byte ExtractCardFromString(string cmd) {
                cmd = cmd.Replace("10", "T");
                cmd = cmd.ToLower();
                if (cmd.Length < 1 || cmd.Length > 2) throw new UnparseableMoveException(Errorstr.CardFormat(),cmd);

                CardVals value = CardVals.NONE;
                CardSuits suit = CardSuits.NONE;
                char cmdValue = cmd[0];
                char cmdSuit = '\0';
                if (cmd.Length == 2) cmdSuit = cmd[1];
                foreach(CardVals val in Enum.GetValues(typeof(CardVals))) {
                    char valabbr = char.ToLower(GetCardValAbbr((byte)val, true)[0]);
                    if(valabbr == cmdValue) {
                        value = val; break;
                    }
                }
                if(value == CardVals.NONE) throw new UnparseableMoveException(Errorstr.CardFormat(), cmd);

                if(cmdSuit != '\0') {
                    cmdSuit = char.ToLower(cmdSuit);
                    foreach (CardSuits suitf in Enum.GetValues(typeof(CardSuits))) {
                        char suitabbr = GetCardSuitAbbr((byte)suitf, true);
                        if (suitabbr == cmdSuit) {
                            suit = suitf; break;
                        }
                    }
                }

                byte result = GetCardDigit(value, suit);
                /*Console.WriteLine("binary: " + Convert.ToString(result,2)); string test2 = "";
                try { test2 = PrintCard(result); } catch { } Console.WriteLine("full rep: " + test2);*/
                return result;
            }

            /// <summary>
            /// Returns a list of all BuildNames contained in a command. Pass the command in as a reference, and
            /// it will remove all instances of those build names, preparing for ExtractCardsFromString().
            /// </summary>
            private static List<BuildNames> ExtractBuildNamesFromString(ref string cmd) {
                if (string.IsNullOrEmpty(cmd)) return null;
                List<BuildNames> res = new List<BuildNames>();
                foreach (BuildNames build in Enum.GetValues(typeof(BuildNames))) {
                    if (cmd.Contains(build.ToString())) {
                        res.Add(build);
                        cmd.Replace(build.ToString(), "");
                    }
                }
                return res;
            }

            /// <summary>
            /// Override of ExtractBuildNamesFromString(), where the string needs not be passed in as a reference.
            /// </summary>
            private static List<BuildNames> ExtractBuildNamesFromString(string cmd) {
                if (string.IsNullOrEmpty(cmd)) return null;
                List<BuildNames> res = new List<BuildNames>();
                foreach (BuildNames build in Enum.GetValues(typeof(BuildNames))) {
                    if (cmd.Contains(build.ToString())) {
                        res.Add(build);
                        cmd.Replace(build.ToString(), "");
                    }
                }
                return res;
            }
        }

    }
}
