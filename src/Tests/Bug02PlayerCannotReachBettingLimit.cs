using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrownAndAnchorGame;
using NSubstitute;
using Xunit;

namespace Tests
{
    public class Bug02PlayerCannotReachBettingLimit
    {
        [Fact]
        public void BroadTestForPlayerCanReachBettingLimit()
        {
            var die1 = Substitute.For<Dice>();
            var die2 = Substitute.For<Dice>();
            var die3 = Substitute.For<Dice>();

            die1.currentValue = DiceValue.ANCHOR;
            die2.currentValue = DiceValue.HEART;
            die3.currentValue = DiceValue.HEART;

            DiceValue pick = DiceValue.CLUB;
            int bet = 5;
            int winnings = 0;
            int funds = 100;

            int winCount = 0;
            int loseCount = 0;

            var player = new Player("Test", funds) { Limit = 0 };
            var game = new Game(die1, die2, die3);

            Program.PlayGame(bet, game, player, ref pick, 1, ref winCount,  ref loseCount);

            Assert.Equal(0, player.Balance);
        }
    }
}
