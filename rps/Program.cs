using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace rps
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 1 && new Validator(args).Validate())
            {
                RpsGame game = new RpsGame(args);
                game.Move();
            }
            else if(args.Length<=1)
            {
                Console.WriteLine("Invalid run. Count of words need to be >=3");
            }
        }
    }

    class RpsGame
    {
        private readonly List<string> GameItems;
        private readonly Random indexPcMove = new Random();
        public RpsGame(string[] args)
        {
            this.GameItems = args.ToList();
        }

        public void Move()
        {
            Hmac hmac = new Hmac();
            var idPC = indexPcMove.Next(0, GameItems.Count);
            var wordPC = GameItems[idPC];
            hmac.PrintHash(wordPC);
            ShowMenu();
            string choose = Console.ReadLine();
            var id = GetFigureId(choose);
            if (id >= 0)
            {
                Console.WriteLine("Your move:" + GameItems[id]);
                Console.WriteLine("Computer move:" + GameItems[idPC]);
                ChooseWinner(id, idPC);
                hmac.PrintKey();
            }
            else
            {
                Console.WriteLine("Invalid input");
                Move();
            }
        }

        private void ChooseWinner(int idPlayer, int idPC)
        {
            int hight = idPC + (GameItems.Count - 1) / 2 >= GameItems.Count ? GameItems.Count - 1 : idPC + (GameItems.Count - 1) / 2;
            int down = idPC + (GameItems.Count - 1) / 2 >= GameItems.Count ? (idPC + (GameItems.Count - 1) / 2) % GameItems.Count : -1;

            if (idPlayer == idPC) Console.WriteLine("Draw");
            else if ((idPlayer > idPC && idPlayer <= hight) || (idPlayer <= down)) Console.WriteLine("You win!");
            else Console.WriteLine("You lose!");
        }

        private int GetFigureId(string id)
        {
            if (id == "0")
                Environment.Exit(0);
            int idWord = 0;
            if (int.TryParse(id, out idWord) && idWord <= GameItems.Count)
            {
                return idWord - 1;
            }

            return -1;
        }

        private void ShowMenu()
        {
            Console.WriteLine("Available moves:");
            foreach (var i in GameItems)
            {
                Console.WriteLine(GameItems.IndexOf(i) + 1 + " - " + i);
            }
            Console.WriteLine("0 - exit");
            Console.WriteLine("Enter your move:");
        }
    }

    class Hmac
    {
        private readonly HMACSHA256 hmac;
        public Hmac()
        {
            hmac = new HMACSHA256(GenerateKey(new byte[16]));
        }

        public void PrintHash(string message)
        {
            Console.WriteLine("\nHMAC: " + BitConverter.ToString(this.hmac.ComputeHash(Encoding.UTF8.GetBytes(message))).Replace("-", string.Empty));
        }

        public void PrintKey()
        {
            Console.WriteLine("HMAC key: " + BitConverter.ToString(this.hmac.Key).Replace("-", string.Empty));
        }

        private byte[] GenerateKey(byte[] bytes)
        {
            RandomNumberGenerator.Create().GetBytes(bytes);
            return bytes;
        }
    }
    class Validator
    {
        private bool isValid = true;
        private readonly List<string> words;

        public Validator(string[] args)
        {
            words = args.ToList();
        }

        public bool Validate()
        {
            if (words.Count != words.Distinct().ToList().Count)
            {
                isValid = false;
                Console.WriteLine("All words needs to be unique! example: rock paper scissors lizard Spock");
            }
            if (words.Count % 2 == 0)
            {
                isValid = false;
                Console.WriteLine("Count of words need to be odd! (>=3 mod 2 == 1)");
            }
            return isValid;
        }
    }
}
