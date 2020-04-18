using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core {
    public static class Errorstr {

        public static string TooManyCards(string locationOfError, int expectedNumber, int actualNumber) {
            return (locationOfError + " expected at most " + expectedNumber + " cards, but received " 
                + actualNumber + " cards.");
        }
        public static string TooFewCards(string locationOfError, int expectedNumber, int actualNumber) {
            return (locationOfError + " expected at least " + expectedNumber + " cards, but received "
                + actualNumber + " cards.");
        }
        public static string WrongCardCount(string locationOfError, int expectedNumber, int actualNumber) {
            return (locationOfError + " expected exactly " + expectedNumber + " cards, but received "
                + actualNumber + " cards.");
        }
        public static string InvalidGameSetup() {
            return "Invalid game setup, ensure there are exactly two named players.";
        }

        public static string UnevenDeck() {
            return "The deck size (" + DECK_SIZE + ") minus the cards dealt to the table (" + INITIAL_CARDS_ON_TABLE + ") modulo " +
                "the number of cards dealt to each player (" + CARDS_PER_PLAYER + ") must be 0, in order for there to be the correct number of " +
                "cards reamining in the final deal.";
        }
        public static string NoMove() {
            return "A move assessment was attempted on a null or empty command.";
        }

        public static string CardFormat() {
            return "Card names are expected to be in the format [value]([suit])";
        }
    }
}
