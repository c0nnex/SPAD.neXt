
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

        public void FixUp() // Create LookupTable for faster processing
        {
            DeviceCommandMappingDict.Clear();
            foreach (var item in Inputs)
            {
                foreach (var m in item.Mappings)
                {
                    m.Tag = item.Tag;
                    DeviceCommandMappingDict[m.In] = m;
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

        public void SetOption<T>(string key, T value) where T:class
        {
            Options.RemoveAll(o => String.Compare(key, o.Key, true) == 0);
            if (value != null)
                Options.Add(new AddonDeviceOption(key, Convert.ToString(value,CultureInfo.InvariantCulture)));
        }

        public T GetOption<T>(string key,T defaultValue = default(T))
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
        
        public AddonDeviceCommandMapping GetOrCreateMapping(string inStr, string outStr)
        {
            var oVal = Mappings.FirstOrDefault(v => v.Out == outStr);
            if (oVal != null)
                return oVal;
            oVal = new AddonDeviceCommandMapping(Tag,inStr, outStr);
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
    }

    [Serializable]
    public sealed class AddonDeviceOption
    {
        [XmlAttribute]
        public string Key { get; set; }
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
