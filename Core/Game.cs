using System;
using System.Collections.Generic;
using System.Text;
using static Casino.Core.Defs;

namespace Casino.Core {
    public static class Game {

        private static Player p1 = null;
        private static Player p2 = null;
        private static Table table = null;


        public static void NewGame(Player[] players) {
            Deck deck = new Deck();
            table = new Table(deck);

            try {
                if (!IsValidGame(players)) {
                    throw new Exception(ErrorMessage.InvalidGameSetup());
                }
            }catch(Exception e) {
                Console.WriteLine(e.ToString());
                return;
            }

            p1 = players[0];
            p2 = players[1];

            PlayGame();

            return;
        }

        private static bool IsValidGame(Player[] players) {
            try {
                //add more in the future
                return (players.Length == 2);
            } catch { 
                throw; 
            }
        }
        private static void PlayGame() {
            throw new NotImplementedException();
        }


    }
}
