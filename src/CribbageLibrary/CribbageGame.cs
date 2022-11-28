namespace Cribbage
{
    using Cribbage.Reporting;

    public class CribbageGame
    {
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

        public void Play(GameReporter? reporter = null)
        {
            bool firstPlayerDealer = false;
            var gameWinningSignal = new GameWinningSignal();
            this.FirstPlayer.SetWinningSignalSource(gameWinningSignal);
            this.SecondPlayer.SetWinningSignalSource(gameWinningSignal);

            reporter?.ReportBeginGame(this.FirstPlayer, this.SecondPlayer);

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
            catch (GameWinningSignalException)
            {
                this.WinningPlayer = gameWinningSignal.WinningPlayer!;
                var losingPlayer = object.Equals(this.WinningPlayer, this.FirstPlayer) ? this.SecondPlayer : this.FirstPlayer;
                reporter?.ReportGameWinner(this.WinningPlayer, 121 - losingPlayer.Score);
            }
        }
    }
}
