using System.Text;
using static DnmEplusPassword.Library.Internal.Constants;

namespace DnmEplusPassword.Library.Internal;

internal static class CommonMethods
{
    public static byte ChangePasswordFontCodeSub(byte characterCodepoint, IReadOnlyList<byte> characterCodepoints)
    {
        for (byte i = 0; i < 0x40; i++)
        {
            if (characterCodepoints[i] == characterCodepoint)
            {
                return i;
            }
        }
        return 0xFF;
    }

    public static void ChangePasswordFontCode(Span<byte> password, IReadOnlyList<byte> characterCodepoints)
    {
        for (int i = 0; i < 32; i++)
        {
            password[i] = ChangePasswordFontCodeSub(password[i], characterCodepoints);
        }
    }

    public static void TranspositionCipher(Span<byte> data, bool negate, int keyIndex)
    {
        var multiplier = negate ? -1 : 1;
        var key = data[KeyIndices[keyIndex]];
        var transpositionTable = TranspositionCipherCharTable[keyIndex];
        var transpositionCipher = transpositionTable[key & 0x0F];

        int cipherIndex = 0;

        for (int i = 0; i < 24; i++)
        {
            if (i != KeyIndices[keyIndex])
            {
                int valueModifier = (transpositionCipher[cipherIndex++] * multiplier) & 0xFF;
                data[i] = (byte)(data[i] + valueModifier);
                if (cipherIndex >= transpositionCipher.Length)
                {
                    cipherIndex = 0;
                }
            }
        }
    }

    public static void BitReverse(Span<byte> data)
    {
        for (int i = 0; i < 24; i++)
        {
            if (i != 1)
            {
                data[i] ^= 0xFF;
            }
        }
    }

    public static void BitArrangeReverse(Span<byte> data)
    {
        Span<byte> buffer = stackalloc byte[23];
        Span<byte> outputBuffer = stackalloc byte[23];

        for (int i = 0, idx = 0; i < 24; i++)
        {
            if (i != 1)
            {
                buffer[idx++] = data[i];
            }
        }

        for (int i = 0; i < 23; i++) // pretty sure this should be < 23
        {
            byte value = buffer[22 - i]; // this should be 22
            byte changedValue = 0;
            for (var x = 0; x < 8; x++)
            {
                changedValue |= (byte)(((value >> x) & 1) << (7 - x));
            }

            outputBuffer[i] = changedValue;
        }

        for (int i = 0, idx = 0; i < 23; i++)
        {
            if (i == 1)
            {
                idx++;
            }
            data[idx++] = outputBuffer[i];
        }
    }

    public static void BitShift(Span<byte> data, int shift)
    {
        Span<byte> buffer = stackalloc byte[23];

        for (int i = 0, idx = 0; i < 24; i++)
        {
            if (i != 1)
            {
                buffer[idx++] = data[i];
            }
        }

        byte[] outputBuffer = new byte[23];

        if (shift > 0)
        {
            int destinationPosition = shift / 8;
            int destinationOffset = shift % 8;

            for (int i = 0; i < 23; i++)
            {
                outputBuffer[(i + destinationPosition) % 23] = (byte)((buffer[i] << destinationOffset)
                    | (buffer[(i + 22) % 23] >> (8 - destinationOffset)));
            }

            // Copy to original buffer
            for (int i = 0, idx = 0; i < 23; i++)
            {
                if (i == 1) // Skip copying the second byte
                {
                    idx++;
                }
                data[idx++] = outputBuffer[i];
            }
        }
        else if (shift < 0)
        {
            for (int i = 0; i < 23; i++)
            {
                outputBuffer[i] = buffer[22 - i];
            }
            shift = -shift;

            int destinationPosition = shift / 8;
            int destinationOffset = shift % 8;

            for (int i = 0; i < 23; i++)
            {
                buffer[(i + destinationPosition) % 23] = outputBuffer[i];
            }

            for (int i = 0; i < 23; i++)
            {
                outputBuffer[i] = (byte)((buffer[i] >> destinationOffset) | ((buffer[(i + 22) % 23]) << (8 - destinationOffset)));
            }

            for (int i = 0, idx = 0; i < 23; i++)
            {
                if (i == 1)
                {
                    idx++;
                }
                data[idx++] = outputBuffer[22 - i];
            }
        }
    }

    public static bool NewPasswordZuruCheck(
        int checksum,
        int CodeType,
        string recipient,
        string townName,
        string sender,
        ushort itemId,
        int npcCode,
        int unknown)
    {
        if (CodeType == 2 || CodeType >= 8)
        {
            return true;
        }

        var calculatedChecksum = 0;
        calculatedChecksum += GetStringByteValue(recipient);
        calculatedChecksum += GetStringByteValue(townName);
        calculatedChecksum += GetStringByteValue(sender);
        calculatedChecksum += itemId;

        bool valid = (calculatedChecksum & 0xF) == checksum
            && CheckDefaultHitRate(CodeType, npcCode)
            && CheckDefaultNpcCode(CodeType, npcCode, unknown);

        return !valid;
    }

    private static bool CheckDefaultHitRate(int codeType, int codeCheck)
    {
        bool hitRate = false;
        if (codeType == 3 && codeCheck < 5)
        {
            hitRate = true;
        }
        else if (codeCheck == 4)
        {
            hitRate = true;
        }
        return hitRate;
    }

    private static bool CheckDefaultNpcCode(int codeType, int npcCode, int r3_9)
    {
        bool valid = false;
        if (codeType >= 5)
        {
            if (codeType == 7)
            {
                if (npcCode == 0xFF)
                {
                    valid = true;
                }
            }
            else
            {
                valid = true;
            }
        }
        else
        {
            if (codeType == 1)
            {
                valid = true;
            }
            else if (codeType >= 0)
            {
                if (r3_9 == 0 && npcCode == 0xFF)
                {
                    valid = true;
                }
            }
        }

        return valid;
    }

    // Custom Functions \\
    public static int GetPasswordChecksum(ReadOnlySpan<byte> passwordData)
    {
        int checksum = 0;

        for (int i = 0x03; i < 0x15; i++)
        {
            checksum += passwordData[i];
        }

        checksum += (passwordData[0x15] << 8) | passwordData[16];
        checksum += passwordData[2];

        return (((checksum >> 2) & 3) << 2) | (((checksum << 6) & 0xC0) >> 6);
    }

    public static bool VerifyChecksum(ReadOnlySpan<byte> passwordData)
    {
        int calculatedChecksum = GetPasswordChecksum(passwordData);
        int storedChecksum = ((passwordData[0] & 3) << 2) | ((passwordData[1] & 0xC0) >> 6);

        Console.WriteLine($"Calculated Checksum: 0x{calculatedChecksum:X}\nStored Checksum: 0x{storedChecksum:X}");

        return calculatedChecksum == storedChecksum;
    }

    public static byte[] StringToAFByteArray(string input)
    {
        var inputRunes = input.EnumerateRunes().ToArray();
        var output = new byte[inputRunes.Length];

        for (int i = 0; i < input.Length; i++)
        {
            var rune = inputRunes[i];
            if (UnicodeCharacterCodepointDictionary.TryGetValue(rune, out var idx))
            {
                output[i] = idx;
            }
            else
            {
                throw new ArgumentException($"Invalid character: {rune}", nameof(input));
            }
        }

        return output;
    }

    private static int GetStringByteValue(string input)
        => StringToAFByteArray(input).Sum(x => x);

    public static string ToPasswordLine(this Span<byte> bytes)
        => ToPasswordLine((ReadOnlySpan<byte>)bytes);

    public static string ToPasswordLine(this ReadOnlySpan<byte> bytes)
    {
        Span<Rune> runes = stackalloc Rune[bytes.Length];
        for (int i = 0; i < bytes.Length; i++)
        {
            runes[i] = UnicodeCharacterCodepoints[bytes[i]];
        }
        return runes.FastToString();
    }
}
