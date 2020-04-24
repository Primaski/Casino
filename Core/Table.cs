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

        public void GiveCardsToPlayers() {

        }

        public void MakeMove(Move move) {
            if (move == null) throw new NullReferenceException();
            currentMove = move;
            switch (move.MoveType) {
                case MoveTypes.Throwaway:
                    GetActivePlayer().RemoveCard(move.CardPlayed);
                    CardsOnTable.Add(move.CardPlayed);
                    break;
                case MoveTypes.Pickup:
                    GetActivePlayer().RemoveCard(move.CardPlayed);
                    GetActivePlayer().AddCardsToLocalDeck(move.CardsPickedUp.Concat(new List<byte> { move.CardPlayed }).ToList());
                    CardsOnTable.RemoveAll(x => move.CardsPickedUp.Contains(x));
                break;
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
        public void DealCards() {
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
            p1.ReceiveCards(p1Cards);
            p2.ReceiveCards(p2Cards);         
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

        /// <summary>
        /// Use this method at round end. Will assign point values and summary to players themselves.
        /// </summary>
        public void AssignPoints(byte currentRound) {
            List<ScoreableAttributes> p1Captures, p2Captures;
            p1Captures = new List<ScoreableAttributes>();
            p2Captures = new List<ScoreableAttributes>();
            SetActivePlayer(Players.One);

            /*STATIC VALUES*/
            List<ScoreableAttributes> staticScoreables = CardNumberByStaticAttribute.Select(x => x.Key).ToList();
            /*DYNAMIC VALUES*/
            int p1CardCount, p2CardCount, p1SpadeCount, p2SpadeCount;
            p1SpadeCount = p2SpadeCount = 0;
            p1CardCount = p1.LocalDeck.CardCount;
            p2CardCount = p2.LocalDeck.CardCount;

            /*EVALUATE CARDS*/
            do {
                Player player = GetActivePlayer();
                List<ScoreableAttributes> captures = new List<ScoreableAttributes>();
                CardVals value = CardVals.NONE;
                CardSuits suit = CardSuits.NONE;

                while(player.LocalDeck.CardCount > 0) {
                    byte card = player.LocalDeck.DrawCards(1)[0];
                    value = GetCardValue(card);
                    suit = GetCardSuit(card);

                    /*STATIC*/
                    staticScoreables.Where(attr => CardNumberByStaticAttribute[attr].Equals(card)).ToList()
                        .ForEach(attr => { captures.Add(attr); });
                    staticScoreables.RemoveAll(attr => captures.Contains(attr)); //efficiency reasons
                    /*DYNAMIC*/
                    if (TURN == Players.One) { p1SpadeCount += (suit == CardSuits.Spades) ? 1 : 0; }
                    if (TURN == Players.Two) { p2SpadeCount += (suit == CardSuits.Spades) ? 1 : 0; }
                }
                if (TURN == Players.One) { p1Captures.AddRange(captures); } else { p2Captures.AddRange(captures); }
                FlipActivePlayer();
            } while (TURN != Players.One);

            /* EVAL DYNAMICS */
            if(p1CardCount > p2CardCount) {
                p1Captures.Add(ScoreableAttributes.MostCards);
            }else if(p2CardCount > p1CardCount) {
                p2Captures.Add(ScoreableAttributes.MostCards);
            }

            if(p1SpadeCount > p2CardCount) {
                p1Captures.Add(ScoreableAttributes.MostSpades);
            }else if(p2SpadeCount > p1SpadeCount) {
                p2Captures.Add(ScoreableAttributes.MostSpades);
            }

            /* Conclusion - scores are evaluated by player */
            p1.AddNewScoreLogEntry(p1Captures);
            p2.AddNewScoreLogEntry(p2Captures);
        }
        



        //MEANT FOR DEBUGGING
        public void DealRandomHands(bool fixedAmounts = false) {
            _deck.ShuffleDeck();
            bool extra = (CountCardsInDeck % 2 == 1) && (fixedAmounts);
            short p1Deck = (fixedAmounts) ? (short)(CountCardsInDeck / 2) : (short)(Util.Misc.randomNumber.Next(0, CountCardsInDeck));
            short p2Deck = (fixedAmounts) ? (short)(CountCardsInDeck / 2) : (short)(CountCardsInDeck - p1Deck);
            p1.AddCardsToLocalDeck(_deck.DrawCards(p1Deck));
            p2.AddCardsToLocalDeck(_deck.DrawCards(p2Deck));
            if (extra) p1.AddCardsToLocalDeck(_deck.DrawCards(1));
        }

        /**********************************************************************************************/
        /**********************************************************************************************/
        /**********************************************************************************************/

        private static class Logic {

            private static Table table = null;

            /*********************************************************************/
            /**************************** CORE METHODS ***************************/
            /*********************************************************************/

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
                if (ExtractBuildNamesFromString(ref throwawayCard)?.Count() != 0)
                    throw new UnparseableMoveException("Build names cannot be used when throwing away cards.",
                        string.Join(' ', cmdArgs));
                try {
                    card = ExtractCardsFromString(throwawayCard)[0];
                } catch (Exception ex) {
                    if (ex is ArgumentOutOfRangeException || ex is ArgumentNullException || ex is UnparseableMoveException){
                        throw new UnparseableMoveException(Errorstr.NoMove(), string.Join(' ', cmdArgs));
                    } else {
                        throw ex;
                    }
                }
                
                card = GetCorrespondingCards(new List<byte> { card }, table.GetActivePlayer().Hand,
                    (table.GetActivePlayer().PlayerNo == Players.One) ? CardLocations.PlayerOneHand : CardLocations.PlayerTwoHand)[0];

                return new Move(MoveTypes.Throwaway, card);
            }

            private static Move Pickup(string[] cmdArgs) {
                if(cmdArgs.Length < 2 || cmdArgs.Length > 3)
                    throw new UnparseableMoveException("Two or three arguments expected. First argument pickup, " +
                    "second argument card on table, third argument card in hand.", string.Join(' ', cmdArgs));
                if (cmdArgs.Length == 2)
                    //"pickup 4" should be interpreted as "use the sole 4 in hand to pick up the sole 4 on table"
                    cmdArgs = new string[3] { cmdArgs[0], cmdArgs[1], cmdArgs[1] };
                string fromTable = cmdArgs[1];
                string fromHand = cmdArgs[2];
                if (ExtractBuildNamesFromString(ref fromTable)?.Count() != 0 || ExtractBuildNamesFromString(ref fromHand)?.Count() != 0)
                    throw new UnparseableMoveException("Build names cannot be used when picking up cards. Use capture instead.",
                        string.Join(' ', cmdArgs));
                List<byte> playOnTableRaw, playOnTable;
                byte playFromHandRaw, playFromHand;
                playOnTableRaw = playOnTable = new List<byte>();
                playFromHandRaw = playFromHand = 0;

                try {
                    playOnTableRaw = ExtractCardsFromString(fromTable);
                    playFromHandRaw = ExtractCardFromString(fromHand);
                } catch (Exception ex) {
                    if (ex is ArgumentOutOfRangeException || ex is ArgumentNullException) {
                        throw new UnparseableMoveException(Errorstr.NoMove(), string.Join(' ', cmdArgs));
                    } else {
                        throw ex;
                    }
                }

                playOnTable = GetCorrespondingCards(playOnTableRaw, table.CardsOnTable, CardLocations.Table);
                playFromHand = GetCorrespondingCards(new List<byte> { playFromHandRaw }, table.GetActivePlayer().Hand,
                    (table.GetActivePlayer().PlayerNo == Players.One) ? CardLocations.PlayerOneHand : CardLocations.PlayerTwoHand)[0];

                if (ContainsPictureCard(playOnTable.ToList())) {
                    if (playOnTable.Count != 1 || !CardsMatchInValue(playFromHand,playOnTable[0])) throw new IllegalPickupException(
                        "Cannot pickup multiple picture cards at once", playFromHand, (byte)GetCardValue(playFromHand));
                } else {
                    //if no error, then valid move
                    Build build = new Build(BuildNames.NONE, playOnTable, (byte)GetCardValue(playFromHand));
                }

                return new Move(MoveTypes.Pickup).PlayCard(playFromHand).PickupCards(playOnTable);
            }

            private static Move Build(string[] cmdArgs) {
                throw new NotImplementedException();
            }
            private static Move Capture(string[] cmdArgs) {
                throw new NotImplementedException();
            }


            /*********************************************************************/
            /************************** UTILITY METHODS **************************/
            /*********************************************************************/

            /// <summary>
            /// Assumes all BuildNames are extracted beforehand (example: alpha, beta). Run 
            /// ExtractBuildNamesFromString() if there is uncertainty.
            /// </summary>
            private static List<byte> ExtractCardsFromString(string cmd) {
                cmd = cmd.Replace("10", "T");
                string remain = cmd;
                List<byte> result = new List<byte>();
                while (remain.Length > 0) {
                    if (!charcardValAbbr.Contains(char.ToUpper(remain[0]))){
                        throw new UnparseableMoveException(Errorstr.CardFormat(), cmd);
                    }
                    if (remain.Length > 1 && cardSuitAbbrAscii.Contains(remain[1])) { //example 5h
                        result.Add(ExtractCardFromString(remain.Substring(0, 2)));
                        remain = remain.Substring(2);
                    } else { //examples 9, 92 (only parses 9 in both scenarios)
                        result.Add(ExtractCardFromString(remain.Substring(0, 1)));
                        remain = remain.Substring(1);
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
                    if(suit == CardSuits.NONE) { throw new UnparseableMoveException("Invalid suit provided to a card", cmd); }
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
            /// Handy little method that matches ambiguous cards with their full local representation, as well as determines if 
            /// such an association exists. If encountering a problem, will throw errors, expect to handle them by providing location.
            /// </summary>
            private static List<byte> GetCorrespondingCards(List<byte> expectedCards, List<byte> evaluatingDeck, CardLocations location = CardLocations.UNKNOWN) {
                //TODO: check for duplicates - so that, for example "44" doesn't result in an error for a deck with only 4h and 4s
                List<byte> modifiableDeck = new List<byte>(evaluatingDeck); //temp deck
                List<byte> updatedCards = new List<byte>(); //includes filled out suits
                foreach (byte card in expectedCards) {
                    try {
                        byte modifiedCard = 0;
                        if (!CardHasASuit(card)) {
                            modifiedCard = GetCardByValue(card, modifiableDeck);
                            updatedCards.Add(modifiedCard); //solves ambiguity
                        } else {
                            if (modifiableDeck.Contains(card)) {
                                modifiedCard = card;
                                updatedCards.Add(card);
                            } else {
                                throw new CardNotPresentException();
                            }
                        }
                        modifiableDeck.Remove(modifiedCard);
                    } catch (CardNotPresentException cnp) {
                        throw new CardNotPresentException("Card is not present in " + location.ToString(), cnp, card, location);
                    } catch (AmbiguousCardException ac) {
                        throw new AmbiguousCardException("Card is ambiguous and can refer to several cards in " + location.ToString(), ac, card, location);
                    } catch {
                        throw;
                    }
                }
                return updatedCards;
            }

            /// <summary>
            /// Returns card in deck that matches an ambiguous card where suit is missing.
            /// </summary>
            /// <exception cref="CardNotPresentException">No cards that match this value.</exception>
            /// <exception cref="AmbiguousCardException">Several cards match this value.</exception>
            private static byte GetCardByValue(byte cardWithNoSuit, List<byte> deckToCheckAgainst) {
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
