namespace Cribbage
{
    using System;

    public readonly struct Card : IComparable<Card>
    {
        public Card(Rank rank, Suit suit)
        {
            if (!Enum.IsDefined(rank))
                throw new ArgumentOutOfRangeException(nameof(rank), rank, "Rank parameter is not valid");

            if (!Enum.IsDefined(suit))
                throw new ArgumentOutOfRangeException(nameof(suit), suit, "Suit parameter is not valid");

            this.Rank = rank;
            this.Suit = suit;
            this.Value = ConvertRankToValue(rank);
        }

        public static int ConvertRankToValue(Rank rank)
        {
            if (!Enum.IsDefined(rank))
                throw new ArgumentOutOfRangeException(nameof(rank), rank, "Rank parameter is not valid");

            int value = (int)rank;
            if (value > 10)
                value = 10;

            return value;
        }

        public Suit Suit { get; }

        public Rank Rank { get; }

        public int Value { get; }

        /// <summary>
        /// Sort first by rank and then by suit.
        /// </summary>
        public int CompareTo(Card other)
        {
            int order = this.Rank.CompareTo(other.Rank);
            if (order == 0)
            {
                order = this.Suit.CompareTo(other.Suit);
            }

            return order;
        }

        public override string ToString()
        {
            return $"{this.Rank}:{this.Suit}";
        }
    }

    public readonly struct PlayedCard
    {
        public PlayedCard(Card card, bool isDealer)
        {
            this.Card = card;
            this.IsDealer = isDealer;
        }

        public Card Card { get; }
        public bool IsDealer { get; }
    }

    public enum Suit
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }

    public enum Rank
    {
        Ace = 1,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }
}