using System;
using System.Threading;
namespace SPAD.neXt.Interfaces.Base
{
    public interface ISPADBackgroundThread
    {
        string WorkerName { get; }
        bool IsRunning { get; }
        TimeSpan Interval { get; }
        bool ScheduleOnTimeout { get; }
        EventWaitHandle SignalHandle { get; }
        EventWaitHandle StopHandle { get; }
        bool IsPaused { get; }


        bool IsActive();
        void Signal();
        void Start();
        void Start(uint waitInterval);
        void Start(uint waitInterval, object argument);
        void Pause();
        void Continue();
        bool CanContinue();
        void Stop();
        void Abort();


        void SetImmuneToPreStop(bool v);
        void SetArgument(object argument);
        void SetContinousMode(bool mode);
        void SetIntervall(TimeSpan newInterval);
        void SetScheduleOnTimeout(bool mode);
    }

    public interface ISPADBackgroundWorker
    {
        void BackgroundProcessingStart(ISPADBackgroundThread workerThread, object argument);
        void BackgroundProcessingContinue(ISPADBackgroundThread workerThread, object argument);
        void BackgroundProcessingStop(ISPADBackgroundThread workerThread, object argument);
        void BackgroundProcessingDoWork(ISPADBackgroundThread workerThread, object argument);
    }
}
