namespace CribbageUnitTests
{
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class DeckTests  
    {
        [TestMethod]
        public void DealRandomHand()
        {
            var deck = new Deck();

            var hand = new List<Card>(capacity: 6);
            for (int i = 0; i < 6; i++)
            {
                var rCard = deck.DealRandomCard();
                hand.Add(rCard);
            }

            deck.Cards.Count.Should().Be(52 - 6);

            var dealtCard = hand[2];
            foreach (Card deckCard in deck.Cards)
            {
                deckCard.Should().NotBe(dealtCard);
            }
        }
    }
}