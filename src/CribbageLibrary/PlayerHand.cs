namespace Cribbage
{
    public interface IPlayerHand
    {
        void AddDealtCards(params Card[] cards);
        IEnumerable<Card> SendCardsToCrib();
        Card? PlayCard(int runningCount);
        IReadOnlyList<Card> Cards { get; }
    }

    public interface IPlayerHandFactory
    {
        IPlayerHand CreatePlayerHand();
    }

    public class PlayerHand : IPlayerHand
    {
        private readonly List<Card> cards = new(capacity: 6);
        private readonly Random random = new(DateTime.UtcNow.Microsecond);

        public void AddDealtCards(params Card[] dealtCards)
        {
            this.cards.AddRange(dealtCards);
        }

        public IEnumerable<Card> SendCardsToCrib()
        {
            if (this.cards.Count != 6)
                throw new InvalidOperationException("Player does not have a full hand.");

            int firstIndex = random.Next(this.cards.Count);
            var firstCard = this.cards[firstIndex];
            this.cards.RemoveAt(firstIndex);

            int secondIndex = random.Next(this.cards.Count);
            var secondCard = this.cards[secondIndex];
            this.cards.RemoveAt(secondIndex);

            return new[] { firstCard, secondCard };
        }

        public Card? PlayCard(int runningCount)
        {
            if (runningCount > 30)
                throw new ArgumentOutOfRangeException(nameof(runningCount), runningCount, "Maximum running total is 30");

            if (this.cards.Count > 0)
            {
                var validCardIndices = new List<int>();
                for (int i = 0; i < this.cards.Count; i++)
                {
                    var card = this.cards[i];
                    switch (card.Value + runningCount)
                    {
                        case 31:
                            // This card is perfect because we reach 31 exactly and get 2 points.
                            this.cards.RemoveAt(i);
                            return card;
                        case < 31:
                            validCardIndices.Add(i);
                            break;
                    }
                }

                if (validCardIndices.Count > 0)
                {
                    int validIndex = validCardIndices[this.random.Next(validCardIndices.Count)];
                    var validCard = this.cards[validIndex];
                    this.cards.RemoveAt(validIndex);
                    return validCard;
                }
            }

            return null;
        }

        public IReadOnlyList<Card> Cards => this.cards.AsReadOnly();
    }

    public class PlayerHandFactory : IPlayerHandFactory
    {
        public IPlayerHand CreatePlayerHand()
        {
            return new PlayerHand();
        }
    }
}
