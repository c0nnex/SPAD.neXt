using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPAD.neXt.Interfaces
{
    public interface IRenderFrameBase
    {
        ulong BeginFrame(ulong lastCacheTimestamp);
        void EndFrame(ulong frameTimestamp);

        bool NeedsToRenderFrame(ulong lastCacheTimestamp);
    }

    public interface ICacheScopeProvider
    {
        string GetScopeID();
        Guid GetScopeGuid();
        object GetVariableDefault(string varName);
    }

    public interface IGaugeDataProvider : IRenderFrameBase, IDataCacheValueProvider
    {
        
    }

    public interface IDataCacheConsumer
    {
        void NotifyDirtyValue(int idx);
        void SetCacheDirty();
    }

    public interface IDataCacheProvider : IDataCacheValueProvider
    {
        int CountDataItems { get; }
        void GetCacheData(ref object[] data);
        
        IDataCacheValueProvider CreateChildCache(string cacheID);

        void RegisterChildCache(IDataCacheConsumer dataCacheConsumer);
        void UnregisterChildCache(IDataCacheConsumer dataCacheConsumer);
        
        int AddToCache(string variableName, IDataCacheConsumer notifyObject, ICacheScopeProvider scopeObject);
        double GetMasterValue(int dataIndex);
    }

    public interface IDataCacheValueProvider : IDisposable
    {
        event EventHandler<string, IEnumerable<int>> OnCacheInvalidated;
        string CacheID { get; }

        double GetNumericValue(int dataIndex);
        object GetValue(int dataIndex);
        void UpdateCache();
        void SetCacheDirty();
        void ClearCache();
        int RegisterCacheValue(string variableName, ICacheScopeProvider scopeObject);
        void RegisterTransformFunction(Func<string, string> fnTransformVariable);

        Dictionary<int, string> GetCacheInfomation();
        HashSet<int> GetDirtySet();
        void BeginCacheConfigUpdate();
        void EndCacheConfigUpdate();
    }
}
