namespace Cribbage
{
    public interface IGameWinningSignalSource
    {
        void Signal(IPlayer winningPlayer);
    }

    public class GameWinningSignalSource : IGameWinningSignalSource
    {
        private bool signaled;

        public void Signal(IPlayer winningPlayer)
        {
            if (signaled)
                throw new InvalidOperationException("Game winner already signaled");

            this.signaled = true;
            throw new GameWinningSignalException(winningPlayer);
        }
    }

    public class GameWinningSignalException : Exception
    {
        public IPlayer WinningPlayer { get; }

        public GameWinningSignalException(IPlayer winningPlayer)
        {
            this.WinningPlayer = winningPlayer;
        }
    }
}
