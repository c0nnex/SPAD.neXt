using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SPAD.neXt.Interfaces.HID
{

    public delegate void InsertedEventHandler();
    public delegate void RemovedEventHandler();

    public enum DeviceMode
    {
        NonOverlapped = 0,
        Overlapped = 1
    }

    [Flags]
    public enum ShareMode
    {
        Exclusive = 0,
        ShareRead =1,
        ShareWrite = 2
    }

    public enum ReadStatus
    {
        Success = 0,
        WaitTimedOut = 1,
        WaitFail = 2,
        NoDataRead = 3,
        ReadError = 4,
        NotConnected = 5
    }

    public interface IHidDeviceData
    {
        byte[] Data { get; }
        ReadStatus Status { get; }
    }

    public interface IHidReport
    {
        byte[] Data { get; set; }
        byte[] RawData { get; set; }
        bool Exists { get; }
        ReadStatus ReadStatus { get; }
        byte ReportId { get; set; }
        ulong Tick { get; }

        byte[] GetBytes();
    }

    public delegate void ReadCallback(IHidDeviceData data);
    public delegate void ReadReportCallback(IHidReport report);
    public delegate void WriteCallback(bool success);

    public interface IHidDevice : IDisposable,IUSBDevice
    {
        

        event InsertedEventHandler Inserted;
        event RemovedEventHandler Removed;

        SafeFileHandle Handle { get; }
        bool IsOpen { get; }
        bool isConnected { get; }
        bool IsConnected { get; }
        IHidDeviceCapabilities Capabilities { get; }
        IHidDeviceAttributes Attributes { get;  }
        bool MonitorDeviceEvents { get; set; }

        void OpenDevice();

        void OpenDevice(DeviceMode readMode, DeviceMode writeMode, ShareMode shareMode);
        
        void CloseDevice();

        IHidDeviceData Read();

        void Read(ReadCallback callback);

        IHidDeviceData Read(int timeout);

        void ReadReport(ReadReportCallback callback);

        IHidReport ReadReport(int timeout);
        IHidReport ReadReport();

        bool ReadFeatureData(out byte[] data, byte reportId = 0);
        

        bool ReadManufacturer(out byte[] data);

       

        void Write(byte[] data, WriteCallback callback);

        bool Write(byte[] data);

        bool Write(byte[] data, int timeout);

        void WriteReport(IHidReport report, WriteCallback callback);

        bool WriteReport(IHidReport report);

        bool WriteReport(IHidReport report, int timeout);

        IHidReport CreateReport();

        bool WriteFeatureData(byte[] data);
    }

    public interface IPololuServoController
    {
        ushort firmwareVersionMajor { get; }
        byte firmwareVersionMinor { get; }
        string firmwareVersionString { get; }
        int ServoCount { get; }

        void clearErrors();
        void disablePWM();
        void reinitialize();
        void setAcceleration(byte servo, ushort value);
        void setPWM(ushort dutyCycle, ushort period);
        void setSpeed(byte servo, ushort value);
        void setTarget(byte servo, ushort value);
    }
}
