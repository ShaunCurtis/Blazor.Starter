As your quoted code deosn't fully show who calls search, here's an adapted versaion of your code that refreshs.

Note it's async and uses `Tasks`.  `await Task.Delay(3000);` emulates your code.

```
@page "/Demo"
<h3>Demoblock</h3>
@if (_loading)
{
    <div class="bg-warning p-2 m-2">Loading...</div>
}
else
{
    <div class="bg-success p-2 m-2">Loaded</div>
}
<div class="p-2 m-2">
    <button class="btn btn-dark" @onclick="DoSearch">New Search</button>
</div>
@code
{
    private bool _loading;

    private async Task Search()
    {
        _loading = true;
        // needed
        StateHasChanged();
        //Mails.Clear();
        //Licenses.Clear();
        //Callings.Clear();
        //Supports.Clear();
        //Leads.Clear();
        //search();
        //Emulate doing the above work
        await Task.Delay(3000);
        _loading = false;
        // not needed
        // StateHasChanged();
    }

    protected async override Task OnInitializedAsync()
    {
        await Search();
    }

    private async Task DoSearch(MouseEventArgs e)
    {
        await Search();
    }
}
```

What's happening is this in pseudo code:

```
Set up a Task when you run the Event - such as OninitializedAsync, On ParametersSetAsync, Mouse/Keyboard Event
If Task has not completed 
{
    Render the Component.
    Now Wait for the Task to Complete
}
Render the Component
```

The key here is that the first render only occurs if your event returns a Task and yields.  
1. If you event yields but returns a void then there's no task for the hanlder to wait on so runs the last render before the event completes.
2. If your code is synchronous i.e. no yields, it completes so the Task has completed and only the second render occurs.
