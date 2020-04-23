using Casino.Core.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core {
    public class Build : Deck{

        private byte _buildValue = 0;
        private bool _isLocked = false;
        public byte BuildValue { get { return _buildValue; } set { _buildValue = value; } }
        public bool IsLocked { get { return _isLocked; } set { _isLocked = value; } }

        public Build(BuildNames buildName, List<byte> cards, byte buildValue) {
            if (cards == null || !cards.Any()) {
                throw new InvalidBuildException("Attempted build with no cards.", 0, null);
            }
            if (!IsValidBuild(cards, buildValue)) {
                throw new InvalidBuildException("Cards have no way of attaining build value.", buildValue, cards);
            }
            BuildValue = buildValue;
            DetermineIfLocked();
            BuildInit(buildName, cards);
        }
        
        public void DetermineIfLocked() {
            OrderDeck();
            if(BuildValue == CardDeck.Last()) {
                IsLocked = true;
            }
            IsLocked = false;
        }

        public override List<byte> DrawCards(short count) {
            throw new Exception("You cannot draw cards from a build.");
        }

        public override Deck AddCards(short atIndex, List<byte> cards) {
            throw new NotImplementedException();
            DetermineIfLocked();
            return base.AddCards(atIndex, cards);
        }

        /// <summary>
        /// Checks if there exists some set of groupings of values in buildCards whereby all groupings sum to buildValue. 
        /// </example>
        /// <example> IsValidBuild({3,5,8},8) = TRUE (because (3+5) and (8) both sum to 8.) </example>
        /// <example> IsValidBuild({2,3,5,8},8) = FALSE (because (3+5) and (8) sum to 8, but (2) is leftover.)</example>
        private bool IsValidBuild(List<byte> cards, byte buildValue) {
            int sumOfCards = 0;
            foreach(byte card in cards) {
                if (CardIsPictureCard(card)) return false;
                sumOfCards += (int)GetCardValue(card);
            }
            return (sumOfCards % buildValue == 0);
        }

    }
}
