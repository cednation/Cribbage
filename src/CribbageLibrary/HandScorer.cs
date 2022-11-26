namespace Cribbage
{
    #nullable enable
    public class HandScorer
    {
        private int runningTotal;
        private IReadOnlyList<Card>? cards;
        public HandScoreCard scoreCard;

        public HandScoreCard ScoreHand(IList<Card> playerHand, Card starterCard)
        {
            if (playerHand is not { Count: 4 })
                throw new ArgumentException("Player hand must be 4 cards", nameof(playerHand));

            var cardsToScore = new List<Card>();
            cardsToScore.AddRange(playerHand);
            cardsToScore.Add(starterCard);

            this.cards = cardsToScore;
            this.runningTotal = 0;
            this.scoreCard = new HandScoreCard();

            this.CheckForFifteens();
            this.CheckForPairs();
            this.CheckForRuns();
            this.CheckForFlush();
            this.CheckForRightJack();

            this.scoreCard.Score = this.runningTotal;
            return this.scoreCard;
        }

        private void CheckForFifteens()
        {
            const int FifteenValue = 2;
            int numFifteens = 0;

            var cardValues = new List<int>(capacity: this.cards!.Count);
            cardValues.AddRange(this.cards.Select(card => card.Value));
            cardValues.Sort();

            void CheckForFifteensRecursive(int fifteenRunningTotal, IEnumerable<int> remainingCards)
            {
                int i = -1;
                foreach (int cardValue in remainingCards)
                {
                    i++;
                    if (fifteenRunningTotal + cardValue == 15)
                        numFifteens++;
                    else if (fifteenRunningTotal + cardValue * 2 <= 15) // This works because the cards are sorted
                        CheckForFifteensRecursive(fifteenRunningTotal + cardValue, remainingCards.Skip(i + 1));
                }
            }

            // No need to check the last card, it cannot be 15 by itself
            for (int i = 0; i < cardValues.Count - 1; i++)
            {
                CheckForFifteensRecursive(cardValues[i], cardValues.Skip(i + 1));
            }

            this.scoreCard.NumFifteens = numFifteens;
            this.runningTotal += numFifteens * FifteenValue;
        }

        private void CheckForPairs()
        {
            const int PairValue = 2;
            int numPairs = 0;
            for (int i = 0; i < this.cards!.Count - 1; i++)
            {
                for (int j = i + 1; j < this.cards.Count; j++)
                {
                    if (this.cards[i].Rank == this.cards[j].Rank)
                        numPairs++;
                }
            }

            this.scoreCard.NumPairs = numPairs;
            this.runningTotal += numPairs * PairValue;
        }

        private void CheckForRuns()
        {
            var rankValues = new List<int>(capacity: this.cards!.Count);
            rankValues.AddRange(this.cards.Select(card => (int)card.Rank));

            rankValues.Sort();

            for (int i = 0; i < rankValues.Count - 2; i++)
            {
                int doubleRun = 1;
                int runLength = 1;

                int j = i + 1;
                while (j < rankValues.Count &&
                       (rankValues[j - 1] == rankValues[j] ||
                       rankValues[j - 1] + 1 == rankValues[j]))
                {
                    if (rankValues[j - 1] == rankValues[j])
                    {
                        if (j >= 2 && rankValues[j - 2] == rankValues[j])
                            doubleRun++;
                        else
                            doubleRun *= 2;
                    }
                    else
                    {
                        runLength++;
                    }

                    j++;
                }

                if (runLength >= 3)
                {
                    int score = runLength * doubleRun;
                    this.scoreCard.NumRuns = doubleRun;
                    this.runningTotal += score;

                    // With 5 cards it is only possible to have one unique 3 card or greater run.
                    break;
                }
            }
        }

        private void CheckForFlush()
        {
            bool handFlush = true;
            Suit handSuit = this.cards![0].Suit;

            for (int i = 1; i < 4; i++)
            {
                if (this.cards[i].Suit != handSuit)
                {
                    handFlush = false;
                    break;
                }
            }

            if (handFlush)
            {
                int flushValue = 4;
                if (this.cards[4].Suit == handSuit)
                    flushValue++;

                this.scoreCard.Flush = true;
                this.runningTotal += flushValue;
            }
        }

        private void CheckForRightJack()
        {
            Suit starterSuit = this.cards![4].Suit;
            for (int i = 0; i < 4; i++)
            {
                if (this.cards[i].Rank == Rank.Jack &&
                    this.cards[i].Suit == starterSuit)
                {
                    this.scoreCard.RightJack = true;
                    this.runningTotal++;
                }
            }
        }
    }
}
