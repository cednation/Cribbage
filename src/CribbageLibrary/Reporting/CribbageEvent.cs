namespace Cribbage.Reporting
{
    public abstract class CribbageEvent
    {
        public CribbageEventType EventType { get; }
        public IPlayer? Player { get; }
        public string TextMessage { get; }

        protected CribbageEvent(CribbageEventType eventType, string textMessage, IPlayer? player = null)
        {
            EventType = eventType;
            TextMessage = textMessage;
            Player = player;
        }

        public override string ToString()
        {
            return this.TextMessage;
        }
    }

    // ReSharper disable once InconsistentNaming
    public enum CribbageEventType
    {
        DealtHand,
        SendCardsToCrib,
        StarterCardCut,
        PlayCard,
        SayGo,
        ScorePointsPlay,
        CountHand,
        CountCrib,
        WonGame,
        BeginSection,
        CutForDeal
    }

    public class BeginSectionEvent : CribbageEvent
    {
        public BeginSectionEvent(string textMessage)
            : base(CribbageEventType.BeginSection, textMessage)
        {
        }
    }

    public class CutForDealEvent : CribbageEvent
    {
        public new IPlayer Player => base.Player!;
        public Rank WinningCut { get; }
        public Rank LosingCut { get; }

        public CutForDealEvent(IPlayer cutWinner, Rank cutWinnerRank, Rank cutLoserRank, string textMessage)
            : base(CribbageEventType.CutForDeal, textMessage, cutWinner)
        {
            this.WinningCut = cutWinnerRank;
            this.LosingCut = cutLoserRank;
        }
    }

    public class WonGameEvent : CribbageEvent
    {
        public new IPlayer Player => base.Player!;
        public int PointDifference { get; }
        public bool Skunk => this.PointDifference > 30;

        public WonGameEvent(IPlayer winningPlayer, int pointDifference, string textMessage)
            : base(CribbageEventType.WonGame, textMessage, winningPlayer)
        {
            this.PointDifference = pointDifference;
        }
    }

    public class DealtCardsEvent : CribbageEvent
    {
        public new IPlayer Player => base.Player!;
        public IReadOnlyCollection<Card> Hand { get; }

        public DealtCardsEvent(IPlayer player, IReadOnlyCollection<Card> hand, string textMessage)
            : base(CribbageEventType.DealtHand, textMessage, player)
        {
            Hand = hand;
        }
    }

    public class SendCardsToCribEvent : CribbageEvent
    {
        public new IPlayer Player => base.Player!;
        public IReadOnlyCollection<Card> ToCribCards { get; }

        public SendCardsToCribEvent(IPlayer player, IReadOnlyCollection<Card> toCribCards, string textMessage)
            : base(CribbageEventType.SendCardsToCrib, textMessage, player)
        {
            this.ToCribCards = toCribCards;
        }
    }

    public class StarterCardCutEvent : CribbageEvent
    {
        public Card StarterCard { get; }

        public StarterCardCutEvent(Card starterCard, string textMessage)
            : base(CribbageEventType.StarterCardCut, textMessage)
        {
            StarterCard = starterCard;
        }
    }

    public class PlayerHandScoredEvent : CribbageEvent
    {
        public new IPlayer Player => base.Player!;
        public PlayerHandScoreCard ScoreCard { get; }
        public bool IsCrib { get; }

        public PlayerHandScoredEvent(IPlayer player, bool isCrib, PlayerHandScoreCard scoreCard, string textMessage)
            : base(isCrib ? CribbageEventType.CountCrib : CribbageEventType.CountHand, textMessage, player)
        {
            this.ScoreCard = scoreCard;
            this.IsCrib = isCrib;
        }
    }

    public class PlayCardEvent : CribbageEvent
    {
        public new IPlayer Player => base.Player!;
        public Card PlayedCard { get; }
        public int RunningTotal { get; }

        public PlayCardEvent(IPlayer player, Card playedCard, int runningTotal, string textMessage)
            : base(CribbageEventType.PlayCard, textMessage, player)
        {
            this.PlayedCard = playedCard;
            this.RunningTotal = runningTotal;
        }
    }

    public class SayGoEvent : CribbageEvent
    {
        public new IPlayer Player => base.Player!;
        public int RunningTotal { get; }

        public SayGoEvent(IPlayer goPlayer, int runningTotal, string textMessage)
            : base(CribbageEventType.SayGo, textMessage, goPlayer)
        {
            this.RunningTotal = runningTotal;
        }
    }

    #region Play Score events
    // ReSharper disable once InconsistentNaming
    public enum ThePlayScoreType
    {
        JackStarterCut,
        Go,
        LastCard,
        ThirtyOne,
        Fifteen,
        Pair,
        Run
    }

    public class ScorePlayPointsEvent : CribbageEvent
    {
        public new IPlayer Player => base.Player!;
        public int Points { get; }
        // ReSharper disable once InconsistentNaming
        public ThePlayScoreType ScoreType { get; }

        public ScorePlayPointsEvent(IPlayer player, int points, ThePlayScoreType scoreType, string textMessage)
            : base(CribbageEventType.ScorePointsPlay, textMessage, player)
        {
            Points = points;
            ScoreType = scoreType;
        }
    }
    
    public class ScorePlayPairEvent : ScorePlayPointsEvent
    {
        public Rank PairRank { get; }
        public int PairCount { get; }

        public ScorePlayPairEvent(IPlayer player, int score, Rank pairRank, int pairCount, string textMessage)
            : base(player, score, ThePlayScoreType.Pair, textMessage)
        {
            this.PairRank = pairRank;
            this.PairCount = pairCount;
        }
    }

    public class ScorePlayRunEvent : ScorePlayPointsEvent
    {
        public IReadOnlyList<Rank> CardRun { get; }

        public ScorePlayRunEvent(IPlayer player, IReadOnlyList<Rank> cardRun, string textMessage)
            : base(player, cardRun.Count, ThePlayScoreType.Run, textMessage)
        {
            this.CardRun = cardRun;
        }
    }
    #endregion
}
