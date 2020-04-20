using Casino.Core.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core {
    public class Table {

        private Deck _deck = null;
        private List<Build> buildsOnTable = null;

        private byte _turnNo = 1;
        private bool initialDeal = true;
        private Move currentMove = null;
        private string moveCmd = "";
        public Player p1 = null;
        public Player p2 = null;
        public Players TURN = Players.NONE;

        private List<byte> _CardsOnTable = new List<byte>();
        public Deck Deck { get { return _deck; } private set { _deck = value; } }
        public List<Build> Builds { get { return buildsOnTable; } private set { buildsOnTable = value; } }
        public short CountCardsInDeck { get { return (short)_deck.CardCount; } }
        public List<byte> CardsOnTable { get { return _CardsOnTable; } private set { _CardsOnTable = value; } }
        public short CountCardsOnTable { get { return (short)_CardsOnTable.Count; } }
        public byte TurnNumber { get { return _turnNo; } private set { _turnNo = value; } }


        public Table(Deck deck, Player playerOne, Player playerTwo) {
            if (playerOne == null || p2 == playerTwo) throw new NullReferenceException();
            p1 = playerOne;
            p2 = playerTwo;
            if((deck.GetDeck().Count - INITIAL_CARDS_ON_TABLE) % CARDS_PER_PLAYER != 0) {
                throw new Exception(Errorstr.UnevenDeck());
            }
            if(deck.GetDeck().Count != DECK_SIZE) {
                throw new Exception(
                    Errorstr.WrongCardCount("The table", DECK_SIZE, deck.GetDeck().Count));
            }
            TURN = Players.One;
            Deck = deck;
        }

        public void MakeMove(Move move) {
            if (move == null) throw new NullReferenceException();
            currentMove = move;
            switch (move.MoveType) {
                case MoveTypes.Throwaway:
                    GetActivePlayer().RemoveCard(move.CardPlayed);
                    CardsOnTable.Add(move.CardPlayed);
                    break;
                case MoveTypes.Pickup: break;
                case MoveTypes.Build: break;
                case MoveTypes.Capture: break;
                default: throw new Exception(Errorstr.NoMove());
            }
            FlipActivePlayer();
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


        public Move GetMove(string moveCommand) {
            this.moveCmd = moveCommand;
            return Logic.GetMove(this);
        }

        public Player GetActivePlayer() {
            if (TURN == Players.NONE) return null;
            return (TURN == Players.One) ? p1 : p2;
        }

        public Players FlipActivePlayer() {
            switch (TURN) {
                case Players.NONE: return Players.NONE;
                case Players.One: TURN = Players.Two; return Players.Two;
                default: TURN = Players.One; return Players.One;
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
            public static Move GetMove (Table origin) {
                if(origin is null)
                    throw new NullReferenceException(); //shouldn't even be theoretically possible
                table = origin;
                return GetMoveAction(origin.moveCmd);
            }

            private static Move GetMoveAction(string attemptedMove) {
                string moveCmd = attemptedMove;
                if (string.IsNullOrEmpty(moveCmd)) {
                    throw new UnparseableMoveException("The attempted move command cannot be null or empty.", moveCmd);
                }
                string[] cmdArgs = moveCmd.Split(' ');
                switch (cmdArgs[0]) {
                    case "t":case "throw": case "throwaway":
                        return Throwaway(cmdArgs);
                    case "p": case "pickup":
                        return Pickup(cmdArgs);
                    case "b": case "build":
                        return Build(cmdArgs);
                    case "c": case "capture":
                        return Capture(cmdArgs);
                    default:
                    throw new UnparseableMoveException("The attempted move uses an unidentified keyword", moveCmd);
                }
            }

            private static Move Throwaway(string[] cmdArgs) {
                byte card = 0;
                if (cmdArgs.Length != 2) throw new UnparseableMoveException("Two arguments expected. First argument throwaway, " +
                    "second argument the card that is being thrown away.", string.Join(' ', cmdArgs));
                string throwawayCard = cmdArgs[1];
                if (ExtractBuildNamesFromString(ref throwawayCard)?.Count() != 0) {
                    throw new UnparseableMoveException("Build names cannot be used when throwing away cards.",
                        string.Join(' ', cmdArgs));
                }
                try {
                    var res = ExtractCardsFromString(throwawayCard);
                    if(res.Count != 1) throw new UnparseableMoveException(); //expects one argument
                    card = res[0];
                } catch (Exception ex) {
                    if (ex is ArgumentOutOfRangeException || ex is ArgumentNullException || ex is UnparseableMoveException){
                        throw new UnparseableMoveException(Errorstr.NoMove(), string.Join(' ', cmdArgs));
                    } else {
                        throw ex;
                    }
                }
                try {
                    if (!CardHasASuit(card)) {
                        card = GetCardByValue(card, table.GetActivePlayer().Hand);
                    } else {
                        if (!table.GetActivePlayer().HasCardInHand(card)) {
                            throw new CardNotPresentException();
                        }
                    }
                }catch(CardNotPresentException cnp) {
                    //necessary to throw new exception since location was unknown to GetCardByValue
                    throw new CardNotPresentException("Card is not present in player's hand.", card, 
                        (table.GetActivePlayer().PlayerNo == Players.One ) ? CardLocations.PlayerOneHand : CardLocations.PlayerTwoHand);
                }catch(AmbiguousCardException ac) {
                    //TODO: find out why this throws the caught exception instead of the new one
                    throw new AmbiguousCardException("Card is not present in player's hand.", ac, card,
                        (table.GetActivePlayer().PlayerNo == Players.One) ? CardLocations.PlayerOneHand : CardLocations.PlayerTwoHand);
                } catch {
                    throw;
                }

                return new Move(MoveTypes.Throwaway, card);
            }

            private static Move Pickup(string[] cmdArgs) {
                if(cmdArgs.Length < 2 || cmdArgs.Length > 3) {
                    throw new UnparseableMoveException("Two or three arguments expected. First argument pickup, " +
                    "second argument card on table, third argument card in hand.", string.Join(' ', cmdArgs));
                }
                if(cmdArgs.Length == 2) {
                    cmdArgs = new string[3] { cmdArgs[0], cmdArgs[1], cmdArgs[1] };
                }
                throw new NotImplementedException();
            }

            private static Move Build(string[] cmdArgs) {
                throw new NotImplementedException();
            }
            private static Move Capture(string[] cmdArgs) {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Assumes all BuildNames are extracted beforehand (example: alpha, beta). Run 
            /// ExtractBuildNamesFromString() if there is uncertainty.
            /// </summary>
            private static List<byte> ExtractCardsFromString(string cmd) {
                cmd = cmd.Replace("10", "T");
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
            private static bool PlayerHasCard(byte card) {
                if (!IsACard(card)) {
                    throw new UnparseableCardException("Player tried to play card that does not exist",card);
                }
                if (!table.GetActivePlayer().HasCardInHand(card)) {
                    return false;
                }
                return true;
            }

            /// <summary>
            /// Returns card in deck that matches an ambiguous card where suit is missing.
            /// </summary>
            /// <exception cref="CardNotPresentException">No cards that match this value.</exception>
            /// <exception cref="AmbiguousCardException">Several cards match this value.</exception>
            public static byte GetCardByValue(byte cardWithNoSuit, List<byte> deckToCheckAgainst) {
                byte card = 0;
                CardVals ambiCardVal = Defs.GetCardValue(cardWithNoSuit);
                foreach(byte deckCard in deckToCheckAgainst) {
                    if(Defs.GetCardValue(deckCard) == ambiCardVal) {
                        if(card == 0) {
                            card = deckCard;
                        } else {
                            throw new AmbiguousCardException("Card suit must be specified, as the card referral is ambiguous.", cardWithNoSuit);
                        }
                    }
                }
                if (card == 0) throw new CardNotPresentException("Card was specified by value, but no such value exists in this grouping of cards.", cardWithNoSuit);
                return card;
            }
        }
    }
}
