using System;
using System.Collections.Generic;
using System.Text;

namespace Casino.Core {
    public class Move {
        /// <summary>
        /// PLEASE SEE Rules.txt FOR INFORMATION ON HOW TO COMPOSE AND PARSE MOVE STRUCTURE.
        /// </summary>
        public string MoveCmd { get { return _moveCmd; } private set { _moveCmd = value; } }
        private string _moveCmd = "";
        public Move(string moveCmd) {
            _moveCmd = moveCmd;
        }

    }
}
