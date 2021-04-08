`@page` isn't C#, it's Razor talk.  Razor files are pre-compiled into c# files during compilation.

As an example, this is the important section of the C# pre-compiled file for Index.razor (Index.razor.g.cs):
```csharp
[Microsoft.AspNetCore.Components.RouteAttribute("/")]
public partial class Index : Microsoft.AspNetCore.Components.ComponentBase
{
    #pragma warning disable 1998
    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
    {
        __builder.AddMarkupContent(0, "<h1>Hello, world!</h1>\r\n\r\nWelcome to your new app.\r\n\r\n");
        __builder.OpenComponent<Blazor.Starter.Shared.SurveyPrompt>(1);
        __builder.AddAttribute(2, "Title", "How is Blazor working for you?");
        __builder.CloseComponent();
    }
    #pragma warning restore 1998
}
````

Note that `@page` has become a compile time attribute `[Microsoft.AspNetCore.Components.RouteAttribute("/")]`.  It's fixed at compiletime, you can't change it at runtime.

Routes are set this way because the router builds a routemap - essentially a dictionary of route url/component class pairs - when the application loads by trawling the application assembly for any component classes with a `Route` attribute.  On a routing event it reads the new url, finds the component class and loads it into the layout.  Any variables - stuff in curly brackets - get passed into the component as `Parameters`.

You haven't made it clear what the line below is supposed to do:

`@page MenuItem.Role`

1. Do you want to capture a variable supplied in the route into `MenuItem.Role`?
2. Do you want to set this page's route to the value in `MenuItem.Role`?
 
If 1, either the other answers will work for you. If 2, you'll need to consider writing your own router.  A subject beyond a simple answer here.
