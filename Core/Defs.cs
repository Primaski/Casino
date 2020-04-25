using Casino.Core.Error;
using System;
using System.Collections.Generic;
using System.Text;

namespace Casino.Core {
    public static class Defs {

        /* MODIFIABLE VALUES */
        public static bool DEBUG_MODE = false;
        public static readonly byte NUMBER_OF_DECKS = 1; //ONLY ONE DECK IS SUPPORTED AS OF NOW, PLEASE DO NOT CHANGE THIS YET
        public static readonly byte CARDS_PER_PLAYER = 4;
        public static readonly byte INITIAL_CARDS_ON_TABLE = 4;
        public static readonly byte SCORE_TO_WIN = 21;
        public static readonly byte MAX_POINTS_PER_ROUND = 11; //only change this if ScoreableAttributes or PointsByAttribute are modified

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


        /********************** ARRAYS / DICTIONARIES / ENUMS  **********************/
        public enum CardVals {
            NONE, Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King
        };
        
        [Flags]
        public enum CardSuits {
            NONE = 0,  Clubs = 16, Diamonds = 32, Hearts = 64, Spades = 128
        };

        //Gamma is backup, one player can have a maximum of one build, meaning it should not be theoretically possible.
        public enum BuildNames {
            NONE, Alpha, Beta, Gamma
        };

        public enum Players {
            NONE, One, Two
        };

        public enum MoveTypes {
            NO_MOVE, Throwaway, Pickup, Build, Capture
        };

        public enum CardLocations {
            UNKNOWN, PlayerOneHand, PlayerTwoHand, PlayerOneLocalDeck, PlayerTwoLocalDeck, Deck, Table
        };

        public enum ScoreableAttributes {
            //dynamic
            NONE, MostCards, MostSpades,
            //static
            TenOfHearts, TwoOfSpades, AceOfClubs, AceOfDiamonds, AceOfHearts, AceOfSpades
        };

        public static Dictionary<ScoreableAttributes, byte> PointsByAttribute = new Dictionary<ScoreableAttributes, byte> {
            {ScoreableAttributes.NONE, 0 }, 
            {ScoreableAttributes.MostCards, 3},
            {ScoreableAttributes.MostSpades, 1},
            {ScoreableAttributes.TenOfHearts, 2},
            {ScoreableAttributes.TwoOfSpades, 1},
            {ScoreableAttributes.AceOfClubs, 1},
            {ScoreableAttributes.AceOfDiamonds, 1},
            {ScoreableAttributes.AceOfHearts, 1},
            {ScoreableAttributes.AceOfSpades, 1}
        };

        public static Dictionary<ScoreableAttributes, byte> CardNumberByStaticAttribute = new Dictionary<ScoreableAttributes, byte> {
            {ScoreableAttributes.TenOfHearts,GetCardDigit(CardVals.Ten,CardSuits.Hearts)},
            {ScoreableAttributes.TwoOfSpades,GetCardDigit(CardVals.Two,CardSuits.Spades)},
            {ScoreableAttributes.AceOfClubs,GetCardDigit(CardVals.Ace,CardSuits.Clubs)},
            {ScoreableAttributes.AceOfDiamonds,GetCardDigit(CardVals.Ace,CardSuits.Diamonds)},
            {ScoreableAttributes.AceOfHearts,GetCardDigit(CardVals.Ace,CardSuits.Hearts)},
            {ScoreableAttributes.AceOfSpades,GetCardDigit(CardVals.Ace,CardSuits.Spades)}
        };

        public static readonly string[] cardValAbbr =
            { "X", "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" 
        };

        public static readonly char[] charcardValAbbr =
            { 'X', 'A', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K'
        };

        public static readonly char[] cardSuitAbbr =
            { 'x', '♣', '♦', '♥', '♠'
        };
        public static readonly char[] cardSuitAbbrAscii =
            { 'x', 'c', 'd', 'h', 's'
        };



        /********************** METHODS **********************/
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
            byte val = (byte)(card & 15);
            if(val == 0 || val > 13) {
                throw new UnparseableCardException("Card value must not equal 0 or exceed 13 (King).",card);
            }
            return (CardVals)val;
        }
        public static CardSuits GetCardSuit(byte card) {
            byte suit = (byte)(card & 240);
            //below bitwise operation throws exception if 0 or 2+ flags are set.
            if(suit == 0 || (suit & (suit - 1)) != 0) {
                throw new UnparseableCardException("A card must have exactly one suit.",card);
            }
            return (CardSuits)(suit);
        }

        public static string GetCardValAbbr(byte val, bool charsOnly = false) {
            return !charsOnly ? cardValAbbr[val & 15] : charcardValAbbr[val & 15].ToString();
        }

        public static char GetCardSuitAbbr(byte suit, bool ascii = false) {
            if(suit == 0) return ((!ascii) ? cardSuitAbbr[0] : cardSuitAbbrAscii[0]);
            int arrPos = (int)((Math.Log2(suit) / Math.Log2(2)) - 3);
            return !ascii ? cardSuitAbbr[arrPos] : cardSuitAbbrAscii[arrPos];
        }

        public static bool IsACard(byte card) {
            try {
                string cardf = PrintCard(card);
                return !string.IsNullOrEmpty(cardf);
            } catch (UnparseableCardException) {
                return false;
            } catch {
                throw;
            }
        }

        public static bool CardHasASuit(byte card) {
            try {
                GetCardSuit(card);
                return true;
            } catch (UnparseableCardException) {
                return false;
            } catch {
                throw;
            }
        }

        public static bool CardHasAValue(byte card) {
            try {
                GetCardValue(card);
                return true;
            } catch (UnparseableCardException) {
                return false;
            } catch {
                throw;
            }
        }

        public static bool CardIsPictureCard(byte card) {
            CardVals value = GetCardValue(card);
            switch (value) {
                case CardVals.Jack:
                case CardVals.Queen:
                case CardVals.King:
                    return true;
                default:
                    return false;
            }
        }

        public static bool ContainsPictureCard(List<byte> cards) {
            foreach (byte card in cards) if (CardIsPictureCard(card)) return true;
            return false;
        }

        public static bool CardsMatchInValue(byte card1, byte card2) {
            return (GetCardValue(card1) == GetCardValue(card2));
        }

    }
}


