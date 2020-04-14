using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core {
    public class Deck {
        
        private List<byte> cardDeck = null;
        private short _countCards = 0;
        public short CountCards { get { return _countCards; } private set { _countCards = value;  } }
        public Deck(bool empty = false, bool shuffle = true, byte noOfDecks = 1) {
            if (noOfDecks != 1) throw new NotImplementedException("Please limit decks to 1 for now.");
            if (!empty) {
                CreateDeck(noOfDecks);
                if (shuffle) ShuffleDeck();
            }
        }

        private Deck CreateDeck(byte noOfDecks) {
            List<byte> cardList = new List<byte>();
            foreach (CardSuits suit in Enum.GetValues(typeof(CardSuits))) {
                foreach(CardVals val in Enum.GetValues(typeof(CardVals))) {
                    if(val != 0) {
                        cardList.Add(GetCardDigit(val, suit));
                        _countCards++;
                    }
                }
            }
            cardDeck = cardList;
            return this;
        }

        private Deck ShuffleDeck() {
            /* Variation on Fisher-Yates algorithm */
            List<byte> deck = cardDeck;
            if(deck?.Count == 0) { return this; }
            int highestIndex = _countCards - 1;
            for(int i = highestIndex; i >= 0; i--) {
                int rand = Util.randomNumber.Next(0,i);
                byte tempCard = deck[i];
                deck[i] = deck[rand];
                deck[rand] = tempCard;
            }
            cardDeck = deck;
            return this;
        }

        public string PrintDeck(bool newLine = false, bool shorthand = false, bool ascii = false) {
            StringBuilder result = new StringBuilder();
            List<byte> localDeck = cardDeck;
            string sep = newLine ? "\n" : ", ";
            for(int i = 0; i < _countCards; i++) {
                string statement = (!shorthand) ? Defs.PrintCard(localDeck[i]) + sep
                    : Defs.PrintCardShorthand(localDeck[i], ascii) + sep;
                result.Append(statement);
            }
            Console.WriteLine(result.ToString());
            return result.ToString();
        }

        public Deck OrderDeck() {
            cardDeck = cardDeck.OrderBy(x => x).ToList();
            return this;
        }

        public List<byte> GetDeck() {
            return cardDeck;
        }
        public Deck RemoveCards(short startIndex, short count) {
            if (count == 0) return this;
            if (count < cardDeck.Count) {
                try {
                    cardDeck.RemoveRange(startIndex, count);
                    _countCards -= count;
                } catch { 
                    throw; 
                }
            } else {
                throw new Exception(ErrorMessage.TooManyCards("Removing cards", cardDeck.Count, count));
            }
            return this;
        }
        public Deck RemoveCards(short count) {
            return RemoveCards(0,count);
        }
        public Deck AddCards(short atIndex, List<byte> cards) {
            short newCardNo = (short)cardDeck.Count;
            if (cards.Count + _countCards >= DECK_SIZE) {
                try {
                    cardDeck.InsertRange(atIndex, cards);
                    this._countCards += newCardNo;
                } catch { 
                    throw; 
                }
            } else {
                throw new Exception(ErrorMessage.TooManyCards("Adding cards", DECK_SIZE, cards.Count));
            }
            return this;
        }

        public Deck AddCardsToTop(List<byte> cards) {
            return AddCards(0, cards);
        }

        public Deck AddCardsToBottom(List<byte> cards) {
            return AddCards((short)cardDeck.Count, cards);
        }

    }
}
