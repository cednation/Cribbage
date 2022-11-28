namespace CribbageUnitTests
{
    using System.Diagnostics.CodeAnalysis;
    using Cribbage.Reporting;
    using FluentAssertions;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ThePlayTests
    {
        [TestMethod]
        public void PlayRunMultiple()
        {
            var poneHand = new ThePlayTestHand(new[]
            {
                new Card(Rank.Four, Suit.Clubs), new Card(Rank.Five, Suit.Diamonds),
                new Card(Rank.Four, Suit.Hearts), new Card(Rank.Two, Suit.Hearts)
            });
            var pone = CreateMockPlayer(poneHand, "pone");

            var dealerHand = new ThePlayTestHand(new[]
            {
                new Card(Rank.Six, Suit.Hearts), new Card(Rank.Seven, Suit.Spades),
                new Card(Rank.Three, Suit.Spades), new Card(Rank.Ace, Suit.Spades)
            });
            var dealer = CreateMockPlayer(dealerHand, "dealer");

            // Sequence: 4, 6, 5, 7, 4, 3, 2 Go! A
            // Pone score: 5->9->17
            // Dealer score: 4->9->10

            var reporter = new ThePlayReporter();
            var thePlay = new ThePlay(pone.Object, dealer.Object);
            thePlay.Run(reporter);

            pone.Object.Score.Should().Be(17);
            dealer.Object.Score.Should().Be(10);

            var playedCardsValues = thePlay.PlayedCards.Select(p => (int)p.Card.Rank);
            playedCardsValues.Should().BeEquivalentTo(new[] { 4, 6, 5, 7, 4, 3, 2, 1 });
        }

        [TestMethod]
        public void PlayTriple()
        {
            var poneHand = new ThePlayTestHand(new[]
            {
                new Card(Rank.Queen, Suit.Clubs), new Card(Rank.Queen, Suit.Hearts),
                new Card(Rank.Ten, Suit.Spades), new Card(Rank.Ten, Suit.Clubs)
            });
            var pone = CreateMockPlayer(poneHand);

            var dealerHand = new ThePlayTestHand(new []
            {
                new Card(Rank.Queen, Suit.Diamonds), new Card(Rank.Ace, Suit.Hearts), 
                new Card(Rank.Five, Suit.Diamonds), new Card(Rank.Jack, Suit.Hearts)
            });
            var dealer = CreateMockPlayer(dealerHand);

            // Sequence: Q, Q, Q, A (break) 10, 5, 10 (break) Jack
            // Pone score: 7
            // Dealer score: 7

            var thePlay = new ThePlay(pone.Object, dealer.Object);
            thePlay.Run();

            pone.Object.Score.Should().Be(7);
            dealer.Object.Score.Should().Be(7);

            var playedCardValues = thePlay.PlayedCards.Select(p => (int)p.Card.Rank);
            playedCardValues.Should().BeEquivalentTo(new[] { 12, 12, 12, 1, 10, 5, 10, 11 });
        }

        [TestMethod]
        public void PlayAllEights()
        {
            var poneHand = new ThePlayTestHand(new[]
            {
                new Card(Rank.Eight, Suit.Clubs), new Card(Rank.Eight, Suit.Hearts),
                new Card(Rank.Nine, Suit.Spades), new Card(Rank.King, Suit.Clubs)
            });
            var pone = CreateMockPlayer(poneHand);

            var dealerHand = new ThePlayTestHand(new[]
            {
                new Card(Rank.Eight, Suit.Diamonds), new Card(Rank.Eight, Suit.Hearts),
                new Card(Rank.Ten, Suit.Diamonds), new Card(Rank.King, Suit.Hearts)
            });
            var dealer = CreateMockPlayer(dealerHand);

            // Sequence: 8, 8, 8 (break) 8, 9, 10 (break) K K
            // Pone: 7
            // Dealer: 9

            var thePlay = new ThePlay(pone.Object, dealer.Object);
            thePlay.Run();

            pone.Object.Score.Should().Be(7);
            dealer.Object.Score.Should().Be(9);
        }

        [TestMethod]
        public void PlayOnYourOwnCards()
        {
            var poneHand = new ThePlayTestHand(new[]
            {
                new Card(Rank.Ten, Suit.Clubs), new Card(Rank.Ten, Suit.Hearts),
                new Card(Rank.Jack, Suit.Spades), new Card(Rank.Jack, Suit.Clubs)
            });
            var pone = CreateMockPlayer(poneHand);

            var dealerHand = new ThePlayTestHand(new[]
            {
                new Card(Rank.Four, Suit.Diamonds), new Card(Rank.Ace, Suit.Hearts),
                new Card(Rank.Three, Suit.Diamonds), new Card(Rank.Two, Suit.Hearts)
            });
            var dealer = CreateMockPlayer(dealerHand);

            // Sequence: 10, 4, 10, 1, 3, 2 (break) J J
            // Pone: 3
            // Dealer: 4

            var thePlay = new ThePlay(pone.Object, dealer.Object);
            thePlay.Run();

            pone.Object.Score.Should().Be(3);
            dealer.Object.Score.Should().Be(4);
        }

        private static Mock<IPlayer> CreateMockPlayer(IPlayerHand hand, string name = "player")
        {
            var player = new Mock<IPlayer>(MockBehavior.Strict);
            player.SetupGet(x => x.Hand).Returns(hand);
            player.SetupGet(x => x.Name).Returns(name);

            int playerScore = 0;
            player.Setup(x => x.AddScore(It.IsAny<int>())).Callback<int>(points => playerScore += points);
            player.SetupGet(x => x.Score).Returns(() => playerScore);

            return player;
        }

        private class ThePlayTestHand : IPlayerHand
        {
            private readonly IList<Card> hand;

            public ThePlayTestHand(IEnumerable<Card> hand)
            {
                this.hand = new List<Card>(hand);
            }

            public void AddDealtCards(params Card[] cards)
            {
                throw new NotImplementedException();
            }

            public void AddReturnCardsAfterPlay(IEnumerable<Card> returnedCards)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<Card> SendCardsToCrib()
            {
                throw new NotImplementedException();
            }

            public Card? PlayCard(int runningCount)
            {
                Card? curCard = null;
                if (this.hand.Count > 0)
                {
                    if (this.hand[0].Value + runningCount <= 31)
                    {
                        curCard = this.hand[0];
                        this.hand.RemoveAt(0);
                    }
                }

                return curCard;
            }

            public IReadOnlyList<Card> Cards => this.hand.AsReadOnly();
        }
    }
}
