using Casino.Core;
using System;
using System.IO;

namespace Casino {
    public class Program {
        public static void Main(string[] args) {
            string[] arr = { "2", "3", "56", null, "q", "c", null };
            string[] arr2 = { "2", "3", "56", "s", "q", "c", "d" };
            string[] arr3 = { null, null, null };
            string[] arr4 = { };
            string[] arr5 = null;
                Console.WriteLine("1: null at pos 4 and 7:");
            try {
                Console.WriteLine(Util.FindFirstNullIndex(arr));
            }catch(Exception e) {
                Console.WriteLine(e.Message);
            }
                Console.WriteLine("2: no nulls");
            try {
                Console.WriteLine(Util.FindFirstNullIndex(arr2));
            } catch (Exception e) {
                Console.WriteLine(e.Message);
             }
            Console.WriteLine("3: all nulls");
            try {
                Console.WriteLine(Util.FindFirstNullIndex(arr3));
            } catch (Exception e) {
                Console.WriteLine(e.Message);
             }
            Console.WriteLine("4: empty array");
            try {
                Console.WriteLine(Util.FindFirstNullIndex(arr4));
            } catch (Exception e) {
                Console.WriteLine(e.Message);
             }
            Console.WriteLine("5: null array");
            try {
                Console.WriteLine(Util.FindFirstNullIndex(arr5));
            } catch (Exception e) {
                Console.WriteLine(e.Message);
             }

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
