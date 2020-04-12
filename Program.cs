using Casino.Core;
using System;
using System.IO;
using static Casino.Core.Defs;

namespace Casino {
    public class Program {
        public static void Main(string[] args) {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            new Deck().CheckForDuplicates();

            /*
            Console.WriteLine("Please state Player 1's name.");
            string p1Name = Console.ReadLine();
            Console.WriteLine("Please state Player 2's name.");
            string p2Name = Console.ReadLine();

            Player p1 = new Player(p1Name);
            Player p2 = new Player(p2Name);
            Deck deck = new Deck();
            Game game = new Game(new Player[] { p1, p2 }, deck);
            */
        }
    }
}
