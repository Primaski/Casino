using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Casino.Core.Error {
    [Serializable]
    public class UnparseableCardException : Exception {

        public byte card { get; set; }
        public UnparseableCardException() : base() { }
        public UnparseableCardException(string message, byte card)
            : base(message) {
            this.card = card;
        }
        public UnparseableCardException(string message, Exception innerException)
            : base(message, innerException) { }
        public UnparseableCardException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
