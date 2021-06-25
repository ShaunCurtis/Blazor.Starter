Add a JS file. 

*site.js*
```

window.SetBodyCss = function (elementId, classname) {
    var link = document.getElementById(elementId);
    if (link !== undefined) {
        link.className = classname;
    }
    return true;
}
```

Reference it in _Host.cshtml_.  
Create an `id` on `<body>`.

```
<body id="BlazorMainBody">

    <script src="/site.js"></script>
    <script src="_framework/blazor.server.js"></script>
</body>
```

Add a helper class 
```
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Blazor.Starter
{
    public class AppJsInterop
    {
        protected IJSRuntime JSRuntime { get; }

        public AppJsInterop(IJSRuntime jsRuntime)
        {
            JSRuntime = jsRuntime;
        }

        public ValueTask<bool> SetBodyCss(string elementId, string cssClass)
          => JSRuntime.InvokeAsync<bool>("SetBodyCss", elementId, cssClass);
    }
}
```

This shows toggling it in a page

```
<button type="button" @onclick="SetCss">SetCss</button>

@code {
    [Inject] private IJSRuntime _js { get; set; }

    private bool _isbodyCss;

    private string _bodyCss => _isbodyCss ? "Theme-Dark" : "Theme-Light";

    private async Task SetCss()
    {
        var appJsInterop = new AppJsInterop(_js);
        await appJsInterop.SetBodyCss("BlazorMainBody", _bodyCss);
        _isbodyCss = !_isbodyCss;
    }
}
```

You can change out the whole stylesheet in a similar way - see this [note of mine](https://shauncurtis.github.io/posts/DynamicCss.html)