namespace Cribbage
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class Deck
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
}