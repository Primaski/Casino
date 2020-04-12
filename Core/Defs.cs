using System;
using System.Collections.Generic;
using System.Text;

namespace Casino.Core {
    public static class Defs {
        public enum CardVals {
            NONE, Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King
        };
        public enum CardSuits {
            Clubs, Diamonds, Hearts, Spades
        };

        private static string[] cardValShorthand =
            { "X", "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" 
        };

        private static char[] cardSuitShorthand =
            { '♣', '♦', '♥', '♠'
        };

        private static char[] cardSuitShorthandascii =
            { 'c', 'd', 'h', 's'
        };
        public static string GetCardValShorthand(byte val) {
            return cardValShorthand[val];
        }

        public static char GetCardSuitShorthand(byte suit, bool ascii = false) {
            return !ascii ? cardSuitShorthand[suit] : cardSuitShorthandascii[suit];
        }

    }
}
