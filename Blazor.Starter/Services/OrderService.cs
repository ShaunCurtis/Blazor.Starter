using System;

namespace Blazor.Starter.Services
{
    public class OrderService
    {

        public string ShippingAddress 
        {
            get => _ShippingAddress;
            set
            {
                if (!_ShippingAddress.Equals(value))
                {
                    NotifyOrderChanged();
                    _ShippingAddress = value;
                }
            }
        }
        private string _ShippingAddress = string.Empty;


        public string BillingAddress
        {
            get => _BillingAddress;
            set
            {
                if (!_BillingAddress.Equals(value))
                {
                    NotifyOrderChanged();
                    _BillingAddress = value;
                }
            }
        }
        private string _BillingAddress = string.Empty;

        public event EventHandler OrderChanged;

        public void NotifyOrderChanged()
            => OrderChanged.Invoke(this, EventArgs.Empty);
    }
}
