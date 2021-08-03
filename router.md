Here's an alternative answer using a more generic methodology of doing what you want.

Create a simple Binary Component

BinaryLogicControl.razor
```
@if (this.Show)
{
    if (this.ChildContent != null)
    {
        @this.ChildContent
    }
    else
    {
        @((MarkupString)this.Body)
    }
}

@code {
    [Parameter] public bool Show { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }
    [Parameter] public string Body { get; set; }
}
```

and then using it:

```
<BinaryLogicControl Show="_showDialog">
    I'm Here!!!
</BinaryLogicControl>
<BinaryLogicControl Show="_showDialog" Body="@_showcontent" />
<div>
    <button class="btn btn-dark" @onclick="ShowDialog">Show Dialog</button>
</div>
@code {
    private bool _showDialog;
    private string _showcontent;
    private void ShowDialog()
    {
        _showDialog = !_showDialog;
        _showcontent = _showDialog ? "<div>Hello There</div>" : string.Empty;
    }
}
```