using System;
using System.Collections.Generic;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core {
    public class Build : Deck{
        public Build(BuildNames buildName, List<byte> cards) {
            BuildInit(buildName, cards);
        }

        public override List<byte> DrawCards(short count) {
            throw new Exception("You cannot draw cards from a build.");
        }
    }
}
