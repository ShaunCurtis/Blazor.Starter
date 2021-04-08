I think what you want is a Service.  

Define a class 
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Starter.Services
{
    public class BaseService
    {
        public string Message { get; set; }
    }
}
```

Add it to services in startup/Program as a singleton

```csharp
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<BaseService>();
        }
```

Display/Consume/set it it in any component/page

```csharp
<h3>@MyBase.Message</h3>

@code {

    [Inject] private BaseService MyBase { get; set; }

    void ConsumerMethod()
    {
        var x = MyBase.Message;
    }

    void SetterMethod()
    {
        MyBase.Message = "Bonjour";
    }
}
```

If you want to change it in one place and see it update somewhere else the you'll need to set up a notifier service