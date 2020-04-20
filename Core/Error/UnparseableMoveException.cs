using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Casino.Core.Error {
    [Serializable]
    public class UnparseableMoveException : Exception {
        public override string Message { get { return "The user-provided command is not in the correct format for a move."; } }

        public string Move { get; set; } = null;
        public UnparseableMoveException() : base() { }
        public UnparseableMoveException(string message, string move) 
            : base(DetailedMessage(message,move)) {
            this.Move = move;
        }

        private static string DetailedMessage(string message, string moveCmd) {
            return message + "\nMove command:" + moveCmd;
        }
        public UnparseableMoveException(string message, Exception innerException)
            : base(message, innerException) { }
        public UnparseableMoveException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
