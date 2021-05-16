The error you report is ` Delegate 'Func<Pen, int, stirng>' doest not take 1 arguement`  yet Property  is `Func<TItem, string>`.

`Func<TItem, string> Property` says I want a function that looks like this `string MyMethod(TItem item)`.  Your anonymous function is `pen => pen.PenNumber`, Property expecting `pen.PenNumber` is string.  Is it?

Here's a simplified working version of what you are doing:

#### TextBox.razor
```razor
@typeparam TItem
<h3>@Value</h3>

@foreach (var item in Data.Select(this.Property))
{
 <div>@item</div>
}

@code {
    [Parameter] public string Value { get; set; } = "Bonjour";
    [Parameter] public Func<TItem, string> Property { get; set; }
    [Parameter] public List<TItem> Data { get; set; }
}
```

#### Index.razor
```
@page "/"
@using Blazor.Starter.Data

<div>
    <TextBox TItem="Model" Data="models" Property="item => item.Value"></TextBox>
</div>

@code {

    public string GetProperty(Model model)
    {
        return model.Value;
    }

    public Model GetProperty1()
    {
        return new Model();
    }

    public List<Model> models => new List<Model>()
    {
        new Model() { Value = "Fred"},
        new Model() { Value = "Jon"},
        new Model() { Value = "Bob"},
     };


    public class Model
    {
        public string Value { get; set; }
    }
}
```