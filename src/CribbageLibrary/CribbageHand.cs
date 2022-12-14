namespace Cribbage
{
    using System.Diagnostics;
    using Cribbage.Reporting;

    public interface ICribbageHand
    {
        IPlayer Dealer { get; }
        IPlayer Pone { get; }
        IReadOnlyCollection<Card> Crib { get; }
        Card Starter { get; }

        void Run(IHandReporter? reporter);
    }

    public class CribbageHand : ICribbageHand
    {
        private readonly List<Card> crib = new();
        private readonly IDeck deck;

        public IPlayer Dealer { get; }
        public IPlayer Pone { get; }

        public IReadOnlyCollection<Card> Crib => crib.AsReadOnly();
        public Card Starter { get; protected set; }

        public CribbageHand(IPlayer dealer, IPlayer pone, IDeck deck)
        {
            this.Dealer = dealer;
            this.Pone = pone;
            this.deck = deck;
        }

        public void Run(IHandReporter? reporter = null)
        {
            this.Dealer.ResetHand();
            this.Pone.ResetHand();

            // Shuffle deck and deal
            for (int i = 0; i < 12; i++)
            {
                var dealtCard = this.deck.DealRandomCard();
                var curPlayer = i % 2 == 0 ? this.Pone : this.Dealer;

                curPlayer.Hand.AddDealtCards(dealtCard);
            }

            // Report hands
            reporter?.ReportPlayerHandDealt(this.Pone, this.Pone.Hand.Cards);
            reporter?.ReportPlayerHandDealt(this.Dealer, this.Dealer.Hand.Cards);

            this.crib.AddRange(this.Dealer.Hand.SendCardsToCrib());
            this.crib.AddRange(this.Pone.Hand.SendCardsToCrib());
            Debug.Assert(crib.Count == 4, "Should be 4 cards in the crib!");

            reporter?.ReportSendCardsToCrib(this.Pone, this.crib.Skip(2).ToArray(), ownCrib: false);
            reporter?.ReportSendCardsToCrib(this.Dealer, this.crib.Take(2).ToArray(), ownCrib: true);

            // Cut the starter card
            var starter = this.Starter = this.deck.DealRandomCard();
            reporter?.ReportStarterCardCut(starter);
            if (starter.Rank == Rank.Jack)
            {
                reporter?.Report2ForHeels(this.Dealer);
                this.Dealer.AddScore(2); // 2 for his heels!
            }

            // Now the play
            var thePlay = new ThePlay(this.Pone, this.Dealer);
            thePlay.Run(reporter?.PlayReporter);

            // Give the cards back to the players
            this.Dealer.Hand.AddReturnCardsAfterPlay(thePlay.PlayedCards.Where(p => p.IsDealer).Select(p => p.Card));
            this.Pone.Hand.AddReturnCardsAfterPlay(thePlay.PlayedCards.Where(p => !p.IsDealer).Select(p => p.Card));

            // Now count hands, starting with the pone
            var scorer = new PlayerHandScorer();
            var scoreCard = scorer.ScoreHand(this.Pone.Hand.Cards, starter);
            reporter?.ReportHandScored(this.Pone, isCrib: false, scoreCard);
            this.Pone.AddScore(scoreCard.Score);

            scoreCard = scorer.ScoreHand(this.Dealer.Hand.Cards, starter);
            reporter?.ReportHandScored(this.Dealer, isCrib: false, scoreCard);
            this.Dealer.AddScore(scoreCard.Score);

            scoreCard = scorer.ScoreHand(this.crib, starter, crib: true);
            reporter?.ReportHandScored(this.Dealer, isCrib: true, scoreCard);
            this.Dealer.AddScore(scoreCard.Score);
        }
    } 
}
