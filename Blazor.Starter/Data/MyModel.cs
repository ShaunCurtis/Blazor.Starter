using System.ComponentModel.DataAnnotations;

namespace Blazor.Starter.Data
{
    public class MyModel
    {
        [StringLength(3)][Required] public string Name { get; set; }
    }
}