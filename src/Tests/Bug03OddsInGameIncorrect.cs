using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public void BroadTestForCorrectOdds()
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

        [Fact]
        public void DiceFacesShouldAppearRandomlyAndStatisticallySimilarCounts()
        {
            var die1 = Substitute.For<Dice>();
            var die2 = Substitute.For<Dice>();
            var die3 = Substitute.For<Dice>();

            int bet = 5;

            int winCount = 0;
            int loseCount = 0;

            var pick = Dice.RandomValue;

            var game = new Game(die1, die2, die3);

            Program.Play100Games(bet, game, ref pick, ref winCount, ref loseCount);

            foreach(var face in Enum.GetValues(typeof(DiceValue)))
            {
                Assert.Contains(face, game.PickCount.Keys.Select(k => k.ToString()));
                Assert.Contains(face, game.RollCount.Keys.Select(k => k.ToString()));
            }

            // Make sure that each face has come up in 100 games.
            Assert.Equal(6, game.PickCount.Keys.Count);
            Assert.Equal(6, game.RollCount.Keys.Count);

            Assert.InRange(game.PickCount.Values.Min(), 100/6 - 5, 100/6 + 5);
            Assert.InRange(game.PickCount.Values.Max(), 100 / 6 - 5, 100 / 6 + 5);

            Assert.InRange(game.RollCount.Values.Min(), 300 / 6 - 15, 300 / 6 + 15);
            Assert.InRange(game.RollCount.Values.Max(), 300 / 6 - 15, 300 / 6 + 15);
        }

        [Fact]
        public void DiceReturnsAllSixFaces()
        {
            var faces = new HashSet<DiceValue>();

            for (int i = 0; i < 100; i++)
            {
                var face = Dice.RandomValue;
                if (!faces.Contains(face)) faces.Add(face);
            }

            Assert.Equal(6, faces.Count);
        }

        [Fact]
        public void WhenDiceRolledValueShouldReflectNewRoll()
        {
            var die1 = new Dice();
            var die2 = new Dice();
            var die3 = new Dice();

            int bet = 5;

            int winCount = 0;
            int loseCount = 0;

            var pick = Dice.RandomValue;
            var player = new Player("Test", 100);

            var game = new Game(die1, die2, die3);

            game.playRound(player, pick, bet);

            var newDie1Value = die1.roll();
            var newDie2Value = die2.roll();
            var newDie3Value = die3.roll();

            Assert.Equal(die1.CurrentValue, game.values[0]);
            Assert.Equal(die2.CurrentValue, game.values[1]);
            Assert.Equal(die3.CurrentValue, game.values[2]);

            Assert.Equal(newDie1Value, die1.CurrentValue);
            Assert.Equal(newDie2Value, die2.CurrentValue);
            Assert.Equal(newDie3Value, die3.CurrentValue);
        }

    }
}
