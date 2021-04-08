using System;
using System.Collections.Generic;

namespace Blazor.Starter.Services
{
    public class DataService
    {

        public string Genre
        {
            get => _genre;
            set
            {
                if (!value.Equals(_genre))
                {
                    _genre = value;
                    RecordChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void SetGenreQuietly(string genre)
            => this._genre = genre;

        private string _genre = string.Empty;

        public event EventHandler RecordChanged;

        private List<string> genres
            => new List<string>() { "Rock", "Folk", "Blues", "Pop" };

    }
}
