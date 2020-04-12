using System;
using System.Collections.Generic;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core {
    public class Player {
        public static byte MAX_NUMBER_OF_CARDS = 5;


        public string name = "[No name]";
        public byte score = 0;
        public byte cardsInHand = 0;
        private Card[] hand = new Card[MAX_NUMBER_OF_CARDS];
        public Player(string name, Card[] cards = null) {
            this.name = name;
            ReceiveCards(cards);
        }

        public bool ReceiveCards(Card[] newCards) {
            if(newCards.Length > (MAX_NUMBER_OF_CARDS - cardsInHand)) {
                throw new Exception(ErrorMessage.TooManyCards(name + "'s hand", 
                    MAX_NUMBER_OF_CARDS, newCards.Length));
            }
            throw new NotImplementedException();
            //hand always expects card array to be shifted left, so we fill in from the right
            

        }
    }
}
