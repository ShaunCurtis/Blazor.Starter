using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Blazor.Starter.Data
{
    public class AppJsInterop
    {
        protected IJSRuntime JSRuntime { get; }

        public AppJsInterop(IJSRuntime jsRuntime)
        {
            JSRuntime = jsRuntime;
        }

        public ValueTask<bool> SetBodyCss(string elementId, string cssClass)
          => JSRuntime.InvokeAsync<bool>("SetBodyCss", elementId, cssClass);
    }
}
