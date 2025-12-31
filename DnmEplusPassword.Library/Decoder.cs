using System.Text;
using DnmEplusPassword.Library.Data;
using static DnmEplusPassword.Library.Internal.ByteCollectionExtensions;
using static DnmEplusPassword.Library.Internal.CommonMethods;
using static DnmEplusPassword.Library.Internal.DecodeMethods;
using static DnmEplusPassword.Library.Internal.Constants;

namespace DnmEplusPassword.Library;

public static class Decoder
{
    private const int pwLength = 32;

    public static PasswordInput Decode(string password, bool englishPasswords = false)
    {
        Span<Rune> runes = stackalloc Rune[pwLength];
        int runeCount = password.FillRunes(runes);
        if (runeCount != pwLength)
        {
            throw new ArgumentException($"Password must contain exactly {pwLength} characters", nameof(password));
        }

        Span<byte> passwordBytes = stackalloc byte[pwLength];
        for (int i = 0; i < pwLength; i++)
        {
            if (UnicodeCharacterCodepointDictionary.TryGetValue(runes[i], out var idx))
            {
                passwordBytes[i] = idx;
            }
            else
            {
                throw new Exception($"The password contains an invalid character: '{runes[i]}'");
            }
        }

        Span<byte> output = stackalloc byte[pwLength];
        Decode(passwordBytes, output, englishPasswords);

        ReadOnlySpan<byte> data = output;

        var codeType = (CodeType)((data[0] >> 5) & 7);
        // var checksum = ((data[0] << 2) & 0x0C) | ((data[1] >> 6) & 3);
        // var r28 = data[2];
        // var unknown = (ushort)((data[15] << 8) | data[16]);
        var presentItemId = (ushort)((data[21] << 8) | data[22]);

        var townName = data.Slice(3, 6).ToUnicodeText().TrimEnd();
        var playerName = data.Slice(9, 6).ToUnicodeText().TrimEnd();
        var senderString = data.Slice(15, 6).ToUnicodeText().TrimEnd();

        return codeType switch
        {
            CodeType.Monument => new PasswordInput
            {
                CodeType = codeType,
                RecipientTown = townName,
                Recipient = playerName,
                Sender = senderString,
                RowAcre = (byte)((data[1] >> 3) & 7),
                ColAcre = (byte)(data[1] & 7),
                ItemId = presentItemId,
            },
            _ => new PasswordInput
            {
                CodeType = codeType,
                RecipientTown = townName,
                Recipient = playerName,
                Sender = senderString,
                ItemId = presentItemId,
            },
        };

        // var codeTypeValue = 0;
        // switch (codeType)
        // {
        //     case CodeType.Famicom:
        //     case CodeType.NPC:
        //     case CodeType.Magazine:
        //         codeTypeValue = (data[0] >> 2) & 7;
        //         break;
        //     case CodeType.Card_E:
        //     case CodeType.Card_E_Mini:
        //     case CodeType.User:
        //     case CodeType.New_NPC:
        //         codeTypeValue = (data[0] >> 2) & 3;
        //         break;
        //     case CodeType.Monument:
        //         codeTypeValue = (data[0] >> 2) & 7;
        //         break;
        // }

    }

    private static void Decode(Span<byte> input, Span<byte> output, bool englishPasswords)
    {
        var characterCodepoints = englishPasswords
            ? TranslatedCharacterCodepoints
            : CharacterCodepoints;

        ChangePasswordFontCode(input, characterCodepoints);
        ChangeEightBitsCode(output, input);
        TranspositionCipher(output, true, 1);
        DecodeBitShuffle(output, true);
        DecodeBitCode(output);
        DecodeRsaCipher(output);
        DecodeBitShuffle(output, false);
        TranspositionCipher(output, false, 0);
        DecodeSubstitutionCipher(output);
    }

    /// <summary>
    /// Writes runes from the source string into the destination buffer.
    /// </summary>
    /// <returns>The total number of runes in the source string.</returns>
    private static int FillRunes(this string source, Span<Rune> dest)
    {
        int i = 0;
        foreach (var rune in source.EnumerateRunes())
        {
            if (i < dest.Length)
            {
                dest[i] = rune;
            }
            i++;
        }
        return i;
    }
}
