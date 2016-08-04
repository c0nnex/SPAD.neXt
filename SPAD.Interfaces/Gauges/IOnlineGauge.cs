using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces.Gauges
{
    public interface IOnlineGauge
    {
        int Active { get;  }
        string Author { get;  }
        string Contact { get;  }
        string Crc { get;  }
        DateTime Created { get;  }
        string Description { get;  }
        string DownloadUrl { get;  }
        string GaugeId { get;  }
        string InfoUrl { get;  }
        DateTime LastChanged { get;  }
        string LicenseModel { get;  }
        string MainCategory { get;  }
        string Name { get;  }
        string Preview { get;  }
        float Price { get;  }
        string ProtectionModel { get;  }
        string ShortName { get;  }
        int Size { get;  }
        string SubCategory1 { get;  }
        string SubCategory2 { get;  }
        string SubCategory3 { get;  }
        string Version { get;  }
    }
}
