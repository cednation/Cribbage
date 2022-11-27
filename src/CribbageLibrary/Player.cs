namespace Cribbage
{
    public interface IPlayer
    {
        string Name { get; }
        IPlayerHand Hand { get; }
        int Score { get; }

        void ResetHand();
        void SetWinningSignalSource(IGameWinningSignal winningSignalSource);
        void AddScore(int points);
    }

    public class Player : IPlayer
    {
        private readonly IPlayerHandFactory playerHandFactory;
        private IGameWinningSignal winningSignal;

        public string Name { get; }
        public IPlayerHand Hand { get; protected set; }
        public int Score { get; protected set; }

        public Player(string name, IPlayerHandFactory factory, IGameWinningSignal? winningSignal = null)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.playerHandFactory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.winningSignal = winningSignal ?? new DummyGameWinningSignal();
            this.Score = 0;

            this.Hand = this.playerHandFactory.CreatePlayerHand();
        }

        public void ResetHand()
        {
            this.Hand = this.playerHandFactory.CreatePlayerHand();
        }

        public void SetWinningSignalSource(IGameWinningSignal winningSignalSource)
        {
            this.winningSignal = winningSignalSource;
        }

        public void AddScore(int points)
        {
            this.Score += points;

            // Did we win?
            if (this.Score >= 121)
                this.winningSignal.Signal(this);
        }
    }
}
