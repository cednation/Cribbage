namespace Cribbage
{
    using System.Diagnostics;
    using Cribbage.Reporting;

    public class ThePlay
    {
        private readonly IPlayer pone;
        private readonly IPlayer dealer;

        private readonly List<PlayedCard> completePlayedCards = new();

        // volatile data as we run the Play
        private int runningValue;
        private readonly List<PlayedCard> playedCards = new();
        private int totalNumPlayedCards;
        private bool dealerTurn;
        private IPlayer? goPlayer;

        public ThePlay(IPlayer pone, IPlayer dealer)
        {
            this.pone = pone;
            this.dealer = dealer;
        }

        public IEnumerable<PlayedCard> PlayedCards => this.completePlayedCards;

        public void Run(ThePlayReport? reporter = null)
        {
            this.runningValue = 0;
            this.totalNumPlayedCards = 0;

            this.goPlayer = null;
            this.dealerTurn = false;

            while (this.totalNumPlayedCards < 8)
            {
                IPlayer currentPlayer = dealerTurn ? this.dealer : this.pone;
                Card? playedCard = currentPlayer.Hand.PlayCard(runningValue);

                if (playedCard != null)
                {
                    this.totalNumPlayedCards++;
                    this.playedCards.Add(new PlayedCard(playedCard.Value, dealerTurn));
                    this.completePlayedCards.Add(this.playedCards[^1]);

                    runningValue += playedCard.Value.Value;
                    Debug.Assert(runningValue <= 31, "Player played an illegal card!");

                    reporter?.ReportCardPlayed(currentPlayer.Name, playedCard.Value, runningValue);

                    this.CheckForScore(currentPlayer, reporter);
                    if (runningValue == 31)
                    {
                        currentPlayer.AddScore(2);
                        reporter?.ReportPlayerScored(currentPlayer.Name, 2, "for 31");

                        this.ResetCount();
                        continue;
                    }

                    // Switch turn if the other player hasn't said 'go!'
                    // Otherwise stay on this player until they are finished playing
                    if (goPlayer == null)
                    {
                        dealerTurn = !dealerTurn;
                    }

                    if (this.totalNumPlayedCards == 8)
                    {
                        // last Card
                        currentPlayer.AddScore(1);
                        reporter?.ReportPlayerScored(currentPlayer.Name, 1, "for last card");
                    }
                }
                else
                {
                    // If nobody has yet said 'go!' then this player says it.
                    if (goPlayer == null)
                    {
                        goPlayer = currentPlayer;
                        dealerTurn = !dealerTurn;
                    }
                    else
                    {
                        // The other player has already said 'go!'
                        // We get a point and reset
                        currentPlayer.AddScore(1);
                        reporter?.ReportPlayerScored(currentPlayer.Name, 1, "for the Go");

                        this.ResetCount();
                    }
                }
            }
        }

        private void ResetCount()
        {
            this.runningValue = 0;
            this.playedCards.Clear();
            this.dealerTurn = !this.dealerTurn;
            this.goPlayer = null;
        }

        private void CheckForScore(IPlayer currentPlayer, ThePlayReport? reporter)
        {
            // Check for 15
            if (this.runningValue == 15)
            {
                currentPlayer.AddScore(2);
                reporter?.ReportPlayerScored(currentPlayer.Name, 2, "value is 15");
            }

            // Check for pairs
            if (this.playedCards.Count >= 2 &&
                this.playedCards[^1].Card.Rank == this.playedCards[^2].Card.Rank)
            {
                int numRankInRow = 2;
                Rank rank = this.playedCards[^1].Card.Rank;
                for (int j = this.playedCards.Count - 3; j >= 0; j--)
                {
                    if (this.playedCards[j].Card.Rank == rank)
                        numRankInRow++;
                }

                int score = numRankInRow switch
                {
                    2 => 2,
                    3 => 6,
                    4 => 12,
                    _ => 12 // Cannot have more than 4 in a deck of cards
                };

                currentPlayer.AddScore(score);

                reporter?.ReportPlayerScored(currentPlayer.Name, score, $"{numRankInRow} {rank}s in a row");
            }

            // Check for runs
            if (this.playedCards.Count >= 3)
            {
                bool IsCardRun(IList<Rank> cards)
                {
                    for (int i = 0; i < cards.Count - 1; i++)
                    {
                        if ((int)cards[i] + 1 != (int)cards[i + 1])
                            return false;
                    }
                    return true;
                }

                for (int skipNum = 0; skipNum < this.playedCards.Count - 2; skipNum++)
                {
                    // Build sorted list of cards from start, skipping more on each pass
                    var remainingCards = this.playedCards.Skip(skipNum).Select(x => x.Card.Rank).ToList();
                    remainingCards.Sort();
                    Debug.Assert(remainingCards.Count >= 3); // No score for runs less than 3

                    if (IsCardRun(remainingCards))
                    {
                        int runCount = remainingCards.Count;
                        currentPlayer.AddScore(runCount);

                        string cardValues = string.Join(' ', remainingCards);
                        reporter?.ReportPlayerScored(currentPlayer.Name, runCount, $"{runCount} cards in a row: {cardValues}");

                        reporter?.ReportPlayerScored(currentPlayer.Name, runCount, ThePlayReport.ThePlayScoreType.Run, new ThePlayReport.ThePlayRunAdditionalInfo(remainingCards));
                        break;
                    }
                }
            }
        }
    }
}
