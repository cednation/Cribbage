namespace CribbageUnitTests
{
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Cribbage.Reporting;
    using FluentAssertions;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class CribbageHandTests
    {
        [TestMethod]
        public void HandTest1()
        {
            var poneHand = new List<Card> { new(Rank.Ace, Suit.Clubs), new(Rank.Two, Suit.Diamonds), new(Rank.Three, Suit.Spades), new(Rank.Four, Suit.Diamonds) };
            var dealerHand = new List<Card> { new(Rank.Five, Suit.Clubs), new(Rank.Five, Suit.Hearts), new(Rank.Three, Suit.Diamonds), new(Rank.Seven, Suit.Diamonds) };
            var crib = new List<Card> { new(Rank.Six, Suit.Diamonds), new(Rank.Nine, Suit.Clubs), new(Rank.Queen, Suit.Clubs), new(Rank.King, Suit.Clubs) };
            var starter = new Card(Rank.Jack, Suit.Clubs);

            var mockDeck = new MockDeck(poneHand, dealerHand, crib, starter);
            var mockDealerHand = new MockPlayerHand();
            var mockPoneHand = new MockPlayerHand();

            var mockDealerFactory = new Mock<IPlayerHandFactory>();
            mockDealerFactory.Setup(x => x.CreatePlayerHand()).Returns(mockDealerHand);
            var dealer = new Player("dealer", mockDealerFactory.Object);

            var mockPoneFactory = new Mock<IPlayerHandFactory>();
            mockPoneFactory.Setup(x => x.CreatePlayerHand()).Returns(mockPoneHand);
            var pone = new Player("pone", mockPoneFactory.Object);

            var handReporter = new HandReporter(new ThePlayReporter());
            var hand = new CribbageHand(dealer, pone, mockDeck);
            hand.Run(handReporter);

            mockDealerHand.Cards.Select(c => c.Value).Should().BeEquivalentTo(new[] { 5, 5, 3, 7 });
            hand.Crib.Select(c => (int)c.Rank).Should().BeEquivalentTo(new[] { 6, 9, 12, 13 });
            hand.Starter.Should().BeEquivalentTo(new Card(Rank.Jack, Suit.Clubs));

            dealer.Score.Should().Be(20);
            pone.Score.Should().Be(8);
        }

        /// <summary>
        /// Mock deck that deals out the cards - pone, dealer, pone crib cards, dealer crib cards.
        /// </summary>
        private class MockDeck : IDeck
        {
            private readonly IList<Card> poneCards;
            private readonly IList<Card> dealerCards;
            private readonly IList<Card> cribCards;
            private readonly Card starter;

            private int cardNumber;

            public MockDeck(IList<Card> poneCards, IList<Card> dealerCards, IList<Card> cribCards, Card starter)
            {
                this.poneCards = poneCards;
                this.dealerCards = dealerCards;
                this.cribCards = cribCards;
                this.starter = starter;
            }

            public Card DealRandomCard()
            {
                Debug.Assert(this.cardNumber < 13);

                Card card;
                switch (this.cardNumber)
                {
                    case < 8:
                    {
                        int cardIndex = this.cardNumber / 2;
                        bool pone = this.cardNumber % 2 == 0;

                        card = pone ? this.poneCards[cardIndex] : this.dealerCards[cardIndex];
                        break;
                    }
                    case < 12:
                    {
                        int cardIndex = this.cardNumber - 8;
                        card = this.cribCards[cardIndex];
                        break;
                    }
                    default:
                        card = this.starter;
                        break;
                }

                this.cardNumber++;
                return card;
            }

            public IReadOnlyList<Card> Cards => poneCards.Concat(dealerCards).Concat(cribCards).ToList().AsReadOnly();
        }

        private class MockPlayerHand : IPlayerHand
        {
            private readonly List<Card> handCards = new();

            public void AddDealtCards(params Card[] dealtCards)
            {
                this.handCards.AddRange(dealtCards);
            }

            public void AddReturnCardsAfterPlay(IEnumerable<Card> returnedCards)
            {
                this.handCards.AddRange(returnedCards);
            }

            public IEnumerable<Card> SendCardsToCrib()
            {
                var toCrib = this.handCards.Skip(4).ToList();
                this.handCards.RemoveRange(4, 2);

                return toCrib;
            }

            public Card? PlayCard(int runningCount)
            {
                Card? curCard = null;
                if (this.handCards.Count > 0)
                {
                    if (this.handCards[0].Value + runningCount <= 31)
                    {
                        curCard = this.handCards[0];
                        this.handCards.RemoveAt(0);
                    }
                }

                return curCard;
            }

            public IReadOnlyList<Card> Cards => this.handCards.AsReadOnly();
        }
    }
}
