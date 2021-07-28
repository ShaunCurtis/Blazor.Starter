To get this running I suggest you set up a blank WASM project from the Blazor template.  Once you have it running you can make the necessary changes to your project.  My project is called *Blazor.Starter.WASM*.

Move the *Client* project *wwwroot* to the *Server* project.

Copy *index.html* to *dashboard.html*
*serverproject/wwwroot/dashboard.html*

```
<!DOCTYPE html>
<html>

<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
    <title>Blazor.Starter.WASM</title>
    <base href="/" />
    <link href="css/bootstrap/bootstrap.min.css" rel="stylesheet" />
    <link href="css/app.css" rel="stylesheet" />
    <link href="Blazor.Starter.WASM.Client.styles.css" rel="stylesheet" />
</head>

<body>
    <div id="app">Loading...</div>

    <div id="blazor-error-ui">
        An unhandled error has occurred.
        <a href="" class="reload">Reload</a>
        <a class="dismiss">🗙</a>
    </div>
    <script src="/site.js"></script>
    <script src="_framework/blazor.webassembly.js"></script>
</body>

</html>
```

My *index.html* looks like this:

```
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
    <title>Blazor.Starter.WASM</title>
    <base href="/"/>
    <link href="css/bootstrap/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <div class="m-4 p-4">
        <a href="/dashboard">Go to Dashboard</a>
    </div>
</body>
</html>
```

*Index.razor* looks like this.  Note the two page references

```
@page "/"
@page "/dashboard/"

<h1>Hello, world!</h1>
    Welcome to your new app.
<SurveyPrompt Title="How is Blazor working for you?" />

@code {
}
```

*Counter.razor* and all other *pages* in the SPA - add `@page "/dashboard/nnnnn"`

```
@page "/counter"
@page "/dashboard/counter"
.....
```

*NavMenu.razor* looks like this:

```
.....
<div class="@NavMenuCssClass" @onclick="ToggleNavMenu">
    <ul class="nav flex-column">
        <li class="nav-item px-3">
            <NavLink class="nav-link" href="dashboard" Match="NavLinkMatch.All">
                <span class="oi oi-home" aria-hidden="true"></span> Home
            </NavLink>
        </li>
        <li class="nav-item px-3">
            <NavLink class="nav-link" href="dashboard/counter">
                <span class="oi oi-plus" aria-hidden="true"></span> Counter
            </NavLink>
        </li>
        <li class="nav-item px-3">
            <NavLink class="nav-link" href="dashboard/fetchdata">
                <span class="oi oi-list-rich" aria-hidden="true"></span> Fetch data
            </NavLink>
        </li>
......
```

Finally update *Startup.cs* on the Server project.

```
....
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

           // New section to map all requests starting with /dashboard to the Blazor Wasm project
            app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/dashboard"), app1 =>
            {
                app1.UseRouting();
                app1.UseEndpoints(endpoints =>
                {
                    endpoints.MapRazorPages();
                    endpoints.MapControllers();
                    endpoints.MapFallbackToFile("dashboard.html");
                });

            });

            // default endpoint
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
```

If I have these instructions correct the application should start on the simple index page.  Once in the WASM try a F5 to check the Endpoint mapping works correctly.  Note that all `@page` references within the SPA must be prefixed with `/dashboard` to ensure the F5 reload works. 
