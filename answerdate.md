The problem is that the two "events" are handled differently in Blazor.

I added some console writes to see what's executed when.

```csharp
<button type="button" @onclick="Increment">Increment using C# Event handler</button>
```

looks like this (`>> false` is the state of `IsModified`)

```
=================Increment - Called >> False
=> Increment - Value++ >> False
=> Increment - ValueChanged.InvokeAsync >> False
++++> ShouldRender Called >> False
=> Increment - EditContext.NotifyFieldChanged >> False
=================Increment - Completed >> True
++++> Render Fragment Run >> True
OnAfterRender >> True
```

While

```csharp
 <button type="button" @ref="btnElementRef">Increment using JS Interop</button>
```
looks a little different - note where Render Fragment Run happens.

```
=================IncrementJsInterop - Called >> False
=================Increment - Called >> False
=> Increment - Value++ >> False
=> Increment - ValueChanged.InvokeAsync >> False
++++> ShouldRender Called >> False
++++> Render Fragment Run >> False
=> Increment - EditContext.NotifyFieldChanged >> False
=================Increment - Completed >> True
=================IncrementJsInterop - Completed >> True
OnAfterRender >> True
```

The normal Blazor Renderer event handler looks something like this: 

```csharp
var task = InvokeAsync(HandlerMethod);
StateHasChanged();
if (!task.IsCompleted)
{
    await task;
    StateHasChanged();
}
```


In the Event sequence we see `ValueChanged` trigger the render in the parent component, however the Rendered doesn't actually run the render until `Increment` completes.

In `IncrementJsInterop` we see `ValueChanged` trigger the render in the parent component, but in this instance the Renderer immediately renders the component.  As `EditContext.NotifyFieldChanged` hasn't yet run, it's false when the parent is rendered.

Both are actually JsInterop "events", but the `Renderer` is handling them slightly differently.

The fix is 
```
    public async Task Increment()
    {
        Value++;
        EditContext.NotifyFieldChanged(FieldIdentifier.Create(ValueExpression));
        await ValueChanged.InvokeAsync(Value);
    }
```

`EditContext` now knows about the field change before any rendering triggered by inter-component updates happen.

===============================================

My Revised re-jigged test code is:

##### Component:
```
@using System.Linq.Expressions

<div style="border: solid; padding: 1em">
    CustomInput <br />
    Value: @Value
    <button type="button" @ref="btnElementRef">Increment using JS Interop</button>
    <button type="button" @onclick="Increment">Increment using C# Event handler</button>
</div>

<script suppress-error="BL9992">

    function init(dotNetRef, elementRef) {
        elementRef.addEventListener('click', () => {
            setTimeout(() => dotNetRef.invokeMethodAsync('IncrementJsInterop'), 50);
        });
    }
</script>

@code {
    ElementReference btnElementRef;
    DotNetObjectReference<TestJSCaller> dotNetRef;

    [Inject] IJSRuntime JS { get; set; }

    [CascadingParameter] EditContext EditContext { get; set; }

    [Parameter] public int Value { get; set; }
    [Parameter] public EventCallback<int> ValueChanged { get; set; }
    [Parameter] public Expression<Func<int>> ValueExpression { get; set; }
    [Parameter] public EventCallback ValueUpdated { get; set; }

    public async Task Increment()
    {
        Console.WriteLine($"=================Increment - Called >> {EditContext.IsModified()}");
        Console.WriteLine($"=> Increment - Value++ >> {EditContext.IsModified()}");
        Value++;
        Console.WriteLine($"=> Increment - ValueChanged.InvokeAsync >> {EditContext.IsModified()}");
        await ValueChanged.InvokeAsync(Value);
        Console.WriteLine($"=> Increment - EditContext.NotifyFieldChanged >> {EditContext.IsModified()}");
        EditContext.NotifyFieldChanged(FieldIdentifier.Create(ValueExpression));
        Console.WriteLine($"=================Increment - Completed >> {EditContext.IsModified()}");
    }

    [JSInvokable]
    public async Task IncrementJsInterop() {
        Console.WriteLine($"=================IncrementJsInterop - Called >> {EditContext.IsModified()}");
        await Increment();
        Console.WriteLine($"=================IncrementJsInterop - Completed >> {EditContext.IsModified()}");
    }


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            dotNetRef = DotNetObjectReference.Create(this);
            await JS!.InvokeVoidAsync("init", dotNetRef, btnElementRef);
        }
    }
}
```

##### Page

```
@page "/Test4"
<h1>Hello, Blazor REPL!</h1>

When you click "Increment using JS Interop" the IsModified: False is not rerendered.
When you click it again, it refreshed to true.
<br />
<br />

This is different from "Increment using C# Event handler".
<br />
<br />
@this.testFrag


<EditForm EditContext="this.editcontext">
    <TestJSCaller @bind-Value="@model.Value" ValueUpdated="ValueUpdated"/>

    <div>
        Value in Main Component: @model.Value
    </div>

    <div>IsModified: @(editcontext.IsModified())</div>
    <div>RenderCounter: @(renderCounter)</div>
</EditForm>

@code {
    private int renderCounter = 1;
    private DataModel model = new DataModel();

    protected EditContext editcontext;

    protected override Task OnInitializedAsync()
    {
        this.editcontext = new EditContext(model);
        return base.OnInitializedAsync();
    }
    protected override void OnAfterRender(bool firstRender)
    {
        Console.WriteLine($"OnAfterRender >> {editcontext.IsModified()}");
    }

    protected void ValueUpdated()
        => StateHasChanged();

    protected override bool ShouldRender()
    {
        Console.WriteLine($"++++> ShouldRender Called >> {editcontext.IsModified()}");
        renderCounter++;
        return true;
    }

    public class DataModel
    {
        public int Value { get; set; }
    }

    private RenderFragment testFrag => (builder) =>
    {
        Console.WriteLine($"++++> Render Fragment Run >> {editcontext.IsModified()}");
        builder.AddContent(0, $"Is Mod: {editcontext.IsModified()}");
    };

}
```

```
public partial class MyComponent : BaseComponent
{
    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
    {
        __builder.OpenElement(0, "div");
        __builder.AddAttribute(1, "class", "my-content");
        __builder.AddAttribute(2, "b-slw7z4imhe");
        __builder.AddMarkupContent(3, "<p b-slw7z4imhe>From derived class</p>\r\n    ");
        __builder.AddContent(4, SayHelloWorld());
        __builder.CloseElement();
    }
}
```