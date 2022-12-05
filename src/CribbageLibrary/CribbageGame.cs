namespace Cribbage
{
    using Cribbage.Reporting;

    public class CribbageGame
    {
        public enum FirstDealerChoice
        {
            FirstPlayer,
            SecondPlayer,
            CutForDeal
        }

        protected readonly IDeckFactory deckFactory;

        public IPlayer FirstPlayer { get; }
        public IPlayer SecondPlayer { get; }
        public IPlayer? WinningPlayer { get; protected set; }
        public int NumHands { get; protected set; }

        public CribbageGame(IPlayer firstPlayer, IPlayer secondPlayer, IDeckFactory deckFactory)
        {
            this.FirstPlayer = firstPlayer;
            this.SecondPlayer = secondPlayer;
            this.deckFactory = deckFactory;
        }

        public void Play(FirstDealerChoice dealerChoice = FirstDealerChoice.FirstPlayer, IGameReporter? reporter = null)
        {
            reporter?.ReportBeginGame(this.FirstPlayer, this.SecondPlayer);

            bool firstPlayerDealer;
            switch (dealerChoice)
            {
                case FirstDealerChoice.FirstPlayer:
                    firstPlayerDealer = true;
                    break;
                case FirstDealerChoice.SecondPlayer:
                    firstPlayerDealer = false;
                    break;
                case FirstDealerChoice.CutForDeal:
                default:
                    firstPlayerDealer = this.CutForDeal(reporter);
                    break;
            }

            var gameWinningSignal = new GameWinningSignalSource();
            this.FirstPlayer.SetWinningSignalSource(gameWinningSignal);
            this.SecondPlayer.SetWinningSignalSource(gameWinningSignal);

            try
            {
                while (true)
                {
                    var dealer = firstPlayerDealer ? this.FirstPlayer : this.SecondPlayer;
                    var pone = firstPlayerDealer ? this.SecondPlayer : this.FirstPlayer;

                    this.NumHands++;
                    reporter?.ReportBeginHand(this.FirstPlayer, this.SecondPlayer, dealer, this.NumHands);
                    var hand = new CribbageHand(dealer, pone, this.deckFactory.CreateDeck());
                    hand.Run(reporter?.CribbageHandReporter);

                    firstPlayerDealer = !firstPlayerDealer;
                }
            }
            catch (GameWinningSignalException ex)
            {
                this.WinningPlayer = ex.WinningPlayer;
                var losingPlayer = object.Equals(this.WinningPlayer, this.FirstPlayer) ? this.SecondPlayer : this.FirstPlayer;
                reporter?.ReportGameWinner(this.WinningPlayer, 121 - losingPlayer.Score);
            }
        }

        private bool CutForDeal(IGameReporter? reporter)
        {
            while (true)
            {
                var cutDeck = this.deckFactory.CreateDeck();
                var firstPlayerCutCard = cutDeck.DealRandomCard();
                var secondPlayerCutCard = cutDeck.DealRandomCard();

                if (firstPlayerCutCard.Rank != secondPlayerCutCard.Rank)
                {
                    bool firstPlayerDealer = firstPlayerCutCard.Rank < secondPlayerCutCard.Rank;

                    if (reporter != null)
                    {
                        var cutWinner = firstPlayerDealer ? this.FirstPlayer : this.SecondPlayer;
                        Rank winRank = firstPlayerDealer ? firstPlayerCutCard.Rank : secondPlayerCutCard.Rank;
                        Rank loseRank = firstPlayerDealer ? secondPlayerCutCard.Rank : firstPlayerCutCard.Rank;

                        reporter.ReportCutForDeal(cutWinner, winRank, loseRank);
                    }

                    return firstPlayerDealer;
                }
                else
                {
                    // Shuffle and cut again!
                    reporter?.ReportCutForDealDraw(firstPlayerCutCard.Rank);
                }
            }
        }
    }
}
