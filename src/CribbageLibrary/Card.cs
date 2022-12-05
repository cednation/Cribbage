namespace Cribbage
{
    using System;
    using System.ComponentModel;

    public readonly struct Card : IComparable<Card>, IEquatable<Card>
    {
        public Card(Rank rank, Suit suit)
        {
            if (!Enum.IsDefined(rank))
                throw new InvalidEnumArgumentException(nameof(rank), (int)rank, typeof(Rank));

            if (!Enum.IsDefined(suit))
                throw new InvalidEnumArgumentException(nameof(suit), (int)suit, typeof(Suit));

            this.Rank = rank;
            this.Suit = suit;
            this.Value = ConvertRankToValue(rank);
        }

        public static int ConvertRankToValue(Rank rank)
        {
            if (!Enum.IsDefined(rank))
                throw new InvalidEnumArgumentException(nameof(rank), (int)rank, typeof(Rank));

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

        #region Equals
        public static bool operator ==(Card c1, Card c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(Card c1, Card c2)
        {
            return !(c1 == c2);
        }

        public bool Equals(Card other)
        {
            return Suit == other.Suit && Rank == other.Rank;
        }

        public override bool Equals(object? obj)
        {
            return obj is Card other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Suit, (int)Rank);
        }
        #endregion
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