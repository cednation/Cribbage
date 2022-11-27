namespace Cribbage
{
    using System.Diagnostics;

    public interface ICribbageHand
    {
        IPlayer Dealer { get; }
        IPlayer Pone { get; }
        IReadOnlyCollection<Card> Crib { get; }
        Card Starter { get; }

        void Run();
    }

    public class CribbageHand : ICribbageHand
    {
        private readonly List<Card> crib = new();
        private readonly IDeck deck;

        public IPlayer Dealer { get; }
        public IPlayer Pone { get; }

        public IReadOnlyCollection<Card> Crib => crib;
        public Card Starter { get; protected set; }

        public CribbageHand(IPlayer dealer, IPlayer pone, IDeck deck)
        {
            this.Dealer = dealer;
            this.Pone = pone;
            this.deck = deck;

            this.Dealer.ResetHand();
            this.Pone.ResetHand();
        }

        public void Run()
        {
            // Shuffle deck and deal
            for (int i = 0; i < 12; i++)
            {
                var dealtCard = this.deck.DealRandomCard();
                var curPlayer = i % 2 == 0 ? this.Pone : this.Dealer;

                curPlayer.Hand.AddDealtCards(dealtCard);
            }

            this.crib.AddRange(this.Dealer.Hand.SendCardsToCrib());
            this.crib.AddRange(this.Pone.Hand.SendCardsToCrib());
            Debug.Assert(crib.Count == 4, "Should be 4 cards in the crib!");

            // Cut the starter card
            var starter = this.deck.DealRandomCard();
            this.Starter = starter;
            if (starter.Rank == Rank.Jack)
                this.Dealer.AddScore(2); // 2 for his heels!

            // Now the play
            var thePlay = new ThePlay(this.Pone, this.Dealer);
            thePlay.Run();

            // Give the cards back to the players
            this.Dealer.Hand.AddDealtCards(thePlay.PlayedCards.Where(p => p.IsDealer).Select(p => p.Card).ToArray());
            this.Pone.Hand.AddDealtCards(thePlay.PlayedCards.Where(p => !p.IsDealer).Select(p => p.Card).ToArray());

            // Now count hands, starting with the pone
            var scorer = new HandScorer();
            var scoreCard = scorer.ScoreHand(this.Pone.Hand.Cards, starter);
            this.Pone.AddScore(scoreCard.Score);

            scoreCard = scorer.ScoreHand(this.Dealer.Hand.Cards, starter);
            this.Dealer.AddScore(scoreCard.Score);

            scoreCard = scorer.ScoreHand(this.crib, starter);
            this.Dealer.AddScore(scoreCard.Score);
        }
    } 
}
