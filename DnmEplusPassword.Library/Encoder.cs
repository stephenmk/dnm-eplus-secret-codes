using DnmEplusPassword.Library.Data;
using static DnmEplusPassword.Library.Cryptography.CommonMethods;
using static DnmEplusPassword.Library.Cryptography.EncodeMethods;

namespace DnmEplusPassword.Library;

public static class Encoder
{
    public static PasswordOutput Encode(in PasswordInput input, bool englishPasswords)
    {
        var plaintext = input.ToPlaintext();

        SubstitutionCipher(plaintext);
        TranspositionCipher(plaintext, true, 0);
        BitShuffle(plaintext, 0);
        ChangeRsaCipher(plaintext);
        BitMixCode(plaintext);
        BitShuffle(plaintext, 1);
        TranspositionCipher(plaintext, false, 1);

        Span<byte> ciphertext = new byte[32];

        ChangeSixBitsCode(plaintext, ciphertext);
        ChangeCommonFontCode(ciphertext, englishPasswords);

        return new PasswordOutput(ciphertext);
    }
}
