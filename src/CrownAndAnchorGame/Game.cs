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
        internal readonly List<Dice> dice;
        internal readonly List<DiceValue> values;

        internal Dictionary<DiceValue, int> RollCount = new Dictionary<DiceValue, int>();
        internal Dictionary<DiceValue, int> PickCount = new Dictionary<DiceValue, int>();


        public IList<DiceValue> CurrentDiceValues
        {
            get { return values.AsReadOnly(); }
        }


        public Game(Dice die1, Dice die2, Dice die3)
        {
            dice = new List<Dice>();
            values = new List<DiceValue>();
            dice.Add(die1);
            dice.Add(die2);
            dice.Add(die3);

            foreach (var die in dice)
            {
                values.Add(die.CurrentValue);
            }
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
                dice[i].roll();
                Log.Information("Dice {Number} is a {Roll}", i, values[i]);

                if (!this.RollCount.ContainsKey(values[i])) this.RollCount.Add(values[i], 0);
                this.RollCount[values[i]]++; // Increment counter for roll count for DiceValue

                if (values[i].Equals(pick))
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
