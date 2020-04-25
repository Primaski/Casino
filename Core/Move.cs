using Casino.Core.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core {
    public class Move {
        /// <summary>
        /// PLEASE SEE Rules.txt FOR INFORMATION ON HOW TO COMPOSE AND PARSE MOVE STRUCTURE. This class is used to explain to 
        /// the Table what to do when Logic interprets a move. In the future, it should also be capable of making
        /// move reversion possible.
        /// 
        /// Please note that this class is stupid, and this is simply meant to be a path for Table and Logic to communicate. It
        /// has no information on which player is making the move, or if the move is actually possible. It just stores content
        /// about the move itself.
        /// </summary>

        MoveTypes _moveType = Defs.MoveTypes.NO_MOVE;
        byte _cardPlayed = 0;
        List<byte> _cardsPickedUp = null;
        Build _buildPickedUp = null;
        List<byte> _newBuildCards = null;
        Tuple<Build, List<byte>> _cardsAddedToExistingBuild = null;

        //impossible to combine more than 2 builds, as each player can only have one build on the board at a time
        Tuple<Build, Build> _combinedExistingBuilds = null; 

        public byte CardPlayed { get { return _cardPlayed; } }
        public List<byte> CardsPickedUp { get { return _cardsPickedUp; } }
        public Build BuildPickedUp { get { return _buildPickedUp; } }
        public List<byte> NewBuildCards { 
            get { return _newBuildCards; } 
            private set { if(value != null && value.Any()) { _newBuildCards = value; } } }
        
        public Tuple<Build, List<byte>> CardsAddedToExistingBuild { 
            get { return _cardsAddedToExistingBuild; } 
            private set { if (value.Item2 != null && value.Item2.Any()) { _cardsAddedToExistingBuild = value; } } }

        public Tuple<Build,Build> CombinedExistingBuilds { get { return _combinedExistingBuilds; } }
        public MoveTypes MoveType { get { return _moveType; } }
        private string _moveCmd = "";

        public Move(MoveTypes moveType) {
            _moveType = moveType;
        }
        public Move(MoveTypes moveType, byte cardPlayed) {
            _moveType = moveType;
            if (IsACard(cardPlayed)) {
                _cardPlayed = cardPlayed;
            } else {
                throw new UnparseableCardException("Attempted to create Move class with invalid card played.", cardPlayed);
            }
        }

        public Move PlayCard(byte cardPlayed) {
            if (IsACard(cardPlayed)) {
                _cardPlayed = cardPlayed;
            } else {
                throw new UnparseableCardException("Attempted to create Move class with invalid card played.", cardPlayed);
            }
            return this;
        }

        public Move PickupCards(List<byte> cards) {
            foreach(byte card in cards) {
                if (!IsACard(card)) {
                    throw new UnparseableCardException("Attempted to create Move class with invalid card picked up.", card);
                }
            }
            _cardsPickedUp = cards;
            return this;
        }


        public Move PickupBuild(Build build) {
            _buildPickedUp = build;
            return this;
        }

        public Move CreateNewBuild(List<byte> cardsInNewBuild) {
            if((cardsInNewBuild != null) && (!cardsInNewBuild.Any())) {
                NewBuildCards = cardsInNewBuild;
            } else {
                throw new Exception(Errorstr.EmptyBuild());
            }
            return this;
        }

        public Move AddCardsToExistingBuild(Build buildName, List<byte> cardsToAdd) {
            if ((cardsToAdd != null) && (!cardsToAdd.Any())) {
                CardsAddedToExistingBuild = new Tuple<Build, List<byte>>(buildName, cardsToAdd);
            } else {
                throw new Exception(Errorstr.EmptyBuild());
            }
            return this;
        }

        public Move CombineExistingBuilds(Build build1, Build build2) {
            _combinedExistingBuilds = new Tuple<Build, Build>(build1, build2);
            return this;
        }



    }
}
