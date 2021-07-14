Another approach using the setter on a property.  This works in both standalone and the `EditForm` context. 

```
@page "/MyDate"
<h3>MyDate Editor</h3>
<EditForm EditContext="editContext">
    <InputDate @bind-Value="model.MyDate"></InputDate>
</EditForm>
<input type="date" @bind-value="this.MyDate" />

@code {

    private dataModel model { get; set; } = new dataModel();
    private EditContext editContext;

    protected override Task OnInitializedAsync()
    {
        this.editContext = new EditContext(model);
        return base.OnInitializedAsync();
    }


    public class dataModel
    {
        public DateTime? MyDate
        {
            get => _myDate;
            set
            {
                if (value != null)
                {
                    var date = (DateTime)value;
                    _myDate = date.AddDays(-(int)date.DayOfWeek);
                }
            }
        }
        private DateTime? _myDate;
    }

    public DateTime? MyDate
    {
        get => _myDate;
        set
        {
            if (value != null)
            {
                var date = (DateTime)value;
                _myDate = date.AddDays(-(int)date.DayOfWeek);
            }
        }
    }
    private DateTime? _myDate;
}
```