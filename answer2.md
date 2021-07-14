
Here's your component.  I've lifted the `BuildRenderTree` code from `InputDate` and added in the `ValidationMessage` component.  You need to do it this way as I don't know a way to do the `For` binding in Razor.  I've tied the `For` directly into the `ValueExpression` property of `InputBase`.

```c#
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System;

#nullable enable
namespace Blazor.Starter.Components.TestComponents
{
    public class CustomDate : InputDate<DateTime?>
    {

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "input");
            builder.AddMultipleAttributes(1, AdditionalAttributes);
            builder.AddAttribute(2, "type", "date");
            builder.AddAttribute(3, "class", CssClass);
            builder.AddAttribute(4, "value", BindConverter.FormatValue(CurrentValueAsString));
            builder.AddAttribute(5, "onchange", EventCallback.Factory.CreateBinder<string?>(this, __value => CurrentValueAsString = __value, CurrentValueAsString));
            builder.CloseElement();
            builder.OpenComponent<ValidationMessage<DateTime?>>(6);
            builder.AddAttribute(7, "For", this.ValueExpression);
            builder.CloseComponent();
        }
    }
}
#nullable disable
``` 

And a demo page.  There's a button to manually set an error message in the `validationMessageStore`.

```c#
@page "/editortest"

<h3>EditorTest</h3>
    <EditForm EditContext="editContext">
        <div>
            <CustomDate @bind-Value="model.Date"></CustomDate>
        </div>
    </EditForm>
<div class="m-3 p-3"><input @bind-value="_errormessage"><button class="btn btn-dark ms-2" @onclick="SetError">Set Error</button></div>


@code {
    private dataModel model { get; set; } = new dataModel();
    private EditContext editContext;
    private string _errormessage { get; set; } = "Error in date";

    protected override Task OnInitializedAsync()
    {
        this.editContext = new EditContext(model);
        return base.OnInitializedAsync();
    }

    private void SetError( MouseEventArgs e)
    {
        var validationMessageStore = new ValidationMessageStore(this.editContext);
        validationMessageStore.Clear();
        var fi = new FieldIdentifier(this.model, "Date");
        validationMessageStore.Add(fi, _errormessage);
    }

    public class dataModel
    {
        public string Email { get; set; }
        public DateTime? Date { get; set; }
    }

}
``` 
