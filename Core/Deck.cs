using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core {
    public class Deck {
        private Card[] cardDeck;
        public Card[] CardDeck { get { return cardDeck; } private set { cardDeck = value; } }
        short noOfCards = 0;
        public Deck(bool shuffle = true) {
            CreateDeck();
            if(shuffle) ShuffleDeck();
        }

        private void CreateDeck() {
            //used this in case of special decks
            int maxSuit = (int)Enum.GetValues(typeof(CardSuits)).Cast<int>().Last();
            int maxVal = (int)Enum.GetValues(typeof(CardVals)).Cast<int>().Last();
            List<Card> cardList = new List<Card>();
            for(int suit = 0; (suit <= maxSuit) && (noOfCards < short.MaxValue); suit++) {
                for(int val = 1; val <= maxVal; val++) {
                    cardList.Add(new Card((CardVals)val, (CardSuits)suit));
                    noOfCards++;
                }
            }
            CardDeck = cardList.ToArray();
        }


        private void ShuffleDeck() {
            /* Variation on Fisher-Yates algorithm */
            Card[] originalDeck = CardDeck;
            int highestIndex = noOfCards - 1;
            for(int i = highestIndex; i >= 0; i--) {
                int rand = Util.randomNumber.Next(0,i);
                Card temp = originalDeck[i];
                originalDeck[i] = originalDeck[rand];
                originalDeck[rand] = temp;
            }
        }

        public string PrintDeck(bool newLine = false, bool shorthand = false, bool ascii = false) {
            StringBuilder result = new StringBuilder();
            Card[] localDeck = CardDeck;
            string sep = newLine ? "\n" : ", ";
            for(int i = 0; i < noOfCards; i++) {
                string statement = shorthand ? (Defs.GetCardValShorthand((byte)localDeck[i].CardVal) +
                    GetCardSuitShorthand((byte)localDeck[i].CardSuit,ascii) + sep) :
                    (localDeck[i].CardVal + " of " + localDeck[i].CardSuit + sep);
                result.Append(statement);
            }
            Console.WriteLine(result.ToString());
            return result.ToString();
        }

        public void OrderDeck() {
            CardDeck = CardDeck.OrderBy(x => x.CardSuit).ThenBy(x => x.CardVal).ToArray();
        }


    }
}
