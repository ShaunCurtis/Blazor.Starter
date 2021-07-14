using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.Starter.Data
{
    public class ScopedService
    {
        private TransientService TService { get; set; }

        public ScopedService( TransientService tService)
        {
            TService = tService;
        }
    }
}
