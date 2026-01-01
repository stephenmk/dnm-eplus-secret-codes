using DnmEplusPassword.Library.Data;
using static DnmEplusPassword.Library.Internal.ByteCollectionExtensions;
using static DnmEplusPassword.Library.Internal.CommonMethods;
using static DnmEplusPassword.Library.Internal.Constants;
using static DnmEplusPassword.Library.Internal.DecodeMethods;
using static DnmEplusPassword.Library.Internal.UnicodeNormalizer;

namespace DnmEplusPassword.Library;

public static class Decoder
{
    private const int pwLength = 32;

    public static PasswordInput Decode(ReadOnlySpan<char> password, bool englishPasswords = false)
    {
        var normalizedPassword = password.Normalize();

        if (normalizedPassword.Length != pwLength)
        {
            throw new ArgumentException($"Password must contain exactly {pwLength} characters", nameof(password));
        }

        Span<byte> passwordBytes = stackalloc byte[pwLength];
        normalizedPassword.EncodeTo(passwordBytes);
        ChangeCharacterSet(passwordBytes, englishPasswords);

        Span<byte> data = stackalloc byte[24];
        Decode(passwordBytes, data);

        var codeType = (CodeType)((data[0] >> 5) & 0b111);
        var hitRate = (HitRate)((data[0] >> 2) & 0b111);
        // var checksum = (byte)(((data[0] & 0b11) << 2) | ((data[1] >> 6) & 0b11));
        var extraData = (byte)(data[1] & 0b0011_1111);
        // var npcCode = data[2];

        var townName = data.Slice(3, 6).DecodeToUnicodeText().TrimEnd();
        var playerName = data.Slice(9, 6).DecodeToUnicodeText().TrimEnd();
        var senderString = data.Slice(15, 6).DecodeToUnicodeText().TrimEnd();
        var itemId = (ushort)((data[21] << 8) | data[22]);

        return new PasswordInput
        {
            CodeType = codeType,
            HitRate = hitRate,
            ExtraData = extraData,
            RecipientTown = townName,
            Recipient = playerName,
            Sender = senderString,
            ItemId = itemId,
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
