using Casino.Core;
using Casino.Core.Error;
using System;
using System.Collections.Generic;
using System.IO;
using static Casino.Core.Defs;

namespace Casino {
    public class Program {
        public static void Main(string[] args) {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Play game in debug mode? (y/n)");
            DEBUG_MODE = (Console.ReadLine() == "y") ? true : false; 
            
            Console.WriteLine("What is Player 1's name?");
            string p1Name = Console.ReadLine();
            Console.WriteLine("What is Player 2's name?");
            string p2Name = Console.ReadLine();
            p1Name = p1Name == "" ? "Player 1" : p1Name;
            p2Name = p2Name == "" ? "Player 2" : p2Name;
            p1Name = p1Name.Length >= 20 ? p1Name.Substring(0, 20) + "..." : p1Name;
            p2Name = p2Name.Length >= 20 ? p2Name.Substring(0, 20) + "..." : p2Name;
            p1Name = char.ToUpper(p1Name[0]) + p1Name.Substring(1);
            p2Name = char.ToUpper(p2Name[0]) + p2Name.Substring(1);
            try {
                Game.NewGame(new string[] { p1Name, p2Name });
            }catch(Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
