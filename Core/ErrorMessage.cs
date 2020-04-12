using System;
using System.Collections.Generic;
using System.Text;

namespace Casino.Core {
    public static class ErrorMessage {

        public static string TooManyCards(string locationOfError, int expectedNumber, int actualNumber) {
            return (locationOfError + " expected at most " + expectedNumber + " cards, but received " 
                + actualNumber + " cards.");
        }
    }
}
