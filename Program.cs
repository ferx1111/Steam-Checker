using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SteamChecker
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Console.Title = "Steam Checker";

            string channels;

            while (true)
            {
                Console.Write("Enter SteamID64 or Normal ID : ");
                channels = Console.ReadLine();

                ShowResults(channels);
            }
        }

        public static void ShowResults(string Accounts)
        {
            if (Accounts == "!clear")
            {
                Console.Clear();
                return;
            }

            if (Accounts.Contains("!loc"))
            {
                try
                {
                    string[] lines = File.ReadAllLines(Accounts.Substring(5));

                    foreach (string acc in lines)
                    {
                        Console.WriteLine();
                        Engine.ShowInformation(new Steam(acc));
                        Console.WriteLine();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.Message);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine();
                    return;
                }

                return;
            }

            string temp = "";

            foreach (char letter in Accounts)
            {
                if (letter != ' ')
                {
                    temp = temp + letter;
                }
                else
                {
                    Console.WriteLine();
                    Engine.ShowInformation(new Steam(temp));
                    Console.WriteLine();

                    temp = "";
                }
            }

            if (temp != string.Empty)
            {
                Console.WriteLine();
                Engine.ShowInformation(new Steam(temp));
                Console.WriteLine();
            }
        }
    }
}
