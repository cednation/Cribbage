namespace Cribbage
{
    public interface IGameWinningSignal
    {
        IPlayer? WinningPlayer { get; }
        void Signal(IPlayer winningPlayer);
    }

    public class GameWinningSignal : IGameWinningSignal
    {
        public IPlayer? WinningPlayer { get; private set; }

        public void Signal(IPlayer winningPlayer)
        {
            this.WinningPlayer = winningPlayer;
            
            throw new GameWinningSignalException(winningPlayer);
        }
    }

    public class DummyGameWinningSignal : IGameWinningSignal
    {
        public IPlayer? WinningPlayer => null;
        public void Signal(IPlayer winningPlayer)
        {
            throw new NotImplementedException();
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
