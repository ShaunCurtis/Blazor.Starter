namespace Blazor.Starter.WASM.Client
{
    public class AppConfiguration
    {
        public string BaseHRef { get; set; }

        public override string ToString()
        {
            return $"BaseHRef: {this.BaseHRef}";
        }
    }
}
