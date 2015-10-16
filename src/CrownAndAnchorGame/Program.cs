using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace CrownAndAnchorGame
{
    class Program
    {
        private static void Main(string[] args)
        {
            // Init a new instance of logger for logging to text file and console.
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
#if DEBUG
                .WriteTo.ColoredConsole()
#endif
                .WriteTo.RollingFile(@"Log-{Date}.txt")
                .CreateLogger();

            Log.Information(
                $"----------------------------------------------\nStarting new instance at {DateTime.Now.ToString()}\n----------------------------------------------\n\n");

            try
            {

                Dice d1 = new Dice();
                Dice d2 = new Dice();
                Dice d3 = new Dice();

                Player p = new Player("Fred", 100);
                Console.WriteLine(p);
                Console.WriteLine();

                Console.WriteLine("New game for {0}", p.Name);
                Game g = new Game(d1, d2, d3);
                IList<DiceValue> cdv = g.CurrentDiceValues;
                Console.WriteLine("Current dice values : {0} {1} {2}", cdv[0], cdv[1], cdv[2]);

                DiceValue rv = Dice.RandomValue;

                Random random = new Random();
                int bet = 5;
                p.Limit = 0;
                int winnings = 0;
                DiceValue pick = Dice.RandomValue;

                int totalWins = 0;
                int totalLosses = 0;

                while (true)
                {
                    if (Play100Games(bet, g, ref pick, ref totalWins, ref totalLosses)) break;
                } //while true
                Console.WriteLine("Overall win rate = {0}%", (float)(totalWins * 100) / (totalWins + totalLosses));
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Quit Application by force\n\n\n\n");
            }
        }

        internal static bool Play100Games(int bet, Game g, ref DiceValue pick, ref int totalWins, ref int totalLosses)
        {
            int winCount = 0;
            int loseCount = 0;
            for (int i = 0; i < 100; i++)
            {
                winCount = PlayGame(bet, g, ref pick, i, winCount, ref loseCount);
            } //for

            Console.WriteLine("Win count = {0}, Lose Count = {1}, {2:0.00}", winCount, loseCount,
                (float) winCount/(winCount + loseCount));
            totalWins += winCount;
            totalLosses += loseCount;

            string ans = Console.ReadLine();
            if (ans.Equals("q")) return true;
            return false;
        }

        private static int PlayGame(int bet, Game g, ref DiceValue pick, int i, int winCount, ref int loseCount)
        {
            Player p;
            p = new Player("Fred", 100);
            Console.Write("Start Game {0}: ", i);
            Console.WriteLine("{0} starts with balance {1}", p.Name, p.Balance);
            int turn = 0;
            while (p.balanceExceedsLimitBy(bet) && p.Balance < 200)
            {
                int winnings;
                try
                {
                    winCount = PlayRound(bet, g, pick, winCount, ref loseCount, p);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("{0}\n\n", e.Message);
                }
                pick = Dice.RandomValue;
                winnings = 0;
                turn++;
            } //while

            Console.Write("{1} turns later.\nEnd Game {0}: ", turn, i);
            Console.WriteLine("{0} now has balance {1}\n", p.Name, p.Balance);
            return winCount;
        }

        private static int PlayRound(int bet, Game g, DiceValue pick, int winCount, ref int loseCount, Player p)
        {
            var winnings = g.playRound(p, pick, bet);
            var cdv = g.CurrentDiceValues;

            Console.WriteLine("Rolled {0} {1} {2}", cdv[0], cdv[1], cdv[2]);
            if (winnings > 0)
            {
                Console.WriteLine("{0} won {1} balance now {2}", p.Name, winnings, p.Balance);
                winCount++;
            }
            else
            {
                Console.WriteLine("{0} lost {1} balance now {2}", p.Name, bet, p.Balance);
                loseCount++;
            }
            return winCount;
        }
    }
}
