namespace CribbageUnitTests
{
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using Cribbage.Reporting;
    using FluentAssertions;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class CribbageGameTests
    {
        [TestMethod]
        public void CribbageGameTest()
        {
            var player1Hand = new MockPlayerHand(
                new Card[] { new(Rank.Four, Suit.Spades), new(Rank.Five, Suit.Clubs), new(Rank.Five, Suit.Hearts), new(Rank.Six, Suit.Spades) },
                new Card[] { new(Rank.Six, Suit.Clubs), new(Rank.Ace, Suit.Diamonds) });

            var player2Hand = new MockPlayerHand(
                new Card[] { new(Rank.Seven, Suit.Clubs), new(Rank.Seven, Suit.Hearts), new(Rank.Eight, Suit.Diamonds), new(Rank.Eight, Suit.Spades) },
                new Card[] { new(Rank.Two, Suit.Clubs), new(Rank.Jack, Suit.Clubs) });

            var player1HandFactory = new Mock<IPlayerHandFactory>();
            player1HandFactory.Setup(x => x.CreatePlayerHand()).Returns(player1Hand);
            var player1 = new Player("player1", player1HandFactory.Object);

            var player2HandFactory = new Mock<IPlayerHandFactory>();
            player2HandFactory.Setup(x => x.CreatePlayerHand()).Returns(player2Hand);
            var player2 = new Player("player2", player2HandFactory.Object);

            var starter = new Card(Rank.Six, Suit.Clubs);
            var deck = new MockDeck(starter);
            var deckFactory = new Mock<IDeckFactory>();
            deckFactory.Setup(x => x.CreateDeck()).Returns(deck);

            var reporter = new GameReporter();
            var game = new CribbageGame(player1, player2, deckFactory.Object);
            game.Play(CribbageGame.FirstDealerChoice.SecondPlayer, reporter);

            game.WinningPlayer.Should().NotBeNull();
            game.WinningPlayer!.Name.Should().Be("player1");
            game.NumHands.Should().Be(5);
            player1.Score.Should().Be(135);
            player2.Score.Should().Be(113);
        }

        [TestMethod]
        public void CribbageGameCutForDeal()
        {
            int drawCardNum = 1;
            var mockDeck = new Mock<IDeck>();

            // first time will be a draw, so cut again!
            mockDeck.Setup(x => x.DealRandomCard()).Returns(() =>
                {
                    Card c = drawCardNum switch
                    {
                        1 => new Card(Rank.Eight, (Suit)drawCardNum),
                        2 => new Card(Rank.Eight, (Suit)drawCardNum),
                        3 => new Card(Rank.Ace, Suit.Hearts),
                        4 => new Card(Rank.Jack, Suit.Diamonds),
                        _ => throw new InternalTestFailureException("The second cut for deal should end the cutting")
                    };

                    drawCardNum++;
                    return c;
                });

            var mockDeckFactory = new Mock<IDeckFactory>();
            mockDeckFactory.Setup(x => x.CreateDeck()).Returns(mockDeck.Object);

            var player1 = new Mock<IPlayer>();
            player1.SetupGet(x => x.Name).Returns("P1");
            player1.Setup(x => x.ResetHand()).Throws(new GameWinningSignalException(player1.Object));

            var player2 = new Mock<IPlayer>();
            player2.SetupGet(x => x.Name).Returns("P1");
            player2.Setup(x => x.ResetHand()).Throws(new GameWinningSignalException(player2.Object));

            var gameReporterMock = new Mock<IGameReporter>(MockBehavior.Loose);
            var game = new CribbageGame(player1.Object, player2.Object, mockDeckFactory.Object);
            game.Play(CribbageGame.FirstDealerChoice.CutForDeal, gameReporterMock.Object);

            gameReporterMock.Verify(x => x.ReportCutForDealDraw(Rank.Eight), Times.Once);
            gameReporterMock.Verify(x => x.ReportCutForDealDraw(It.IsAny<Rank>()), Times.Once);
            gameReporterMock.Verify(x => x.ReportCutForDeal(player1.Object, Rank.Ace, Rank.Jack), Times.Once);
            gameReporterMock.Verify(x => x.ReportCutForDeal(It.IsAny<IPlayer>(), It.IsAny<Rank>(), It.IsAny<Rank>()), Times.Once);

            // winning signal should have happened right away
            game.NumHands.Should().Be(1);
            mockDeckFactory.Verify(x => x.CreateDeck(), Times.AtLeast(3)); // 2 for cut, then at least one more for the game
        }

        private class MockPlayerHand : IPlayerHand
        {
            private readonly Card[] hand;
            private readonly Card[] crib;
            private int playNum;

            public MockPlayerHand(Card[] hand, Card[] crib)
            {
                this.hand = hand;
                this.crib = crib;
            }

            public void AddDealtCards(params Card[] dealtCards)
            {
                // Ignore
            }

            public void AddReturnCardsAfterPlay(IEnumerable<Card> returnedCards)
            {
                // Ignore
            }

            public IEnumerable<Card> SendCardsToCrib()
            {
                this.playNum = 0;
                return this.crib;
            }

            public Card? PlayCard(int runningCount)
            {
                if (playNum < 4)
                {
                    Card c = this.hand[playNum];
                    if (runningCount + c.Value <= 31)
                    {
                        this.playNum++;
                        return c;
                    }
                }

                return null;
            }

            public IReadOnlyList<Card> Cards => this.hand;
        }

        private class MockDeck : IDeck
        {
            private readonly Card starter;

            public MockDeck(Card starter)
            {
                this.starter = starter;
            }

            public Card DealRandomCard()
            {
                // Only the starter matters. The other dealt cards are ignored.
                return this.starter;
            }

            // This is not used so ok to do anything
            public IReadOnlyList<Card> Cards => throw new NotSupportedException();
        }
    }
}
