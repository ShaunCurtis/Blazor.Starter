using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Starter.Components.TestComponents
{
    public partial class InputIncrementer : InputBase<int>
    {
        [DisallowNull] public ElementReference Element { get; protected set; }

        [Inject] IJSRuntime JS { get; set; }

        [Parameter] public EventCallback ValueUpdated { get; set; }

        DotNetObjectReference<InputIncrementer> dotNetRef;

        protected override bool TryParseValueFromString(string? value, out int result, [NotNullWhen(false)] out string? validationErrorMessage)
            => throw new NotSupportedException($"This component does not parse string inputs. Bind to the '{nameof(CurrentValue)}' property, not '{nameof(CurrentValueAsString)}'.");

        public async Task Increment()
        {
            CurrentValue++;
            await ValueChanged.InvokeAsync(Value);
            EditContext.NotifyFieldChanged(FieldIdentifier.Create(ValueExpression));
            if (ValueUpdated.HasDelegate) 
                ValueUpdated.InvokeAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                dotNetRef = DotNetObjectReference.Create(this);
                await JS!.InvokeVoidAsync("init", dotNetRef, Element);
            }
        }

    }
}

