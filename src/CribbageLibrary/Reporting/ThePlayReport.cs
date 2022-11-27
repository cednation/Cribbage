namespace Cribbage.Reporting
{
    using System.Diagnostics;

    public abstract class CribbageReport
    {
        private readonly List<string> messages = new();

        public IReadOnlyList<string> Messages => this.messages;

        protected void ReportMessage(string message)
        {
            this.messages.Add(message);
        }
    }

    public class ThePlayReport : CribbageReport
    {
        // ReSharper disable once InconsistentNaming
        public enum ThePlayScoreType
        {
            Go,
            LastCard,
            ThirtyOne,
            Fifteen,
            Pair,
            Run
        }

        public class ThePlayPairAdditionalInfo
        {
            public Rank CardRank;
            public int NumCardsInARow;
        }

        public class ThePlayRunAdditionalInfo
        {
            public IList<Rank> CardRun;

            public ThePlayRunAdditionalInfo(IList<Rank> cardRun)
            {
                this.CardRun = cardRun;
            }
        }

        public void ReportCardPlayed(string playerName, Card card, int runningValue)
        {
            base.ReportMessage($"Player {playerName} plays card {card}. Running total: {runningValue}");
        }

        public void ReportPlayerScored(string playerName, int score, string reason)
        {
            base.ReportMessage($"Player {playerName} scored {score} point(s) from {reason}");
        }

        public void ReportPlayerScored(string playerName, int score, ThePlayScoreType scoreType, object? additionalInfo = null)
        {
            string reason;
            switch (scoreType)
            {
                case ThePlayScoreType.Go:
                    reason = "for the Go!";
                    break;
                case ThePlayScoreType.LastCard:
                    reason = "for the last card";
                    break;
                case ThePlayScoreType.ThirtyOne:
                    reason = "for reaching 31";
                    break;
                case ThePlayScoreType.Fifteen:
                    reason = "for running value equals 15";
                    break;
                case ThePlayScoreType.Pair:
                    var pairInfo = (ThePlayPairAdditionalInfo)additionalInfo!;
                    reason = $"for {pairInfo.NumCardsInARow} {pairInfo.CardRank}s in a row";
                    break;
                case ThePlayScoreType.Run:
                    var runInfo = (ThePlayRunAdditionalInfo)additionalInfo!;
                    string cardValues = string.Join(' ', runInfo.CardRun!);
                    reason = $"for {runInfo.CardRun!.Length} cards in a row: {cardValues}";
                    break;
                default:
                    Debug.Fail("Score type unknown");
                    reason = "reason unknown";
                    break;
            }

            base.ReportMessage($"Player {playerName} scored {score} point(s) {reason}");
        }
    }
}
