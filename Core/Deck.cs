using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core {
    public class Deck {
        
        private List<byte> cardDeck = null;
        public List<byte> CardDeck { get { return cardDeck; } private set { cardDeck = value; } }
        short noOfCards = 0;
        public Deck(bool shuffle = true, byte noOfDecks = 1) {
            if (noOfDecks != 1) throw new NotImplementedException("Please limit decks to 1 for now.");
            CreateDeck(noOfDecks);
            if(shuffle) ShuffleDeck();
        }

        private void CreateDeck(byte noOfDecks) {
            List<byte> cardList = new List<byte>();
            foreach (CardSuits suit in Enum.GetValues(typeof(CardSuits))) {
                foreach(CardVals val in Enum.GetValues(typeof(CardVals))) {
                    if(val != 0) {
                        cardList.Add(GetCardDigit(val, suit));
                    }
                }
            }
            cardDeck = cardList;
        }


        private Deck ShuffleDeck() {
            /* Variation on Fisher-Yates algorithm */
            List<byte> deck = CardDeck;
            int highestIndex = noOfCards - 1;
            for(int i = highestIndex; i >= 0; i--) {
                int rand = Util.randomNumber.Next(0,i);
                byte tempCard = deck[i];
                deck[i] = deck[rand];
                deck[rand] = tempCard;
            }
            CardDeck = deck;
            return this;
        }

        public string PrintDeck(bool newLine = false, bool shorthand = false, bool ascii = false) {
            StringBuilder result = new StringBuilder();
            List<byte> localDeck = CardDeck;
            string sep = newLine ? "\n" : ", ";
            for(int i = 0; i < noOfCards; i++) {
                string statement = (!shorthand) ? Defs.PrintCard(localDeck[i]) + sep
                    : Defs.PrintCardShorthand(localDeck[i], ascii) + sep;
                result.Append(statement);
            }
            Console.WriteLine(result.ToString());
            return result.ToString();
        }

        public Deck OrderDeck() {
            CardDeck = CardDeck.OrderBy(x => x).ToList();
            return this;
        }

    
    }
}
