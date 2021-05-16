I think your answer over complicates this.

The code below demonstrates a basic setup (it's demo code not production).

It uses the `EditForm` with a model.  There are radio buttons and checkboxes linked into a model that get updated correctly.  `Selected` has a setter so you can put a breakpoint in ans see who's being updated.

```c#
@page "/"
@using Blazor.Starter.Data

<EditForm EditContext="this.editContext">

    @foreach (var model in models)
    {
        <h3>@model.Value</h3>

        <h5>Check boxes</h5>
        foreach (var option in model.Options)
        {
            <div>
                <InputCheckbox @bind-Value="option.Selected" />@option.Value
            </div>
        }
        <h5>Option Select</h5>
        <div>
            <InputRadioGroup @bind-Value="model.Selected">
                @foreach (var option in model.Options)
                    {
                    <div>
                        <InputRadio Value="option.Value" /> @option.Value
                    </div>
                    }
            </InputRadioGroup>
            <div>
                Selected: @model.Selected
            </div>
        </div>
    }
</EditForm>
<button class="btn btn-dark" @onclick="OnClick">Check</button>

@code {

    private EditContext editContext;

    private List<Model> models;

    protected override Task OnInitializedAsync()
    {
        models = Models;
        editContext = new EditContext(models);
        return Task.CompletedTask;
    }

    public void OnClick(MouseEventArgs e)
    {
        var x = true;
    }

    public List<Model> Models => new List<Model>()
    {
        new Model() { Value = "Fred"},
        new Model() { Value = "Jon"},
     };


    public class ModelOptions
    {
        public string Value { get; set; }
        public bool Selected
        {
            get => _Selected;
            set
            {
                _Selected = value;
            }
        }
        public bool _Selected;
    }

    public class Model
    {
        public string Value { get; set; }
        public string Selected { get; set; }
        public List<ModelOptions> Options { get; set; } = new List<ModelOptions>()
        {
            new ModelOptions() {Value="Tea", Selected=true},
            new ModelOptions() {Value="Coffee", Selected=false},
            new ModelOptions() {Value="Water", Selected=false},

        };
    }
}
``` 