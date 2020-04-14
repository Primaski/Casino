using System;
using System.Collections.Generic;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core {
    public class Table {
        private Deck _deck = null;
        private List<byte> _countCardsOnTable = new List<byte>();
        public Deck Deck { get { return _deck; } private set { _deck = value; } }
        public List<byte> CountCardsOnTable { get { return _countCardsOnTable; } private set { _countCardsOnTable = value; } }
        private bool initialDeal = true;
        public Table(Deck deck) {
            if((deck.GetDeck().Count - INITIAL_CARDS_ON_TABLE) % CARDS_PER_PLAYER != 0) {
                throw new Exception(ErrorMessage.UnevenDeck());
            }
            if(deck.GetDeck().Count != DECK_SIZE) {
                throw new Exception(
                    ErrorMessage.WrongCardCount("The table", DECK_SIZE, deck.GetDeck().Count));
            }
            Deck = deck;
        }

        /// <summary>
        /// Meant for use only for Game.cs. Will put new cards on table, and return player cards.
        /// </summary>
        /// <returns>T1 of tuple will be array of cards to be dealt to Player 1, and T2 to player 2.</returns>
        public Tuple<List<byte>,List<byte>> DealCards() {
            if (initialDeal) InitialDeal();
            //testing statements
            Console.WriteLine("If reached this part with no errors, then good so far. Here comes an intentional error, though:");
            throw new NotImplementedException();
        }

        private void InitialDeal() {
            var cardsMoveToTable = Deck.GetDeck().GetRange(0, INITIAL_CARDS_ON_TABLE);
            _countCardsOnTable.AddRange(cardsMoveToTable);
            Deck.RemoveCards(INITIAL_CARDS_ON_TABLE);
            //testing statements
            Console.WriteLine("Cards on Table: " + PrintCard(_countCardsOnTable[0]) + ", " + PrintCard(_countCardsOnTable[1]) + ", " + 
                PrintCard(_countCardsOnTable[2]) + ", " + PrintCard(_countCardsOnTable[3]));
            Console.WriteLine(Deck.CountCards + " cards left in the deck after dealing ONLY to the table.");
        }
    }
}
