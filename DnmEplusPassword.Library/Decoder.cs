using DnmEplusPassword.Library.Data;
using static DnmEplusPassword.Library.Internal.ByteCollectionExtensions;
using static DnmEplusPassword.Library.Internal.CommonMethods;
using static DnmEplusPassword.Library.Internal.Constants;
using static DnmEplusPassword.Library.Internal.DecodeMethods;
using static DnmEplusPassword.Library.Internal.UnicodeNormalizer;

namespace DnmEplusPassword.Library;

public static class Decoder
{
    public static PasswordInput Decode(ReadOnlySpan<char> password, bool englishPasswords = false)
    {
        var normalizedPassword = password.DnmNormalize();
        Span<byte> passwordBytes = stackalloc byte[32];

        if (normalizedPassword.Length != passwordBytes.Length)
        {
            throw new ArgumentException($"Password must contain exactly {passwordBytes.Length} characters", nameof(password));
        }

        normalizedPassword.EncodeTo(passwordBytes);
        ChangeCharacterSet(passwordBytes, englishPasswords);

        Span<byte> data = stackalloc byte[24];
        Decode(passwordBytes, data);

        return new PasswordInput
        {
            CodeType = (CodeType)((data[0] >> 5) & 0b111),
            HitRate = (HitRate)((data[0] >> 2) & 0b111),
            CheckSum = (byte)(((data[0] & 0b11) << 2) | ((data[1] >> 6) & 0b11)),
            ExtraData = (byte)(data[1] & 0b0011_1111),
            NpcCode = data[2],
            RecipientTown = data.Slice(3, 6).DecodeToUnicodeText().TrimEnd(),
            Recipient = data.Slice(9, 6).DecodeToUnicodeText().TrimEnd(),
            Sender = data.Slice(15, 6).DecodeToUnicodeText().TrimEnd(),
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
