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
        object GetVariableDefault(string varName, VARIABLE_SCOPE scope);
    }

    public interface IGaugeDataProvider : IRenderFrameBase, IDataCacheValueProvider
    {
       
    }

    public interface IDataCacheConsumer
    {
        void NotifyDirtyValue(int idx);
        void SetCacheDirty();
    }

    public interface IDataCacheProvider : IDataCacheValueProvider, IDisposable 
    {
        event EventHandler<string, IEnumerable<int>> OnCacheInvalidated;
        int CountDataItems { get; }
        void RegisterTransformFunction(Func<string, string> fnTransformVariable);
        void GetCacheData(ref object[] data);
        IDataCacheProvider CreateChildCache(string cacheID);
        void RegisterChildCache(IDataCacheConsumer dataCacheConsumer);
        void UnregisterChildCache(IDataCacheConsumer dataCacheConsumer);
        void BeginCacheConfigUpdate();
        void EndCacheConfigUpdate();
        int AddToCache(string variableName, IDataCacheConsumer notifyObject, ICacheScopeProvider scopeObject);
    }

    public interface IDataCacheValueProvider
    {
        string CacheID { get; }

        double GetNumericValue(int dataIndex);
        object GetValue(int dataIndex);
        void UpdateCache();
        int RegisterCacheValue(string variableName, ICacheScopeProvider scopeObject);
    }
}
