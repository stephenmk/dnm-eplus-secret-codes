using DnmEplusPassword.Library.Data;
using static DnmEplusPassword.Library.Cryptography.CommonMethods;
using static DnmEplusPassword.Library.Cryptography.EncodeMethods;

namespace DnmEplusPassword.Library;

public static class Encoder
{
    public static PasswordOutput Encode(in PasswordInput input, bool englishPasswords)
    {
        var passcode = input.ToBytes();

        SubstitutionCipher(passcode);
        TranspositionCipher(passcode, true, 0);
        BitShuffle(passcode, 0);
        ChangeRsaCipher(passcode);
        BitMixCode(passcode);
        BitShuffle(passcode, 1);
        TranspositionCipher(passcode, false, 1);

        Span<byte> password = new byte[32];

        ChangeSixBitsCode(passcode, password);
        ChangeCommonFontCode(password, englishPasswords);

        return new PasswordOutput(password);
    }
}
