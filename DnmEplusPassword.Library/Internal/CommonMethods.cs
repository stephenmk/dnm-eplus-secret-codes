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

    // Custom Functions \\
    public static string ToUnicodeText(this Span<byte> bytes)
        => ToUnicodeText((ReadOnlySpan<byte>)bytes);

    public static string ToUnicodeText(this ReadOnlySpan<byte> bytes)
    {
        Span<Rune> runes = stackalloc Rune[bytes.Length];
        for (int i = 0; i < bytes.Length; i++)
        {
            runes[i] = UnicodeCharacterCodepoints[bytes[i]];
        }
        return runes.FastToString();
    }
}
