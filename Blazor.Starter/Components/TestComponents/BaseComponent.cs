using Microsoft.AspNetCore.Components;

namespace Blazor.Starter.Components.TestComponents
{
    public abstract class BaseComponent : ComponentBase
    {
        protected static MarkupString SayHelloWorld()
        {
            return new("<p>Hello World</p>");
        }
    }
}