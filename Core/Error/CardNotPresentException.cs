using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core.Error {
    [Serializable]
    public class CardNotPresentException : Exception {

        //public override string Message { get { return "The requested card does not exist in the location specified."; } }
        public byte Card { get; set; } = 0;
        public CardLocations Location { get; set; } = CardLocations.UNKNOWN;
        public CardNotPresentException() : base() {
            //bad practice, but the override makes it impossible for me to pass in AmbiguousCardExceptions by message. Hoping to learn how to fix this.
            throw new CardNotPresentException("The requested card does not exist in the location specified.", 0);

        }
        public CardNotPresentException(string message, byte card, CardLocations location = CardLocations.UNKNOWN)
            : base(DetailedMessage(message,card,location)) {
            this.Card = card;
            this.Location = location;
        }
        private static string DetailedMessage(string message, byte card, CardLocations location) {
            return message + "\nCard ID: " + card.ToString() + "\nLocation: " + location.ToString();
        }
        public CardNotPresentException(string message, Exception innerException)
            : base(message, innerException) { }
        public CardNotPresentException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
