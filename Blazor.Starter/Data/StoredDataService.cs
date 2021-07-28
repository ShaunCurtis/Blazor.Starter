using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Starter.Data
{
    public class StoredDataService : IStoredDataService
    {
        public ValueTask<StoredData?> GetStoredData(Guid id)
        {
            return ValueTask.FromResult<StoredData?>(GetData(id));
        }

        public ValueTask<bool> SetStoredData(StoredData data)
        {
            var ret = SetData(data);
            return ValueTask.FromResult<bool>(ret);
        }
        
        private List<StoredData> DataList = new List<StoredData>();


        private bool SetData(StoredData data)
        {
            if (DataList.Any(item => item.UniqueId == data.UniqueId))
            {
                var rec = DataList.First(item => item.UniqueId == data.UniqueId);
                DataList.Remove(rec);
            }
            DataList.Add(data);
            return DataList.Any(item => item.UniqueId == data.UniqueId);
        }

        private StoredData? GetData(Guid id)
        {
            return DataList.FirstOrDefault(item => item.UniqueId == id);
        }
    }
}
