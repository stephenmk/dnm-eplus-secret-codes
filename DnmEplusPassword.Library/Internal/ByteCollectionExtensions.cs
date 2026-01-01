using static DnmEplusPassword.Library.Internal.Constants;

namespace DnmEplusPassword.Library.Internal;

internal static class ByteCollectionExtensions
{
    public static int Sum(this ReadOnlySpan<byte> bytes)
    {
        int sum = 0;
        foreach (var b in bytes)
        {
            sum += b;
        }
        return sum;
    }

    public static string DecodeToUnicodeText(this ReadOnlySpan<byte> bytes)
    {
        Span<char> unicodeChars = stackalloc char[bytes.Length];
        for (int i = 0; i < bytes.Length; i++)
        {
            unicodeChars[i] = UnicodeCharacterCodepoints[bytes[i]];
        }
        return new string(unicodeChars);
    }

    public static Span<byte> EncodeToNameBytes(this ReadOnlySpan<char> unicodeText, int size)
    {
        Span<byte> nameBytes = new byte[size];
        var normalizedText = unicodeText.DnmNormalize();
        if (normalizedText.Length > size)
        {
            throw new ArgumentException($"Normalized text must not contain more than {size} characters", nameof(unicodeText));
        }
        normalizedText.EncodeTo(nameBytes);
        return nameBytes;
    }

    private static void EncodeTo(this ReadOnlySpan<char> unicodeText, Span<byte> bytes)
    {
        int i = 0;
        foreach (var unicodeChar in unicodeText)
        {
            if (i == bytes.Length)
            {
                throw new ArgumentException($"Length of input text '{unicodeText}' exceeds maximum size = {bytes.Length}", nameof(unicodeText));
            }
            if (UnicodeCharacterCodepointDictionary.TryGetValue(unicodeChar, out var @byte))
            {
                bytes[i++] = @byte;
            }
            else
            {
                throw new ArgumentException($"Invalid character: '{unicodeChar}'", nameof(unicodeText));
            }
        }
        if (i == bytes.Length)
        {
            return;
        }
        // Fill the rest of the output with spaces.
        if (!UnicodeCharacterCodepointDictionary.TryGetValue('　', out var spaceByte))
        {
            throw new ArgumentException($"Invalid character: '　'", nameof(unicodeText));
        }
        while (i < bytes.Length)
        {
            bytes[i++] = spaceByte;
        }
    }
}
