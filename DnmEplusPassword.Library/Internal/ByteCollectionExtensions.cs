using System.Text;
using static DnmEplusPassword.Library.Internal.Constants;

namespace DnmEplusPassword.Library.Internal;

internal static class ByteCollectionExtensions
{
    public static int Sum(this Span<byte> bytes)
    {
        int sum = 0;
        foreach (var b in bytes)
        {
            sum += b;
        }
        return sum;
    }

    public static string ToUnicodeText(this Span<byte> bytes)
        => ToUnicodeText((ReadOnlySpan<byte>)bytes);

    public static string ToUnicodeText(this ReadOnlySpan<byte> bytes)
    {
        Span<Rune> runes = stackalloc Rune[bytes.Length];
        for (int i = 0; i < bytes.Length; i++)
        {
            runes[i] = UnicodeCharacterCodepoints[bytes[i]];
        }
        return runes.FastToString();
    }
}
