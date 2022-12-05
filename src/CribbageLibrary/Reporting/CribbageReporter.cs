namespace Cribbage.Reporting
{
    using System.Text;

    public abstract class CribbageReporter
    {
        private readonly List<CribbageEvent> cribbageEvents = new();

        public IReadOnlyList<CribbageEvent> ReportedEvents => this.cribbageEvents.AsReadOnly();

        public event CribbageEventHandler? CribbageEventNotification;

        protected void ReportCribbageEvent(CribbageEvent cribbageEvent)
        {
            this.cribbageEvents.Add(cribbageEvent);
            this.CribbageEventNotification?.Invoke(this, cribbageEvent);
        }
    }

    public class ThePlayReporter : CribbageReporter, IThePlayReporter
    {
        public void ReportCardPlayed(IPlayer player, Card card, int runningTotal)
        {
            string textMessage = $"{player.Name} plays card {card} Running total: {runningTotal}";
            var e = new PlayCardEvent(player, card, runningTotal, textMessage);

            base.ReportCribbageEvent(e);
        }

        public void ReportSayGo(IPlayer goPlayer, int runningTotal)
        {
            int minimumValue = 31 - runningTotal;
            string minimumValueText = minimumValue == 1 ? "Ace" : minimumValue.ToString();
            bool hasNoCards = goPlayer.Hand.Cards.Count == 0;
            string reason = hasNoCards ? "has no more cards" : $"does not have a(n) {minimumValueText} or less";
            string textMessage = $"{goPlayer.Name} says Go! He/She {reason}";

            var e = new SayGoEvent(goPlayer, runningTotal, textMessage);
            base.ReportCribbageEvent(e);
        }

        /// <summary>
        /// Reports when a player scores during the play.
        /// </summary>
        /// <remarks>
        /// When scoring with a pair (or more), additionalInfo must be a Tuple of Rank, int
        /// When scoring with a run, additionalInfo must be a list of the run
        /// </remarks>
        public void ReportPlayerScored(IPlayer player, int score, ThePlayScoreType scoreType, object? additionalInfo = null)
        {
            if (score < 1)
                throw new ArgumentOutOfRangeException(nameof(score), "Score must be at least 1");

            string MakeTextMessage(string reason)
            {
                string pointsText = score == 1 ? "point" : "points";
                return $"{player.Name} scored {score} {pointsText} {reason}";
            }

            ScorePlayPointsEvent e;
            string textMessage;
            switch (scoreType)
            {
                case ThePlayScoreType.Fifteen:
                    textMessage = MakeTextMessage("for running value equals 15");
                    e = new ScorePlayPointsEvent(player, 2, scoreType, textMessage);
                    break;
                case ThePlayScoreType.Pair:
                    var rankAndCount = (Tuple<Rank, int>)additionalInfo!;
                    textMessage = MakeTextMessage($"for {rankAndCount.Item2} {rankAndCount.Item1}s in a row");
                    e = new ScorePlayPairEvent(player, score, rankAndCount.Item1, rankAndCount.Item2, textMessage);
                    break;
                case ThePlayScoreType.Run:
                    var cardRun = (IReadOnlyList<Rank>)additionalInfo!;
                    string cardRunMessage = string.Join(' ', cardRun);
                    textMessage = MakeTextMessage($"for {cardRun.Count} cards in a row: {cardRunMessage}");
                    e = new ScorePlayRunEvent(player, cardRun, textMessage);
                    break;
                case ThePlayScoreType.Go:
                    textMessage = MakeTextMessage("for the Go!");
                    e = new ScorePlayPointsEvent(player, 1, scoreType, textMessage);
                    break;
                case ThePlayScoreType.ThirtyOne:
                    textMessage = MakeTextMessage("for reaching 31");
                    e = new ScorePlayPointsEvent(player, 2, scoreType, textMessage);
                    break;
                case ThePlayScoreType.LastCard:
                    textMessage = MakeTextMessage("for the last card");
                    e = new ScorePlayPointsEvent(player, 1, scoreType, textMessage);
                    break;
                case ThePlayScoreType.JackStarterCut: // This event occurs before the play, but is counted in a scoring type
                default:
                    throw new NotImplementedException($"Play score type {scoreType} not implemented");
            }

            base.ReportCribbageEvent(e);
        }
    }

    public class HandReporter : CribbageReporter, IHandReporter
    {
        public IThePlayReporter? PlayReporter { get; }

        public HandReporter()
        {

        }

        public HandReporter(ThePlayReporter playReporter)
        {
            PlayReporter = playReporter;
        }

        public void ReportPlayerHandDealt(IPlayer player, IReadOnlyCollection<Card> hand)
        {
            var sortedHand = new List<Card>(hand);
            sortedHand.Sort();

            string sortedHandMessage = string.Join(' ', sortedHand);
            string textMessage = $"{player.Name} dealt hand: {sortedHandMessage}";
            var e = new DealtCardsEvent(player, hand, textMessage);

            base.ReportCribbageEvent(e);
        }

        public void ReportSendCardsToCrib(IPlayer player, IReadOnlyCollection<Card> cards, bool ownCrib)
        {
            var sortedCards = new List<Card>(cards);
            sortedCards.Sort();

            string sortedHandMessage = string.Join(' ', sortedCards);
            string cribOwnershipMessage = ownCrib ? "his/her crib" : "opponent's crib";
            string textMessage = $"{player.Name} throws cards {sortedHandMessage} into {cribOwnershipMessage}";

            var e = new SendCardsToCribEvent(player, cards, textMessage);
            base.ReportCribbageEvent(e);
        }

        public void ReportStarterCardCut(Card starterCard)
        {
            string textMessage = $"Starter card {starterCard} is cut";
            var e = new StarterCardCutEvent(starterCard, textMessage);

            base.ReportCribbageEvent(e);
        }

        public void Report2ForHeels(IPlayer dealer)
        {
            string textMessage = $"{dealer.Name} scores 2 points for his heels";
            var e = new ScorePlayPointsEvent(dealer, 2, ThePlayScoreType.JackStarterCut, textMessage);

            base.ReportCribbageEvent(e);
        }

        public void ReportHandScored(IPlayer player, bool isCrib, PlayerHandScoreCard scoreCard)
        {
            string handOrCrib = isCrib ? "crib" : "hand";
            string textMessage = $"{player.Name} {handOrCrib} scored {scoreCard.Score} point(s)";
            
            StringBuilder textBuilder = new StringBuilder();
            textBuilder.AppendLine(textMessage);
            textBuilder.AppendLine(); // empty line

            string hand = string.Join(' ', scoreCard.SortedHand);
            textBuilder.Append('\t');
            textBuilder.AppendLine(hand);
            textBuilder.Append('\t');
            textBuilder.AppendLine(scoreCard.StarterCard.ToString());
            textBuilder.AppendLine(); // empty line

            if (scoreCard.NumFifteens > 0)
            {
                textMessage = $"\tPoints for 15s: {scoreCard.NumFifteens * 2}";
                textBuilder.AppendLine(textMessage);
            }

            if (scoreCard.NumPairs > 0)
            {
                textMessage = $"\tPoints for pairs: {scoreCard.NumPairs * 2}";
                textBuilder.AppendLine(textMessage);
            }

            if (scoreCard.NumRuns > 0)
            {
                textMessage = $"\t{scoreCard.NumRuns} run(s) for a total of {scoreCard.RunsPoints} points";
                textBuilder.AppendLine(textMessage);
            }

            if (scoreCard.Flush)
            {
                textMessage = $"\t{scoreCard.FlushPoints} points for the flush";
                textBuilder.AppendLine(textMessage);
            }

            if (scoreCard.RightJack)
            {
                textMessage = "\t1 point for the right Jack";
                textBuilder.AppendLine(textMessage);
            }

            var e = new PlayerHandScoredEvent(player, isCrib, scoreCard, textBuilder.ToString());
            base.ReportCribbageEvent(e);
        }
    }

    public class GameReporter : CribbageReporter, IGameReporter
    {
        public IHandReporter? CribbageHandReporter { get; }

        public GameReporter(){}

        public GameReporter(HandReporter handReporter)
        {
            this.CribbageHandReporter = handReporter;
        }

        public void ReportBeginGame(IPlayer player1, IPlayer player2)
        {
            string textMessage = $"A new cribbage game begins between {player1.Name} and {player2.Name}";

            var e = new BeginSectionEvent(textMessage);
            base.ReportCribbageEvent(e);
        }

        public void ReportCutForDeal(IPlayer cutWinner, Rank cutWinnerRank, Rank cutLoserRank)
        {
            string textMessage = $"{cutWinner.Name} wins the deal with a cut of {cutWinnerRank} vs {cutLoserRank}";

            var e = new CutForDealEvent(cutWinner, cutWinnerRank, cutLoserRank, textMessage);
            base.ReportCribbageEvent(e);
        }

        public void ReportCutForDealDraw(Rank tieRank)
        {
            string textMessage = $"The players each cut a(n) {tieRank}. Shuffle the deck and cut for draw again";
            var e = new CutForDealEvent(tieRank, textMessage);
            base.ReportCribbageEvent(e);
        }

        public void ReportBeginHand(IPlayer player1, IPlayer player2, IPlayer dealer, int handNumber)
        {
            string textMessage = $"Hand {handNumber} begins with {dealer.Name} as the dealer";
            if (handNumber > 1)
                textMessage += $". Score stands at: {player1.Name} with {player1.Score} point(s) and {player2.Name} with {player2.Score} point(s)";
            var e = new BeginSectionEvent(textMessage);
            base.ReportCribbageEvent(e);
        }

        public void ReportGameWinner(IPlayer winner, int pointDifference)
        {
            bool skunk = pointDifference > 30;
            string gameOrSkunk = skunk ? "a skunk(!)" : "the game";
            string textMessage = $"{winner.Name} wins {gameOrSkunk} by {pointDifference} point(s)";

            var e = new WonGameEvent(winner, pointDifference, textMessage);
            base.ReportCribbageEvent(e);
        }
    }
}
