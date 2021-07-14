
Add a JS file to the client app. 

*site.js*
```

window.SetBaseHref = function (elementId, hRef) {
    var link = document.getElementById(elementId);
    if (link !== undefined) {
        link.href = hRef;
    }
    return true;
}
```

*wwwroot/appsettings.json*
```
{
  "Configuration": {
    "BaseHRef": "/blue"
  }
}
```

Reference it in _index.html_ and create an `id` on `<base>`.

```
    <base href="~/" id="BlazorHRef" />

.......

    <script src="/site.js"></script>

```

##### Demo in the index page

```
@page "/"
@page "/red/"

<h1>Hello, world!</h1>

    Welcome to your new app.

<button type="button" @onclick="this.SetBaseHRef">SetHRef</button>

<SurveyPrompt Title="How is Blazor working for you?" />
@code {

    [Inject] private IJSRuntime _js { get; set; }

    private bool _isAltHRef;

    private string _altHRef => _isAltHRef ? "/" : "/red";

    public async Task SetBaseHRef()
    {
        await this.SetBaseHref("BlazorHRef", _altHRef);
        _isAltHRef = !_isAltHRef;
    }

    public ValueTask<bool> SetBaseHref(string elementId, string hRef)
      => _js.InvokeAsync<bool>("SetBaseHref", elementId, hRef);

}
```

##### Setting it at startup in *App.razor*

```
<Router AppAssembly="@typeof(Program).Assembly" PreferExactMatches="@true">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
    </Found>
    <NotFound>
        <LayoutView Layout="@typeof(MainLayout)">
            <p>Sorry, there's nothing at this address.</p>
        </LayoutView>
    </NotFound>
</Router>

@code {
    [Inject] IConfiguration Configuration { get; set; }

    [Inject] private IJSRuntime _js { get; set; }

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        var baseHRef = Configuration.GetValue<string>("Configuration:BaseHRef");
        if (!string.IsNullOrEmpty(baseHRef)) await this.SetBaseHref("BlazorHRef", baseHRef);
    }

    public ValueTask<bool> SetBaseHref(string elementId, string hRef)
          => _js.InvokeAsync<bool>("SetBaseHref", elementId, hRef);

}
```

You can change out most things in the header in a similar way.