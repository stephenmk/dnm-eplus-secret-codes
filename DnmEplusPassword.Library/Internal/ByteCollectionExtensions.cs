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
        Span<char> runes = stackalloc char[bytes.Length];
        for (int i = 0; i < bytes.Length; i++)
        {
            runes[i] = UnicodeCharacterCodepoints[bytes[i]];
        }
        return new string(runes);
    }

    public static void EncodeTo(this ReadOnlySpan<char> unicodeText, Span<byte> bytes)
    {
        int i = 0;
        foreach (var unicodeChar in unicodeText)
        {
            if (i == bytes.Length)
            {
                return;
            }
            if (UnicodeCharacterCodepointDictionary.TryGetValue(unicodeChar, out var @byte))
            {
                bytes[i] = @byte;
                i++;
            }
            else
            {
                throw new ArgumentException($"Invalid character: '{unicodeChar}'", nameof(unicodeText));
            }
        }
        // Fill the rest of the output with spaces.
        if (!UnicodeCharacterCodepointDictionary.TryGetValue('　', out var spaceByte))
        {
            throw new ArgumentException($"Invalid character: '　'", nameof(unicodeText));
        }
        while (i < bytes.Length)
        {
            bytes[i] = spaceByte;
            i++;
        }
    }
}
