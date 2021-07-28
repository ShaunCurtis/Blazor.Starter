using System;

namespace Blazor.Starter.Data
{
    public class StoredData
    {

        public Guid UniqueId { get; set; }

        public string? Data { get; set; }

        public DateTimeOffset Date { get; } = DateTimeOffset.Now;

    }
}
