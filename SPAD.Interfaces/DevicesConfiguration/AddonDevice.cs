
using SPAD.neXt.Interfaces.Aircraft.CDU;
using SPAD.neXt.Interfaces.Base;
using SPAD.neXt.Interfaces.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace SPAD.neXt.Interfaces.Extension
{
    public interface IAddonDevicePreInitialize
    { }

    public interface ICustomDisplayProvider
    {
        UserControl CreateDisplay(string tag); 
    }


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
                        if (!Enum.TryParse<TextAlignment>(item.GetOption("RowAlign", "Right"), true, out var tAlign))
                            tAlign = TextAlignment.Right;
                        var dspRow = new AddonDeviceDisplayRow(item.Tag, dspRowID, r, -1, item.Display.Length, tAlign);
                        dspRow.DeviceDisplayIndex = item.DeviceCommandIndex;
                        dspRow.NoPadding = item.GetOption<bool>("NoPadding", false);
                        dspRow.NoSegmentRowEvents = item.GetOption<bool>("NoSegmentRowEvents", false);
                        DeviceDisplayDict[dspRowID] = dspRow;
                        var segAlign = item.GetOption("SegmentAlign", "Right").Split(',');
                        for (int i = 0; i < item.Display.Segments; i++)
                        {
                            var dspSegID = dspRowID + "_SEGMENT_" + (i + 1);
                            TextAlignment sAlign = tAlign;
                            if (i < segAlign.Length)
                            {
                                if (!Enum.TryParse<TextAlignment>(segAlign[i], true, out sAlign))
                                    sAlign = tAlign;
                            }

                            var dspSeg = new AddonDeviceDisplaySegment(item.Tag, dspSegID, r, i, item.Display.SegmentLength, sAlign);
                            dspSeg.NoSegmentRowEvents = item.GetOption<bool>("NoSegmentRowEvents", false);
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

        public AddonDeviceElement AddInput(AddonDeviceElement addonDeviceElement)
        {
            var input = Inputs.FirstOrDefault(x => x.Tag == addonDeviceElement.Tag);    
            if (input == null)
            {
                Inputs.Add(addonDeviceElement);
                return addonDeviceElement;
            }
            return input;
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

        public void SetOption<T>(string key, T value)
        {
            Options.RemoveAll(o => String.Compare(key, o.Key, true) == 0);
            if (value != null)
                Options.Add(new AddonDeviceOption(key, Convert.ToString(value, CultureInfo.InvariantCulture)));
        }

        public T GetOption<T>(string key, T defaultValue = default(T)) where T: IConvertible
        {
            try
            {
                var v = Options.FirstOrDefault(o => String.Compare(key, o.Key, true) == 0);
                if (v == null)
                    return defaultValue;

                return v.GetValue<T>();
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
        protected ILogger logger;
        public int RowIndex;
        public int Index;
        public int Length;
        public string Value = "";
        public string EventID;
        public string Tag;
        public object Data;
        public event EventHandler<AddonDeviceDisplayData, string> OnValueUpdated;
        public event EventHandler<AddonDeviceDisplayData, string> OnDeviceUpdate;
        public TextAlignment TextAlignment = TextAlignment.Right;
        public bool IsRow => Index == -1;
        public int DisplayCacheIndex = -1;
        public int DeviceDisplayIndex = 0;
        public bool NoPadding = false;
        public bool NoSegmentRowEvents = false;
        public Func<string, int, string> PadMe = (input, len) => input == null ? "".PadRight(len).Left(len) : input.PadRight(len).Left(len);
        protected AddonDeviceDisplayData(string tag, string eventID, int rowIndex, int index, int length, TextAlignment alignment)
        {
            Tag = tag;
            EventID = eventID;
            RowIndex = rowIndex;
            Index = index;
            Length = length;
            TextAlignment = alignment;
            if (alignment == TextAlignment.Left)
                PadMe = (input, len) => input == null ? "".PadLeft(len).Right(len) : input.PadLeft(len).Right(len);
            Value = "".PadRight(Length);
        }

        protected void RaiseOnValueUpdated(bool sendToDevice = true)
        {
            logger?.Debug($"DisplayOnValueUpdated {this} {sendToDevice}");
            if (sendToDevice)
                OnDeviceUpdate?.Invoke(this, Value);
            OnValueUpdated?.Invoke(this, Value);
        }
        public abstract void UpdateValue(string newValue, bool sendToDevice = true);

        public void SetLogger(ILogger logger)
        {
            this.logger = logger;
        }
    }

    public class AddonDeviceDisplayRow : AddonDeviceDisplayData
    {
        public List<AddonDeviceDisplaySegment> Segments = new List<AddonDeviceDisplaySegment>();

        public AddonDeviceDisplayRow(string tag, string eventID, int rowIndex, int index, int length, TextAlignment alignment) : base(tag, eventID, rowIndex, index, length, alignment)
        {
        }

        public void Segment_ValueUpdated(AddonDeviceDisplayData segment, string newValue)
        {
            if (!NoPadding)
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
            }
            else
            {
                // TODO Compose segments unpadded
            }
            RaiseOnValueUpdated();
        }

        public void AddSegment(AddonDeviceDisplaySegment segment)
        {
            if (!NoSegmentRowEvents)
                segment.OnValueUpdated += Segment_ValueUpdated;
            Segments.Add(segment);
        }

        public override void UpdateValue(string newValue, bool sendToDevice)
        {
            logger?.Debug($"UpdateRow {EventID} old '{Value}' new '{newValue}' {sendToDevice}");
            if (!NoPadding)
            {
                newValue = PadMe(newValue, Length);
                if (Segments.Count > 0)
                {
                    foreach (var item in Segments)
                    {
                        var segVal = newValue.Substring(item.Index * item.Length, item.Length);
                        item.UpdateValue(segVal, sendToDevice);
                    }
                }
                else
                {
                    Value = newValue;
                }
            }
            else
            {
                Value = newValue;
            }
            RaiseOnValueUpdated(sendToDevice);
        }

        public override string ToString()
        {
            return $"DisplayRow {RowIndex} '{Value}'";
        }
    }

    public class AddonDeviceDisplaySegment : AddonDeviceDisplayData
    {
        public AddonDeviceDisplaySegment(string tag, string eventID, int rowIndex, int index, int length, TextAlignment alignment) : base(tag, eventID, rowIndex, index, length, alignment)
        {
            Value = "".PadRight(length);
        }

        public override void UpdateValue(string newValue, bool sendToDevice)
        {
            var nVal = newValue;
            if (!NoPadding)
               nVal= PadMe(newValue, Length);
            logger?.Debug($"UpdateSegment {EventID} old '{Value}' new '{nVal}' {sendToDevice}");
            Value = nVal;
            RaiseOnValueUpdated(sendToDevice);
        }

        public override string ToString()
        {
            return $"DisplaySegment {RowIndex}:{Index} '{Value}'";
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
        [XmlAttribute]
        [Category("Data")]
        public int DeviceCommandIndex { get; set; } = -1;
        public bool ShouldSerializeDeviceCommandIndex() => DeviceCommandIndex != -1;

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
        public bool IsInput { get; set; } = true;

        [XmlIgnore]
        public AddonDeviceDisplay Display { get; private set; } = null;
        [XmlIgnore]
        public bool NeedMapping { get; set; } = true;

        [XmlIgnore]
        public DeviceInputTypes InputType
        {
            get
            {
                switch (Type)
                {
                    case "DISPLAY": return DeviceInputTypes.Display;
                    case "ENCODER": return DeviceInputTypes.Encoder;
                    case "PUSHBUTTON": return DeviceInputTypes.Button;
                    case "SWITCH": return DeviceInputTypes.Switch;
                    case "LED": return DeviceInputTypes.Led;
                    default:
                        return DeviceInputTypes.Unkown;
                }
            }
        }

        public bool HasPosition => Width != 0 || Height != 0 || Left != 0 || Top != 0;

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
            if (inStr == "UNKNOWN")
                return null;
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

        public bool HasOption(string key)
        {
            return Options.Any(o => String.Compare(o.Key, key, true) == 0);
        }

        public AddonDeviceElement WithOption(string key, object value)  
        {
            if (!HasOption(key))
                Options.Add(new AddonDeviceOption(key, value.ToString()));
            return this;
        }
        public void AddInherit(string baseClass)
        {
            if (String.IsNullOrEmpty(Inherit))
            {
                Inherit = baseClass;
                return;
            }
            var oI = new HashSet<string>(Inherit.Split(','));
            oI.Add(baseClass);
            Inherit = String.Join(",", oI);
        }
        public bool DoesInherit(string baseClass)
        {
            if (String.IsNullOrEmpty(Inherit))
            {
                return false;
            }
            var oI = new HashSet<string>(Inherit.Split(','));
            return oI.Contains(baseClass);
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

        public T GetValue<T>() where T : IConvertible
        {
            try
            {
                object res;
                if (typeof(T) == typeof(Guid))
                {
                    res = Guid.Parse(Value);
                    return (T)res;
                }
                if (typeof(T) == typeof(bool))
                {
                    res = Value == "1" || String.Compare(Value, "true", true) == 0;
                }
                else
                    res = Convert.ChangeType(Value, typeof(T),CultureInfo.InvariantCulture);
                return (T)res; 
            }
            catch
            {
                return default;
            }
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
        [XmlAttribute]
        public string DisplayAs { get; set; }
        [XmlAttribute]
        public string StateStore { get; set; }
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
