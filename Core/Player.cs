using System;
using System.Collections.Generic;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core {
    public class Player {
        public static byte MAX_NUMBER_OF_CARDS = 4;


        public string name = "";
        public byte score = 0;
        public byte cardsInHand = 0;
        private byte[] hand = new byte[MAX_NUMBER_OF_CARDS];
        private byte[] localDeck = new byte[DECK_SIZE];
        public Player(string name, byte[] hand = null, byte[] localDeck = null) {
            this.name = name;
        }

        /// <summary>
        /// These cards will enter the player's Hand, and not their local deck (earned cards).
        /// </summary>
        /// <param name="newCards">Cards received directly from Deck. Must be limited to max number of cards available in Hand at once.</param>
        public bool ReceiveCards(byte[] newCards) {
            if(newCards.Length > (MAX_NUMBER_OF_CARDS - cardsInHand)) {
                throw new Exception(ErrorMessage.TooManyCards(name + "'s hand", 
                    MAX_NUMBER_OF_CARDS, newCards.Length));
            }
            throw new NotImplementedException();
            //hand always expects card array to be shifted left, so we fill in from the right
        }

        public bool AddCardsToLocalDeck(byte[] cards) {
            throw new NotImplementedException();
        }
    }
}
