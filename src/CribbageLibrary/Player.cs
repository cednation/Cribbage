namespace Cribbage
{
    public interface IPlayer
    {
        string Name { get; }
        IPlayerHand Hand { get; }
        int Score { get; }

        void ResetHand();
        void SetWinningSignalSource(IGameWinningSignalSource winningSignalSourceSource);
        void AddScore(int points);
    }

    public class Player : IPlayer
    {
        private readonly IPlayerHandFactory playerHandFactory;
        private IGameWinningSignalSource? winningSignalSource;

        public string Name { get; }
        public IPlayerHand Hand { get; protected set; }
        public int Score { get; protected set; }

        public Player(string name, IPlayerHandFactory factory)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Not a valid player name", nameof(name));

            this.Name = name;
            this.playerHandFactory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.Score = 0;

            this.Hand = this.playerHandFactory.CreatePlayerHand();
        }

        public void ResetHand()
        {
            this.Hand = this.playerHandFactory.CreatePlayerHand();
        }

        public void SetWinningSignalSource(IGameWinningSignalSource winningSignalSourceSource)
        {
            this.winningSignalSource = winningSignalSourceSource;
        }

        public void AddScore(int points)
        {
            this.Score += points;

            // Did we win?
            if (this.Score >= 121 && this.winningSignalSource != null)
                this.winningSignalSource.Signal(this);
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
