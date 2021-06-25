using Blazor.Starter.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Threading.Tasks;

namespace Blazor.Starter.Components.TestComponents
{
    public class ObjectTest : IComponent
    {
        [Parameter] public DataModel Model { get; set; }

        [Parameter] public int Value { get; set; }

        private RenderHandle _renderHandle;
        private int _renders;

        public void Attach(RenderHandle renderHandle)
        {
            _renderHandle = renderHandle;
        }

        public Task SetParametersAsync(ParameterView parameters)
        {
            parameters.SetParameterProperties(this);
            _renders++;
            this.Render();
            return Task.CompletedTask;
        }

        public void Render()
            => _renderHandle.Render(RenderComponent);

        private void RenderComponent(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddContent(1, $"Rendered {_renders}");
            builder.CloseElement();
        }

    }
}
