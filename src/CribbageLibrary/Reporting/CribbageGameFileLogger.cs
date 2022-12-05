namespace Cribbage.Reporting
{
    public sealed class CribbageGameFileLogger : IDisposable
    {     
        private TextWriter? writer;
        
        public string FileName { get; }


        public CribbageGameFileLogger(string fileName)
        {
            this.FileName = fileName;
        }

        public void SubscribeToReporter(IGameReporter? reporter)
        {
            if (reporter != null)
            {
                reporter.CribbageEventNotification += this.WriteCribbageEvent;
                var handReporter = reporter.CribbageHandReporter;
                if (handReporter != null)
                {
                    handReporter.CribbageEventNotification += this.WriteCribbageEvent;
                    var thePlayReporter = handReporter.PlayReporter;
                    if (thePlayReporter != null)
                    {
                        thePlayReporter.CribbageEventNotification += this.WriteCribbageEvent;
                    }
                }
            }
        }

        public void Start()
        {
            this.writer = new StreamWriter(File.Create(this.FileName));
        }

        public void WriteCribbageEvent(CribbageReporter _, CribbageEvent cribbageEvent)
        {
            if (this.writer == null)
                throw new InvalidOperationException("Logger must have been started");

            this.writer.WriteLine(cribbageEvent.TextMessage);
        }

        public void Stop()
        {
            if (this.writer == null)
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
