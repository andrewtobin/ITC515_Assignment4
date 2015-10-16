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

                Player player = new Player("Fred", 100);
                Console.WriteLine(player);
                Console.WriteLine();

                Console.WriteLine("New game for {0}", player.Name);
                Game game = new Game(d1, d2, d3);
                IList<DiceValue> currentDiceValues = game.CurrentDiceValues;
                Console.WriteLine("Current dice values : {0} {1} {2}", currentDiceValues[0], currentDiceValues[1], currentDiceValues[2]);

                int bet = 5;
                player.Limit = 0;
                int winnings = 0;
                DiceValue pick = Dice.RandomValue;

                int totalWins = 0;
                int totalLosses = 0;

                while (true)
                {
                    Play100Games(bet, game, ref pick, ref totalWins, ref totalLosses);

                    string ans = Console.ReadLine();
                    if (ans.Equals("q")) break;
                } //while true

                Log.Information("Overall win rate = {Rate}%", (float)(totalWins * 100) / (totalWins + totalLosses));
                Console.WriteLine("Overall win rate = {0}%", (float)(totalWins * 100) / (totalWins + totalLosses));
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Quit Application by force\n\n\n\n");
            }
        }

        internal static void Play100Games(int bet, Game game, ref DiceValue pick, ref int totalWins, ref int totalLosses)
        {
            int winCount = 0;
            int loseCount = 0;

            var player = new Player("Fred", 100);

            for (int i = 0; i < 100; i++)
            {
                PlayGame(bet, game, player, ref pick, i, ref winCount, ref loseCount);
            } //for


            Log.Information("Win count = {Wins}, Lose Count = {Losses}, {Rate}", winCount, loseCount,
                (float)winCount / (winCount + loseCount));
            Console.WriteLine("Win count = {0}, Lose Count = {1}, {2:0.00}", winCount, loseCount,
                (float) winCount/(winCount + loseCount));

            totalWins += winCount;
            totalLosses += loseCount;
        }

        internal static void PlayGame(int bet, Game game, Player player, ref DiceValue pick, int currentGame, ref int winCount, ref int loseCount)
        {
            Console.Write("Start Game {0}: ", currentGame);
            Console.WriteLine("{0} starts with balance {1}", player.Name, player.Balance);

            int turn = 0;

            while (player.balanceExceedsLimitBy(bet) && player.Balance < 200)
            {
                try
                {
                    PlayRound(bet, game, player, pick, ref winCount, ref loseCount);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("{0}\n\n", e.Message);
                }

                pick = Dice.RandomValue;
                turn++;
            } //while

            Console.Write("{1} turns later.\nEnd Game {0}: ", turn, currentGame);
            Console.WriteLine("{0} now has balance {1}\n", player.Name, player.Balance);
        }

        internal static void PlayRound(int bet, Game game, Player player, DiceValue pick, ref int winCount, ref int loseCount)
        {
            var winnings = game.playRound(player, pick, bet);
            var currentDiceValues = game.CurrentDiceValues;

            Console.WriteLine("Rolled {0} {1} {2}", currentDiceValues[0], currentDiceValues[1], currentDiceValues[2]);
            if (winnings > 0)
            {
                Console.WriteLine("{0} won {1} balance now {2}", player.Name, winnings, player.Balance);
                winCount++;
            }
            else
            {
                Console.WriteLine("{0} lost {1} balance now {2}", player.Name, bet, player.Balance);
                loseCount++;
            }
        }
    }
}
