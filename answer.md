Take a look at this.

Step 1 is to create a `UiLoader` component that manages what gets displayed.

The razor code for *UILoader.razor*:
```
@inherits ComponentBase
    @if (this.Loaded)
    {
        @this.ChildContent
    }
    else
    {
        <div id="app">
                <div class="mt-4" style="margin-right:auto; margin-left:auto; width:100%;">
                    <div class="loader"></div>
                    <div style="width:100%; text-align:center;"><h4>@Message</h4></div>
                </div>
            </div>
    }

@code {
    [Parameter] public RenderFragment ChildContent { get; set; }

    [Parameter] public bool Loaded { get; set; }

    [Parameter] public string Message { get; set; } = "Display Loading";
}
```

The Component CSS to make it pretty

```css
.page-loader {
    position: absolute;
    left: 50%;
    top: 50%;
    z-index: 1;
    width: 150px;
    height: 150px;
    margin: -75px 0 0 -75px;
    border: 16px solid #f3f3f3;
    border-radius: 50%;
    border-top: 16px solid #3498db;
    width: 120px;
    height: 120px;
    -webkit-animation: spin 2s linear infinite;
    animation: spin 2s linear infinite;
}

.loader {
    border: 16px solid #f3f3f3;
    /* Light grey */
    border-top: 16px solid #3498db;
    /* Blue */
    border-radius: 50%;
    width: 120px;
    height: 120px;
    animation: spin 2s linear infinite;
    margin-left: auto;
    margin-right: auto;
}

@-webkit-keyframes spin {
    0% {
        -webkit-transform: rotate(0deg);
    }

    100% {
        -webkit-transform: rotate(360deg);
    }
}

@keyframes spin {
    0% {
        transform: rotate(0deg);
    }

    100% {
        transform: rotate(360deg);
    }
}
```

Now the test page.  We enclose all the code that depends on load completion inside the `UILoader` component.  We use async coding and a 2 second delay to emulate a slow data load.  You need to make sure all your dataloading is done before changing `Loaded` to true.  `Reload` destroys all the components inside `UILoader` and renders new instances when Loaded is set back to true.  `LoadEverything` should call a loader in a service that gets all the data that all the components in the page need loaded before the root component sets `Loaded` to true.

```razor
@page "/loader"
<UILoader Loaded="this.Loaded">
    <h3>UILoader Test</h3>
    <button class="btn btn-success" @onclick="Reload">Reload</button>
</UILoader>

@code {

    private bool Loaded = false;

    protected async override Task OnInitializedAsync()
    {
        await LoadEverything();
        this.Loaded = true;
    }


    private async Task LoadEverything()
    {
        await Task.Delay(2000);
        // make sure everything is loaded before completing i.e. returning a completed Task
    }

    private async Task Reload(MouseEventArgs e)
    {
        this.Loaded = false;
        await InvokeAsync(StateHasChanged);
        await LoadEverything();
        this.Loaded = true;
        await InvokeAsync(StateHasChanged);
    }
}
```

