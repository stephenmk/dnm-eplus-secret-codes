using System.Text;
using DnmEplusPassword.Library.Data;
using DnmEplusPassword.Library.Internal;

namespace DnmEplusPassword.Library;

public static class Encoder
{
    public static (string, string) Encode(in PasswordInput input, bool englishPasswords)
    {
        Span<byte> passwordData = stackalloc byte[24];

        EncodeMethods.MakePasscode(input, passwordData);
        EncodeMethods.SubstitutionCipher(passwordData);
        Common.TranspositionCipher(passwordData, true, 0);
        EncodeMethods.BitShuffle(passwordData, 0);
        EncodeMethods.ChangeRsaCipher(passwordData);
        EncodeMethods.BitMixCode(passwordData);
        EncodeMethods.BitShuffle(passwordData, 1);
        Common.TranspositionCipher(passwordData, false, 1);

        Span<byte> password = stackalloc byte[32];
        EncodeMethods.ChangeSixBitsCode(passwordData, password);
        EncodeMethods.ChangeCommonFontCode(password, englishPasswords);

        var line1 = password[..16].ToPasswordLine();
        var line2 = password[16..].ToPasswordLine();

        return (line1, line2);
    }

    public static (string, string) DebugEncode(in PasswordInput input)
    {
        Span<byte> passwordData = stackalloc byte[24];

        EncodeMethods.MakePasscode(input, passwordData);
        PrintByteBuffer("mMpswd_make_passcode", passwordData);
        EncodeMethods.SubstitutionCipher(passwordData);
        PrintByteBuffer("mMpswd_substitution_cipher", passwordData);
        Common.TranspositionCipher(passwordData, true, 0);
        PrintByteBuffer("mMpswd_transposition_cipher", passwordData);
        EncodeMethods.BitShuffle(passwordData, 0); // this doesn't change the last byte. Is that necessary? Doesn't seem to be.
        PrintByteBuffer("mMpswd_bit_shuffle", passwordData);
        EncodeMethods.ChangeRsaCipher(passwordData);
        PrintByteBuffer("mMpswd_chg_RSA_cipher", passwordData);
        EncodeMethods.BitMixCode(passwordData); // the problem appears to be in the bit mix function.
        PrintByteBuffer("mMpswd_bit_mix_code", passwordData);
        EncodeMethods.BitShuffle(passwordData, 1);
        PrintByteBuffer("mMpswd_bit_shuffle", passwordData);
        Common.TranspositionCipher(passwordData, false, 1);
        PrintByteBuffer("mMpswd_transposition_cipher", passwordData);

        Span<byte> password = stackalloc byte[32];
        EncodeMethods.ChangeSixBitsCode(passwordData, password);

        PrintByteBuffer("mMpswd_chg_6bits_code", password);
        EncodeMethods.ChangeCommonFontCode(password, false);
        PrintByteBuffer("mMpswd_chg_common_font_code", password);

        var line1 = password[..16].ToPasswordLine();
        var line2 = password[16..].ToPasswordLine();

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
