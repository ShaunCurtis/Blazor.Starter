using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Linq.Expressions;

namespace Blazor.Starter.Components.UIComponents
{
    public class MyMudBlazorTextField<TValue> : ComponentBase
    {
        //[Parameter]
        //public TValue? Value { get; set; }

        //[Parameter] public EventCallback<TValue> ValueChanged { get; set; }

        //[Parameter] public Expression<Func<TValue>>? ValueExpression { get; set; }

        //[Parameter] public string Label { get; set; }

        //[Parameter] public Variant Variant { get; set; } = Variant.Outlined;

        //[Parameter] public Margin Margin { get; set; } = Margin.Dense;

        //protected override void BuildRenderTree(RenderTreeBuilder builder)
        //{
        //    builder.OpenComponent(0, typeof(MudTextField));
        //    builder.AddAttribute(1, "Variant", this.Variant);
        //    builder.AddAttribute(2, "Margin", this.Margin);
        //    builder.AddAttribute(3, "Label", this.Label);
        //    builder.AddAttribute(4, "Value", this.Value);
        //    builder.AddAttribute(5, "ValueChanged", EventCallback.Factory.Create(this, this.ValueChanged));
        //    builder.AddAttribute(6, "ValueExpression", this.ValueExpression);
        //    builder.CloseComponent();
        //}
    }
}
