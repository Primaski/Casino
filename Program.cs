using Casino.Core;
using System;
using System.IO;
using static Casino.Core.Defs;

namespace Casino {
    public class Program {
        public static void Main(string[] args) {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            Console.WriteLine("Please state Player 1's name.");
            string p1Name = Console.ReadLine();
            Console.WriteLine("Please state Player 2's name.");
            string p2Name = Console.ReadLine();
            p1Name = p1Name.Length >= 20 ? p1Name.Substring(0, 20) + "..." : p1Name;
            p2Name = p2Name.Length >= 20 ? p2Name.Substring(0, 20) + "..." : p2Name;
            try {
                Deck deck = new Deck();
                Player p1 = new Player(p1Name);
                Player p2 = new Player(p2Name);
                Game game = new Game(new Player[]{ p1, p2 }, deck);
            }catch(Exception e) {
                Console.WriteLine(e.ToString());
            }

        }
    }
}
