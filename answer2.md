You code doesn't take account of the fact that `MultiStepNavigation` is child content of `MultiStepComponent`.

Look in */obj/debug/net5.0/razor/...*, find your razor version of your component and look at the code.

You probably need to do something like this:

```csharp
private RenderFragment _MultiStepNavigationFragment => b =>
{
        b.OpenComponent<MultiStepNavigation>(0);
        b.AddAttribute(1, "Name", "First Step");
        b.CloseComponent();
};
```

```csharp
private RenderFragment _MultiStepComponentFragment => b =>
{
        b.OpenComponent<MultiStepComponent>(0);
        b.AddAttribute(1, "id", "MultiStepContainer");
        b.AddAttribute(2, "ChildContent", this._MultiStepNavigationFragment);
        b.CloseComponent();
};
```


You state 
> Now, the problem is that when I add new razor component and use AdminLayout.razor layout the view doesnot use css and js of _HostAdmin.cshtml.

and I assume your question is "Why".

You're adding a new razor component page to the existing application - probably with a route of something like "/admin/myadminpage". You are misunderstanding what's actually going on.

*_Host.cshtml* loads the SPA, but that's the only get/post that happens.  Navigation after that is changing out components in the DOM.  Loading a component with the layout `AdminLayout` just changes out the Layout component.  There's no toing and froing with the server.

What you are trying to do requires a reload of the SPA.  You could use:

```
NavigationManager.NavigateTo("/admin/myadminpage", true);
```

Also even if you do force a reload, the Fallbacks are in the wrong order - the default before the specific.
```
    endpoints.MapFallbackToPage("/_Host");
    endpoints.MapFallbackToPage("~/Admin/{*clientroutes:nonfile}", "/_HostAdmin");
```
  
Every event in the UI has two steps, the call to the event handler followed by a call to `StateHasChanged`.  If the event handler returns a `Task` then the caller waits on it's completion before calling `StateHasChanged`.  If it returns a `void` then `StateHasChanged` is called when the event handler yields (which may or may not be when it completes).

Let's look at what you have set up.

```
<input @ref="input" class="in" type="text" @bind-value="@_command" @bind-value:event="oninput" @onkeypress="@(e => KeyPressed(e))">
```

 1. `input` value is wired to `_command` and set it to update every time the user enters a value (at every keystroke including an *enter*).  
2. `onkeypress' is wired up `KeyPressed` to every keypress.  
 
You have two events kicking off whenever the user enters a value.

Now to `KeyPressed`

```
    commandRunning = true;
    await Task.Delay(1000);
    commandRunning = false;

    await input.FocusAsync();
```

The first await yields control back to the UI thread which almost certainly updates `_command` and fires `StateHasChanged` at the completion of that event.  With `Commandrunning` set to `false` the html elements are wiped out and cease to exist.  

After the `TaskDelay` completes, `commandRunning` is set back to true.  You then try to set the focus to `input` which doesn't exist (the component hasn't been rerendered and input created).  Adding the `StateHasChanged` before `FocusAsync` re-renders the component, setting up all the html elements including `input`.
After `KeyPressed` completes `StateHasChanged` is called by the orginating event (which doesn't do anything new).

To ensure what you want to do works correctly:

```
    private async Task KeyPressed(KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
        {
            commandRunning = true;
            // make sure the UI has re-rendered and the html is destroyed
            await InvokeAsync(StateHasChanged);
            //  Do whatever you're doing
            await Task.Delay(1000);
            commandRunning = false;
            // make sure the UI has re-rendered and the html is re-built
            await InvokeAsync(StateHasChanged);
            await input.FocusAsync();
        }
    }
```

A classic ASP.Net deals with page requests - here's what I want, get it, return the new page.  It's transactional, everything needs to happen before the end result gets returned to the browser.  You can pool/share/manage database connections....  With Blazor there's no longer a single transaction.  Multiple components on a page may be interacting with the database at the same time so there's potential a lot more independant async behaviour going.  Hence Blazor recommends using short lived contexts per component and the Factory.