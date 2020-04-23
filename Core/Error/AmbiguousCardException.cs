using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core.Error {
    [Serializable]
    public class AmbiguousCardException : Exception {

        //public override string Message { get { return "The requested card by value or suit has more than one potential matching cards in the relevant deck."; }}

        public byte Card { get; set; } = 0;
        public CardLocations Location { get; set; } = CardLocations.UNKNOWN;
        public AmbiguousCardException() : base() {
            //TODO: bad practice, but the override makes it impossible for me to pass in AmbiguousCardExceptions by message. Hoping to learn how to fix this.
            throw new AmbiguousCardException("The requested card by value or suit has more than one potential matching cards in the relevant deck.", 0);
        }
        public AmbiguousCardException(string message, byte card, CardLocations location = CardLocations.UNKNOWN)
            : base(DetailedMessage(message, card, location)) {
            this.Card = card;
            this.Location = location;
        }
        private static string DetailedMessage(string message, byte card, CardLocations location) {
            return message + "\nCard ID: " + card.ToString() + "\nLocation: " + location.ToString();
        }
        public AmbiguousCardException(string message, Exception innerException, byte card, CardLocations location = CardLocations.UNKNOWN)
            : base(DetailedMessage(message, card, location), innerException) {
            this.Card = card;
            this.Location = location;
        }
        public AmbiguousCardException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
