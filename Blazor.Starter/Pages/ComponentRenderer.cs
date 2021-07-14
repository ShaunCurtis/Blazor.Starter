using Blazor.Starter.Components.TestComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.Starter.Pages
{
    [Route("/ComponentRenderer")]
    public class ComponentRenderer : ComponentBase
    {
        [Parameter] public List<Type> Components { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            foreach (var component in Components)
            {
                if (component.GetInterfaces().Contains(typeof(IComponent)))
                {
                    builder.OpenComponent(0, component);
                    builder.CloseComponent();
                }
            }
        }

    }
}
