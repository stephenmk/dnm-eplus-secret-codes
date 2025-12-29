using System.Text;

namespace DnmEplusPassword.Library;

public static class Encoder
{
    public static byte[] mMpswd_make_passcode(
        CodeType codeType,
        int hitRateIndex,
        string recipientTown,
        string recipient,
        string sender,
        ushort itemId,
        int extraData)
    {
        var output = new byte[24];

        int realHitRateIndex;
        int npcCode = 0;

        switch (codeType)
        {
            case CodeType.Famicom:
            case CodeType.User:
            case CodeType.Card_E_Mini:
                realHitRateIndex = 4;
                extraData = 0;
                npcCode = 0xFF;
                break;
            case CodeType.NPC:
            case CodeType.New_NPC:
                extraData &= 3;
                realHitRateIndex = 4;
                break;
            case CodeType.Magazine:
                // Valid indices are 0 - 4. Hit rates are: { 80.0f, 60.0f, 30.0f, 0.0f, 100.0f }.
                // The hit is RNG based and the player "wins" if hit < hitRate.
                realHitRateIndex = hitRateIndex & 7;
                extraData = 0;
                npcCode = 0xFF;
                break;
            case CodeType.Monument:
                extraData &= 0xFF;
                realHitRateIndex = 4;
                npcCode = 0xFF;
                break;
            default:
                realHitRateIndex = 4;
                codeType = CodeType.User;
                break;
        }

        int byte0 = ((int)codeType << 5) & 0xE0;
        byte0 |= realHitRateIndex << 2;

        output[0] = (byte)byte0;
        output[1] = (byte)extraData;
        output[2] = (byte)npcCode;

        int checksum = npcCode + itemId;
        Span<byte> nameBytes = stackalloc byte[6];

        AfNameToBytes(recipientTown, ref nameBytes);
        nameBytes.CopyTo(output.AsSpan(3));
        checksum += nameBytes.Sum();

        AfNameToBytes(recipient, ref nameBytes);
        nameBytes.CopyTo(output.AsSpan(9));
        checksum += nameBytes.Sum();

        AfNameToBytes(sender, ref nameBytes);
        nameBytes.CopyTo(output.AsSpan(15));
        checksum += nameBytes.Sum();

        // Copy Item ID
        output[21] = (byte)(itemId >> 8);
        output[22] = (byte)itemId;

        output[0] |= (byte)((checksum >> 2) & 3);
        output[1] |= (byte)((checksum & 3) << 6);

#if DEBUG
        for (int i = 0; i < output.Length; i++)
        {
            Console.WriteLine($"Output[{i}]: {output[i]:X2}");
        }
#endif

        return output;
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

    public static void AfNameToBytes(ReadOnlySpan<char> input, ref Span<byte> output)
    {
        int i = 0;
        foreach (var inputRune in input.EnumerateRunes())
        {
            if (i < output.Length)
            {
                var idx = Common.AFe_CharList.IndexOf(inputRune);
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
        var spaceIdx = Common.AFe_CharList.IndexOf(spaceRune);
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

    public static void mMpswd_substitution_cipher(ref byte[] data)
    {
        for (int i = 0; i < 24; i++)
        {
            data[i] = Common.mMpswd_chg_code_table[data[i]];
        }
    }

    public static void mMpswd_bit_shuffle(ref byte[] data, int key)
    {
        int charOffset = key == 0 ? 0xD : 9;
        int charCount = key == 0 ? 0x16 : 0x17;

        var buffer = data.Take(charOffset)
            .Concat(data.Skip(charOffset + 1)
            .Take(23 - charOffset))
            .ToArray();

        var output = new byte[charCount];

        var indexTable = Common.mMpswd_select_idx_table[data[charOffset] & 3];

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

    public static void mMpswd_chg_RSA_cipher(ref byte[] data)
    {
        byte[] buffer = [.. data];

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

    public static void mMpswd_bit_mix_code(ref byte[] data)
    {
        int switchType = data[1] & 0x0F;
        if (switchType > 0x0C)
        {
            Common.mMpswd_bit_arrange_reverse(ref data);
            Common.mMpswd_bit_reverse(ref data);
            Common.mMpswd_bit_shift(ref data, switchType * 3);
        }
        else if (switchType > 0x08)
        {
            Common.mMpswd_bit_arrange_reverse(ref data);
            Common.mMpswd_bit_shift(ref data, switchType * -5);
        }
        else if (switchType > 0x04)
        {
            Common.mMpswd_bit_shift(ref data, switchType * -5);
            Common.mMpswd_bit_reverse(ref data);
        }
        else
        {
            Common.mMpswd_bit_shift(ref data, switchType * 3);
            Common.mMpswd_bit_arrange_reverse(ref data);
        }
    }

    public static byte[] mMpswd_chg_6bits_code(byte[] data)
    {
        byte[] password = new byte[32];

        int bit6Idx = 0;
        int bit8Idx = 0;
        int byte6Idx = 0;
        int byte8Idx = 0;

        int value = 0;
        int total = 0;

        while (true)
        {
            value |= ((data[byte8Idx] >> bit8Idx) & 1) << bit6Idx;
            bit8Idx++;
            bit6Idx++;

            if (bit6Idx == 6)
            {
                password[byte6Idx] = (byte)value;
                value = 0;
                bit6Idx = 0;
                byte6Idx++;
                total++;
                if (total == 32)
                {
                    return password;
                }
            }

            if (bit8Idx == 8)
            {
                bit8Idx = 0;
                byte8Idx++;
            }
        }
    }

    public static void mMpswd_chg_common_font_code(ref byte[] password, bool englishPasswords)
    {
        if (englishPasswords)
        {
            for (int i = 0; i < 32; i++)
            {
                password[i] = Common.usable_to_fontnum_new_translation[password[i]];
            }
        }
        else
        {
            for (int i = 0; i < 32; i++)
            {
                password[i] = Common.usable_to_fontnum_new[password[i]];
            }
        }
    }

    public static (string, string) DebugEncode(
        CodeType codeType,
        int hitRateIndex,
        string recipientTown,
        string recipient,
        string sender,
        ushort itemId,
        int extraData)
    {
        byte[] passwordData = mMpswd_make_passcode(codeType, hitRateIndex, recipientTown, recipient, sender, itemId, extraData);
        PrintByteBuffer("mMpswd_make_passcode", passwordData);
        mMpswd_substitution_cipher(ref passwordData);
        PrintByteBuffer("mMpswd_substitution_cipher", passwordData);
        Common.mMpswd_transposition_cipher(ref passwordData, true, 0);
        PrintByteBuffer("mMpswd_transposition_cipher", passwordData);
        mMpswd_bit_shuffle(ref passwordData, 0); // this doesn't change the last byte. Is that necessary? Doesn't seem to be.
        PrintByteBuffer("mMpswd_bit_shuffle", passwordData);
        mMpswd_chg_RSA_cipher(ref passwordData);
        PrintByteBuffer("mMpswd_chg_RSA_cipher", passwordData);
        mMpswd_bit_mix_code(ref passwordData); // the problem appears to be in the bit mix function.
        PrintByteBuffer("mMpswd_bit_mix_code", passwordData);
        mMpswd_bit_shuffle(ref passwordData, 1);
        PrintByteBuffer("mMpswd_bit_shuffle", passwordData);
        Common.mMpswd_transposition_cipher(ref passwordData, false, 1);
        PrintByteBuffer("mMpswd_transposition_cipher", passwordData);
        byte[] password = mMpswd_chg_6bits_code(passwordData);
        PrintByteBuffer("mMpswd_chg_6bits_code", password);
        mMpswd_chg_common_font_code(ref password, false);
        PrintByteBuffer("mMpswd_chg_common_font_code", password);

        var line1 = string.Join("", password.Take(16).Select(x => Common.AFe_CharList[x]));
        var line2 = string.Join("", password.Skip(16).Select(x => Common.AFe_CharList[x]));

        return (line1, line2);
    }

    private static void PrintByteBuffer(string stage, byte[] buffer)
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

    public static (string, string) Encode(
        CodeType codeType,
        int hitRateIndex,
        string recipientTown,
        string recipient,
        string sender,
        ushort itemId,
        int extraData,
        bool englishPasswords)
    {
        byte[] passwordData = mMpswd_make_passcode(codeType, hitRateIndex, recipientTown, recipient, sender, itemId, extraData);

        mMpswd_substitution_cipher(ref passwordData);
        Common.mMpswd_transposition_cipher(ref passwordData, true, 0);
        mMpswd_bit_shuffle(ref passwordData, 0);
        mMpswd_chg_RSA_cipher(ref passwordData);
        mMpswd_bit_mix_code(ref passwordData);
        mMpswd_bit_shuffle(ref passwordData, 1);
        Common.mMpswd_transposition_cipher(ref passwordData, false, 1);

        byte[] password = mMpswd_chg_6bits_code(passwordData);

        mMpswd_chg_common_font_code(ref password, englishPasswords);

        var line1 = string.Join("", password.Take(16).Select(x => Common.AFe_CharList[x]));
        var line2 = string.Join("", password.Skip(16).Select(x => Common.AFe_CharList[x]));

        return (line1, line2);
    }
}
