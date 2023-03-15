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
        IEnumerable<IDataMonitorValue> GetCacheItems();
    }

    public interface IDataCacheConsumer
    {
        void NotifyDirtyValue(int idx);
        void SetCacheDirty();       
        void ResizeCache(int newSize);
        void ClearCache();
    }

    public interface IDataCacheProvider : IDataCacheValueProvider
    {
        int CountDataItems { get; }
        int CacheSize { get; }

        long GetCacheData(ref object[] data, long myIteration = -1);
        
        IDataCacheValueProvider CreateChildCache(string cacheID);

        void RegisterChildCache(IDataCacheConsumer dataCacheConsumer);
        void UnsubscribeChildCache(IDataCacheConsumer dataCacheConsumer);
        void UnregisterChildCache(IDataCacheConsumer dataCacheConsumer);
        
        int AddToCache(string variableName, IDataCacheConsumer notifyObject, ICacheScopeProvider scopeObject);
        double GetMasterValue(int dataIndex);
        IDataMonitorValue GetCacheItem(int dataIndex);  
    }

    public interface IDataCacheValueProvider : IDisposable
    {
        event EventHandler<string, IEnumerable<int>> OnCacheInvalidated;
        string CacheID { get; }
        int[] AllDataIndexes { get; }
        double GetNumericValue(int dataIndex);
        object GetValue(int dataIndex);
        bool UpdateCache();
        void SetCacheDirty();
        bool IsCacheDirty { get; }
        void ClearCache();
        int RegisterCacheValue(string variableName, ICacheScopeProvider scopeObject);
        void RegisterTransformFunction(Func<string, string> fnTransformVariable);

        Dictionary<int, string> GetCacheInfomation();
        HashSet<int> GetDirtySet();
        void BeginCacheConfigUpdate();
        void EndCacheConfigUpdate();

        void UpdateValue(int dataIndex, object newVal);
    }
}
