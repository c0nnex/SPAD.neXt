
using SPAD.neXt.Interfaces.Aircraft.CDU;
using SPAD.neXt.Interfaces.Base;
using SPAD.neXt.Interfaces.Configuration;
using SPAD.neXt.Interfaces.Events;
using SPAD.neXt.Interfaces.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
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

    public class AddonDeviceColorSet : GenericOptionObject
    {
        public string Key { get; set; }
    }

    [Serializable]
    public class AddonDevice : GenericOptionObject, IObjectWithVariables
    {
        public event AsyncEventHandler<VariableChangedEventArgs> VariableChanged;
        protected ILogger logger { get; private set; }

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
        [XmlIgnore]
        public bool HasSyncSupport => GetOption("DEVICE.SYNC_SUPPORT", false);
        [XmlIgnore]
        public bool NeedsInitialSync => GetOption("DEVICE.SYNC_NEEDED", false);
        public string ImageData { get; set; }

        [XmlElement(ElementName = "Input")]
        public List<AddonDeviceElement> Inputs { get; set; } = new List<AddonDeviceElement>();

        [XmlIgnore]
        public List<AddonDeviceElement> Outputs => Inputs.Where(ii => ii.IsOutput).ToList();
        [XmlIgnore]
        public List<AddonDeviceElement> InputsOnly => Inputs.Where(ii => !ii.IsOutput && ii.InputType != DeviceInputTypes.UIElement).ToList();

        [XmlIgnore]
        public List<AddonDeviceElement> UIElements => Inputs.Where(ii => ii.InputType == DeviceInputTypes.UIElement).ToList();

        [XmlElement("EventMapping", IsNullable = false)]
        public List<EventMapping> EventMappings { get; set; } = new List<EventMapping>();
        [XmlElement(ElementName = "Colorset")]
        public List<AddonDeviceColorSet> Colorsets { get; set; } = new List<AddonDeviceColorSet>();
        [XmlElement(ElementName = "Mapping")]
        public List<AddonDeviceCommandMapping> CommandMappings { get; set; } = new List<AddonDeviceCommandMapping>();

        [XmlElement(ElementName = "OutputMapping")]
        [Category("Data")]
        public List<AddonDeviceCommandMapping> OutputMappings { get; set; } = new List<AddonDeviceCommandMapping>();
        [XmlElement(ElementName = "Variable")]
        public List<GenericVariable> Variables { get; set; } = new List<GenericVariable>();

        [XmlElement(ElementName = "DeviceEvent")]
        public List<string> DeviceEvents { get; set; } = new List<string>();

        // [XmlArray(ElementName = "Routing", Namespace = "http://www.fsgs.com/SPAD", IsNullable = true)] XmlArrayItem ... No we make root elements
        [XmlElement(ElementName = "Route", Type = typeof(AddonInputRouting))]
        public List<AddonInputRouting> Routing { get; set; } = new List<AddonInputRouting>();

        [XmlIgnore]
        public GenericOptionObject PrivateOptions { get; set; } = new GenericOptionObject();

        public List<AddonInputRouting> GetAllRoutings()
        {
            IEnumerable<AddonInputRouting> ret = new List<AddonInputRouting>();
            ret = ret.Concat(Routing);
            InputsOnly.ForEach(ii => ret = ret.Concat(ii.Routing));
            return ret.ToList();
        }

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
        [XmlIgnore]
        public ConcurrentDictionary<string, IDynamicExpression> DeviceInputMappingDict = new ConcurrentDictionary<string, IDynamicExpression>(StringComparer.InvariantCultureIgnoreCase);
        [XmlIgnore]
        public ConcurrentDictionary<string, object> DeviceSessionVariables = new ConcurrentDictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

        public AddonDevice GetSubPanelDevice(string subPanel, bool remove = false)
        {
            var retVal = new AddonDevice();
            retVal.Inputs = Inputs.Where(ii => ii.GetOption("PANEL", "UGHUIGH") == subPanel).ToList();
            retVal.Options = Options.Where(oo => oo.Key.StartsWith("PANEL." + subPanel + ".UI")).ToList();
            if (remove)
            {
                retVal.Inputs.ForEach(x => Inputs.Remove(x));
                Options.RemoveAll(oo => oo.Key.StartsWith("PANEL." + subPanel + ".UI"));
            }
            return retVal;
        }

        public bool AddRouting(AddonInputRouting routingBase)
        {
            if (Routing.Any(rt => rt.From == routingBase.From && rt.To == routingBase.To))
                return false;
            Routing.Add(routingBase);
            return true;
        }

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
                    importDevice.Variables.ForEach(v => Variables.Add(v));
                    importDevice.Inputs.ForEach(input => AddInput(input));
                    importDevice.Routing.ForEach(rt => AddRouting(rt));
                    importDevice.OutputMappings.ForEach( m => OutputMappings.Add(m));
                    importDevice.CommandMappings.ForEach(m => CommandMappings.Add(m));
                    importDevice.EventMappings.ForEach(m => EventMappings.Add(m));
                    importDevice.Colorsets.ForEach(m => Colorsets.Add(m));
                }
            }
        }

        public string MapInput(string input)
        {
            if (DeviceInputMappingDict.TryGetValue(input, out var mapping))
            {
                var val = Convert.ToString(mapping.Evaluate());
                logger.Debug(() => $"Map {input} => {val}");
                return val;
            }
            return input;
        }

        public void FixUp(IApplication applicationProxy) // Create LookupTable for faster processing
        {
            DeviceCommandMappingDict.Clear();
            DeviceDisplayDict.Clear();
            logger = applicationProxy.GetLogger($"AddonDevice.{VendorID}.{ProductID}");
            // reorder if any element has an order and then only those
            if (Inputs.Any(ii => ii.SortOrder != 0))
            {
                var tmpList = Inputs.Where(x => x.SortOrder != 0).OrderBy(x => x.SortOrder).ToList();
                Inputs.RemoveAll(x => x.SortOrder != 0);
                Inputs.InsertRange(0, tmpList);
            }
            var doStateStore = GetOption("DEVICE.STORESTATE", false);
            UIElements.ForEach(ui => ui.FixUp());
            foreach (var item in InputsOnly)
            {
                item.FixUp();
                if (item.TargetMapping != null)
                {
                    foreach (var mapItem in item.TargetMapping.InputValues)
                    {
                        DeviceInputMappingDict[mapItem] = item.TargetMapping.Mapping;
                    }
                }
                foreach (var m in item.Mappings)
                {
                    m.Tag = item.Tag;
                    m.FixUp();
                    DeviceCommandMappingDict[m.In] = m;
                    if (doStateStore && String.IsNullOrEmpty(m.StateStore))
                    {
                        if (item.InputType != DeviceInputTypes.Encoder)
                            m.StateStore = item.Tag;
                    }
                }
            }

            foreach (var item in Outputs)
            {
                item.FixUp();
                foreach (var m in item.Mappings)
                {
                    m.Tag = item.Tag;
                    m.FixUp();
                    DeviceCommandMappingDict[m.In] = m;
                    if (doStateStore && String.IsNullOrEmpty(m.StateStore))
                    {
                        if (item.InputType != DeviceInputTypes.Encoder)
                            m.StateStore = item.Tag;
                    }
                }
                foreach (var m in item.OutputMappings)
                {
                    m.Condition?.RegisterPrivateVariableFunction(GetDeviceSessionValueCallback);
                    m.Compute?.RegisterPrivateVariableFunction(GetDeviceSessionValueCallback);
                }
                SetDeviceSessionValue(item.Tag, 0);
                if (item.IsDisplay)
                {
                    SetDeviceSessionValue(item.Tag, "");
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
                            dspSeg.NoPadding = item.GetOption<bool>("NoSegmentPadding", GetOption("DSP_NOPADDING", false));
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
            foreach (var item in OutputMappings)
            {
                item.FixUp("DEVICEGLOBAL");
                item.Condition?.RegisterPrivateVariableFunction(GetDeviceSessionValueCallback);
                item.Compute?.RegisterPrivateVariableFunction(GetDeviceSessionValueCallback);
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

        public bool HasDeviceSessionValue(string name) => DeviceSessionVariables.ContainsKey(name);
        public T GetDeviceSessionValue<T>(string name, T defValue = default) 
        {
            if (DeviceSessionVariables.TryGetValue(name, out var value))
            {
                return value.GetValueAs<T>(defValue);
            }
            return defValue;
        }
        public void SetDeviceSessionValue(string name, object value)
        {
            logger.Debug("SetDeviceSessionValue {0} => {1}", name, value);
            DeviceSessionVariables[name] = value;
        }
        private ExpressionEvaluationResult GetDeviceSessionValueCallback(string name)
        {
            if (DeviceSessionVariables.TryGetValue(name, out var value))
            {
                logger.Debug("GetDeviceSessionValue('{0}') => {1}", name, value);
                return ExpressionEvaluationResult.CreateResult(value);
            }
            if (!name.Contains(':') && (name != "value"))
            {
                logger.Debug("GetDeviceSessionValue('{0}') => not known yet", name);
                // Not yet set, but a local session var
                return ExpressionEvaluationResult.CreateResult( 0 );
            }
            return ExpressionEvaluationResult.Empty;
            // Fall through. We do not know about any simvar stuff
        }

        public void SetVariableBaseKey(string newKey) => variableBaseKey = newKey;

        public AddonDevice WithVendor(string vendorId, string productId)
        {
            SetOption("VID", vendorId);
            SetOption("PID", productId);
            return this;
        }
        public AddonDevice WithOption(string key, string value, bool overwrite = false)
        {
            if (overwrite)
                SetOption(key, value);
            else
                AddOption(key, value);
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

        public bool HasRouting(string inputName) => Inputs.Any(x => x.Routing.Any(y => y.From == inputName));

        public AddonDeviceElement AddInput(AddonDeviceElement addonDeviceElement, bool ignoreIfRouted = true)
        {
            var input = Inputs.FirstOrDefault(x => x.Tag == addonDeviceElement.Tag);
            if (input == null)
            {
                if (HasRouting(addonDeviceElement.Tag))
                {
                    return null; // Routed input. Ignore it
                }
                Inputs.Add(addonDeviceElement);
                return addonDeviceElement;
            }
            return input;
        }

        public bool HasInput(string tag) => Inputs.Any(x => x.Tag == tag) || Inputs.Any(x => x.Routing.Any(y => y.From == tag));
        public AddonDeviceElement GetInput(string tag) => Inputs.FirstOrDefault(x => x.Tag == tag);
        public AddonDeviceElement GetOutput(string tag) => Inputs.FirstOrDefault(x => x.Tag == tag && x.IsOutput);
        public T GetOutput<T>(string tag) where T : class => Inputs.FirstOrDefault(x => x.Tag == tag && x.IsOutput) as T;
        public T GetInput<T>(string tag) where T : class => Inputs.FirstOrDefault(x => x.Tag == tag) as T;
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

        public void RegisterColorSet(string cIndex, GenericOptionObject cList)
        {
            Colorsets.RemoveAll(c => String.Compare(c.Key, cIndex, true) == 0);
            Colorsets.Add(new AddonDeviceColorSet() { Key = cIndex, Options = cList.Options });
        }

        public AddonDeviceColorSet GetColorSet(string key)
        {
            return Colorsets.FirstOrDefault(c => c.Key == key);
        }

        #region Variables
        IEnumerable<IGenericOption> IObjectWithVariables.Variables => Variables;

        public T GetVariable<T>(string key, T defaultValue = default(T)) where T : IConvertible
        {
            var opt = Variables.FirstOrDefault(o => String.Compare(o.Key, key, true) == 0);

            if (opt == null)
                return defaultValue;

            try
            {
                return (T)opt.GetValue<T>(defaultValue);
            }
            catch
            {
                return defaultValue;
            }
        }

        public bool HasVariable(string key)
        {
            return Variables.Any(o => String.Compare(o.Key, key, true) == 0);
        }
        public bool AddVariable(string key, object value, int pos = -1)
        {
            if (!HasVariable(key))
            {
                if (pos == -1)
                    Variables.Add(new GenericVariable(key, Convert.ToString(value, CultureInfo.InvariantCulture)));
                else
                    Variables.Insert(pos, new GenericVariable(key, Convert.ToString(value, CultureInfo.InvariantCulture)));
                return true;
            }
            return false;
        }
        public void SetVariable<T>(string key, T value)
        {
            if (value == null)
            {
                RemoveVariable(key);
            }
            else
            {
                var v = Variables.FirstOrDefault(o => String.Compare(o.Key, key, true) == 0);
                if (v == null)
                {
                    Variables.Add(new GenericVariable(key, Convert.ToString(value, CultureInfo.InvariantCulture)));
                }
                else
                {
                    v.SetValue(value);
                    VariableChanged?.FireAndForget(this, new VariableChangedEventArgs(key, value));
                }
            }
        }

        public int RemoveVariable(string key) => Variables.RemoveAll(o => String.Compare(key, o.Key, true) == 0);

        public bool MergeVariables(IObjectWithVariables src)
        {
            bool res = false;
            foreach (var item in src.Variables)
            {
                res |= AddVariable(item.Key, item.Value);
            }
            return res;
        }
        #endregion
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
        public Func<string, int, string> FillMe = (input, len) => input == null ? "".PadRight(len) : input.PadRight(len);
        protected AddonDeviceDisplayData(string tag, string eventID, int rowIndex, int index, int length, TextAlignment alignment)
        {
            Tag = tag;
            EventID = eventID;
            RowIndex = rowIndex;
            Index = index;
            Length = length;
            TextAlignment = alignment;
            if (alignment == TextAlignment.Left)
            {
                PadMe = (input, len) => input == null ? "".PadLeft(len).Right(len) : input.PadLeft(len).Right(len);
                FillMe = (input, len) => input == null ? "".PadLeft(len) : input.PadLeft(len);
            }
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
            else
                nVal = FillMe(newValue, Length);
            logger?.Debug($"UpdateSegment {EventID} old '{Value}' new '{nVal}' {sendToDevice}");
            Value = nVal;
            RaiseOnValueUpdated(sendToDevice);
        }

        public override string ToString()
        {
            return $"DisplaySegment {RowIndex}:{Index} '{Value}'";
        }

    }

    // Encoder that works like bundled switches
    public class AddonDeviceSwitchEncoder : AddonDeviceInputBase
    {
        private int _NumPositions;
        private int[] _PositionMasks;
        private string[] _PositionNames;
        private int _PositionCurrent = 0;

        [XmlIgnore]
        public int NumPositions { get => _NumPositions; set { SetOption("NumPositions", value); _NumPositions = value; } }
        [XmlIgnore]
        public int[] PositionMasks { get => _PositionMasks; set => SetOption("PositionMasks", String.Join(",", value)); }
        [XmlIgnore]
        public string[] PositionNames { get => _PositionNames; set => SetOption("PositionNames", String.Join(",", value)); }


        public AddonDeviceSwitchEncoder()
        {
            Type = "ROTARY";
        }

        public AddonDeviceSwitchEncoder AsEncoder()
        {
            Inherit = "SPAD_ENCODER_NOACC";
            Type = "ROTARY";
            return this;
        }
        public AddonDeviceSwitchEncoder AsRotarySwitch(string inherit = null)
        {
            if (!String.IsNullOrEmpty(inherit))
                Inherit = inherit;
            Type = "ROTARYSWITCH";
            return this;
        }

        public AddonDeviceSwitchEncoder WithRotaryEncoderPosition(string name, int mask, int uiValue)
        {
            RegisterRotaryPosition(name, mask, uiValue);
            return this;
        }

        public override void FixUp()
        {
            base.FixUp();
            if (NumPositions == 0)
            {
                if (RotaryPositions.Count > 0)
                {
                    NumPositions = RotaryPositions.Count;
                    PositionMasks = RotaryPositions.Select(p => p.Value).ToArray();
                    PositionNames = RotaryPositions.Select(p => p.Name).ToArray();
                }
            }
            if (NumPositions > 0)
            {
                _PositionMasks = new int[NumPositions];
                var dta = GetOption("PositionMasks", "").Split(',');
                for (int i = 0; i < NumPositions; i++)
                {
                    int.TryParse(dta[i], out _PositionMasks[i]);
                }
                _PositionNames = GetOption("PositionNames", "").Split(',');
            }
        }

        public override void ProcessInput(byte[] inputReport, Action<string, string, int, int, bool> raiseEventCallback, bool isStateScan = false, int startIndex = 0)
        {
            var iVal = getInputValRaw(inputReport, startIndex);
            if (ReportMask != 0)
                iVal &= ReportMask;
            for (int i = 0; i < NumPositions; i++)
            {
                var nVal = (iVal == _PositionMasks[i]);
                if (nVal)
                {
                    if (_PositionCurrent != i || isStateScan)
                    {
                        raiseEventCallback?.Invoke(Tag, _PositionNames[i], _PositionMasks[i], _PositionMasks[i], isStateScan);
                        _PositionCurrent = i;
                    }
                    break;
                }
            }
        }
    }

    public class AddonDeviceRotaryPosition : GenericOptionObject
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

    public class AddonDeviceInputBase : AddonDeviceElement
    {

        [XmlIgnore]
        public int ReportIndex
        {
            get { if (!_ReportIndex.HasValue) _ReportIndex = GetOption("ReportIndex", 0); return _ReportIndex.Value; }
            set { SetOption("ReportIndex", value); _ReportIndex = value; }
        }
        private int? _ReportIndex;



        [XmlIgnore]
        public int ReportMask
        {
            get { if (!_ReportMask.HasValue) _ReportMask = GetOption("ReportMask", 0); return _ReportMask.Value; }
            set { SetOption("ReportMask", value); _ReportMask = value; }
        }
        private int? _ReportMask;
        [XmlIgnore]
        public int ReportLen
        {
            get { if (!_ReportLen.HasValue) _ReportLen = GetOption("ReportLen", 1); return _ReportLen.Value; }
            set { SetOption("ReportLen", value); _ReportLen = value; }
        }
        private int? _ReportLen;
        [XmlIgnore]
        public bool Inverse
        {
            get { if (!_Inverse.HasValue) _Inverse = GetOption("Inverse", false); return _Inverse.Value; }
            set { SetOption("Inverse", value); _Inverse = value; }
        }
        private bool? _Inverse;

        [XmlIgnore]
        public int ReportBit
        {
            get { if (!_ReportBit.HasValue) _ReportBit = GetOption("ReportBit", 0); return _ReportBit.Value; }
            set { SetOption("ReportBit", value); _ReportBit = value; }
        }
        private int? _ReportBit;

        [XmlIgnore]
        public int UIRow
        {
            get { if (!_UIRow.HasValue) _UIRow = GetOption("UI.Row", 0); return _UIRow.Value; }
            set { SetOption("UI.Row", value); _UIRow = value; }
        }
        private int? _UIRow;

        protected bool lastBoolValue = false;

        public override void FixUp()
        {
            if (!HasOption("ReportMask") && HasOption("ReportBit"))
            {
                ReportMask = 1 << ReportBit;
            }
            base.FixUp();
        }

        public override void ProcessInput(byte[] inputReport, Action<string, string, int, int, bool> raiseEventCallback, bool isStateScan = false, int startIndex = 0)
        {

            var nVal = getInputVal(inputReport, startIndex);
            if (Inverse)
                nVal = !nVal;

            if (lastBoolValue != nVal)
            {
                raiseEventCallback?.Invoke(Tag, nVal ? GetOption("PRESS", "PRESS") : GetOption("RELEASE", "RELEASE"), nVal ? 1 : 0, nVal ? 1 : 0, isStateScan);
            }
            lastBoolValue = nVal;
        }

        protected int getInputValRaw(byte[] inputReport, int startIndex = 0)
        {
            if (ReportLen == 1)
                return inputReport[ReportIndex + startIndex];
            int val = 0;
            for (int i = ReportIndex; i < ReportIndex + ReportLen; i++)
            {
                val = val << 8;
                val |= (inputReport[i + startIndex]);
            }
            return val;
        }

        protected bool getInputVal(byte[] inputReport, int startIndex = 0)
        {
            if (ReportLen == 1)
                return (inputReport[ReportIndex + startIndex] & ReportMask) != 0;
            int val = 0;
            for (int i = ReportIndex; i < ReportIndex + ReportLen; i++)
            {
                val = val << 8;
                val |= (inputReport[i + startIndex]);
            }
            return (val & ReportMask) != 0;
        }
    }

    public class AddonDeviceEncoder : AddonDeviceInputBase
    {
        public AddonDeviceEncoder()
        {
            Type = "ENCODER";
            Inherit = "SPAD_ENCODER";
        }

        private volatile int _LastEncoderVal = 0;
        private int _EncoderDelta = 1;
        private volatile int _expectedClicks = 1;
        public override void FixUp()
        {
            _EncoderDelta = GetOption("EncoderDelta", 1);
            base.FixUp();
        }


        public override void ProcessInput(byte[] inputReport, Action<string, string, int, int, bool> raiseEventCallback, bool isStateScan = false, int startIndex = 0)
        {
            if (isStateScan)
            {
                _LastEncoderVal = getInputValRaw(inputReport, startIndex);
                _expectedClicks = _EncoderDelta;
                return;

            }
            var nVal = getInputValRaw(inputReport, startIndex);

            var dir = (nVal > _LastEncoderVal) ? 1 : -1;
            var dif = Math.Abs(nVal - _LastEncoderVal);
            if (dif > 127) // EdgeCase
            {
                if (nVal > _LastEncoderVal)
                {
                    dif = 256 - nVal + _LastEncoderVal;
                    dir = -1;
                }
                else
                {
                    dif = 256 - _LastEncoderVal + nVal;
                    dir = 1;
                }
            }

            if (dif >= _expectedClicks)
            {
                if (Inverse)
                    dir = -1 * dir;
                raiseEventCallback?.Invoke(Tag, dir > 0 ? GetOption("CW", "TUNER_CLOCKWISE") : GetOption("CCW", "TUNER_COUNTERCLOCKWISE"), dir, nVal, isStateScan);
                _LastEncoderVal = nVal;
            }
            _expectedClicks = _EncoderDelta;
        }
    }

    public class AddonDeviceOutputGauge : AddonDeviceElement
    {
        public override void FixUp()
        {
            this.Type = "GAUGE";
            AddInherit("SPAD_GAUGE");
            AddOption("WIDTH", "72");
            AddOption("HEIGHT", "72");
            AddOption("UI_TYPE", "3");
            AddOption("CUSTOM_TYPE", "GAUGE");
        }
    }

    public abstract class AddonDeviceOutputBase : AddonDeviceInputBase
    {
        [XmlIgnore]
        public bool isON { get; protected set; } = false;

        [XmlIgnore]
        public byte ReportValue
        {
            get { if (!_ReportValue.HasValue) _ReportValue = GetOption("ReportValue", (byte)0); return _ReportValue.Value; }
            set { SetOption("ReportValue", value); _ReportValue = value; }
        }

        private byte? _ReportValue;

        Action<AddonDeviceOutputBase, byte[]> generatePayLoadAction;

        public void SetState(bool ison)
        {
            isON = ison;
        }
        public void SetPayLoadAction(Action<AddonDeviceOutputBase, byte[]> payLoadAction) => generatePayLoadAction = payLoadAction;
        public void GeneratePayLoad(ref byte[] payload) => generatePayLoadAction?.Invoke(this, payload);
    }

    public class AddonDeviceOutputDisplay : AddonDeviceOutputBase
    {
        [XmlIgnore]
        public string currentValue { get; protected set; } = "";
        public AddonDeviceOutputDisplay()
        {
            Type = "DISPLAY";
            Inherit = "SPAD_DISPLAY";
            SetPayLoadAction((a, b) => GeneratePayLoadDisplay(a, b));
            SetState(true);
        }

        public void SetCurrentValue(string val) => currentValue = val;

        void GeneratePayLoadDisplay(AddonDeviceOutputBase target, byte[] payload)
        {
            if (isON)
                for (int i = 0; i < ReportLen; i++)
                    payload[ReportIndex + i] = (byte)(i < currentValue.Length ? GetOption<byte>("CHARTABLE." + currentValue[i], (byte)currentValue[i]) : 0);
            else
            {
                for (int i = 0; i < ReportLen; i++)
                    payload[ReportIndex + i] = 0;
            }
        }
    }

    public class AddonDeviceOutputLED : AddonDeviceOutputBase
    {
        public override void FixUp()
        {
            SetState(GetOption("DEFAULT", false));
            base.FixUp();
        }
        public AddonDeviceOutputLED()
        {
            Type = "LED";
            Inherit = "SPAD_LED";
            SetPayLoadAction((a, b) => GeneratePayLoadLED(a, b));
            SetState(false);
        }

        void GeneratePayLoadLED(AddonDeviceOutputBase target, byte[] payload)
        {
            var val = isON;
            if (Inverse)
                val = !val;
            if (val)
                payload[ReportIndex] |= (byte)(1 << ReportBit);
            else
                payload[ReportIndex] &= (byte)(~(1 << ReportBit));
        }
    }


    public class AddonDeviceButton : AddonDeviceInputBase
    {
        public AddonDeviceButton()
        {
            Type = "PUSHBUTTON";
            Inherit = "SPAD_PUSHBUTTON";
        }
    }

    public class AddonDeviceSwitch : AddonDeviceInputBase
    {
        public AddonDeviceSwitch()
        {
            Type = "SWITCH"; Inherit = "SPAD_SWITCH";
        }

        public AddonDeviceSwitch WithRotaryEncoderPosition(string name, int mask, int uiValue)
        {
            RegisterRotaryPosition(name, mask, uiValue);
            return this;
        }

        // Switches will raise stateScan Events
        public override void ProcessInput(byte[] inputReport, Action<string, string, int, int, bool> raiseEventCallback, bool isStateScan = false, int startIndex = 0)
        {

            var nVal = getInputVal(inputReport, startIndex);
            if (Inverse)
                nVal = !nVal;

            if (lastBoolValue != nVal || isStateScan)
            {
                raiseEventCallback?.Invoke(Tag, nVal ? GetOption("PRESS", "PRESS") : GetOption("RELEASE", "RELEASE"), nVal ? 1 : 0, nVal ? 0 : 1, isStateScan);
            }
            lastBoolValue = nVal;
        }

    }

    public class AddonInputRouting : GenericOptionObject
    {
        [XmlAttribute]
        public string From { get; set; }
        [XmlAttribute]
        public string To { get; set; }

        [XmlAttribute]
        public string Press { get; set; } = "PRESS";
        [XmlIgnore] public string CW => Press;
        [XmlAttribute]
        public string Release { get; set; } = "RELEASE";
        [XmlIgnore] public string CCW => Release;
        [XmlAttribute]
        public bool IsSwitch { get; set; } = false;
        public bool ShouldSerializeIsSwitch() => IsSwitch;


        public AddonInputRouting(string from, string to, string event1 = "PRESS", string event2 = "RELEASE", bool isSwitch = false)
        {
            From = from;
            To = to;
            Press = event1;
            Release = event2;
            IsSwitch = isSwitch;
        }

        public AddonInputRouting()
        {
        }
    }
    /*
    public class RouteAxis : RouteBase
    {
        public RouteAxis()
        {
        }

        public RouteAxis(string from, string to) : base(from, to)
        {
        }

        public RouteAxis(string from, string to,int min=0, int max=1024) : this(from,to)
        {
            Min = min;
            Max = max;
        }

        [XmlAttribute]
        public int Min { get; set; } = 0;
        [XmlAttribute]
        public int Max { get; set; } = 1024;
    }
*/

    [Serializable]
    [XmlInclude(typeof(AddonDeviceSwitch))]
    [XmlInclude(typeof(AddonDeviceSwitchEncoder))]
    [XmlInclude(typeof(AddonDeviceButton))]
    [XmlInclude(typeof(AddonDeviceEncoder))]
    [XmlInclude(typeof(AddonDeviceOutputDisplay))]
    [XmlInclude(typeof(AddonDeviceOutputLED))]
    [XmlInclude(typeof(AddonDeviceOutputGauge))]
    public class AddonDeviceElement : GenericOptionObject
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

        [XmlAttribute]
        [Category("Data")]
        public int SortOrder { get; set; } = 0;


        [XmlElement(ElementName = "Mapping")]
        [Category("Data")]
        public List<AddonDeviceCommandMapping> Mappings { get; set; } = new List<AddonDeviceCommandMapping>();
        [XmlElement(ElementName = "OutputMapping")]
        [Category("Data")]
        public List<AddonDeviceCommandMapping> OutputMappings { get; set; } = new List<AddonDeviceCommandMapping>();

        [XmlElement(ElementName = "RotaryPosition")]
        public List<AddonDeviceRotaryPosition> RotaryPositions { get; set; } = new List<AddonDeviceRotaryPosition>();

        [XmlElement(ElementName = "Route", Type = typeof(AddonInputRouting))]
        public List<AddonInputRouting> Routing { get; set; } = new List<AddonInputRouting>();

        [XmlElement(ElementName = "TargetMapping")]
        public AddonDeviceTargetMapping TargetMapping { get; set; }

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

        [XmlAttribute(AttributeName = "Col")]
        [Category("Position")]
        public int Col { get; set; }
        [XmlAttribute(AttributeName = "Row")]
        [Category("Position")]
        public int Row { get; set; }
        [XmlAttribute(AttributeName = "ColSpan")]
        [Category("Position")]
        public int ColSpan { get; set; }
        [XmlAttribute(AttributeName = "RowSpan")]
        [Category("Position")]
        public int RowSpan { get; set; }

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
        public bool IsImage => Type == "IMAGE";
        [XmlIgnore]
        public bool IsGauge => Type == "GAUGE";
        [XmlIgnore]
        public bool IsInput => !IsOutput;

        [XmlIgnore]
        public AddonDeviceDisplay Display { get; private set; } = null;
        [XmlIgnore]
        public bool NeedMapping { get; set; } = true;
        [XmlIgnore]
        public bool IsPanelChange { get; set; } = false;

        [XmlIgnore]
        public bool IsOutput => InputType == DeviceInputTypes.Display || InputType == DeviceInputTypes.Led || InputType == DeviceInputTypes.Image || InputType == DeviceInputTypes.Gauge;
        private static readonly DeviceInputTypes[] _SwitchTypes = new DeviceInputTypes[] { DeviceInputTypes.Switch, DeviceInputTypes.StatefulSwitch, DeviceInputTypes.RotarySwitch };
        [XmlIgnore]
        public bool IsSwitch => _SwitchTypes.Contains(InputType);

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
                    case "ROTARYSWITCH": return DeviceInputTypes.RotarySwitch;
                    case "SWITCH3": return DeviceInputTypes.StatefulSwitch;
                    case "LABEL": return DeviceInputTypes.Label;
                    case "UI": return DeviceInputTypes.UIElement;
                    case "IMAGE": return DeviceInputTypes.Image;
                    case "GAUGE": return DeviceInputTypes.Gauge;
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

        public AddonDeviceElement WithRotaryPosition(string name, int mask, int uiValue, string uiLabel = null)
        {
            var tmp = RegisterRotaryPosition(name, mask, uiValue);
            if (!string.IsNullOrEmpty(uiLabel))
                tmp.AddOption("LABEL", uiLabel);
            return this;
        }


        public AddonDeviceRotaryPosition RegisterRotaryPosition(string posName, int posValue, int posUIValue)
        {
            if (RotaryPositions.Any(x => x.Name == posName))
                return null;
            var newPos = new AddonDeviceRotaryPosition() { Name = posName, Value = posValue, UIValue = posUIValue };
            RotaryPositions.Add(newPos);
            return newPos;
        }

        public bool AddRouting(AddonInputRouting routingBase)
        {
            if (Routing.Any(rt => rt.From == routingBase.From && rt.To == routingBase.To))
                return false;
            Routing.Add(routingBase);
            return true;
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
            Width = GetOption("WIDTH", Width);
            Height = GetOption("HEIGHT", Height);
            Top = GetOption("Top", Top);
            Left = GetOption("Left", Left);
            Col = GetOption("UI.COL", Col);
            Row = GetOption("UI.ROW", Row);
            ColSpan = GetOption("UI.COLSPAN", ColSpan);
            RowSpan = GetOption("UI.ROWSPAN", RowSpan);

            var tList = RotaryPositions.OrderBy(r => r.Value).ToList();

            RotaryPositions = tList;
            if (TargetMapping != null)
            {
                TargetMapping.Tag = Tag;
                TargetMapping.FixUp();
            }
            foreach (var item in OutputMappings)
            {
                item.FixUp(Tag);
            }
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

        public AddonDeviceElement AddGridOptions(int row = 0, int col = 0, int rowSpan = 0, int colSpan = 0)
        {
            this.WithOption("UI.ROW", row);
            this.WithOption("UI.COL", col);
            this.WithOption("UI.ROWSPAN", rowSpan);
            this.WithOption("UI.COLSPAN", colSpan);
            return this;
        }


        public AddonDeviceElement WithGridPosition(int row, int col, int rowSpan = 0, int colSpan = 0)
        {
            if (row != 0)
                this.WithOption("UI.ROW", row);
            if (col != 0)
                this.WithOption("UI.COL", col);
            if (rowSpan != 0)
                this.WithOption("UI.ROWSPAN", rowSpan);
            if (colSpan != 0)
                this.WithOption("UI.COLSPAN", colSpan);
            return this;
        }
        public AddonDeviceElement WithOption(string key, object value, bool overwrite = false)
        {
            if (overwrite || !HasOption(key))
                SetOption(key, Convert.ToString(value, CultureInfo.InvariantCulture));
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

        public virtual void ProcessInput(byte[] inputReport, Action<string, string, int, int, bool> raiseEventCallback, bool isStateScan = false, int startIndex = 0)
        { }

        public override string ToString()
        {
            return $"{this.GetType()} {Type} {Tag} Options {Options.Count}";
        }
    }

    public enum VariableValueTypes
    {
        Number,
        String
    }

    public class GenericVariable : GenericOptionObject, IGenericOption
    {
        [XmlAttribute]
        public string Key { get; set; }
        [XmlAttribute]
        public string Value { get; set; }
        [XmlAttribute]
        public VariableValueTypes ValueType { get; set; } = VariableValueTypes.Number;
        public bool ShouldSerializeValueType() => ValueType != VariableValueTypes.Number;
        [XmlAttribute]
        public VARIABLE_SCOPE Scope { get; set; } = VARIABLE_SCOPE.SESSION;
        public bool ShouldSerializeScope() => Scope != VARIABLE_SCOPE.SESSION;
        [XmlAttribute]
        public string DefaultValue { get; set; }
        public bool ShouldSerializeDefaultValue() => !String.IsNullOrEmpty(DefaultValue);
        [XmlAttribute]
        public string SettingName { get; set; }
        public bool ShouldSerializeSettingName() => !String.IsNullOrEmpty(SettingName);

        public GenericVariable()
        {
        }
        public GenericVariable(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public void SetValue(object newValue) { Value = Convert.ToString(newValue, CultureInfo.InvariantCulture); }
        public object GetTypedValue()
        {
            switch (ValueType)
            {
                case VariableValueTypes.Number:
                    return GetValue<double>();
                case VariableValueTypes.String:
                    return Value;
                default:
                    return Value;
            }
        }

        public T GetValue<T>(T defValue = default) where T : IConvertible
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
                    res = !(Value == "0" || String.IsNullOrEmpty(Value) || String.Compare(Value, "false", true) == 0);
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
                return defValue;
            }
        }

        public override string ToString()
        {
            return Key + "=" + Value;
        }

        public GenericVariable WithOption(string name, object value)
        {
            AddOption(name, value);
            return this;
        }
    }

    [Serializable]
    public class GenericOption : IGenericOption
    {
        [XmlAttribute]
        public string Key { get; set; }
        [XmlAttribute]
        public string Value { get; set; }

        public GenericOption()
        {
        }
        public GenericOption(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public T GetValue<T>(T defValue = default) where T : IConvertible
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
                    res = !(Value == "0" || String.IsNullOrEmpty(Value) || String.Compare(Value, "false", true) == 0);
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
                return defValue;
            }
        }

        public override string ToString()
        {
            return Key + "=" + Value;
        }
    }


    public class GenericVariablesObject : IObjectWithVariables
    {
        [XmlElement(ElementName = "Variable")]
        public List<GenericOption> Variables { get; set; } = new List<GenericOption>();

        public event AsyncEventHandler<VariableChangedEventArgs> VariableChanged;

        public virtual bool ShouldSerializeVariables() => Variables != null && Variables.Count > 0;
        public int CountVariables => (Variables == null ? 0 : Variables.Count);

        IEnumerable<IGenericOption> IObjectWithVariables.Variables => Variables;

        public T GetVariable<T>(string key, T defaultValue = default(T)) where T : IConvertible
        {
            var opt = Variables.FirstOrDefault(o => String.Compare(o.Key, key, true) == 0);

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

        public bool HasVariable(string key)
        {
            return Variables.Any(o => String.Compare(o.Key, key, true) == 0);
        }
        public bool AddVariable(string key, object value, int pos = -1)
        {
            if (!HasVariable(key))
            {
                if (pos == -1)
                    Variables.Add(new GenericOption(key, Convert.ToString(value, CultureInfo.InvariantCulture)));
                else
                    Variables.Insert(pos, new GenericOption(key, Convert.ToString(value, CultureInfo.InvariantCulture)));
                return true;
            }
            return false;
        }
        public void SetVariable<T>(string key, T value)
        {
            Variables.RemoveAll(o => String.Compare(key, o.Key, true) == 0);
            if (value != null)
                Variables.Add(new GenericOption(key, Convert.ToString(value, CultureInfo.InvariantCulture)));
            VariableChanged?.FireAndForget(this, new VariableChangedEventArgs(key, value));
        }

        public int RemoveVariable(string key) => Variables.RemoveAll(o => String.Compare(key, o.Key, true) == 0);

        public bool MergeVariables(IObjectWithVariables src)
        {
            bool res = false;
            foreach (var item in src.Variables)
            {
                res |= AddVariable(item.Key, item.Value);
            }
            return res;
        }

        public override string ToString()
        {
            return string.Join(",", Variables.Select(o => o.ToString()));
        }
    }

    public class GenericOptionObject : IObjectWithOptions
    {
        [XmlElement(ElementName = "Option")]
        public List<GenericOption> Options { get; set; } = new List<GenericOption>();
        public virtual bool ShouldSerializeOptions() => Options != null && Options.Count > 0;
        public int OptionsCount => (Options == null ? 0 : Options.Count);

        IEnumerable<IGenericOption> IObjectWithOptions.Options => Options;

        public IObjectWithOptions WithInitialOption<T>(string key, T value) where T : IConvertible
        {
            SetOption<T>(key, value);
            return this;
        }

        public bool TryGetOptionIf<T>(string key, out T outVar, T defaultValue = default(T)) where T : IConvertible, IComparable<T>
        {
            outVar = defaultValue;
            if (!HasOption(key))
                return false;
            outVar = GetOption<T>(key, defaultValue);
            if (outVar.CompareTo(defaultValue) == 0) return false;

            return true;
        }

        public bool TryGetOption<T>(string key, out T outVar, T defaultValue = default(T)) where T : IConvertible
        {
            outVar = defaultValue;
            if (!HasOption(key))
                return false;
            outVar = GetOption<T>(key, defaultValue);
            return true;
        }

        public T GetFirstMatchingOption<T>(T defaultValue, params string[] keys) where T : IConvertible
        {
            foreach (var key in keys)
            {
                var opt = Options.FirstOrDefault(o => String.Compare(o.Key, key, true) == 0);

                if (opt != null)
                {
                    try
                    {
                        return (T)opt.GetValue<T>();
                    }
                    catch
                    {
                    }
                }
            }
            return defaultValue;
        }

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
        public bool HasOption(params string[] keys)
        {
            foreach (var key in keys)
            {
                if (!String.IsNullOrEmpty(key))
                {
                    if (Options.Any(o => String.Compare(o.Key, key, true) == 0))
                        return true;
                }
            }
            return false;
        }
        public bool AddOption(string key, object value, int pos = -1)
        {
            if (!HasOption(key))
            {
                if (pos == -1)
                    Options.Add(new GenericOption(key, Convert.ToString(value, CultureInfo.InvariantCulture)));
                else
                    Options.Insert(pos, new GenericOption(key, Convert.ToString(value, CultureInfo.InvariantCulture)));
                return true;
            }
            return false;
        }
        public void SetOption<T>(string key, T value) where T : IConvertible
        {
            Options.RemoveAll(o => String.Compare(key, o.Key, true) == 0);
            if (value != null)
                Options.Add(new GenericOption(key, Convert.ToString(value, CultureInfo.InvariantCulture)));
        }

        public int RemoveOption(string key) => Options.RemoveAll(o => String.Compare(key, o.Key, true) == 0);

        public bool MergeOptions(IObjectWithOptions src)
        {
            foreach (var item in src.Options)
            {
                AddOption(item.Key, item.Value);
            }
            return true;
        }

        public override string ToString()
        {
            return string.Join(",", Options.Select(o => o.ToString()));
        }

        public static IObjectWithOptions Create(string initData)
        {
            var obj = new GenericOptionObject();
            var parts = initData.Split(new char[] { ';' });
            foreach (var part in parts)
            {
                obj.SetOption(part.GetPart(0,"="),part.GetPart(1,"="));
            }
            return obj;
        }
    }

    [Serializable]
    public class AddonDeviceTargetMapping : GenericOptionObject
    {
        [XmlAttribute]
        public string In { get; set; }
        [XmlIgnore]
        public List<string> InputValues { get; private set; }

        [XmlAttribute]
        public string Vals { get; set; }
        public List<string> InputValueDatas => !String.IsNullOrEmpty(Vals) ? new List<string>(Vals.Split(',')) : new List<string>();
        [XmlText]
        public string MappingExpression { get; set; }
        [XmlIgnore]
        public IDynamicExpression Mapping { get; set; }
        [XmlAttribute]
        public string Tag { get; set; }
        public AddonDeviceTargetMapping() { }

        public void FixUp()
        {
            var SwitchNames = new List<string>();
            if (String.IsNullOrEmpty(In))
            {
                InputValues = new List<string>();
            }
            else
            {
                var strs = In.Split(',');
                var vls = InputValueDatas;
                var res = new List<string>();

                foreach (var item in strs)
                {
                    if (vls.Count > 0)
                    {
                        foreach (var val in vls)
                            res.Add(item.Replace("$", val));
                    }
                    else
                        res.Add(item);
                    SwitchNames.Add(item.Replace("$", ""));
                }
                InputValues = res;
            }

            if (!String.IsNullOrEmpty(MappingExpression))
            {
                for (var i = 0; i < SwitchNames.Count; i++)
                {
                    MappingExpression = MappingExpression.Replace("$" + i, SwitchNames[i]);
                }
                Mapping = EventSystem.CreateExpression(MappingExpression.Replace("{TAG}", Tag));
            }
        }

    }

    [Serializable]
    public class AddonDeviceCommandMapping : GenericOptionObject
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
        [XmlAttribute]
        public bool Final { get; set; } = true;
        public bool ShouldSerializeFinal() => !Final;
        [XmlAttribute(AttributeName = "Condition")]
        public string ConditionExpression { get; set; }
        [XmlAttribute(AttributeName = "Compute")]
        public string ComputeExpression { get; set; }
        [XmlIgnore]
        public IDynamicExpression Condition { get; set; }
        [XmlIgnore]
        public IDynamicExpression Compute { get; set; }

        public AddonDeviceCommandMapping() { }
        public AddonDeviceCommandMapping(string tag, string @in, string @out, int activateDirection = 1)
        {
            Tag = tag;
            In = @in;
            Out = @out;
            StateValue = activateDirection;
        }

        public string ComputeOutput(object value)
        {
            if (Compute != null)
                return Convert.ToString(Compute.Evaluate(new SPADEventArgs("dummy", value, value)), CultureInfo.InvariantCulture);
            else return Convert.ToString(value,CultureInfo.InvariantCulture);
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

        public void FixUp(string tag = null)
        {
            if (!String.IsNullOrEmpty(tag) && String.IsNullOrEmpty(Tag))
            {
                Tag = tag;
            }
            if (StateValue == int.MaxValue) // No Data in config
            {
                StateValue = 1;
                if (Out.StartsWith("TUNER_"))
                {
                    if (Out.Contains("COUNTERCLOCKWISE"))
                        StateValue = -1;
                    else
                        StateValue = 1;
                }
                else
                {
                    if (IsOffName(Out))
                        StateValue = 0;
                }
            }
            if (!String.IsNullOrEmpty(ConditionExpression))
            {
                Condition = EventSystem.CreateExpression(ConditionExpression);
            }
            if (!String.IsNullOrEmpty(ComputeExpression))
            {
                Compute = EventSystem.CreateExpression(ComputeExpression);
            }
            if (!String.IsNullOrEmpty(Tag))
            {
                In = In.Replace("{TAG}", Tag);
                Out = Out.Replace("{TAG}", Tag);
            }
        }

        public bool CanExecute(object value = null)
        {
            if (Condition != null)
                return Condition.EvaluateBool(value);
            return true;
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
        [XmlAttribute]
        public string Action { get; set; } = "NONE";
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
