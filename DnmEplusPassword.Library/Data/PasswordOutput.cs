namespace DnmEplusPassword.Library.Data;

public sealed class PasswordOutput
{
    public string Line1 { get; }
    public string Line2 { get; }
    public IReadOnlyList<byte> Ciphertext { get; }

    public PasswordOutput(string line1, string line2)
    {
        Line1 = line1;
        Line2 = line2;

        Span<byte> bytes = stackalloc byte[32];
        line1.EncodeToDnmText(16).CopyTo(bytes);
        line2.EncodeToDnmText(16).CopyTo(bytes[16..]);
        Ciphertext = bytes.ToArray();
    }

    public PasswordOutput(byte[] ciphertext)
    {
        Line1 = ciphertext[..16].DecodeToUnicodeText();
        Line2 = ciphertext[16..].DecodeToUnicodeText();
        Ciphertext = ciphertext;
    }
}
