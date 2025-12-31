using DnmEplusPassword.Library.Data;
using static DnmEplusPassword.Library.Internal.ByteCollectionExtensions;
using static DnmEplusPassword.Library.Internal.CommonMethods;
using static DnmEplusPassword.Library.Internal.EncodeMethods;

namespace DnmEplusPassword.Library;

public static class Encoder
{
    public static (string, string) Encode(in PasswordInput input, bool englishPasswords)
    {
        Span<byte> passcode = stackalloc byte[24];

        MakePasscode(input, passcode);
        SubstitutionCipher(passcode);
        TranspositionCipher(passcode, true, 0);
        BitShuffle(passcode, 0);
        ChangeRsaCipher(passcode);
        BitMixCode(passcode);
        BitShuffle(passcode, 1);
        TranspositionCipher(passcode, false, 1);

        Span<byte> password = stackalloc byte[32];
        ChangeSixBitsCode(passcode, password);
        ChangeCommonFontCode(password, englishPasswords);

        var line1 = password[..16].DecodeToUnicodeText();
        var line2 = password[16..].DecodeToUnicodeText();

        return (line1, line2);
    }

    public static (string, string) DebugEncode(in PasswordInput input)
    {
        Span<byte> passwordData = stackalloc byte[24];

        MakePasscode(input, passwordData);
        PrintByteBuffer("mMpswd_make_passcode", passwordData);
        SubstitutionCipher(passwordData);
        PrintByteBuffer("mMpswd_substitution_cipher", passwordData);
        TranspositionCipher(passwordData, true, 0);
        PrintByteBuffer("mMpswd_transposition_cipher", passwordData);
        BitShuffle(passwordData, 0); // this doesn't change the last byte. Is that necessary? Doesn't seem to be.
        PrintByteBuffer("mMpswd_bit_shuffle", passwordData);
        ChangeRsaCipher(passwordData);
        PrintByteBuffer("mMpswd_chg_RSA_cipher", passwordData);
        BitMixCode(passwordData); // the problem appears to be in the bit mix function.
        PrintByteBuffer("mMpswd_bit_mix_code", passwordData);
        BitShuffle(passwordData, 1);
        PrintByteBuffer("mMpswd_bit_shuffle", passwordData);
        TranspositionCipher(passwordData, false, 1);
        PrintByteBuffer("mMpswd_transposition_cipher", passwordData);

        Span<byte> password = stackalloc byte[32];
        ChangeSixBitsCode(passwordData, password);

        PrintByteBuffer("mMpswd_chg_6bits_code", password);
        ChangeCommonFontCode(password, false);
        PrintByteBuffer("mMpswd_chg_common_font_code", password);

        var line1 = password[..16].DecodeToUnicodeText();
        var line2 = password[16..].DecodeToUnicodeText();

        return (line1, line2);
    }

    private static void PrintByteBuffer(string stage, ReadOnlySpan<byte> buffer)
    {
        Console.Write((stage + ":").PadRight(32));
        for (var i = 0; i < buffer.Length; i++)
        {
            if (i > 0 && i % 8 == 0)
            {
                Console.Write(("\n").PadRight(32) + " ");
            }
            Console.Write(buffer[i].ToString("X2"));
        }
        Console.Write("\n\n");
    }
}
