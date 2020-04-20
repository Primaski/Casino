using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Casino.Core.Error {
    [Serializable]
    public class UnparseableMoveException : Exception {
        //public override string Message { get { return ""; } }

        public string Move { get; set; } = null;
        public UnparseableMoveException() : base() {
            //bad practice, but the override makes it impossible for me to pass in AmbiguousCardExceptions by message. Hoping to learn how to fix this.
            throw new UnparseableMoveException("The user-provided command is not in the correct format for a move.","");
        }
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
