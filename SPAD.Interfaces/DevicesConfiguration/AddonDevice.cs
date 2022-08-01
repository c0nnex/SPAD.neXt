
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

    public enum DeviceAllowLocal
    {
        NONE,
        ANY,
        AUTHOR
    }

    public class AddonDeviceColorSet : AddonDeviceOptionObject
    {
        public string Key { get; set; }
    }

    [Serializable]
    public class AddonDevice : AddonDeviceOptionObject
    {

        [XmlAttribute(AttributeName = "Version")]
        public string _Version { get; set; } = "0.0";

        [XmlIgnore]
        public Version Version
        {
            get
            {
                if (Version.TryParse(_Version, out var v)) return v;
                return new Version(0, 0);
            }
            set { _Version = value.ToString(); }
        }

        [XmlAttribute]
        public string ID { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Author { get; set; }
        [XmlIgnore]
        public ulong AuthorID { get; set; } = 0;
        [XmlIgnore]
        public DeviceAllowLocal AllowDeviceEdit { get; set; } = DeviceAllowLocal.NONE;

        [XmlIgnore]
        public DeviceAllowLocal AllowProfileEdit { get; set; } = DeviceAllowLocal.ANY;

        [XmlAttribute]
        public string PublishName { get; set; }

        [XmlAttribute]
        public long Created { get; set; }


        [XmlIgnore]
        public string VendorID => GetOption<string>("VID", null);

        [XmlIgnore]
        public string ProductID => GetOption<string>("PID", null);

        public string ImageData { get; set; }

        [XmlElement(ElementName = "Input")]
        public List<AddonDeviceElement> Inputs { get; set; } = new List<AddonDeviceElement>();

        [XmlIgnore]
        public List<AddonDeviceElement> Outputs => Inputs.Where(ii => ii.IsOutput).ToList();
        [XmlIgnore]
        public List<AddonDeviceElement> InputsOnly => Inputs.Where(ii => !ii.IsOutput).ToList();

        [XmlElement("EventMapping", IsNullable = false)]
        public List<EventMapping> EventMappings { get; set; } = new List<EventMapping>();
        [XmlElement(ElementName = "Colorset")]
        public List<AddonDeviceColorSet> Colorsets { get; set; } = new List<AddonDeviceColorSet>();
        [XmlElement(ElementName = "Mapping")]
        public List<AddonDeviceCommandMapping> CommandMappings { get; set; } = new List<AddonDeviceCommandMapping>();
        [XmlElement(ElementName = "Import")]
        public List<string> Imports { get; set; } = new List<string>();

        [XmlIgnore]
        public string BasePath { get; set; }
        [XmlIgnore]
        public DateTime CreationDate => DateTime.FromBinary(Created);
        [XmlIgnore]
        public string ResourceBaseKey => GetOption("RESOURCE_KEY", VariableBaseKey);
        [XmlIgnore]
        public string VariableBaseKey => variableBaseKey;

        private string variableBaseKey;

        [XmlIgnore]
        public ConcurrentDictionary<string, AddonDeviceCommandMapping> DeviceCommandMappingDict = new ConcurrentDictionary<string, AddonDeviceCommandMapping>(StringComparer.InvariantCultureIgnoreCase);
        [XmlIgnore]
        public ConcurrentDictionary<string, AddonDeviceDisplayData> DeviceDisplayDict = new ConcurrentDictionary<string, AddonDeviceDisplayData>(StringComparer.InvariantCultureIgnoreCase);


        public void ProcessImports(Func<string, AddonDevice> loadCallback)
        {
            foreach (var item in Imports)
            {
                AddonDevice importDevice = null;
                if (loadCallback != null)
                    importDevice = loadCallback(item);
                if (importDevice != null)
                {
                    importDevice.Options.ForEach(option => AddOption(option.Key, option.Value));
                    importDevice.Inputs.ForEach(input => AddInput(input));
                }
            }
        }

        public void FixUp(IApplication applicationProxy) // Create LookupTable for faster processing
        {
            DeviceCommandMappingDict.Clear();
            DeviceDisplayDict.Clear();
            foreach (var item in InputsOnly)
            {
                item.FixUp();
                foreach (var m in item.Mappings)
                {
                    m.Tag = item.Tag;
                    m.FixUp();
                    DeviceCommandMappingDict[m.In] = m;
                }
               
            }
            foreach (var item in Outputs)
            {
                item.FixUp();
                if (item.IsDisplay)
                {
                    for (int r = 0; r < item.Display.Rows; r++)
                    {
                        var dspRowID = item.Tag + "_ROW_" + (r + 1);
                        if (!Enum.TryParse<TextAlignment>(item.GetOption("RowAlign", "Right"), true, out var tAlign))
                            tAlign = TextAlignment.Right;
                        var dspRow = new AddonDeviceDisplayRow(item.Tag, dspRowID, r, -1, item.Display.Length, tAlign);
                        dspRow.DeviceDisplayIndex = item.DeviceCommandIndex;
                        dspRow.NoPadding = item.GetOption<bool>("NoPadding", GetOption("DSP_NOPADDING", false));
                        dspRow.NoSegmentRowEvents = item.GetOption<bool>("NoSegmentRowEvents", GetOption("DSP_NoSegmentRowEvents", false));
                        DeviceDisplayDict[dspRowID] = dspRow;
                        item.Display.DisplayRows.Add(dspRow);
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
                            dspSeg.NoSegmentRowEvents = item.GetOption<bool>("NoSegmentRowEvents", dspRow.NoSegmentRowEvents);
                            dspRow.AddSegment(dspSeg);
                            DeviceDisplayDict[dspSegID] = dspSeg;
                        }
                    }
                }
            }
            foreach (var m in CommandMappings)
            {
                m.FixUp();
                DeviceCommandMappingDict[m.In] = m;
            }
            var baseVarkey = "";
            if (HasOption("VARIABLE_KEY"))
                baseVarkey = GetOption<string>("VARIABLE_KEY");
            else
            {
                if (!String.IsNullOrEmpty(ID))
                {
                    var knownDevKey = applicationProxy.GetApplicationOption<string>("AddonDevice." + ID);
                    if (!String.IsNullOrEmpty(knownDevKey))
                        baseVarkey = knownDevKey;
                }
            }
            if (String.IsNullOrEmpty(baseVarkey))
            {
                if (String.IsNullOrEmpty(VendorID) || String.IsNullOrEmpty(ProductID))
                {
                    //applicationProxy.GetLogger("AddonDevice").WarnWithNotification(ID+" : Device without Vendor/Product ID!");
                    baseVarkey = "";
                }
                else
                    baseVarkey = VendorID + "_" + ProductID + "_";
            }
            variableBaseKey = baseVarkey;
        }

        public AddonDevice WithVendor(string vendorId, string productId)
        {
            SetOption("VID", vendorId);
            SetOption("PID", productId);
            return this;
        }
        public AddonDevice WithOption(string key, string value)
        {
            SetOption(key, value);
            return this;
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

        public bool HasInput(string tag) => Inputs.Any(x => x.Tag == tag);
        public AddonDeviceElement GetInput(string tag) => Inputs.FirstOrDefault(x =>x.Tag == tag);
        public AddonDeviceElement GetOutput(string tag) => Inputs.FirstOrDefault(x => x.Tag == tag && x.IsOutput);

        public AddonDeviceElement GetOrCreateInput(string tag, Func<AddonDeviceElement> pCreate)
        {
            if (HasInput(tag))
                return Inputs.FirstOrDefault(i => i.Tag == tag);
            if (pCreate == null)
                return null;
            var x = pCreate();
            Inputs.Add(x);
            return x;
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






        public void UpdateDisplay(string displayTag, string value, bool sendToDevice)
        {
            if (DeviceDisplayDict.TryGetValue(displayTag, out var display))
                display.UpdateValue(value, sendToDevice);
        }

        public void RegisterColorSet(string cIndex, AddonDeviceOptionObject cList)
        {
            Colorsets.RemoveAll(c => String.Compare(c.Key, cIndex, true) == 0);
            Colorsets.Add(new AddonDeviceColorSet() { Key = cIndex, Options = cList.Options });
        }

        public AddonDeviceColorSet GetColorSet(string key)
        {
            return Colorsets.FirstOrDefault(c => c.Key == key);
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

        public List<AddonDeviceDisplayRow> DisplayRows = new List<AddonDeviceDisplayRow>();
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
                nVal = PadMe(newValue, Length);
            logger?.Debug($"UpdateSegment {EventID} old '{Value}' new '{nVal}' {sendToDevice}");
            Value = nVal;
            RaiseOnValueUpdated(sendToDevice);
        }

        public override string ToString()
        {
            return $"DisplaySegment {RowIndex}:{Index} '{Value}'";
        }

    }

    public class AddonDeviceSwitchEncoder : AddonDeviceElement
    {
        [XmlIgnore]
        public int NumPositions { get => GetOption("NumPositions", 0); set => SetOption("NumPositions", value); }

        private int[] _PositionMasks;
        private string[] _PositionNames;
        [XmlIgnore]
        public int[] PositionMasks { get => _PositionMasks; set => SetOption("PositionMasks", String.Join(",", value)); }
        [XmlIgnore]
        public string[] PositionNames { get => _PositionNames; set => SetOption("PositionNames", String.Join(",", value)); }
        [XmlIgnore]
        public int ReportIndex { get => GetOption("ReportIndex", 0); set => SetOption("ReportIndex", value); }

        private int PositionCurrent = 0;

        public override void FixUp()
        {
            base.FixUp();
            _PositionMasks = new int[NumPositions];
            var dta = GetOption("PositionMasks", "").Split(',');
            for (int i = 0; i < NumPositions; i++)
            {
                int.TryParse(dta[i], out _PositionMasks[i]);
            }
            _PositionNames = GetOption("PositionNames", "").Split(',');
        }

        public override void ProcessInput(byte[] inputReport, Action<string, string, int, int, bool> raiseEventCallback, bool isStateScan = false)
        {
            for (int i = 0; i < NumPositions; i++)
            {
                var nVal = (inputReport[ReportIndex] & _PositionMasks[i]) != 0;
                if (nVal)
                {
                    if (PositionCurrent != i || isStateScan)
                    {
                        raiseEventCallback?.Invoke(Tag, _PositionNames[i], _PositionMasks[i], _PositionMasks[i], isStateScan);
                        PositionCurrent = i;
                        break;
                    }
                }
            }
        }
    }

    public class AddonDeviceRotaryPosition
    {
        private int? uIValue = null;

        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public int UIValue { get => uIValue.GetValueOrDefault(Value); set => uIValue = value; }
        public bool ShouldSerializeUIValue() => uIValue.HasValue;
        [XmlAttribute]
        public int Value { get; set; } = 0;

    }


    public class AddonDeviceButton : AddonDeviceElement
    {
        [XmlIgnore]
        public int ReportIndex { get => GetOption("ReportIndex", 0); set => SetOption("ReportIndex", value); }
        [XmlIgnore]
        public int ReportMask { get => GetOption("ReportMask", 0); set => SetOption("ReportMask", value); }
        [XmlIgnore]
        public int ReportLen { get => GetOption("ReportLen", 1); set => SetOption("ReportLen", value); }
        [XmlIgnore]
        public bool Inverse { get => GetOption("Inverse", false); set => SetOption("Inverse", value); }

        private bool lastValue = false;

        public override void ProcessInput(byte[] inputReport, Action<string, string, int, int, bool> raiseEventCallback, bool isStateScan = false)
        {

            var nVal = getInputVal(inputReport);
            if (Inverse)
                nVal = !nVal;

            if (lastValue != nVal)
            {
                raiseEventCallback?.Invoke(Tag, nVal ? GetOption("PRESS", "PRESS") : GetOption("RELEASE", "RELEASE"), nVal ? 1 : 0, nVal ? 1 : 0, isStateScan);
            }
            lastValue = nVal;
        }

        private bool getInputVal(byte[] inputReport)
        {
            if (ReportLen == 1)
                return (inputReport[ReportIndex] & ReportMask) != 0;
            int val = 0;
            for (int i = ReportIndex; i < ReportLen; i++)
            {
                val = val << 8;
                val |= (inputReport[i]);
            }
            return (val & ReportMask) != 0;
        }
    }

    public class AddonDeviceSwitch : AddonDeviceElement
    {
        [XmlIgnore]
        public int ReportIndex { get => GetOption("ReportIndex", 0); set => SetOption("ReportIndex", value); }
        [XmlIgnore]
        public int ReportMask { get => GetOption("ReportMask", 0); set => SetOption("ReportMask", value); }
        [XmlIgnore]
        public int ReportLen { get => GetOption("ReportLen", 1); set => SetOption("ReportLen", value); }
        [XmlIgnore]
        public bool Inverse { get => GetOption("Inverse", false); set => SetOption("Inverse", value); }

        private bool lastValue = false;

        public override void ProcessInput(byte[] inputReport, Action<string, string, int, int, bool> raiseEventCallback, bool isStateScan = false)
        {

            var nVal = getInputVal(inputReport);
            if (Inverse)
                nVal = !nVal;

            if (lastValue != nVal || isStateScan)
            {
                raiseEventCallback?.Invoke(Tag, nVal ? GetOption("PRESS", "PRESS") : GetOption("RELEASE", "RELEASE"), nVal ? 1 : 0, nVal ? 1 : 0, isStateScan);
            }
            lastValue = nVal;
        }

        private bool getInputVal(byte[] inputReport)
        {
            if (ReportLen == 1)
                return (inputReport[ReportIndex] & ReportMask) != 0;
            int val = 0;
            for (int i = ReportIndex; i < ReportLen; i++)
            {
                val = val << 8;
                val |= (inputReport[i]);
            }
            return (val & ReportMask) != 0;
        }
    }

    [Serializable]
    [XmlInclude(typeof(AddonDeviceSwitch))]
    [XmlInclude(typeof(AddonDeviceSwitchEncoder))]
    [XmlInclude(typeof(AddonDeviceButton))]


    public class AddonDeviceElement : AddonDeviceOptionObject
    {
        [XmlAttribute]
        [Category("Data")]
        public string Type { get; set; } = "PUSHBUTTON";
        [XmlAttribute]
        [Category("Data")]
        public string Tag { get; set; }
        [XmlIgnore]
        public string VariableName { get; private set; }
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
        [XmlElement(ElementName = "RotaryPosition")]
        public List<AddonDeviceRotaryPosition> RotaryPositions { get; set; } = new List<AddonDeviceRotaryPosition>();

        [XmlAttribute]
        [Category("Position")]
        public double Width { get; set; }
        [XmlAttribute]
        [Category("Position")]
        public double Height { get; set; }
        [XmlAttribute(AttributeName = "Left")]
        [Category("Position")]
        public double Left { get; set; }
        [XmlAttribute(AttributeName = "Top")]
        [Category("Position")]
        public double Top { get; set; }

        [XmlAttribute(AttributeName = "Canvas.Left")]
        [Category("Obsolete")]
        public double _LeftOld { get => Left; set => Left = value; }
        public bool ShouldSerialize_LeftOld() => false;

        [XmlAttribute(AttributeName = "Canvas.Top")]
        [Category("Obsolete")]
        public double _TopOld { get => Top; set => Top = value; }
        public bool ShouldSerialize_TopOld() => false;

        [XmlIgnore]
        public bool IsDisplay => Type == "DISPLAY";
        [XmlIgnore]
        public bool IsInput { get; set; } = true;

        [XmlIgnore]
        public AddonDeviceDisplay Display { get; private set; } = null;
        [XmlIgnore]
        public bool NeedMapping { get; set; } = true;
        [XmlIgnore]
        public bool IsPanelChange { get; set; } = false;

        [XmlIgnore]
        public bool IsOutput => InputType == DeviceInputTypes.Display || InputType == DeviceInputTypes.Led;

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
                    case "PUSHBUTTON_NOMODE": return DeviceInputTypes.Button;
                    case "SWITCH": return DeviceInputTypes.Switch;
                    case "LED": return DeviceInputTypes.Led;
                    case "AXIS": return DeviceInputTypes.Axis;
                    case "ROTARY": return DeviceInputTypes.Rotary;
                    case "SWITCH3": return DeviceInputTypes.StatefulSwitch;
                    case "LABEL": return DeviceInputTypes.Label;
                    default:
                        return DeviceInputTypes.Unkown;
                }
            }
        }

        public bool HasPosition => Left != 0 || Top != 0;
        public void SetVariableName(string varName) => VariableName = varName;
        public AddonDeviceElement WithVariableName(string varName)
        {
            VariableName = varName;
            return this;
        }

        public double GetPositionValueOrDefault(double val, double defaultVal)
        {
            if (double.IsNaN(val) || val == 0)
                return defaultVal;
            return val;
        }

        public void RegisterRotaryPosition(string posName, int posValue, int posUIValue)
        {
            if (RotaryPositions.Any(x => x.Name == posName))
                return;
            RotaryPositions.Add(new AddonDeviceRotaryPosition() { Name = posName, Value = posValue, UIValue = posUIValue });
        }
        public virtual void FixUp()
        {
            if (String.IsNullOrEmpty(Inherit))
                Inherit = "MASTER";
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
            IsPanelChange = HasOption("TARGET_PANEL");

            if (HasOption("POS_NAMES"))
            {
                var posNames = GetOption("POS_NAMES", "").Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                var posValues = GetOption("POS_VALUES", "");
                var posUIValues = GetOption("POS_UIVALUES", "");
                if (posNames.Length > 0)
                {
                    for (int i = 0; i < posNames.Length; i++)
                    {
                        RegisterRotaryPosition(posNames[i], posValues.GetPart(i, "#", i), posUIValues.GetPart(i, "#", i));
                    }
                }
                RemoveOption("POS_NAMES");
                RemoveOption("POS_VALUES");
                RemoveOption("POS_UIVALUES");
            }

            var tList = RotaryPositions.OrderBy(r => r.Value).ToList();
            RotaryPositions = tList;
        }

        public AddonDeviceCommandMapping GetOrCreateMapping(string inStr, string outStr)
        {
            if (inStr == "UNKNOWN")
                return null;
            var oVal = Mappings.FirstOrDefault(v => v.In == inStr);
            if (oVal != null)
                return oVal;
            oVal = new AddonDeviceCommandMapping(Tag, inStr, outStr);
            Mappings.Add(oVal);
            return oVal;
        }
        public AddonDeviceCommandMapping CreateMapping(string inStr, string outStr, int activateDirection = 1)
        {
            if (inStr == "UNKNOWN")
                return null;
            var oVal = new AddonDeviceCommandMapping(Tag, inStr, outStr, activateDirection);
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


        public AddonDeviceElement WithOption(string key, object value)
        {
            if (!HasOption(key))
                Options.Add(new AddonDeviceOption(key, Convert.ToString(value, CultureInfo.InvariantCulture)));
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
        public void RemoveInherit(string baseClass)
        {
            if (String.IsNullOrEmpty(Inherit))
            {
                return;
            }
            var oI = new HashSet<string>(Inherit.Split(','));
            oI.Remove(baseClass);
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

        public void SetPosition(double top, double left, double height, double width)
        {
            Top = top;
            Left = left;
            Height = height;
            Width = width;
        }

        public virtual void ProcessInput(byte[] inputReport, Action<string, string, int, int, bool> raiseEventCallback, bool isStateScan = false)
        { }

        public override string ToString()
        {
            return $"{this.GetType()} {Type} {Tag} Options {Options.Count}";
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
                    res = !(Value == "0" || String.Compare(Value, "false", true) == 0);
                    return (T)res;
                }
                if (typeof(T) == typeof(char))
                {
                    res = Value.FirstOrDefault();
                    return (T)res;
                }
                if (typeof(T).IsEnum)
                    return (T)Enum.Parse(typeof(T), Value, true);

                res = Convert.ChangeType(Value, typeof(T), CultureInfo.InvariantCulture);
                return (T)res;
            }
            catch
            {
                return default;
            }
        }

        public override string ToString()
        {
            return Key + "=" + Value;
        }
    }

    public class AddonDeviceOptionObject : IObjectWithOptions
    {
        [XmlElement(ElementName = "Option")]
        public List<AddonDeviceOption> Options { get; set; } = new List<AddonDeviceOption>();
        public bool ShouldSerializeOptions() => Options != null && Options.Count > 0;
        public int Count => (Options == null ? 0 : Options.Count);

        public T GetOption<T>(string key, T defaultValue = default(T)) where T : IConvertible
        {
            var opt = Options.FirstOrDefault(o => String.Compare(o.Key, key, true) == 0);

            if (opt == null)
                return defaultValue;

            try
            {
                return (T)opt.GetValue<T>();
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
        public void AddOption(string key, object value, int pos = -1)
        {
            if (!HasOption(key))
            {
                if (pos == -1)
                    Options.Add(new AddonDeviceOption(key, Convert.ToString(value, CultureInfo.InvariantCulture)));
                else
                    Options.Insert(pos, new AddonDeviceOption(key, Convert.ToString(value, CultureInfo.InvariantCulture)));
            }
        }
        public void SetOption<T>(string key, T value) where T : IConvertible
        {
            Options.RemoveAll(o => String.Compare(key, o.Key, true) == 0);
            if (value != null)
                Options.Add(new AddonDeviceOption(key, Convert.ToString(value, CultureInfo.InvariantCulture)));
        }

        public int RemoveOption(string key) => Options.RemoveAll(o => String.Compare(key, o.Key, true) == 0);

        public void MergeOptions(AddonDeviceOptionObject src)
        {
            foreach (var item in src.Options)
            {
                SetOption(item.Key, item.Value);
            }
        }

        public override string ToString()
        {
            return string.Join(",", Options.Select(o => o.ToString()));
        }
    }

    [Serializable]
    public class AddonDeviceCommandMapping : AddonDeviceOptionObject
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
        [XmlAttribute]
        public int StateValue { get; set; } = int.MaxValue;
        public bool ShouldSerializeStateValue() => StateValue != int.MaxValue;
      
        public AddonDeviceCommandMapping() { }
        public AddonDeviceCommandMapping(string tag, string @in, string @out, int activateDirection = 1)
        {
            Tag = tag;
            In = @in;
            Out = @out;
            StateValue = activateDirection;
        }
        public bool IsOffName(string inputName)
        {
            switch (inputName)
            {
                case "OFF": return true;
                case "RELEASE": return true;
                default:
                    return false;
            }
        }

        public void FixUp()
        {
            if (StateValue == int.MaxValue) // No Data in config
            {
                StateValue = 1;
                if (Out.StartsWith("TUNER_"))
                {
                    if (Out.Contains("CLOCKWISE"))
                        StateValue = 1;
                    else
                        StateValue = -1;
                }
                else
                {
                    if (IsOffName(Out))
                        StateValue = 0;
                }
            }
        }

        public AddonDeviceCommandMapping WithDisplayAs(string str)
        {
            DisplayAs = str;
            return this;
        }
        public bool DoesStoreState => !String.IsNullOrEmpty(StateStore);

        public override string ToString()
        {
            return $"Mapped Event {In} => {Tag}.{Out} ({StateValue})";
        }
    }
    [Serializable]
    public sealed class EventMapping
    {
        [XmlAttribute]
        public string EventName { get; set; } = "ALL";
        [XmlAttribute]
        public string FromTrigger { get; set; }
        [XmlAttribute]
        public string ToTrigger { get; set; }

        [XmlAttribute]
        public string FromEvent { get; set; }
        [XmlAttribute]
        public string ToEvent { get; set; }
        [XmlAttribute]
        public int Priority { get; set; } = 0;
        [XmlAttribute]
        public int Version { get; set; } = 0;

        public bool IsEventMove => !String.IsNullOrEmpty(FromEvent) && !String.IsNullOrEmpty(ToEvent);
        public string Key
        {
            get
            {
                if (IsEventMove)
                    return $"{FromEvent}.{FromTrigger}.{ToEvent}.{ToTrigger}";
                return $"{EventName}.{FromTrigger}.{ToTrigger}";
            }
        }
        public override string ToString()
        {
            return $"EventMapping {FromEvent}.{FromTrigger} => {ToEvent}.{ToTrigger} P {Priority} V {Version}";
        }
    }
}
