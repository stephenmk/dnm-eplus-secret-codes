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

    public static string DecodeToUnicodeText(this Span<byte> bytes)
        => DecodeToUnicodeText((ReadOnlySpan<byte>)bytes);

    public static string DecodeToUnicodeText(this ReadOnlySpan<byte> bytes)
    {
        Span<Rune> runes = stackalloc Rune[bytes.Length];
        for (int i = 0; i < bytes.Length; i++)
        {
            runes[i] = UnicodeCharacterCodepoints[bytes[i]];
        }
        return runes.FastToString();
    }

    public static void EncodeTo(this ReadOnlySpan<char> unicodeText, Span<byte> bytes)
    {
        int i = 0;
        foreach (var rune in unicodeText.EnumerateRunes())
        {
            if (i == bytes.Length)
            {
                return;
            }
            if (UnicodeCharacterCodepointDictionary.TryGetValue(rune, out var idx))
            {
                bytes[i] = idx;
                i++;
            }
            else
            {
                throw new ArgumentException($"Invalid character: '{rune}'", nameof(unicodeText));
            }
        }
        // Fill the rest of the output with spaces.
        var spaceRune = new Rune(' ');
        if (!UnicodeCharacterCodepointDictionary.TryGetValue(spaceRune, out var spaceIdx))
        {
            throw new ArgumentException($"Invalid character: '{spaceRune}'", nameof(unicodeText));
        }
        while (i < bytes.Length)
        {
            bytes[i] = spaceIdx;
            i++;
        }
    }
}
