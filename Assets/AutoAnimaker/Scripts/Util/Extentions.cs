
namespace AutoAnimaker
{
    public static class Extensions
    {
        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null or "" => "",
                _ => input[0].ToString().ToUpper() + input.Substring(1).ToLower()
            };
    }
}