using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Blazor.Starter.Pages
{
    [RouteAttribute("/render")]
    public class RendererComponent : IComponent
    {
        private RenderHandle _renderHandle;
        private bool _firstRender = true;
        private bool messageswitch;

        public void Attach(RenderHandle renderHandle)
        {
            _renderHandle = renderHandle;
        }

        public Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);
            if (_firstRender)
            {
                _firstRender = false;
            }
            this.Render();
            _renderHandle.Render(RenderComponent);
            return Task.CompletedTask;
        }

        public void Render()
            => _renderHandle.Render(RenderComponent);

        private void RenderComponent(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.OpenElement(1, "button");
            builder.AddAttribute(2, "class", "btn btn-dark");
            builder.AddAttribute(4, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, SwitchMessage));
            builder.AddContent(3, "Switch");
            builder.CloseElement();
            builder.CloseElement();
            builder.AddContent(5, HelloWorld);
        }

        private RenderFragment HelloWorld => (builder) =>
        {
            builder.OpenElement(0, "div");
            if (!messageswitch)
            {
                builder.AddAttribute(1, "class", "m-3 p-2");
                builder.AddContent(2, "Hello World");
            }
            else
            {
                builder.AddAttribute(1, "class", "m-3 p-2 bg-warning");
                builder.AddContent(2, "Hello World Switched");
            }
            builder.CloseElement();
        };

        private void SwitchMessage(MouseEventArgs e)
        {
            this.messageswitch = !this.messageswitch;
            Render();
        }

    }
}
