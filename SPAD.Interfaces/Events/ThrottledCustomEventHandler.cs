using System;
using System.Timers;

namespace SPAD.neXt.Interfaces.Events
{
    public class ThrottledCustomEventHandler<TArgs> : IDisposable
    {
        private readonly Action<TArgs> _innerHandler;
        private readonly Action<TArgs> _outerHandler;
        private readonly Timer _throttleTimer;

        private readonly object _throttleLock = new object();
        private Action _delayedHandler = null;
        private bool disposedValue;

        public ThrottledCustomEventHandler(Action<TArgs> handler, TimeSpan delay)
        {
            _innerHandler = handler;
            _outerHandler = HandleIncomingEvent;
            if (delay != TimeSpan.Zero)
            {
                _throttleTimer = new Timer(delay.TotalMilliseconds);
                _throttleTimer.Elapsed += Timer_Tick;
            }
        }

        public void HandleIncomingEvent(TArgs args)
        {
            lock (_throttleLock)
            {
                if (_throttleTimer != null && _throttleTimer.Enabled)
                {
                    _delayedHandler = () => SendEventToHandler(args);
                }
                else
                {
                    SendEventToHandler(args);
                }
            }
        }

        private void SendEventToHandler(TArgs args)
        {
            if (_innerHandler != null)
            {
                _innerHandler(args);
                _throttleTimer?.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs args)
        {
            lock (_throttleLock)
            {
                _throttleTimer.Stop();
                if (_delayedHandler != null)
                {
                    _delayedHandler();
                    _delayedHandler = null;
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _throttleTimer?.Stop();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ThrottledCustomEventHandler()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
