using System;

namespace Blazor.Starter.Services
{
    public class RandomNumberService
    {
        public int Value => _Value;
        private int _Value = 0;

        public event EventHandler NumberChanged;

        public void NewNumber()
        {
            var rand = new Random();
            NotifyNumberChanged(rand.Next(0, 100));
        }

        public void NewNumberWithoutEvent()
        {
            var rand = new Random();
            _Value = rand.Next(0, 100);

        }

        public void NotifyNumberChanged(int value)
        {
            if (!value.Equals(_Value))
            {
                _Value = value;
                NumberChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
