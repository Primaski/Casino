using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Casino.Core.Error {
    [Serializable]
    public class UnparseableMoveException : Exception {
        
        public string move { get; set; }
        public UnparseableMoveException() : base() { }
        public UnparseableMoveException(string message, string move) 
            : base(message) {
            this.move = move;
        }
        public UnparseableMoveException(string message, Exception innerException)
            : base(message, innerException) { }
        public UnparseableMoveException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
