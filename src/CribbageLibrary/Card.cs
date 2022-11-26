namespace Cribbage
{
    using System;

    public readonly struct Card
    {
        public Card(Rank rank, Suit suit)
        {
            if (!Enum.IsDefined(rank))
                throw new ArgumentOutOfRangeException(nameof(rank), rank, "Rank parameter is not valid");

            if (!Enum.IsDefined(suit))
                throw new ArgumentOutOfRangeException(nameof(suit), suit, "Suit parameter is not valid");

            this.Rank = rank;
            this.Suit = suit;
            this.Value = CardFacts.ConvertRankToValue(rank);
        }

        public Suit Suit { get; }

        public Rank Rank { get; }

        public int Value { get; }
    }

    public static class CardFacts
    {
        public static bool IsFaceCard(Rank rank)
        {
            if (!Enum.IsDefined(rank))
                throw new ArgumentOutOfRangeException(nameof(rank), rank, "Rank parameter is not valid");

            int value = (int)rank;
            return value > 10;
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