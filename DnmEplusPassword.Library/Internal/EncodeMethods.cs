using DnmEplusPassword.Library.Data;
using static DnmEplusPassword.Library.Internal.CommonMethods;
using static DnmEplusPassword.Library.Internal.Constants;

namespace DnmEplusPassword.Library.Internal;

internal static class EncodeMethods
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

        input.RecipientTown.EncodeTo(nameBytes);
        nameBytes.CopyTo(output[3..]);
        checksum += nameBytes.Sum();

        input.Recipient.EncodeTo(nameBytes);
        nameBytes.CopyTo(output[9..]);
        checksum += nameBytes.Sum();

        input.Sender.EncodeTo(nameBytes);
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

    public static void SubstitutionCipher(Span<byte> data)
    {
        for (int i = 0; i < 24; i++)
        {
            data[i] = ChangeCodeTable[data[i]];
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

        var indexTable = SelectIndexTable[data[charOffset] & 3];

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
            BitArrangeReverse(data);
            BitReverse(data);
            BitShift(data, switchType * 3);
        }
        else if (switchType > 0x08)
        {
            BitArrangeReverse(data);
            BitShift(data, switchType * -5);
        }
        else if (switchType > 0x04)
        {
            BitShift(data, switchType * -5);
            BitReverse(data);
        }
        else
        {
            BitShift(data, switchType * 3);
            BitArrangeReverse(data);
        }
    }

    public static void ChangeSixBitsCode(ReadOnlySpan<byte> input, Span<byte> output)
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

    public static void ChangeCommonFontCode(Span<byte> password, bool englishPasswords)
    {
        if (englishPasswords)
        {
            for (int i = 0; i < 32; i++)
            {
                password[i] = TranslatedCharacterCodepoints[password[i]];
            }
        }
        else
        {
            for (int i = 0; i < 32; i++)
            {
                password[i] = CharacterCodepoints[password[i]];
            }
        }
    }
}
