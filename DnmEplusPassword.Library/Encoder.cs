using System.Text;

namespace DnmEplusPassword.Library;

public static class Encoder
{
    public static void MakePasscode(in PasswordInput input, Span<byte> output)
    {
        int byte0 = ((int)input.CodeType << 5) & 0xE0;
        byte0 |= (byte)input.HitRate << 2;

        output[0] = (byte)byte0;
        output[1] = (byte)input.ExtraData;
        output[2] = input.NpcCode;

        int checksum = input.NpcCode + input.ItemId;
        Span<byte> nameBytes = stackalloc byte[6];

        AfNameToBytes(input.RecipientTown, nameBytes);
        nameBytes.CopyTo(output[3..]);
        checksum += nameBytes.Sum();

        AfNameToBytes(input.Recipient, nameBytes);
        nameBytes.CopyTo(output[9..]);
        checksum += nameBytes.Sum();

        AfNameToBytes(input.Sender, nameBytes);
        nameBytes.CopyTo(output[15..]);
        checksum += nameBytes.Sum();

        output[21] = (byte)(input.ItemId >> 8);
        output[22] = (byte)input.ItemId;

        output[0] |= (byte)((checksum >> 2) & 3);
        output[1] |= (byte)((checksum & 3) << 6);

#if DEBUG
        for (int i = 0; i < output.Length; i++)
        {
            Console.WriteLine($"Output[{i}]: {output[i]:X2}");
        }
#endif
    }

    private static int Sum(this Span<byte> bytes)
    {
        int sum = 0;
        foreach (var b in bytes)
        {
            sum += b;
        }
        return sum;
    }

    public static void AfNameToBytes(ReadOnlySpan<char> input, Span<byte> output)
    {
        int i = 0;
        foreach (var inputRune in input.EnumerateRunes())
        {
            if (i < output.Length)
            {
                var idx = Common.UnicodeCharacterCodepoints.IndexOf(inputRune);
                if (idx < 0)
                {
                    throw new ArgumentException($"Invalid character: '{inputRune}'", nameof(input));
                }
                output[i] = (byte)idx;
                i++;
            }
            else
            {
                break;
            }
        }
        if (i == output.Length)
        {
            return;
        }
        // Fill the rest of the output with spaces.
        var spaceRune = new Rune(' ');
        var spaceIdx = Common.UnicodeCharacterCodepoints.IndexOf(spaceRune);
        if (spaceIdx < 0)
        {
            throw new ArgumentException($"Invalid character: '{spaceRune}'", nameof(input));
        }
        while (i < output.Length)
        {
            output[i] = (byte)spaceIdx;
            i++;
        }
    }

    public static void SubstitutionCipher(Span<byte> data)
    {
        for (int i = 0; i < 24; i++)
        {
            data[i] = Common.ChangeCodeTable[data[i]];
        }
    }

    public static void BitShuffle(Span<byte> data, int key)
    {
        int charOffset = key == 0 ? 0xD : 9;
        int charCount = key == 0 ? 0x16 : 0x17;

        Span<byte> buffer = stackalloc byte[data.Length - 1];
        data[..charOffset].CopyTo(buffer);
        data[(charOffset + 1)..].CopyTo(buffer[charOffset..]);

        Span<byte> output = stackalloc byte[charCount];

        var indexTable = Common.SelectIndexTable[data[charOffset] & 3];

        for (int i = 0; i < charCount; i++)
        {
            var selectedByte = buffer[i];
            for (var x = 0; x < 8; x++)
            {
                var outputOffset = indexTable[x] + i;
                if (outputOffset >= charCount)
                {
                    outputOffset -= charCount;
                }

                output[outputOffset] |= (byte)(((selectedByte >> x) & 1) << x);
            }
        }

        for (int i = 0; i < charOffset; i++)
        {
            data[i] = output[i];
        }

        for (int i = charOffset; i < charCount; i++)
        {
            data[i + 1] = output[i]; // Data[i + 1] to skip the "Char" byte
        }
    }

    public static void ChangeRsaCipher(Span<byte> data)
    {
        Span<byte> buffer = stackalloc byte[data.Length];
        data.CopyTo(buffer);

        var rsa = new RsaKeyCode(data);

        byte cipherValue = 0;
        int primeProduct = rsa.Prime1 * rsa.Prime2;

        for (int i = 0; i < 8; i++)
        {
            int value = data[rsa.IndexTable[i]];
            int currentValue = value;

            for (int x = 0; x < rsa.Prime3 - 1; x++)
            {
                value = value * currentValue % primeProduct;
            }

            buffer[rsa.IndexTable[i]] = (byte)value;
            value = (value >> 8) & 1;
            cipherValue |= (byte)(value << i);
        }
        buffer[23] = cipherValue;

        for (int i = 0; i < 24; i++)
        {
            data[i] = buffer[i];
        }
    }

    public static void BitMixCode(Span<byte> data)
    {
        int switchType = data[1] & 0x0F;
        if (switchType > 0x0C)
        {
            Common.BitArrangeReverse(data);
            Common.BitReverse(data);
            Common.BitShift(data, switchType * 3);
        }
        else if (switchType > 0x08)
        {
            Common.BitArrangeReverse(data);
            Common.BitShift(data, switchType * -5);
        }
        else if (switchType > 0x04)
        {
            Common.BitShift(data, switchType * -5);
            Common.BitReverse(data);
        }
        else
        {
            Common.BitShift(data, switchType * 3);
            Common.BitArrangeReverse(data);
        }
    }

    public static void CangeSixBitsCode(ReadOnlySpan<byte> input, Span<byte> output)
    {
        int bit6Idx = 0;
        int bit8Idx = 0;
        int byte6Idx = 0;
        int byte8Idx = 0;

        int value = 0;
        int total = 0;

        while (true)
        {
            value |= ((input[byte8Idx] >> bit8Idx) & 1) << bit6Idx;
            bit8Idx++;
            bit6Idx++;

            if (bit6Idx == 6)
            {
                output[byte6Idx] = (byte)value;
                value = 0;
                bit6Idx = 0;
                byte6Idx++;
                total++;
                if (total == 32)
                {
                    return;
                }
            }

            if (bit8Idx == 8)
            {
                bit8Idx = 0;
                byte8Idx++;
            }
        }
    }

    public static void CangeCommonFontCode(Span<byte> password, bool englishPasswords)
    {
        if (englishPasswords)
        {
            for (int i = 0; i < 32; i++)
            {
                password[i] = Common.TranslatedCharacterCodepoints[password[i]];
            }
        }
        else
        {
            for (int i = 0; i < 32; i++)
            {
                password[i] = Common.CharacterCodepoints[password[i]];
            }
        }
    }

    public static (string, string) DebugEncode(in PasswordInput input)
    {
        Span<byte> passwordData = stackalloc byte[24];

        MakePasscode(input, passwordData);
        PrintByteBuffer("mMpswd_make_passcode", passwordData);
        SubstitutionCipher(passwordData);
        PrintByteBuffer("mMpswd_substitution_cipher", passwordData);
        Common.TranspositionCipher(passwordData, true, 0);
        PrintByteBuffer("mMpswd_transposition_cipher", passwordData);
        BitShuffle(passwordData, 0); // this doesn't change the last byte. Is that necessary? Doesn't seem to be.
        PrintByteBuffer("mMpswd_bit_shuffle", passwordData);
        ChangeRsaCipher(passwordData);
        PrintByteBuffer("mMpswd_chg_RSA_cipher", passwordData);
        BitMixCode(passwordData); // the problem appears to be in the bit mix function.
        PrintByteBuffer("mMpswd_bit_mix_code", passwordData);
        BitShuffle(passwordData, 1);
        PrintByteBuffer("mMpswd_bit_shuffle", passwordData);
        Common.TranspositionCipher(passwordData, false, 1);
        PrintByteBuffer("mMpswd_transposition_cipher", passwordData);

        Span<byte> password = stackalloc byte[32];
        CangeSixBitsCode(passwordData, password);

        PrintByteBuffer("mMpswd_chg_6bits_code", password);
        CangeCommonFontCode(password, false);
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

    public static (string, string) Encode(in PasswordInput input, bool englishPasswords)
    {
        Span<byte> passwordData = stackalloc byte[24];

        MakePasscode(input, passwordData);
        SubstitutionCipher(passwordData);
        Common.TranspositionCipher(passwordData, true, 0);
        BitShuffle(passwordData, 0);
        ChangeRsaCipher(passwordData);
        BitMixCode(passwordData);
        BitShuffle(passwordData, 1);
        Common.TranspositionCipher(passwordData, false, 1);

        Span<byte> password = stackalloc byte[32];
        CangeSixBitsCode(passwordData, password);
        CangeCommonFontCode(password, englishPasswords);

        var line1 = password[..16].ToPasswordLine();
        var line2 = password[16..].ToPasswordLine();

        return (line1, line2);
    }

    private static string ToPasswordLine(this Span<byte> bytes)
    {
        Span<Rune> runes = stackalloc Rune[bytes.Length];
        int length = 0;
        for (int i = 0; i < bytes.Length; i++)
        {
            var rune = Common.UnicodeCharacterCodepoints[bytes[i]];
            runes[i] = rune;
            length += rune.Utf16SequenceLength;
        }
        return string.Create(length, state: runes, static (output, state) =>
        {
            int charsWritten = 0;
            foreach (var rune in state)
            {
                charsWritten += rune.EncodeToUtf16(output[charsWritten..]);
            }
        });
    }
}
