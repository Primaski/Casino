﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Casino.Core.Error {
    [Serializable]
    public class UnparseableCardException : Exception {

        public override string Message { get { return "The provided byte does not match any legal playing card."; } }

        public byte Card { get; set; } = 0;
        public UnparseableCardException() : base() {
        }
        public UnparseableCardException(string message, byte card)
            : base(DetailedMessage(message,card)) {
            this.Card = card;
        }
        private static string DetailedMessage(string message, byte card) {
            return message + "\nCard ID: " + card.ToString();
        }
        public UnparseableCardException(string message, Exception innerException)
            : base(message, innerException) { }
        public UnparseableCardException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
