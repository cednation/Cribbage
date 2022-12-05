namespace CribbageConsole
{
    internal class UserConsoleHand : IPlayerHand
    {
        private readonly List<Card> cards = new(capacity: 6);

        public IReadOnlyList<Card> Cards => this.cards.AsReadOnly();

        public void AddDealtCards(params Card[] dealtCards)
        {
            this.cards.AddRange(dealtCards);
            this.cards.Sort();
        }

        public void AddReturnCardsAfterPlay(IEnumerable<Card> returnedCards)
        {
            this.cards.AddRange(returnedCards);
            this.cards.Sort();
        }

        // TODO: Need more information to make informed decision: whose crib is it? What is the current score?
        public IEnumerable<Card> SendCardsToCrib()
        {
            Console.WriteLine("Choose cards to send to crib");
            Console.WriteLine(string.Join(' ', this.cards));

            // TODO: bug - I allow selecting a card that exceeds 31
            Card crib1, crib2;
            while (true)
            {
                crib1 = this.ReadCard();
                crib2 = this.ReadCard();

                if (crib1 == crib2)
                {
                    Console.WriteLine("Crib cards must be unique. Try again.");
                    continue;
                }

                break;
            }

            this.cards.Remove(crib1);
            this.cards.Remove(crib2);
            return new[] { crib1, crib2 };
        }

        public Card? PlayCard(int runningCount)
        {
            if (!CanPlayCard(runningCount))
                return null;

            // TODO: We need more information then this to make the right decision.
            Console.WriteLine($"Choose card to play. The running count is {runningCount}");
            Console.WriteLine(string.Join(' ', this.cards));
            Card playedCard = this.ReadCard(31 - runningCount);

            this.cards.Remove(playedCard);
            return playedCard;
        }

        private bool CanPlayCard(int runningTotal)
        {
            return this.cards.Any(card => card.Value + runningTotal <= 31);
        }

        private Card ReadCard(int maximumValue = 31)
        {
            while (true)
            {
                var key = Console.ReadKey(intercept: true);
                if (!char.IsDigit(key.KeyChar))
                {
                    Console.WriteLine("Enter the index of the card to play");
                    continue;
                }

                int pressedIndex = int.Parse(new[] { key.KeyChar });
                if (pressedIndex < 1 || pressedIndex > this.cards.Count)
                {
                    Console.WriteLine($"Index out of range. Enter index 1-{this.cards.Count}");
                    continue;
                }

                Card readCard = this.cards[pressedIndex - 1];
                if (readCard.Value > maximumValue)
                {
                    Console.WriteLine($"The {readCard.Rank} cannot be played. Doing so would exceed 31");
                    continue;
                }

                return readCard;
            }
        }
    }

    internal class UserConsoleHandFactory : IPlayerHandFactory
    {
        public IPlayerHand CreatePlayerHand()
        {
            return new UserConsoleHand();
        }
    }
}
