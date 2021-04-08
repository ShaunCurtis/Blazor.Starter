Your problem is trying to do a update on `s`.  The for loop has walked `s` to one past the end - i.e. `null`.  You need to get the `EditForm` to tell you which record is being submitted. 

Here's some code that demonstrates a working methodology for your problem.  I'm using a GUID which I generate for each record to give it a unique ID. I use this to look up the submitted record.  The  code is a working component/pageso you can see it working.

```csharp
@page "/multieditor"

<h3>MultiEditor</h3>
@foreach (var country in Countries)
{
    <EditForm class="input-control" Model="country" OnValidSubmit="() => ValidSubmit(country.ID)">
        <div class="container">
            <div class="row pb-2">
                <div class="col-4">
                    @country.ID.ToString()
                </div>
                <div class="col-6">
                    <InputText @bind-Value="country.Name"></InputText>
                </div>
                <div class="col-2">
                    <button type="submit" class="btn btn-dark">Save</button>
                </div>
            </div>
        </div>
    </EditForm>
}
<div>
    @_submittedGuid
</div>
<div>
@_editcountry
</div>

@code {

    private Guid _submittedGuid;

    private string _editcountry;

    public List<Country> Countries = new List<Country>()
{
      new Country() { Name =  "UK" },
      new Country() { Name=  "Australia" }
    };

    public class Country
    {
        public Guid ID { get; } = Guid.NewGuid();
        public string Name { get; set; }
    }

    private void ValidSubmit(Guid ID)
    {
        _submittedGuid = ID;
        var c = Countries.FirstOrDefault(item => item.ID.Equals(ID));
        _editcountry = c.Name;
        var x = true;
    }
}
```