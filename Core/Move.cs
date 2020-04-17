using System;
using System.Collections.Generic;
using System.Text;

namespace Casino.Core {
    public class Move {
        public string moveCmd { get { return _moveCmd; } private set { _moveCmd = value; } }
        private string _moveCmd = "";
        public Move(string moveCmd) {
            _moveCmd = moveCmd;
        }

    }
}
