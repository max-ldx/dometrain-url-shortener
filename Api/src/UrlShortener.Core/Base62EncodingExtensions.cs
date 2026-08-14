namespace UrlShortener.Core;

public static class Base62EncodingExtensions
{
    private const string Alphanumeric = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    public static string EncodeToBase62(this int number)
    {
        if (number == 0) return Alphanumeric[0].ToString();

        var result = new Stack<char>();

        while (number > 0)
        {
            result.Push(Alphanumeric[number % Alphanumeric.Length]);
            number /= Alphanumeric.Length;
        }

        return new string(result.ToArray());
    }
}