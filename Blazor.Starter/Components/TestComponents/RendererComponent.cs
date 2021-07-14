using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.Starter.Components
{
    public class RendererComponent : ComponentBase
    {
        [Parameter] public List<RenderComponent> Components { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            foreach (var component in Components)
            {
                if (!component.ComponentType.GetInterfaces().Contains(typeof(IComponent)))
                    throw new Exception($" {component.ComponentType.Name} must implement IComponent.");
                if (component.ComponentType.GetInterfaces().Contains(typeof(IComponent)))
                {
                    builder.OpenComponent(0, component.ComponentType);
                    foreach (var kvp in component.Parameters)
                    {
                        builder.AddAttribute(1, kvp.Key, kvp.Value);
                    }
                    builder.CloseComponent();
                }
            }
        }

    }

    public class RenderComponent
    {
        public Type ComponentType { get; set; }
        public IDictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
    }

    public class TestRenderComponent : ComponentBase
    {
        [Parameter] public string Message { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(0, Message);
        }
    }
}
