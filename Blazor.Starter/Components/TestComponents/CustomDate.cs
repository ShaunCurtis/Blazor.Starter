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
            builder.AddAttribute(8, "For", this.ValueExpression);
            builder.CloseComponent();
        }
    }
}
#nullable disable
