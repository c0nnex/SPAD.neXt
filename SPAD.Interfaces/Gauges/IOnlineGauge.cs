using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Gauges
{
    public interface IOnlineGaugeList
    {
        int ErrorCode { get; set; }
        List<IOnlineGauge> Gauges { get; set; }
    }

    public interface IOnlineGauge
    {
        string Author { get; set; }
        string Contact { get; set; }
        DateTime Created { get; set; }
        string Description { get; set; }
        int Downloads { get; set; }
        string Filename { get; set; }
        string GaugeId { get; set; }
        Guid GaugeID { get; }
        string InfoUrl { get; set; }
        bool IsLicensed { get; set; }
        DateTime LastChanged { get; set; }
        string LicenseModel { get; set; }
        string MainCategory { get; set; }
        string Name { get; set; }
        byte[] Preview { get; set; }
        float Price { get; set; }
        int Rating { get; set; }
        string SubCategory1 { get; set; }
        string SubCategory2 { get; set; }
        string SubCategory3 { get; set; }
        string Version { get; set; }
    }
}
