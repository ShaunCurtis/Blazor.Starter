You are trying to reload `roleList` in `OnAfterRenderAsync`.  The name of the method should be a bit of a give away.  **AFTERRENDER** means the component has already rendered and any changes only get into the UI when the component get re-rendered.  Add your `roleList` update to `DeleteRole`.  The component won't re-render till `DeleteRole` completes.


```csharp
public async Task DeleteRole(string roleId)
    {
        var role = await roleManager.FindByIdAsync(roleId);
        bool confirmed = await jSRuntime.InvokeAsync<bool>("confirm", "Are you sure?");
        if (confirmed)
        {
            var result = await roleManager.DeleteAsync(role);       
            if (result.Succeeded)
            {
                MessageStaus.MessageId = 3;
                // Added here to update roleList after it changes
                roleList = mainService.GetAllRoles();
            }
        }
    }
```

You're binding to `@bind-Value="Skill.Name"` so I'm assuming `Skill` isn't null.  Your test is on `skill`, not `skill.name`, so as `skill` isn't `null` you hit the `else` option.  Try:

```
  @if (string.IsNullOrEmpty(skill.Name))
    {
        ......
    }

However, you don't need to do any of this as the placeholder will only diplay when the field is empty.
```