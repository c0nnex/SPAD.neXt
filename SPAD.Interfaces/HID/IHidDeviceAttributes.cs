using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.HID
{
    public class IHidDeviceAttributes
    {
        public string ProductHexId ;
        public ushort ProductId ;
        public string VendorHexId;
        public ushort VendorId ;
        public int Version ;
    }

    public class IHidDeviceCapabilities
    {
        public short Usage ;
        public short UsagePage ;
        public short InputReportByteLength ;
        public short OutputReportByteLength ;
        public short FeatureReportByteLength ;
        public short NumberLinkCollectionNodes ;
        public short NumberInputButtonCaps ;
        public short NumberInputValueCaps ;
        public short NumberInputDataIndices ;
        public short NumberOutputButtonCaps ;
        public short NumberOutputValueCaps ;
        public short NumberOutputDataIndices ;
        public short NumberFeatureButtonCaps ;
        public short NumberFeatureValueCaps ;
        public short NumberFeatureDataIndices ;
        public short InputReportByteLengthNet ;
    }
}
