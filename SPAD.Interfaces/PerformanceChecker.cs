﻿using SPAD.neXt.Interfaces;
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
        public static ILogger logger { get; set; }
        public static bool DoLogPerformance { get; set; } = false;
        bool doLog = false;
        ulong stTicks;
        string Name;
        int marker = 0;
        static PerformanceChecker()
        {
            // DoLogPerformance = System.Environment.MachineName == "DESKTOP-40RM7LC";
        }
        public PerformanceChecker(string name,bool doLog = false)
        {
            Name = name;
            stTicks = EnvironmentEx.TickCount64;
            this.doLog = doLog;
        }

        public void Mark(string info=null)
        {
            if (DoLogPerformance || doLog) logger?.Info($"{Name} {marker++} {info} {EnvironmentEx.TickCount64 - stTicks}ms");
        }
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (DoLogPerformance || doLog) logger?.Info($"{Name} . {EnvironmentEx.TickCount64 - stTicks}ms");
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
