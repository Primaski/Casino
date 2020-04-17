using System;
using System.Collections.Generic;
using System.Text;

namespace Casino.Core {
    public static class Defs {

        /* MODIFIABLE VALUES */
        public static readonly byte NUMBER_OF_DECKS = 1;
        public static readonly byte CARDS_PER_PLAYER = 4;
        public static readonly byte INITIAL_CARDS_ON_TABLE = 4;
        public static readonly char RULES_SECTION_OF_CMD_FORMATTING = 'F';

        ///* PLEASE DO NOT MODIFY VALUES BELOW *///
        /// ***********************************************************
        /// HOW IT WORKS:
        /// A card is stored in a one-byte digit. This digit uses flags in the first four digits to indicate the
        /// suit. The last four digits are used for storing the value of the card:
        /// 0 is null, A is 1 (0001) ... J is 11 (1011), Q is 12 (1100), K is 13 (1101), 14 and 15 are null. 
        /// Here are some examples:
        /// 0001 0001 -> Ace of Clubs
        /// 0001 0010 -> Two of Clubs
        /// 0010 0001 -> Ace of Diamonds
        /// 1000 1011 -> Jack of Spades
        /// 0000 0001 -> Null
        /// 0001 1111 -> Null
        /// We can use bitwise operations to extract the suit and value of the card (using below methods), and it only takes 1 byte!
        /// ***********************************************************///

        public static readonly short DECK_SIZE = (short)(52 * NUMBER_OF_DECKS);
        public enum CardVals {
            NONE, Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King
        };
        
        [Flags]
        public enum CardSuits {
            Clubs = 16, Diamonds = 32, Hearts = 64, Spades = 128
        };
        private static string[] cardValAbbr =
            { "X", "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" 
        };
        private static char[] cardSuitAbbr =
            { '♣', '♦', '♥', '♠'
        };
        private static char[] cardSuitAbbrAscii =
            { 'c', 'd', 'h', 's'
        };

        public static string PrintCard(byte card) {
            return GetCardValue(card) + " of " + GetCardSuit(card);
        }

        public static string PrintCards(List<byte> cards) {
            StringBuilder str = new StringBuilder();
            foreach(var card in cards) {
                str.Append(PrintCard(card) + ", ");
            }
            return str.ToString();
        }
        public static string PrintCardShorthand(byte card, bool ascii = false) {
            return GetCardValAbbr(card) + GetCardSuitAbbr(card,ascii);
        }

        public static string PrintCardsShorthand(List<byte> cards, bool ascii = false) {
            StringBuilder str = new StringBuilder();
            foreach (var card in cards) {
                str.Append(PrintCardShorthand(card,ascii) + ", ");
            }
            return str.ToString();
        }
        public static byte GetCardDigit(CardVals value, CardSuits suit) {
            return (byte)((byte)value + (byte)suit);
        }
        public static CardVals GetCardValue(byte card) {
            return (CardVals)(card & 15);
        }
        public static CardSuits GetCardSuit(byte card) {
            return (CardSuits)(card & 240);
        }

        public static string GetCardValAbbr(byte val) {
            return cardValAbbr[val & 15];
        }

        public static char GetCardSuitAbbr(byte suit, bool ascii = false) {
            int arrPos = (int)((Math.Log2(suit) / Math.Log2(2)) - 4);
            return !ascii ? cardSuitAbbr[arrPos] : cardSuitAbbrAscii[arrPos];
        }


    }
}


