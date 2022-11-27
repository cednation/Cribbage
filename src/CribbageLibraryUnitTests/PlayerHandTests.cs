namespace CribbageUnitTests
{
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PlayerHandTests
    {
        [TestMethod]
        public void PlayCard31()
        {
            var card1 = new Card(Rank.Ace, Suit.Clubs);
            var card2 = new Card(Rank.Ten, Suit.Clubs);
            var card3 = new Card(Rank.Eight, Suit.Hearts);
            var card4 = new Card(Rank.Eight, Suit.Diamonds);

            var hand = new PlayerHand();
            hand.AddDealtCards(card1, card2, card3, card4);

            var playedCard = hand.PlayCard(23);
            playedCard.Should().Be(card3);
        }

        [TestMethod]
        public void PlayCardGo()
        {
            var card1 = new Card(Rank.Four, Suit.Clubs);
            var card2 = new Card(Rank.Five, Suit.Clubs);
            var card3 = new Card(Rank.Eight, Suit.Hearts);
            var card4 = new Card(Rank.Eight, Suit.Diamonds);

            var hand = new PlayerHand();
            hand.AddDealtCards(card1, card2, card3, card4);

            Card? playedCard = hand.PlayCard(29);
            playedCard.Should().BeNull();
        }

        [TestMethod]
        public void PlayCardRandom()
        {
            var card1 = new Card(Rank.Four, Suit.Clubs);
            var card2 = new Card(Rank.Five, Suit.Clubs);
            var card3 = new Card(Rank.Ten, Suit.Hearts);
            var card4 = new Card(Rank.Six, Suit.Diamonds);

            var hand = new PlayerHand();
            hand.AddDealtCards(card1, card2, card3, card4);

            Card? randomCard = hand.PlayCard(23);
            randomCard.Should().NotBeNull();
            randomCard.Should().NotBe(card3);
        }

        [TestMethod]
        public void SendCardsToCrib()
        {
            var deck = new Deck();
            var hand = new PlayerHand();

            var dealtCards = new List<Card>();

            for (int i = 0; i < 6; i++)
            {
                var dealtCard = deck.DealRandomCard();
                dealtCards.Add(dealtCard);
                hand.AddDealtCards(dealtCard);
            }

            hand.Cards.Count.Should().Be(6);

            var toCrib = hand.SendCardsToCrib();
            toCrib.Should().NotBeNull();
            toCrib.Count().Should().Be(2);
            hand.Cards.Count.Should().Be(4);

            foreach (Card cribCard in toCrib)
            {
                dealtCards.Should().Contain(cribCard);
                hand.Cards.Should().NotContain(cribCard);
            }
        }
    }
}
