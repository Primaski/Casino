using System;
using System.Collections.Generic;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core {
    public class Table {
        private Deck deck = null;
        private List<byte> cardsOnTable = new List<byte>();
        public Table(Deck deck) {
            if(deck.CardDeck.Count != DECK_SIZE) {
                throw new Exception(
                    ErrorMessage.WrongCardCount("The table", DECK_SIZE, deck.CardDeck.Count));
            }
            this.deck = deck;
        }

        /// <summary>
        /// Meant for use only for Game.cs. Will put new cards on table, and return player cards.
        /// </summary>
        /// <returns>T1 of tuple will be array of cards to be dealt to Player 1, and T2 to player 2.</returns>
        public Tuple<List<byte>,List<byte>> DealCards() {
            throw new NotImplementedException();
        }

    }
}
