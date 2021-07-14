using Blazor.SPA.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Blazor.Starter.Components.UIComponents
{
    public class FormEditControl<TValue> : ComponentBase
    {
        [Parameter]
        public TValue? Value { get; set; }

        [Parameter] public EventCallback<TValue> ValueChanged { get; set; }

        [Parameter] public Expression<Func<TValue>>? ValueExpression { get; set; }

        [Parameter] public string? Label { get; set; }

        [Parameter] public string? HelperText { get; set; }

        [Parameter] public string DivCssClass { get; set; } = "mb-2";

        [Parameter] public string LabelCssClass { get; set; } = "form-label";

        [Parameter] public string ControlCssClass { get; set; } = "form-control";

        [Parameter] public Type ControlType { get; set; } = typeof(InputText);

        [Parameter] public bool ShowValidation { get; set; }

        [Parameter] public bool ShowLabel { get; set; } = true;

        [Parameter] public bool IsRequired { get; set; }

        [CascadingParameter] EditContext CurrentEditContext { get; set; } = default!;


        private readonly string formId = Guid.NewGuid().ToString();

        private bool IsLabel => this.ShowLabel && (!string.IsNullOrWhiteSpace(this.Label) || !string.IsNullOrWhiteSpace(this.FieldName));

        private bool IsValid;

        private FieldIdentifier _fieldIdentifier;

        private ValidationMessageStore? _messageStore;

        private string? DisplayLabel => this.Label ?? this.FieldName;
        private string? FieldName
        {
            get
            {
                string? fieldName = null;
                if (this.ValueExpression != null)
                    ParseAccessor(this.ValueExpression, out var model, out fieldName);
                return fieldName;
            }
        }

        private string MessageCss => CSSBuilder.Class()
            .AddClass("invalid-feedback", !this.IsValid)
            .AddClass("valid-feedback", this.IsValid)
            .Build();

        private string ControlCss => CSSBuilder.Class(this.ControlCssClass)
            .AddClass("is-valid", this.IsValid)
            .AddClass("is-invalid", !this.IsValid)
            .Build();

        protected override void OnInitialized()
        {
            if (CurrentEditContext is null)
                throw new InvalidOperationException($"No Cascading Edit Context Found!");

            if (ValueExpression is null)
                throw new InvalidOperationException($"No ValueExpression defined for the Control!  Define a Bind-Value.");

            if (!ValueChanged.HasDelegate)
                throw new InvalidOperationException($"No ValueChanged defined for the Control! Define a Bind-Value.");

            CurrentEditContext.OnFieldChanged += FieldChanged;
            _messageStore = new ValidationMessageStore(this.CurrentEditContext);
            if (_messageStore is null)
                throw new InvalidOperationException($"Cannot set the Validation Message Store!");

            if (this.IsRequired && !string.IsNullOrWhiteSpace(this.HelperText))
            {
                _fieldIdentifier = FieldIdentifier.Create(ValueExpression);
                if (!string.IsNullOrWhiteSpace(this.HelperText))
                    _messageStore.Add(_fieldIdentifier, this.HelperText);
            }
        }

        protected void FieldChanged(object sender, FieldChangedEventArgs e)
        {
            if (e.FieldIdentifier.Equals(_fieldIdentifier))
                _messageStore.Clear();
        }

        protected override void OnParametersSet()
        {
            this.IsValid = true;
            {
                if (this.IsRequired)
                {
                    this.IsValid = false;
                    var messages = CurrentEditContext.GetValidationMessages(_fieldIdentifier).ToList();
                    if (messages is null || messages.Count == 0)
                        this.IsValid = true;
                }
            }
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            if (this.IsLabel)
            {
                builder.AddAttribute(10, "class", this.DivCssClass);
                builder.OpenElement(20, "label");
                builder.AddAttribute(30, "for", this.formId);
                builder.AddAttribute(40, "class", this.LabelCssClass);
                builder.AddContent(45, this.DisplayLabel);
            }
            builder.CloseElement();
            builder.OpenComponent(50, this.ControlType);
            builder.AddAttribute(55, "class", this.ControlCss);
            builder.AddAttribute(60, "Value", this.Value);
            builder.AddAttribute(70, "ValueChanged", EventCallback.Factory.Create(this, this.ValueChanged));
            builder.AddAttribute(80, "ValueExpression", this.ValueExpression);
            builder.CloseComponent();
            if (this.ShowValidation && !this.IsValid)
            {
                builder.OpenElement(110, "div");
                builder.AddAttribute(120, "class", MessageCss);
                builder.OpenComponent<ValidationMessage<TValue>>(130);
                builder.AddAttribute(140, "For", this.ValueExpression);
                builder.CloseComponent();
                builder.CloseElement();
            }
            else if (!string.IsNullOrWhiteSpace(this.HelperText))
            {
                builder.OpenElement(110, "div");
                builder.AddAttribute(120, "class", MessageCss);
                builder.AddContent(130, this.HelperText);
                builder.CloseElement();
            }
            builder.CloseElement();
        }

        // Code lifted from FieldIdentifier.cs
        private static void ParseAccessor<T>(Expression<Func<T>> accessor, out object model, out string fieldName)
        {
            var accessorBody = accessor.Body;
            if (accessorBody is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert && unaryExpression.Type == typeof(object))
                accessorBody = unaryExpression.Operand;

            if (!(accessorBody is MemberExpression memberExpression))
                throw new ArgumentException($"The provided expression contains a {accessorBody.GetType().Name} which is not supported. {nameof(FieldIdentifier)} only supports simple member accessors (fields, properties) of an object.");

            fieldName = memberExpression.Member.Name;
            if (memberExpression.Expression is ConstantExpression constantExpression)
            {
                if (constantExpression.Value is null)
                    throw new ArgumentException("The provided expression must evaluate to a non-null value.");
                model = constantExpression.Value;
            }
            else if (memberExpression.Expression != null)
            {
                var modelLambda = Expression.Lambda(memberExpression.Expression);
                var modelLambdaCompiled = (Func<object?>)modelLambda.Compile();
                var result = modelLambdaCompiled();
                if (result is null)
                    throw new ArgumentException("The provided expression must evaluate to a non-null value.");
                model = result;
            }
            else
                throw new ArgumentException($"The provided expression contains a {accessorBody.GetType().Name} which is not supported. {nameof(FieldIdentifier)} only supports simple member accessors (fields, properties) of an object.");
        }


    }
}
