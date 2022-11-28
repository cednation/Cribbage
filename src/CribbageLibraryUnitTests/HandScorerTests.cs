namespace CribbageUnitTests
{
    using FluentAssertions;

    [TestClass]
    public class HandScorerTests
    {
        [TestMethod]
        public void Count24Hand()
        {
            var hand = new[]
            {
                new Card(Rank.Eight, Suit.Clubs), new Card(Rank.Eight, Suit.Spades),
                new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Seven, Suit.Diamonds)
            };
            var starter = new Card(Rank.Six, Suit.Hearts);

            var scorer = new PlayerHandScorer();
            var scoreCard = scorer.ScoreHand(hand, starter);

            scoreCard.NumFifteens.Should().Be(4);
            scoreCard.NumRuns.Should().Be(4);
            scoreCard.NumPairs.Should().Be(2);
            scoreCard.Score.Should().Be(24);
        }

        [TestMethod]
        public void Count29Hand()
        {
            var hand = new[]
            {
                new Card(Rank.Five, Suit.Clubs), new Card(Rank.Five, Suit.Diamonds), new Card(Rank.Five, Suit.Hearts),
                new Card(Rank.Jack, Suit.Spades)
            };
            var starter = new Card(Rank.Five, Suit.Spades);

            var scorer = new PlayerHandScorer();
            var scoreCard = scorer.ScoreHand(hand, starter);

            scoreCard.NumPairs.Should().Be(6);
            scoreCard.NumFifteens.Should().Be(8);
            scoreCard.RightJack.Should().BeTrue();
            scoreCard.Score.Should().Be(29);
        }

        [TestMethod]
        public void CountFlushHand()
        {
            var hand = new[]
            {
                new Card(Rank.Ace, Suit.Clubs), new Card(Rank.Queen, Suit.Clubs),
                new Card(Rank.Two, Suit.Clubs), new Card(Rank.Seven, Suit.Clubs)
            };
            var starter = new Card(Rank.King, Suit.Hearts);

            var scorer = new PlayerHandScorer();
            var scoreCard = scorer.ScoreHand(hand, starter);

            scoreCard.Flush.Should().BeTrue();
            scoreCard.Score.Should().Be(4);

            // Now change starter to same suit, and score should increase by 1.
            starter = new Card(Rank.King, Suit.Clubs);
            scoreCard = scorer.ScoreHand(hand, starter);

            scoreCard.Flush.Should().BeTrue();
            scoreCard.Score.Should().Be(5);

            // Now change one card of the hand to another suit. Should no longer have a flush.
            hand[0] = new Card(Rank.Ace, Suit.Spades);
            scoreCard = scorer.ScoreHand(hand, starter);

            scoreCard.Flush.Should().BeFalse();
            scoreCard.Score.Should().Be(0);
        }

        [TestMethod]
        public void CountMultiCardsFifteen()
        {
            var hand = new[]
            {
                new Card(Rank.Four, Suit.Hearts), new Card(Rank.Three, Suit.Clubs),
                new Card(Rank.Queen, Suit.Clubs), new Card(Rank.Ace, Suit.Clubs)
            };
            var starter = new Card(Rank.King, Suit.Spades);

            var scorer = new PlayerHandScorer();
            var scoreCard = scorer.ScoreHand(hand, starter);

            scoreCard.NumFifteens.Should().Be(2);
            scoreCard.Score.Should().Be(4);
        }

        [TestMethod]
        public void CountAllCardsFifteen()
        {
            var hand = new[]
            {
                new Card(Rank.Two, Suit.Clubs), new Card(Rank.Five, Suit.Clubs),
                new Card(Rank.Four, Suit.Hearts), new Card(Rank.Ace, Suit.Hearts)
            };
            var starter = new Card(Rank.Three, Suit.Clubs);

            var scorer = new PlayerHandScorer();
            var scoreCard = scorer.ScoreHand(hand, starter);

            scoreCard.NumRuns.Should().Be(1);
            scoreCard.NumFifteens.Should().Be(1);
            scoreCard.Score.Should().Be(7);
        }

        [TestMethod]
        public void Count24TheHardWay()
        {
            var hand = new[]
            {
                new Card(Rank.Four, Suit.Diamonds), new Card(Rank.Four, Suit.Clubs),
                new Card(Rank.Seven, Suit.Clubs), new Card(Rank.Four, Suit.Spades)
            };
            var starter = new Card(Rank.Four, Suit.Hearts);

            var scorer = new PlayerHandScorer();
            var scoreCard = scorer.ScoreHand(hand, starter);

            scoreCard.NumPairs.Should().Be(6);
            scoreCard.NumFifteens.Should().Be(6);
            scoreCard.Score.Should().Be(24); // 24 the hard way!
        }

        [TestMethod]
        public void CountCribFlush()
        {
            var hand = new[]
            {
                new Card(Rank.Ace, Suit.Clubs), new Card(Rank.Queen, Suit.Clubs),
                new Card(Rank.Two, Suit.Clubs), new Card(Rank.Seven, Suit.Clubs)
            };
            var starter = new Card(Rank.King, Suit.Hearts);

            // This is a four card flush, so it will not count for the crib.
            var scorer = new PlayerHandScorer();
            var scoreCard = scorer.ScoreHand(hand, starter, crib: true);

            scoreCard.Flush.Should().BeFalse();
            scoreCard.Score.Should().Be(0);

            // Now change the starter to the same suit. Then we have a crib flush
            starter = new Card(Rank.King, Suit.Clubs);
            scoreCard = scorer.ScoreHand(hand, starter, crib: true);

            scoreCard.Flush.Should().BeTrue();
            scoreCard.Score.Should().Be(5);
        }
    }
}
