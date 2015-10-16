using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Context;
using Serilog.Core.Enrichers;

namespace CrownAndAnchorGame
{
    public class Game
    {
        internal readonly List<IDice> dice;

        internal Dictionary<DiceValue, int> RollCount = new Dictionary<DiceValue, int>();
        internal Dictionary<DiceValue, int> PickCount = new Dictionary<DiceValue, int>();


        public IList<DiceValue> CurrentDiceValues
        {
            get { return dice.Select(d => d.CurrentValue).ToList().AsReadOnly(); }
        }


        public Game(IDice die1, IDice die2, IDice die3)
        {
            dice = new List<IDice>();
            dice.Add(die1);
            dice.Add(die2);
            dice.Add(die3);
        }

        public int playRound(Player player, DiceValue pick, int bet)
        {
            using (LogContext.PushProperties(new PropertyEnricher("Player", player, true)))
            {
                Log.Information("Player {Name} has bet {Bet} on {Pick}\tBalance: {Balance}", player.Name, bet, pick, player.Balance);
            }

            if (player == null) throw new ArgumentException("Player cannot be null");
            if (player == null) throw new ArgumentException("Pick cannot be null");
            if (bet < 0) throw new ArgumentException("Bet cannot be negative");

            if (!this.PickCount.ContainsKey(pick)) this.PickCount.Add(pick, 0);
            this.PickCount[pick]++; // Increment counter for pick count for DiceValue

            Log.Information("Deducting bet");
            player.takeBet(bet);
            Log.Information("Balance: {Balance}", player.Balance);

            int matches = 0;
            for (int i = 0; i < dice.Count; i++)
            {
                var value = dice[i].roll();
                Log.Information("Dice {Number} is a {Roll}", i, value);

                if (!this.RollCount.ContainsKey(value)) this.RollCount.Add(value, 0);
                this.RollCount[value]++; // Increment counter for roll count for DiceValue

                if (value.Equals(pick))
                {
                    matches += 1;
                    Log.Information("Match!");
                }
                else
                {
                    Log.Information("Not a Match!");
                }
            }

            int winnings = matches * bet;

            if (matches > 0)
            {
                player.receiveWinnings(winnings);
                player.returnBet(bet);
            }

            Log.Information("Winnings are {Winnings}", winnings);
            Log.Information("Player's Balance is now {Balance}", player.Balance);

            return winnings;
        }
    }
}
