﻿using System;
using System.Collections.Generic;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core {
    public class Player {
        public static byte MAX_NUMBER_OF_CARDS = 4;

        private string _name = "";
        private byte _score = 0;
        private byte _cardsInHand = 0;
        public string Name { get { return _name; } private set { _name = value; } }
        public byte Score { get { return _score; } private set { _score = value; } }
        public byte CardsInHand { get { return _cardsInHand; } private set { _cardsInHand = value; } }

        private List<byte> hand = null;
        private List<byte> localDeck = null;
        public Player(string name, List<byte> hand = null, List<byte> localDeck = null) {
            Name = name;
            hand ??= new List<byte>();
            localDeck ??= new List<byte>();
            this.hand = hand;
            this.localDeck = localDeck;
        }

        /// <summary>
        /// These cards will enter the player's Hand, and not their local deck (earned cards).
        /// </summary>
        /// <param name="newCards">Cards received directly from Deck. Must be limited to max number of cards available in Hand at once.</param>
        public bool ReceiveCards(List<byte> newCards) {
            if(newCards.Count > (MAX_NUMBER_OF_CARDS - CardsInHand)) {
                throw new Exception(ErrorMessage.TooManyCards(Name + "'s hand", 
                    MAX_NUMBER_OF_CARDS, newCards.Count));
            }
            throw new NotImplementedException();
            //hand always expects card array to be shifted left, so we fill in from the right
        }

        public bool AddCardsToLocalDeck(List<byte> cards) {
            throw new NotImplementedException();
        }
    }
}
