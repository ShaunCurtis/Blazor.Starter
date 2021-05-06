There's a few misconceptions around the component lifecycle cycle and rendering. There's only one set-in-stone call to `StateHasChanged`.  This occurs at the completion of `SetParametersAsync` - the end of the synchronous part of the component refresh cycle.  There are two other conditional calls to `StateHasChanged`.  One is in initialization, and runs only when `OnInitializedAsync` yields but isn't complete.  The second is in parameter set and runs only if `OnParametersSetAsync` yields but isn't complete.  It's clearly stated in the Microsoft diagram, but I'm not sure most people understand it. [Ms Docs - Razor Component Lifecycle](https://docs.microsoft.com/en-us/aspnet/core/blazor/components/lifecycle?view=aspnetcore-5.0)



Controller Code:
```csharp
[MVC.Route("/api/weatherforecast/read")]
[HttpPost]
public async Task<WeatherForecast> Read([FromBody]int id) => await DataService.GetRecordAsync<WeatherForecast>(id);
```

API Data Service
```csharp
public override async Task<WeatherForecast> GetRecordAsync(int id)
{
    var response = await this.HttpClient.PostAsJsonAsync($"/api/weatherforecast/read", id);
    var result = await response.Content.ReadFromJsonAsync<WeatherForecast>();
    return result;
}
```

[Blazor CRUDL Database Template](https://github.com/ShaunCurtis/Blazor.Database)

`MyParameter` is extracted from the Route by the Router and passed into the page component as a Parameter in `SetParametersAsync`.  In theory you could put in on the setter for `MayParameter`.  Don't - this is very definitely not recommended practice.  Also, Lets say you're on "/mypage/1" and you navigate to "/mypage/2", `OnInitialized` won't be called.  you may think you're navigating between pages, but in reality, your just calling `SetParametersAsync` on the same component with a new value for `MyParamater`.

Therefore something like: 
```
    protected override void OnParametersSet() 
    {
        if (!ViewModel.myParameter.Equals(myParameter)) ViewModel.myParameter = myParameter;
    }
```

Will ensure it is set if it changes, otherwise not (I don't know what getters/setters are on `myParameter` and what changing it every time on `OnParametersSet` precipitates!).

```
<div>

    <!-- ... -->

    <div class="modal fade" id="id" tabindex="-1" aria-labelledby="label" aria-hidden="true">

        <!-- ... -->
        @if (showMessage) 
            {
            <div class="@css" role="alert">
            @message
            </div>
            }

        <!-- ... -->

    </div>

    <!-- ... -->

</div>

@code {
    private String resultMessage;

private bool showMessage = false;
privare bool success = true;
private string message = success ? "Query Saved": "Query Not Saved";
private string css = success ? "alert alert-success": "alert alert-danger";

    public void doSomething(){

        success = is200Success;
        showMessage = true;
    }
}
```