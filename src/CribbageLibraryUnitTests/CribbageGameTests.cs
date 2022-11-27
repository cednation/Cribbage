namespace CribbageUnitTests
{
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
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

            var game = new CribbageGame(player1, player2, deckFactory.Object);
            game.Play();

            game.winningPlayer.Should().NotBeNull();
            game.winningPlayer!.Name.Should().Be("player1");
            game.NumHands.Should().Be(5);
            player1.Score.Should().Be(135);
            player2.Score.Should().Be(113);
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

            public void AddDealtCards(params Card[] cards)
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
            public ReadOnlyCollection<Card> Cards => throw new NotSupportedException();
        }
    }
}
