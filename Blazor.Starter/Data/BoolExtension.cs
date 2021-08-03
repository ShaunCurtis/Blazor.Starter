namespace Blazor.Starter
{
    public static class BoolExtension
    {
        public static string AsYesNo(this bool value)
            => value ? "Yes" : "No";
    }
}
