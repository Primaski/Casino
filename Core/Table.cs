using System;
using System.Collections.Generic;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core {
    public class Table {
        private Deck _deck = null;
        private byte _turnNo = 1;
        private bool initialDeal = true;
        private Move currentMove = null;

        private List<byte> _CardsOnTable = new List<byte>();
        public Deck Deck { get { return _deck; } private set { _deck = value; } }
        public short CountCardsInDeck { get { return (short)_deck.CardCount; } }
        public List<byte> CardsOnTable { get { return _CardsOnTable; } private set { _CardsOnTable = value; } }
        public short CountCardsOnTable { get { return (short)_CardsOnTable.Count; } }
        public byte TurnNumber { get { return _turnNo; } private set { _turnNo = value; } }


        public Table(Deck deck) {
            if((deck.GetDeck().Count - INITIAL_CARDS_ON_TABLE) % CARDS_PER_PLAYER != 0) {
                throw new Exception(Errorstr.UnevenDeck());
            }
            if(deck.GetDeck().Count != DECK_SIZE) {
                throw new Exception(
                    Errorstr.WrongCardCount("The table", DECK_SIZE, deck.GetDeck().Count));
            }
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
            currentMove = new Move(moveCmd);
            Logic.IsValidMove(this);
            return null;
        }

        private static class Logic {

            /// <summary>
            /// Diagnoses whether a move cmd is valid. No need to pass in string, since reference to Table is required.
            /// </summary>
            /// <param name="reference">Reference to the Table class that calls it.</param>
            public static bool IsValidMove (Table origin) {
                if(origin is null)
                    throw new NullReferenceException(); //shouldn't even be theoretically possible
                GetMoveAction(origin.currentMove);
                return true;
            }

            private static bool GetMoveAction(Move moveCmd) {
                if (string.IsNullOrEmpty(moveCmd)) {
                    throw new Exception(Errorstr.NoMove());
                }
                throw new NotImplementedException();

            }
        }

    }
}
