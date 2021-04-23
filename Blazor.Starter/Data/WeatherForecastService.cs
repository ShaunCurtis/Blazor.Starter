using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Starter.Data
{
    public class WeatherForecastService
    {
        public List<WeatherForecast> Records
        {
            get => _Records;
            private set
            {
                _Records = value;
                RecordsChanged?.Invoke(value, EventArgs.Empty);
            }
        }
        private List<WeatherForecast> _Records = new List<WeatherForecast>();

        public WeatherForecast Record
        {
            get => _Record;
            private set
            {
                _Record = value;
                RecordChanged?.Invoke(value, EventArgs.Empty);
            }
        }
        private WeatherForecast _Record = new WeatherForecast();

        public event EventHandler<EventArgs> RecordsChanged;

        public event EventHandler<EventArgs> RecordChanged;

        public Task<bool> GetForecastsAsync(DateTime startDate)
        {
            var rng = new Random();
            this.Records = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            }).ToList();
            return Task.FromResult(this.Records != null);
        }

        public Task<bool> GetForecastAsync()
        {
            this.Record = new WeatherForecast();
            return Task.FromResult(this.Record != null);
        }

        public Task<bool> SaveForecastAsync()
        {
            return Task.FromResult(true);
        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
    }
}
