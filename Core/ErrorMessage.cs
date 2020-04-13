using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core {
    public static class ErrorMessage {

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
    }
}
