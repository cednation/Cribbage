namespace Cribbage
{
    public struct PlayerHandScoreCard
    {
        public IReadOnlyList<Card> SortedHand;
        public Card StarterCard;

        public int NumFifteens;
        public int NumPairs;
        public int NumRuns;
        public int RunsPoints;
        public bool RightJack;
        public bool Flush;
        public int FlushPoints;

        public int Score;
    }
}
