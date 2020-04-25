using Casino.Core.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core {
    public class Player {
        public static byte MAX_NUMBER_OF_CARDS = 4;

        private string _name = "";
        private byte _score = 0;
        private byte _currRoundScore = 0;

        private List<byte> _hand = null;
        private Deck _localDeck = null;
        private Players _playerNo = Players.NONE;
        private List<List<ScoreableAttributes>> _scoreLog = new List<List<ScoreableAttributes>>();
        public Players PlayerNo { get { return _playerNo; } }
        public string Name { get { return _name; } }
        public byte Score { get { return _score; } }
        public byte CurrRoundScore { get { return _currRoundScore; } }
        public byte CountCardsInHand {
            get { return (byte)Hand.Count; }
        }
        public short CountCardsInDeck {
            get { return (short)_localDeck.CardCount; }
        }
        public List<byte> Hand { get { return _hand; } }
        public Deck LocalDeck { get { return _localDeck; } }
        private List<List<ScoreableAttributes>> ScoreLog { get { return _scoreLog; } }


        public Player(string name, Players playerNumber, List<byte> hand = null, Deck localDeck = null) {
            _name = name;
            _playerNo = playerNumber;
            hand ??= new List<byte>();
            localDeck ??= new Deck(true);
            this._hand = hand;
            this._localDeck = localDeck;
        }

        public string PrintHand() {
            return PrintCards(Hand);
        }

        public bool HasEmptyHand() {
            if (CountCardsInHand == 0) return true;
            return false;
        }

        /// <summary>
        /// These cards will enter the player's Hand (playable cards), and not their local deck (earned cards).
        /// </summary>
        /// <param name="newCards">Cards received directly from Deck. Must be limited to max number of cards available in Hand at once.</param>
        public bool ReceiveCards(List<byte> newCards) {
            if (newCards.Count > (MAX_NUMBER_OF_CARDS - CountCardsInHand)) {
                throw new Exception(Errorstr.TooManyCards(Name + "'s hand",
                    MAX_NUMBER_OF_CARDS, newCards.Count));
            }
            _hand.AddRange(newCards);
            return true;
        }

        /// <summary>
        /// This card will be removed from player's Hand (playable cards), and not their local deck (earned cards). If card does not exist, this will return false.
        /// </summary>
        public bool RemoveCard(byte card) {
            return RemoveCards(new List<byte> { card });
        }

        /// <summary>
        /// These cards will be removed from player's Hand (playable cards), and not their local deck (earned cards). If any card does not exist, this will return false and
        /// no changes will be made.
        /// </summary>
        public bool RemoveCards(List<byte> cardsToRemove) {
            List<int> removableCards = new List<int>();
            foreach (byte card in cardsToRemove) {
                int removeIndex = Hand.IndexOf(card);
                if (removeIndex != -1) {
                    removableCards.Add(removeIndex);
                } else {
                    return false;
                }
            }
            Hand.RemoveAll(x => removableCards.Contains(Hand.IndexOf(x)));
            return true;
        }

        /// <summary>
        /// These cards will enter the player's Local Deck (earned cards), and not their Hand (playable cards).
        /// </summary>
        /// <param name="newCards">Cards received directly from Deck. Must be limited to max number of cards available in Hand at once.</param>
        public bool AddCardsToLocalDeck(List<byte> cards) {
            LocalDeck.AddCardsToTop(cards);
            return true;
        }

        /// <summary>
        /// This card will be removed from player's Hand (playable cards), and not their local deck (earned cards). If card does not exist, this will return false.
        /// </summary>
        public bool RemoveCardFromLocalDeck(byte card) {
            return RemoveCardsFromLocalDeck(new List<byte> { card });
        }

        /// <summary>
        /// These cards will be removed from player's Hand (playable cards), and not their local deck (earned cards). If any card does not exist, this will return false and
        /// no changes will be made.
        /// </summary>
        public bool RemoveCardsFromLocalDeck(List<byte> cardsToRemove) {
            try {
                LocalDeck.RemoveCards(cardsToRemove);
            } catch (CardNotPresentException cnp) {
                if (PlayerNo == Players.One) {
                    cnp.Location = CardLocations.PlayerOneLocalDeck;
                } else {
                    cnp.Location = CardLocations.PlayerTwoLocalDeck;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets cards in hand by value.
        /// </summary>
        public List<byte> GetCardsByValue(CardVals value) {
            List<byte> result = new List<byte>();
            foreach (byte card in Hand) {
                if (GetCardValue(card) == value) {
                    result.Add(card);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets cards in hand by suit.
        /// </summary>
        public List<byte> GetCardsBySuit(CardSuits suit) {
            List<byte> result = new List<byte>();
            foreach (byte card in Hand) {
                if (GetCardSuit(card) == suit) {
                    result.Add(card);
                }
            }
            return result;
        }

        /// <summary>
        /// Returns true if player has card in hand.
        /// </summary>
        public bool HasCardInHand(byte card) {
            if (!IsACard(card)) return false;
            foreach (byte cardf in Hand) {
                if (cardf == card) return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if player has card in local deck.
        /// </summary>
        public bool HasCardInLocalDeck(byte card) {
            if (!IsACard(card)) return false;
            var deck = LocalDeck.GetDeck();
            foreach (byte cardf in deck) {
                if (cardf == card) return true;
            }
            return false;
        }

        public List<ScoreableAttributes> GetScoreLog(byte roundNumber) {
            if (roundNumber >= _scoreLog.Count) throw new NullReferenceException();
            return _scoreLog[roundNumber];
        }

        public void AddNewScoreLogEntry(List<ScoreableAttributes> roundScores) {
            if (roundScores == null) throw new ArgumentNullException();
            foreach(var attribute in roundScores) {
                _currRoundScore += PointsByAttribute[attribute];
            }
            _score += _currRoundScore;
            _scoreLog.Add(roundScores);
        }

        public void ReplaceScoreLogEntry(byte roundNumber, List<ScoreableAttributes> newEntry) {
            if (roundNumber > _scoreLog.Count) throw new NullReferenceException();
            _scoreLog[roundNumber - 1] = newEntry ?? throw new ArgumentNullException();
        }

        public void RemoveScoreLogEntry(byte roundNumber, bool isThisRound = false) {
            if (roundNumber > _scoreLog.Count) throw new NullReferenceException();
            var log = _scoreLog[roundNumber - 1];
            foreach (var attribute in log) {
                byte removePoints = PointsByAttribute[attribute];
                if (isThisRound) _currRoundScore -= (_currRoundScore >= removePoints) ? removePoints : throw new Exception("Round score is below 0");
                _score -= (_score >= removePoints) ? removePoints : throw new Exception("Score is below 0.");
            }
            _scoreLog.RemoveAt(roundNumber);
        }

        public string PrintScoreLogEntry(byte roundNumber) {
            if (roundNumber > _scoreLog.Count) throw new NullReferenceException();
            StringBuilder str = new StringBuilder();
            str.Append("Player " + PlayerNo + "\nRound " + roundNumber + "\nSCORES:\n");
            var currLogEntry = _scoreLog[roundNumber - 1] ?? throw new NullReferenceException();
            byte roundScore = 0;
            foreach(var attr in currLogEntry) {
                byte attrScore = PointsByAttribute[attr];
                roundScore += attrScore;
                str.AppendLine("[ " + attr.ToString() + " is worth " + attrScore + " points.]");
            }
            str.AppendLine("For a total of " + roundScore + " points.");
            return str.ToString();
        }
    }
}
