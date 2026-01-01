using DnmEplusPassword.Library.Data;
using static DnmEplusPassword.Library.Internal.CommonMethods;
using static DnmEplusPassword.Library.Internal.Constants;
using static DnmEplusPassword.Library.Internal.DecodeMethods;

namespace DnmEplusPassword.Library;

public static class Decoder
{
    public static PasswordInput Decode(PasswordOutput output, bool englishPasswords = false)
    {
        Span<byte> passwordBytes = output.RawBytes.ToArray();
        ChangeCharacterSet(passwordBytes, englishPasswords);

        Span<byte> data = stackalloc byte[24];
        Decode(passwordBytes, data);

        return new PasswordInput
        {
            CodeType = (CodeType)((data[0] >> 5) & 0b111),
            HitRate = (HitRate)((data[0] >> 2) & 0b111),
            Checksum = (byte)(((data[0] & 0b11) << 2) | ((data[1] >> 6) & 0b11)),
            ExtraData = (byte)(data[1] & 0b0011_1111),
            NpcCode = data[2],
            Name1 = data.Slice(3, 6).ToArray(),
            Name2 = data.Slice(9, 6).ToArray(),
            Name3 = data.Slice(15, 6).ToArray(),
            ItemId = (ushort)((data[21] << 8) | data[22]),
        };
    }

    private static void ChangeCharacterSet(Span<byte> input, bool englishPasswords)
    {
        var characterCodepoints = englishPasswords
            ? TranslatedCharacterCodepoints
            : CharacterCodepoints;

        ChangePasswordFontCode(input, characterCodepoints);
    }

    private static void Decode(ReadOnlySpan<byte> input, Span<byte> output)
    {
        ChangeEightBitsCode(output, input);
        TranspositionCipher(output, true, 1);
        DecodeBitShuffle(output, true);
        DecodeBitCode(output);
        DecodeRsaCipher(output);
        DecodeBitShuffle(output, false);
        TranspositionCipher(output, false, 0);
        DecodeSubstitutionCipher(output);
    }
}
