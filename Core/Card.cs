using System;
using System.Collections.Generic;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core {
    public class Card {
        public CardVals CardVal { get; }
        public CardSuits CardSuit { get; }
        public Card(CardVals value, CardSuits suit) {
            this.CardVal = value;
            this.CardSuit = suit;
        }
    }
}
