using System;
using System.Threading.Tasks;

namespace Blazor.Starter.Data
{
    public interface IStoredDataService
    {
        public ValueTask<StoredData?> GetStoredData(Guid id);

        public ValueTask<bool> SetStoredData(StoredData data);
    }
}
