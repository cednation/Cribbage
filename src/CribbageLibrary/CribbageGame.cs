namespace Cribbage
{
    public class CribbageGame
    {
        private int numHands;
        private readonly IDeckFactory deckFactory;

        public IPlayer firstPlayer { get; }
        public IPlayer secondPlayer { get; }
        public IPlayer? winningPlayer { get; protected set; }
        public int NumHands => this.numHands;

        public CribbageGame(IPlayer firstPlayer, IPlayer secondPlayer, IDeckFactory deckFactory)
        {
            this.firstPlayer = firstPlayer;
            this.secondPlayer = secondPlayer;
            this.deckFactory = deckFactory;
        }

        public void Play()
        {
            bool firstPlayerDealer = false;
            var gameWinningSignal = new GameWinningSignal();
            this.firstPlayer.SetWinningSignalSource(gameWinningSignal);
            this.secondPlayer.SetWinningSignalSource(gameWinningSignal);

            try
            {
                while (true)
                {
                    var dealer = firstPlayerDealer ? this.firstPlayer : this.secondPlayer;
                    var pone = firstPlayerDealer ? this.secondPlayer : this.firstPlayer;

                    this.numHands++;
                    var hand = new CribbageHand(dealer, pone, this.deckFactory.CreateDeck());
                    hand.Run();

                    firstPlayerDealer = !firstPlayerDealer;
                }
            }
            catch (GameWinningSignalException)
            {
                this.winningPlayer = gameWinningSignal.WinningPlayer!;
            }
        }
    }
}
