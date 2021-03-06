﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrownAndAnchorGame;
using NSubstitute;
using TestStack.BDDfy;
using Xunit;

namespace Tests
{
    public class Bug01PayoutOnSingleMatch
    {
        [Fact]
        public void BroadTestForUserPayoutOnSingleMatch()
        {
            var die1 = Substitute.For<IDice>();
            var die2 = Substitute.For<IDice>();
            var die3 = Substitute.For<IDice>();

            die1.roll().Returns(DiceValue.ANCHOR);
            die2.roll().Returns(DiceValue.HEART);
            die3.roll().Returns(DiceValue.HEART);

            DiceValue pick = DiceValue.ANCHOR;
            int bet = 10;
            int winnings = 0;
            int funds = 100;

            var player = Substitute.For<Player>("Test", funds);

            var game = new Game(die1, die2, die3);

            winnings = game.playRound(player, pick, bet);

            Assert.Equal(bet, winnings);
            Assert.Equal(funds + bet, player.Balance);
        }

        [Fact]
        public void SideEffectBetIsDeductedDuringGame()
        {
            var die1 = Substitute.For<Dice>();
            var die2 = Substitute.For<Dice>();
            var die3 = Substitute.For<Dice>();

            DiceValue pick = DiceValue.ANCHOR;
            int bet = 10;
            int winnings = 0;
            int funds = 100;

            var player = Substitute.For<Player>("Test", funds);

            var game = new Game(die1, die2, die3);

            winnings = game.playRound(player, pick, bet);

            // Make sure at some point that the bet is deducted by the funds.
            player.Received().takeBet(bet);
        }

        [Fact]
        public void SideEffectFundsAreIncrementedByWinningsDuringGame()
        {
            var die1 = Substitute.For<Dice>();
            var die2 = Substitute.For<Dice>();
            var die3 = Substitute.For<Dice>();

            DiceValue pick = DiceValue.ANCHOR;
            int bet = 10;
            int winnings = 0;
            int funds = 100;

            var player = Substitute.For<Player>("Test", funds);

            var game = new Game(die1, die2, die3);

            winnings = game.playRound(player, pick, bet);

            // Make sure at some point that the winnings are added to the funds during the game.
            player.Received().receiveWinnings(winnings);
        }

        [Fact]
        public void BugFixForReturningStakeWhenWon()
        {
            var die1 = Substitute.For<Dice>();
            var die2 = Substitute.For<Dice>();
            var die3 = Substitute.For<Dice>();

            die1.currentValue = DiceValue.ANCHOR;
            die2.currentValue = DiceValue.HEART;
            die3.currentValue = DiceValue.HEART;

            DiceValue pick = DiceValue.ANCHOR;
            int bet = 10;
            int winnings = 0;
            int funds = 100;

            var player = Substitute.For<Player>("Test", funds);

            var game = new Game(die1, die2, die3);

            winnings = game.playRound(player, pick, bet);

            player.Received().returnBet(Arg.Is(bet));            

            Assert.Equal(bet, winnings);
            Assert.Equal(funds + bet, player.Balance);
        }
    }
}
