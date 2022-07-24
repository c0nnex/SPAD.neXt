using SPAD.neXt.Interfaces.Base;
using SPAD.neXt.Interfaces.Extension;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SPAD.neXt.Interfaces.DevicesConfiguration
{
    public interface IDeviceConfiguration : IOptionsProvider
    {
        Guid DeviceSessionId { get; }
        IReadOnlyList<IDeviceSwitch> DeviceSwitches { get; }


        string Name { get; }
        string PublishName { get; set; }
        string Panel { get; }
        string ProductID { get; }
        IDeviceVendor Vendor { get; }
        int ProductIDAsInt { get; }
        int Order { get; }
        bool ProcessDeviceData { get; }
        bool EnableFeatureData { get; }
        int FeatureUpdateThrottle { get; }
        string DeviceType { get; }
        string DeviceMenu { get; }
        bool NoEventsAutoRemove { get; }
        bool NoCalibration { get; set; }
        bool PageSupport { get; set; }
        bool HasDeviceSwitch(string name);
        IList<EventMapping> EventMappings { get; }

        void CreateVirtualInputs(IInputDevice gameDevice);
        void Clear();
        void Save();
        void AddDeviceSwitch(IDeviceSwitch newSwitch);
        bool AddDeviceSwitch(string xmlSwitchFragment);
        IDeviceSwitch GetDeviceSwitch(string name);
        IDeviceSwitch CreateDeviceSwitch();
        string TransformSwitchNameForResources(string switchName);
    }

    public interface IDeviceCalibration
    {
        IReadOnlyList<IAxisCalibration> Axes { get; }

        void ApplyCalibration(IInputDevice joystick);
        IAxisCalibration GetAxisCalibration(IInputAxis axis);
        void ImportJoystick(IInputDevice joystick);
        void SaveToFile();
    }

    public interface IAxisCalibration
    {
        int Deadzone { get; set; }
        string DisplayName { get; set; }
        int Maximum { get; set; }
        int Minimum { get; set; }
        string Name { get; }
        int Smoothness { get; set; }
        int MinimumDelta { get; set; }

        void ApplyTo(IInputAxis axis);
        void ImportFrom(IInputAxis axis);

    }
    [Serializable]
    public sealed class DeviceLocalization
    {
        [XmlElement(ElementName = "Entry")]
        public List<DeviceLocalizationEntry> Entries { get; set; } = new List<DeviceLocalizationEntry>();
    }
    [Serializable]
    public sealed class DeviceLocalizationEntry
    {
        [XmlAttribute]
        public string Id { get; set; }
        [XmlAttribute]
        public string Add { get; set; }
        [XmlAttribute]
        public string All { get; set; }
        [XmlAttribute]
        public string Label { get; set; }
        [XmlAttribute]
        public string Config { get; set; }
        [XmlText]
        public string Description { get; set; }
    }

}
