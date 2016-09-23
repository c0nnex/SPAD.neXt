using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Callout
{    

    public interface ICallout : IDisposable
    {
        /// <summary>
        /// Unique id for this callout
        /// </summary>
        long CalloutID { get; }

        /// <summary>
        /// Timestamp when the callout was scheduled
        /// </summary>
        DateTime When { get; }

        /// <summary>
        /// Timespan when a reschedule will occour
        /// </summary>
        TimeSpan Delay { get; set; }

        /// <summary>
        /// Set to true if the callout should not be rescheduled
        /// </summary>
        bool Done { get; set; }

        /// <summary>
        /// True if the callout will reschedule itself
        /// </summary>
        bool Persistent { get; }       

        /// <summary>
        /// Owner of the callout
        /// </summary>
        object Owner { get; }  

        /// <summary>
        /// Optional parameter
        /// </summary>
        object Parameter { get; }

        /// <summary>
        /// terminate any pending schedules for this callout
        /// </summary>
        void EndSchedule();

        /// <summary>
        /// Enforce the callout to be rescheduled (even if it was marked as one-time only)
        /// </summary>
        void Reschdedule();

    }
    
    /// <summary>
    /// Interface for periodic scheduled tasks. 
    /// Do not use Timers!
    /// </summary>
    public interface ICalloutManager
    {
        /// <summary>
        /// Create a callout (timer)
        /// </summary>
        /// <param name="owner">Owner of the callout. There is a limit of 50 callouts per owner</param>
        /// <param name="callback">the callback that will be called when the callout occours</param>
        /// <param name="parameter">optional parameter for the callback</param>
        /// <param name="delay">When or how often shall the callout occour. Callouts have a accurateness of +/- 150ms</param>
        /// <param name="persistant">if true the callout will reschedule itself again after begin executed. if false the callout will be disposed automatically after one occourance</param>
        /// <returns></returns>
        ICallout AddCallout(object owner, EventHandler<ICallout> callback, object parameter , TimeSpan delay, bool persistant);

        /// <summary>
        /// Unschedule a callout
        /// </summary>
        /// <param name="callout"></param>
        void RemoveCallout(ICallout callout);

        /// <summary>
        /// Unschedule a callout
        /// </summary>
        /// <param name="calloutId"></param>
        void RemoveCallout(long calloutId);

        /// <summary>
        /// Unschedule all callouts for a specific owner
        /// </summary>
        /// <param name="owner"></param>
        void RemoveAllCalloutsFor(object owner);
    }
}
