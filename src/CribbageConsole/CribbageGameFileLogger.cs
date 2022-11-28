namespace CribbageConsole
{
    using Cribbage.Reporting;

    public sealed class CribbageGameFileLogger : IDisposable
    {     
        private TextWriter? writer;
        
        public string FileName { get; }


        public CribbageGameFileLogger(string fileName)
        {
            this.FileName = fileName;
        }

        public void Start()
        {
            this.writer = new StreamWriter(File.Create(this.FileName));
        }

        public void WriteCribbageEvent(CribbageReporter _, CribbageEvent cribbageEvent)
        {
            if (writer == null)
                throw new InvalidOperationException("Logger must have been started");

            this.writer.WriteLine(cribbageEvent.TextMessage);
        }

        public void Stop()
        {
            if (writer == null)
                throw new InvalidOperationException("Logger must have been started");

            this.writer.Flush();
            this.writer.Close();
            this.writer = null;
        }

        void IDisposable.Dispose()
        {
            if (this.writer != null)
                this.Stop();
        }
    }
}
