namespace DnmEplusPassword.Library.Data;

public sealed class PasswordOutput
{
    public string Line1 { get; }
    public string Line2 { get; }
    public IReadOnlyList<byte> Ciphertext { get; }

    public PasswordOutput(ReadOnlySpan<char> line1, ReadOnlySpan<char> line2)
    {
        Line1 = line1.ToString();
        Line2 = line2.ToString();

        Span<byte> bytes = stackalloc byte[32];
        line1.EncodeToNameBytes(16).CopyTo(bytes);
        line2.EncodeToNameBytes(16).CopyTo(bytes[16..]);
        Ciphertext = bytes.ToArray();
    }

    public PasswordOutput(ReadOnlySpan<byte> ciphertext)
    {
        Line1 = ciphertext[..16].DecodeToUnicodeText();
        Line2 = ciphertext[16..].DecodeToUnicodeText();
        Ciphertext = ciphertext.ToArray();
    }
}
