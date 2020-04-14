using System;
using System.Collections.Generic;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core {
    public class Player {
        public static byte MAX_NUMBER_OF_CARDS = 4;

        private string _name = "";
        private byte _score = 0;
        private List<byte> _hand = null;
        private Deck _localDeck = null;
        public string Name { get { return _name; } private set { _name = value; } }
        public byte Score { get { return _score; } private set { _score = value; } }
        public byte CountCardsInHand { 
            get { return (byte)Hand.Count; } 
        }
        public short CountCardsInDeck {
            get { return (short)_localDeck.CardCount; }
        }
        public List<byte> Hand { get { return _hand; } private set { _hand = value;  } }
        public Deck LocalDeck { get { return _localDeck; } private set { _localDeck = value;  } }


        public Player(string name, List<byte> hand = null, Deck localDeck = null) {
            Name = name;
            hand ??= new List<byte>();
            localDeck ??= new Deck(true);
            this._hand = hand;
            this._localDeck = localDeck;
        }

        /// <summary>
        /// These cards will enter the player's Hand (playable cards), and not their local deck (earned cards).
        /// </summary>
        /// <param name="newCards">Cards received directly from Deck. Must be limited to max number of cards available in Hand at once.</param>
        public bool ReceiveCards(List<byte> newCards) {
            if(newCards.Count > (MAX_NUMBER_OF_CARDS - CountCardsInHand)) {
                throw new Exception(ErrorMessage.TooManyCards(Name + "'s hand", 
                    MAX_NUMBER_OF_CARDS, newCards.Count));
            }
            _hand.AddRange(newCards);
            return true;
        }

        /// <summary>
        /// These cards will enter the player's Local Deck (earned cards), and not their Hand (playable cards).
        /// </summary>
        /// <param name="newCards">Cards received directly from Deck. Must be limited to max number of cards available in Hand at once.</param>
        public bool AddCardsToLocalDeck(List<byte> cards) {
            throw new NotImplementedException();
        }
    }
}
