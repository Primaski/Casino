using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core.Error {
    [Serializable]
    public class IllegalPickupException : Exception {
        public byte PickupCard { get; set; } = 0;
        public byte BuildValue { get; set; } = 0;
        public CardLocations Location { get; set; } = CardLocations.UNKNOWN;
        public IllegalPickupException() : base() {
            //bad practice, but the override makes it impossible for me to pass in AmbiguousCardExceptions by message. Hoping to learn how to fix this.
            throw new IllegalPickupException("A player can only pick up with a card value equal to the build on the board.", 0, 0);

        }
        public IllegalPickupException(string message, byte pickupCard, byte buildValue)
            : base(DetailedMessage(message, pickupCard, buildValue)) {
            this.PickupCard = pickupCard;
            this.BuildValue = buildValue;
        }
        private static string DetailedMessage(string message, byte pickupCard, byte buildValue) {
            return message + "\nCard Value: " + GetCardValue(pickupCard) + "\nBuild Value: " + buildValue.ToString() + "\nThese values must be non-zero and equal";
        }
        public IllegalPickupException(string message, Exception innerException, byte pickupCard, byte buildValue)
            : base(DetailedMessage(message, pickupCard, buildValue), innerException) {
            this.PickupCard = pickupCard;
            this.BuildValue = buildValue;
        }
        public IllegalPickupException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
