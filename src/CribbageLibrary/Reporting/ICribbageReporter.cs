namespace Cribbage.Reporting
{
    public delegate void CribbageEventHandler(CribbageReporter reporter, CribbageEvent cribbageEvent);

    public interface ICribbageReporter
    {
        IReadOnlyList<CribbageEvent> ReportedEvents { get; }
        event CribbageEventHandler? CribbageEventNotification;
    }
    
    public interface IGameReporter : ICribbageReporter
    {
        IHandReporter? CribbageHandReporter { get; }

        void ReportBeginGame(IPlayer player1, IPlayer player2);
        void ReportCutForDeal(IPlayer cutWinner, Rank cutWinnerRank, Rank cutLoserRank);
        void ReportCutForDealDraw(Rank tieRank);
        void ReportBeginHand(IPlayer player1, IPlayer player2, IPlayer dealer, int handNumber);
        void ReportGameWinner(IPlayer winner, int pointDifference);
    }

    public interface IHandReporter : ICribbageReporter
    {
        IThePlayReporter? PlayReporter { get; }

        void ReportPlayerHandDealt(IPlayer player, IReadOnlyCollection<Card> hand);
        void ReportSendCardsToCrib(IPlayer player, IReadOnlyCollection<Card> cards, bool ownCrib);
        void ReportStarterCardCut(Card starterCard);
        void Report2ForHeels(IPlayer dealer);
        public void ReportHandScored(IPlayer player, bool isCrib, PlayerHandScoreCard scoreCard);
    }

    public interface IThePlayReporter : ICribbageReporter
    {
        void ReportCardPlayed(IPlayer player, Card card, int runningTotal);
        void ReportSayGo(IPlayer goPlayer, int runningTotal);
        void ReportPlayerScored(IPlayer player, int score, ThePlayScoreType scoreType, object? additionalInfo = null);
    }
}
