using DnmEplusPassword.Library.Data;
using static DnmEplusPassword.Library.Cryptography.CommonMethods;
using static DnmEplusPassword.Library.Cryptography.DecodeMethods;

namespace DnmEplusPassword.Library;

public static class Decoder
{
    public static PasswordInput Decode(PasswordOutput output, bool englishPasswords = false)
    {
        var ciphertext = output.Ciphertext.ToArray();
        ChangeCharacterSet(ciphertext, englishPasswords);

        Span<byte> plaintext = new byte[24];

        ChangeEightBitsCode(plaintext, ciphertext);
        TranspositionCipher(plaintext, true, 1);
        DecodeBitShuffle(plaintext, true);
        DecodeBitCode(plaintext);
        DecodeRsaCipher(plaintext);
        DecodeBitShuffle(plaintext, false);
        TranspositionCipher(plaintext, false, 0);
        DecodeSubstitutionCipher(plaintext);

        return new PasswordInput
        {
            CodeType = (CodeType)((plaintext[0] >> 5) & 0b111),
            HitRate = (HitRate)((plaintext[0] >> 2) & 0b111),
            Checksum = (byte)(((plaintext[0] & 0b11) << 2) | ((plaintext[1] >> 6) & 0b11)),
            ExtraData = (byte)(plaintext[1] & 0b0011_1111),
            NpcCode = plaintext[2],
            Name1 = plaintext.Slice(3, 6),
            Name2 = plaintext.Slice(9, 6),
            Name3 = plaintext.Slice(15, 6),
            ItemId = (ushort)((plaintext[21] << 8) | plaintext[22]),
        };
    }
}
