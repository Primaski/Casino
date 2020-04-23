using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core.Error {
    [Serializable]
    public class InvalidBuildException : Exception {
        public List<byte> Build { get; set; } = null;
        public byte BuildValue { get; set; } = 0;
        public InvalidBuildException() : base() {
            //bad practice, but the override makes it impossible for me to pass in AmbiguousCardExceptions by message. Hoping to learn how to fix this.
            throw new IllegalPickupException("The attempted build was invalid.", 0, 0);

        }
        public InvalidBuildException(string message, byte buildValue, List<byte> build)
            : base(DetailedMessage(message, buildValue, build)) {
            this.Build = build;
            this.BuildValue = buildValue;
        }
        private static string DetailedMessage(string message, byte buildValue, List<byte> build = null) {
            return message + "\nBuild: " + PrintCards(build ?? new List<byte>()) + "\nBuild Value: " + buildValue;
        }
        public InvalidBuildException(string message, Exception innerException, byte buildValue, List<byte> build)
            : base(DetailedMessage(message, buildValue, build), innerException) {
            this.Build = build;
            this.BuildValue = buildValue;
        }
        public InvalidBuildException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
