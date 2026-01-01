using DnmEplusPassword.Library.Data;
using static DnmEplusPassword.Library.Cryptography.CommonMethods;
using static DnmEplusPassword.Library.Cryptography.DecodeMethods;

namespace DnmEplusPassword.Library;

public static class Decoder
{
    public static PasswordInput Decode(PasswordOutput output, bool englishPasswords = false)
    {
        var passwordBytes = output.RawBytes.ToArray();
        ChangeCharacterSet(passwordBytes, englishPasswords);

        Span<byte> data = new byte[24];

        ChangeEightBitsCode(data, passwordBytes);
        TranspositionCipher(data, true, 1);
        DecodeBitShuffle(data, true);
        DecodeBitCode(data);
        DecodeRsaCipher(data);
        DecodeBitShuffle(data, false);
        TranspositionCipher(data, false, 0);
        DecodeSubstitutionCipher(data);

        return new PasswordInput
        {
            CodeType = (CodeType)((data[0] >> 5) & 0b111),
            HitRate = (HitRate)((data[0] >> 2) & 0b111),
            Checksum = (byte)(((data[0] & 0b11) << 2) | ((data[1] >> 6) & 0b11)),
            ExtraData = (byte)(data[1] & 0b0011_1111),
            NpcCode = data[2],
            Name1 = data.Slice(3, 6),
            Name2 = data.Slice(9, 6),
            Name3 = data.Slice(15, 6),
            ItemId = (ushort)((data[21] << 8) | data[22]),
        };
    }
}
