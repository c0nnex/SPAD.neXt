using SPAD.neXt.Interfaces;
using SPAD.neXt.Interfaces.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces
{
    public class PerformanceChecker : IDisposable
    {
        public event EventHandler<string> OnMark;
        public static ILogger logger { get; set; }
        public static bool DoLogPerformance { get; set; } = false;
        bool doLog = false;
        Stopwatch Stopwatch;
        TimeSpan stLastMark = TimeSpan.Zero;
        string Name;
        int marker = 0;
        static PerformanceChecker()
        {           
        }
        public PerformanceChecker(string name, bool doLog = false)
        {
            Name = name;
            this.doLog = doLog;
            Stopwatch = Stopwatch.StartNew();
        }

        public void Mark(string info = null, string userMsg = null, bool isInternal = false)
        {
            if (DoLogPerformance || doLog)
            {
                var stNow = Stopwatch.Elapsed;
                logger?.Info($"{Name} MARK {marker++} {info} {stNow - stLastMark} / {stNow}");
                stLastMark = stNow;
            }
            if (!isInternal)
                OnMark?.Invoke(this, String.IsNullOrEmpty(userMsg) ? info : userMsg);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stopwatch.Stop();
                    if (DoLogPerformance || doLog)
                    {
                        var stNow = Stopwatch.Elapsed;
                        logger?.Info($"{Name} DONE {stNow - stLastMark} / {stNow}");
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~PeformanceChecker() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
