﻿
using SPAD.neXt.Interfaces.Base;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SPAD.neXt.Interfaces.Extension
{
    [Serializable]
    public class AddonDevice
    {
        [XmlAttribute]
        public string Version { get; set; }
        [XmlAttribute]
        public string ID { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Author { get; set; }
        [XmlAttribute]
        public string Image { get; set; }
        public bool ShouldSerializeImage() => false;
        [XmlAttribute]
        public string PublishName { get; set; }

        [XmlAttribute]
        public long Created { get; set; }
        public string ImageData { get; set; }

        [XmlElement(ElementName = "Input")]
        public List<AddonDeviceElement> Inputs { get; set; } = new List<AddonDeviceElement>();
        [XmlElement(ElementName = "Option")]
        public List<AddonDeviceOption> Options { get; set; } = new List<AddonDeviceOption>();


        [XmlIgnore]
        public string BasePath { get; set; }
        [XmlIgnore]
        public DateTime CreationDate => DateTime.FromBinary(Created);

        [XmlIgnore]
        public ConcurrentDictionary<string, AddonDeviceCommandMapping> DeviceCommandMappingDict = new ConcurrentDictionary<string, AddonDeviceCommandMapping>();
        [XmlIgnore]
        public ConcurrentDictionary<string, AddonDeviceDisplayData> DeviceDisplayDict = new ConcurrentDictionary<string, AddonDeviceDisplayData>();

        public void FixUp() // Create LookupTable for faster processing
        {
            DeviceCommandMappingDict.Clear();
            DeviceDisplayDict.Clear();
            foreach (var item in Inputs)
            {
                item.FixUp();
                foreach (var m in item.Mappings)
                {
                    m.Tag = item.Tag;
                    DeviceCommandMappingDict[m.In] = m;
                }
                if (item.IsDisplay)
                {
                    for (int r = 0; r < item.Display.Rows; r++)
                    {
                        var dspRowID = item.Tag + "_ROW_" + (r + 1);
                        var dspRow = new AddonDeviceDisplayRow(item.Tag,dspRowID, r, -1, item.Display.Length);

                        DeviceDisplayDict[dspRowID] = dspRow;

                        for (int i = 0; i < item.Display.Segments; i++)
                        {
                            var dspSegID = dspRowID + "_SEGMENT_" + (i + 1);
                            var dspSeg = new AddonDeviceDisplaySegment(item.Tag,dspSegID, r, i, item.Display.SegmentLength);
                            dspRow.AddSegment(dspSeg);
                            DeviceDisplayDict[dspSegID] = dspSeg;
                        }
                    }
                }
            }
        }

        public AddonDeviceElement CreateNewInput(string baseName)
        {
            int i = 0;
            string newTag = "ERROR";
            do
            {
                i++;
                newTag = baseName + "_" + i;
                if (i > 1024)
                    return null;
            } while (Inputs.Any(e => e.Tag == newTag));
            return new AddonDeviceElement() { Tag = newTag };
        }

        public string CreateNewTag(string baseName)
        {
            int i = 0;
            string newTag = "ERROR";
            do
            {
                i++;
                newTag = baseName + "_" + i;
                if (i > 1024)
                    return "ERROR";
            } while (Inputs.Any(e => e.Tag == newTag));
            return newTag;
        }

        public void SetOption<T>(string key, T value) where T : class
        {
            Options.RemoveAll(o => String.Compare(key, o.Key, true) == 0);
            if (value != null)
                Options.Add(new AddonDeviceOption(key, Convert.ToString(value, CultureInfo.InvariantCulture)));
        }

        public T GetOption<T>(string key, T defaultValue = default(T))
        {
            try
            {
                var v = Options.FirstOrDefault(o => String.Compare(key, o.Key, true) == 0);
                if (v != null)
                    return (T)Convert.ChangeType(v.Value, typeof(T), CultureInfo.InvariantCulture);
            }
            catch
            {

            }
            return defaultValue;
        }
    }

    public class AddonDeviceDisplay
    {
        public string ID;
        public int Length;
        public int Rows = 1;
        public int Segments;
        public int SegmentLength;

        public string DefaultValue;
    }

    public abstract class AddonDeviceDisplayData
    {
        public int RowIndex;
        public int Index;
        public int Length;
        public string Value = "";
        public string EventID;
        public string Tag;
        public object Data;
        public event EventHandler<AddonDeviceDisplayData, string> OnValueUpdated;

        public bool IsRow => Index == -1;
        protected AddonDeviceDisplayData(string tag, string eventID, int rowIndex, int index, int length)
        {
            Tag = tag;
            EventID = eventID;
            RowIndex = rowIndex;
            Index = index;
            Length = length;
            Value = "".PadRight(Length);
        }

        protected void RaiseOnValueUpdated()
        {
            OnValueUpdated?.Invoke(this, Value);
        }
        public abstract void UpdateValue(string newValue);
    }

    public class AddonDeviceDisplayRow : AddonDeviceDisplayData
    {
        public List<AddonDeviceDisplaySegment> Segments = new List<AddonDeviceDisplaySegment>();

        public AddonDeviceDisplayRow(string tag, string eventID, int rowIndex, int index, int length) : base(tag, eventID, rowIndex, index, length)
        {
        }

        public void Segment_ValueUpdated(AddonDeviceDisplayData segment, string newValue)
        {
            if (Segments.Count > 1)
            {
                var oldValue = Value.PadRight(Length);
                Value = oldValue.Left(segment.Index * segment.Length);
                Value += newValue;
                Value += oldValue.Right(Length - (segment.Index * segment.Length) - segment.Length);
            }
            else
            {
                Value = newValue.PadRight(Length).Left(Length);
            }
            RaiseOnValueUpdated();
        }

        public void AddSegment(AddonDeviceDisplaySegment segment)
        {
            segment.OnValueUpdated += Segment_ValueUpdated;
            Segments.Add(segment);
        }

        public override void UpdateValue(string newValue)
        {
            newValue = newValue.PadRight(Length).Left(Length);
            foreach (var item in Segments)
            {
                var segVal = newValue.Substring(item.Index * item.Length, item.Length);
                item.UpdateValue(segVal);
            }
            RaiseOnValueUpdated();
        }
    }

    public class AddonDeviceDisplaySegment : AddonDeviceDisplayData
    {
        public AddonDeviceDisplaySegment(string tag, string eventID, int rowIndex, int index, int length) : base(tag, eventID, rowIndex, index, length)
        {
        }

        public override void UpdateValue(string newValue)
        {
            if (newValue != Value)
            {
                Value = newValue.PadRight(Length).Left(Length);
                RaiseOnValueUpdated();
            }
        }

    }


    [Serializable]
    public class AddonDeviceElement
    {
        [XmlAttribute]
        [Category("Data")]
        public string Type { get; set; } = "PUSHBUTTON";
        [XmlAttribute]
        [Category("Data")]
        public string Tag { get; set; }
        [XmlAttribute]
        [Category("Data")]
        public string Inherit { get; set; }

        [XmlElement(ElementName = "Mapping")]
        [Category("Data")]
        public List<AddonDeviceCommandMapping> Mappings { get; set; } = new List<AddonDeviceCommandMapping>();

        [XmlElement(ElementName = "Option")]
        [Category("Options")]
        public List<AddonDeviceOption> Options { get; set; } = new List<AddonDeviceOption>();

        public bool ShouldSerializeOptions() => Options != null && Options.Count > 0;

        [XmlAttribute]
        [Category("Position")]
        public double Width { get; set; }
        [XmlAttribute]
        [Category("Position")]
        public double Height { get; set; }
        [XmlAttribute]
        [Category("Position")]
        public double Left { get; set; }
        [XmlAttribute]
        [Category("Position")]
        public double Top { get; set; }

        [XmlIgnore]
        public bool IsDisplay => Type == "DISPLAY";

        [XmlIgnore]
        public AddonDeviceDisplay Display { get; private set; } = null;

        public void FixUp()
        {
            if (IsDisplay)
            {
                Display = new AddonDeviceDisplay()
                {
                    ID = Tag,
                    Length = GetOption<int>("LENGTH"),
                    Rows = GetOption<int>("ROWS", 1),
                    Segments = GetOption<int>("SEGMENTS"),
                    SegmentLength = GetOption<int>("SEGMENTLENGTH"),
                    DefaultValue = GetOption<string>("DEFAULT")
                };
            }
        }

        public AddonDeviceCommandMapping GetOrCreateMapping(string inStr, string outStr)
        {
            var oVal = Mappings.FirstOrDefault(v => v.Out == outStr);
            if (oVal != null)
                return oVal;
            oVal = new AddonDeviceCommandMapping(Tag, inStr, outStr);
            Mappings.Add(oVal);
            return oVal;
        }

        public AddonDeviceCommandMapping RemoveMapping(string cfgEvent)
        {
            var oVal = Mappings.FirstOrDefault(v => v.Out == cfgEvent);
            if (oVal != null)
            {
                Mappings.Remove(oVal);
                return oVal;
            }
            return null;
        }

        public AddonDeviceCommandMapping FindMappingByEvent(string cfgEvent)
        {
            var oVal = Mappings.FirstOrDefault(v => v.Out == cfgEvent);
            if (oVal != null)
            {
                return oVal;
            }
            return null;
        }

        public T GetOption<T>(string key, T defaultValue = default(T))
        {
            var opt = Options.FirstOrDefault(o => String.Compare(o.Key, key, true) == 0);

            if (opt == null)
                return defaultValue;

            try
            {
                return (T)Convert.ChangeType(opt.Value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
    }

    [Serializable]
    public sealed class AddonDeviceOption
    {
        [XmlAttribute]
        public string Key { get; set; }
        [XmlAttribute]
        public string Value { get; set; }

        public AddonDeviceOption()
        {
        }
        public AddonDeviceOption(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
    [Serializable]
    public class AddonDeviceCommandMapping
    {
        [XmlIgnore]
        public Action ActivateAction = () => { };
        [XmlIgnore]
        public Action DeactivateAction = () => { };

        [XmlIgnore]
        public string Tag { get; set; }

        [XmlAttribute]
        public string In { get; set; }

        [XmlAttribute]
        public string Out { get; set; }

        public AddonDeviceCommandMapping() { }
        public AddonDeviceCommandMapping(string tag, string @in, string @out)
        {
            Tag = tag;
            In = @in;
            Out = @out;
        }

        public override string ToString()
        {
            return $"Mapped Event {In} => {Tag}.{Out}";
        }
    }


}