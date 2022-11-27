namespace Cribbage
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public interface IDeck
    {
        Card DealRandomCard();
        ReadOnlyCollection<Card> Cards { get; }
    }

    public interface IDeckFactory
    {
        IDeck CreateDeck();
    }

    public class Deck : IDeck
    {
        private readonly List<Card> cards;
        private readonly Random random = new(DateTime.UtcNow.Microsecond);

        public Deck()
        {
            this.cards = new List<Card>(capacity: 52);

            foreach (Suit s in Enum.GetValues<Suit>())
            {
                foreach (Rank r in Enum.GetValues<Rank>())
                {
                    this.cards.Add(new Card(r, s));
                }
            }
        }

        public Card DealRandomCard()
        {
            int cardIndex = random.Next(this.cards.Count);
            var randomCard = this.cards[cardIndex];
            this.cards.RemoveAt(cardIndex);

            return randomCard;
        }

        public ReadOnlyCollection<Card> Cards => this.cards.AsReadOnly();
    }

    public class DeckFactory : IDeckFactory
    {
        public IDeck CreateDeck()
        {
            return new Deck();
        }
    }
}