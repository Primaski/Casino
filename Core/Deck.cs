using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Casino.Core.Defs;
using Casino.Core.Util;
using Casino.Core.Error;

namespace Casino.Core {
    public class Deck {

        private BuildNames _buildName = BuildNames.NONE;
        private List<byte> _cardDeck = null;
        private bool canCreateNewDeck = true; //ensure that once a card has been added to the deck, this is set to false to ensure no new CreateDeck() calls.
        public short CardCount { get { return (short)_cardDeck.Count; } }
        public List<byte> CardDeck { get { return _cardDeck; } private set { _cardDeck = value; } }
        public BuildNames BuildName { get { return _buildName; } private set { _buildName = value; } }
        public Deck(bool empty = false, bool shuffle = true, byte noOfDecks = 1) {
            if (noOfDecks != 1) throw new NotImplementedException("Please limit decks to 1 for now.");
            if (!empty) {
                CreateDeck(noOfDecks);
                if (shuffle) ShuffleDeck();
            } else {
                _cardDeck = new List<byte>();
            }
        }

        protected void BuildInit(BuildNames buildName, List<byte> cardDeck) {
            BuildName = buildName;
            CardDeck = cardDeck;
        }

        /// <summary>
        /// Writes all remaining cards in Deck to the Command Prompt, and returns what was printed as a string.
        /// </summary>
        public string PrintDeck(bool newLine = false, bool shorthand = false, bool ascii = false) {
            //TODO: replace this with Defs.PrintCards
            StringBuilder result = new StringBuilder();
            List<byte> localDeck = _cardDeck;
            string sep = newLine ? "\n" : ", ";
            for (int i = 0; i < CardCount; i++) {
                string statement = (!shorthand) ? Defs.PrintCard(localDeck[i]) + sep
                    : Defs.PrintCardShorthand(localDeck[i], ascii) + sep;
                result.Append(statement);
            }
            Console.WriteLine(result.ToString());
            return result.ToString();
        }

        /// <summary>
        /// Orders deck by suit and then by value smallest to largest.
        /// </summary>
        /// <remarks>See Defs.cs to see how cards are stored.</remarks>
        public Deck OrderDeck() {
            _cardDeck = _cardDeck.OrderBy(x => x).ToList();
            return this;
        }

        /// <summary>
        /// Returns string of ordered deck by suit and then by value smallest to largest.
        /// </summary>
        /// <remarks>See Defs.cs to see how cards are stored.</remarks>
        public Deck PrintOrderedDeck() {
            //TODO Just return string
            Console.WriteLine(PrintCards(_cardDeck.OrderBy(x => x).ToList()));
            return this;
        }

        /// <summary>
        /// Returns a copy List of all cards in this deck.
        /// </summary>
        public List<byte> GetDeck() {
            return _cardDeck;
        }

        /// <summary>
        /// Remove cards directly from this deck. NOTE: Use DrawCards() instead to have the function return the removed cards.
        /// </summary>
        /// <param name="startIndex">For startIndex = 0 being the top card, state index of first removable card.</param>
        /// <param name="count">From startIndex, how many cards should be removed?</param>
        public Deck RemoveCards(short startIndex, short count) {
            if (count == 0) return this;
            if (count < _cardDeck.Count) {
                _cardDeck.RemoveRange(startIndex, count);
            } else {
                throw new Exception(Errorstr.TooManyCards("Removing cards", CardCount, count));
            }
            return this;
        }

        /// <summary>
        /// Remove cards directly from this deck from the top. NOTE: Use DrawCards() instead to have the function return the removed cards.
        /// </summary>
        /// <param name="count">How many cards should be removed from the top?</param>
        public Deck RemoveCards(short count) {
            return RemoveCards(0, count);
        }

        /// <summary>
        /// Remove cards with these values.
        /// </summary>
        /// <param name="count">How many cards should be removed from the top?</param>
        public Deck RemoveCards(List<byte> cards) {
            List<int> removableCards = new List<int>();
            foreach (byte card in CardDeck) {
                int removeIndex = CardDeck.IndexOf(card);
                if (removeIndex != -1) {
                    removableCards.Add(removeIndex);
                } else {
                    throw new CardNotPresentException("Card was not found when attempting to remove it from local deck.", card, CardLocations.UNKNOWN);
                }
            }
            _cardDeck.RemoveAll(x => removableCards.Contains(_cardDeck.IndexOf(x))); //TODO: what
            return this;
        }

        /// <summary>
        /// Removes cards directly from the top of the deck, and returns them to you.
        /// </summary>
        /// <param name="count">How many cards to remove from the top?</param>
        public virtual List<byte> DrawCards(short count) {
            if(count > CardCount) {
                throw new Exception(Errorstr.TooManyCards("Drawing cards",CardCount,count));
            }
            List<byte> removedCards = _cardDeck.GetRange(0, count);
            RemoveCards(count);
            return removedCards;

        }

        /// <summary>
        /// Adds cards directly to the deck at a specified index.
        /// </summary>
        /// <param name="atIndex">For atIndex = 0 being the top-most position, state the index to insert the first card.</param>
        /// <param name="cards">How many cards should be inserted?</param>
        public Deck AddCards(short atIndex, List<byte> cards) {
            short newCardNo = (short)_cardDeck.Count;
            if (cards.Count + CardCount >= DECK_SIZE) {
                _cardDeck.InsertRange(atIndex, cards);
            } else {
                throw new Exception(Errorstr.TooManyCards("Adding cards", DECK_SIZE, cards.Count));
            }
            canCreateNewDeck = false;
            return this;
        }

        /// <summary>
        /// Adds cards directly to the top of the deck, in the order they were provided.
        /// </summary>
        public Deck AddCardsToTop(List<byte> cards) {
            return AddCards(0, cards);
        }

        /// <summary>
        /// Adds cards directly to the bottom of the deck, in the order they were provided.
        /// </summary>
        public Deck AddCardsToBottom(List<byte> cards) {
            return AddCards((short)_cardDeck.Count, cards);
        }



        private Deck CreateDeck(byte noOfDecks) {
            if (!canCreateNewDeck) return this; 

            List<byte> cardList = new List<byte>();
            foreach (CardSuits suit in Enum.GetValues(typeof(CardSuits))) {
                foreach(CardVals val in Enum.GetValues(typeof(CardVals))) {
                    if(val != 0 && suit != 0) {
                        cardList.Add(GetCardDigit(val, suit));
                    }
                }
            }
            
            _cardDeck = cardList;
            canCreateNewDeck = false;
            return this;
        }

        private Deck ShuffleDeck() {
            /* Variation on Fisher-Yates algorithm */
            List<byte> deck = _cardDeck;
            if(deck?.Count == 0) { return this; }
            int highestIndex = CardCount - 1;
            for(int i = highestIndex; i >= 0; i--) {
                int rand = Misc.randomNumber.Next(0,i);
                byte tempCard = deck[i];
                deck[i] = deck[rand];
                deck[rand] = tempCard;
            }
            _cardDeck = deck;
            return this;
        }


    }
}
