Without re-writing the `RouteView` component and doing some big changes to the way Blazor components are loaded, you can't "Blank" the entire page.  Why would you.  The only time you get "blink" on the side menus/top bar is when you first load the SPA.  After that it only get re-rendered if you change the layout, and bits of it get rerendered when something changes - like login status,....

Take a look at this.  It may or may not suit your needs.

The basic premise is to replace the Layout with your own Layout component that only displays the layout when the main component sets a boolean parameter.

Create a new Layout *LoaderLayout.razor* - basically just renders `@Body` - you can't set `@layout = null`.
```html
@inherits LayoutComponentBase
@Body
```

Create a `UILoader` component that is effectively your layout and manages what gets displayed.  I've created with the standard Blazor Layout.

The razor code for *UILoader.razor*:
```
@inherits ComponentBase
@if (this.Loaded)
{
    <div class="page">
        <div class="sidebar">
            <NavMenu />
        </div>

        <div class="main">
            <div class="top-row px-4">
                <a href="https://docs.microsoft.com/aspnet/" target="_blank">About</a>
            </div>

            <div class="content px-4">
                @this.ChildContent
            </div>
            <div class="px-4">
                @this.Footer
            </div>
        </div>
    </div>
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

    [Parameter] public RenderFragment Footer { get; set; }

    [Parameter] public bool Loaded { get; set; }

    [Parameter] public string Message { get; set; } = "Display Loading";
}
```

The Component CSS to make it pretty - includes the MainLayout css

```css
*/ Copy into here the contents of mainlayout.razor.css/*
*/ To big to show!/*

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

Now the test page.  Enclose all the code that depends on load completion inside the `UILoader` component.  I use async coding and a 2 second delay to emulate a slow data load.  You need to make sure all your data loading is done before changing `Loaded` to true.  `Reload` destroys all the components inside `UILoader` and renders new instances when Loaded is set back to true.  `LoadEverything` should call a loader in a service to get all the data all the components in the page need loaded before the root component sets `Loaded` to true.

```razor
@page "/loader"
@layout LoaderLayout
<UILoader Loaded="this.Loaded">
    <ChildContent>
        <h3>UILoader Test</h3>
        <button class="btn btn-success" @onclick="Reload">Reload</button>
    </ChildContent>
    <Footer>
        <div class="bg-primary text-white m-2 p-4">My Newsletter Data</div>
    </Footer>
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
You can find:

 - [A Running demo on my demo site here](https://blazor-starter.azurewebsites.net/loader)
 - [My Test Repo with the code here](https://github.com/ShaunCurtis/Blazor.Starter)
