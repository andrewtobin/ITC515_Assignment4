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
    public class Bug03OddsInGameIncorrect
    {
        [Fact]
        public void BroadTestForPlayerCanReachBettingLimit()
        {
            var die1 = Substitute.For<Dice>();
            var die2 = Substitute.For<Dice>();
            var die3 = Substitute.For<Dice>();

            var gamesPlayed = 0;
            var gamesWon = 0;


            for (int i = 0; i < 100; i++)
            {
                int bet = 5;
                int winnings = 0;
                int funds = 100;

                int winCount = 0;
                int loseCount = 0;

                var player = new Player("Test", funds) {Limit = 0};
                var game = new Game(die1, die2, die3);

                var pick = Dice.RandomValue;

                Program.PlayGame(bet, game, player, ref pick, i, ref winCount, ref loseCount);

                gamesPlayed += (winCount + loseCount);
                gamesWon += winCount;
            }

            // Make sure the games come in close to 0.42.
            Assert.InRange(gamesWon/(double) gamesPlayed, 0.41, 0.43);
        }
    }
}
