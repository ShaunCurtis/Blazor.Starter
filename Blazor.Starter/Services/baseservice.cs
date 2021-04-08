using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Starter.Services
{
    public class BaseService
    {
        public string Message
        {
            get => _message;
            set
            {
                if (!_message.Equals(value))
                    _message = value;
                MessageChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private string _message = string.Empty;

        public event EventHandler MessageChanged;
    }
}
