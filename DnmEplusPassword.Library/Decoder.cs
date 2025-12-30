namespace DnmEplusPassword.Library;

public class Decoder
{
    public static void mMpswd_decode_bit_shuffle(Span<byte> data, bool keyIdx)
    {
        int count = keyIdx ? 0x17 : 0x16; // Count
        int bitIdx = keyIdx ? 0x09 : 0x0D; // Bit index

        byte tableIndex = data[bitIdx];
        Span<byte> shuffledData = stackalloc byte[23]; // Exclude the r31 byte

        for (int i = 0, idx = 0; i < 23; i++)
        {
            if (i == bitIdx)
            {
                idx++; // Skip r31 byte
            }
            shuffledData[i] = data[idx++];
        }

        Span<byte> zeroedData = stackalloc byte[23];
        var shuffleTable = Common.mMpswd_select_idx_table[data[bitIdx] & 3];
        int offsetIdx = 0;
        int zeroedDataIdx = 0;

        while (offsetIdx < count)
        {
            int outputIdx = 0;
            int bit = 0;

            for (int x = 0; x < 8; x++)
            {
                int outputOffset = shuffleTable[outputIdx++] + offsetIdx;
                if (outputOffset >= count)
                {
                    outputOffset -= count;
                }

                zeroedData[zeroedDataIdx] |= (byte)(((shuffledData[outputOffset] >> bit) & 1) << bit);
                bit++;
            }

            offsetIdx++;
            zeroedDataIdx++;
        }

        zeroedData.Slice(0, bitIdx).CopyTo(data);
        data[bitIdx] = tableIndex;
        zeroedData.Slice(bitIdx + 1, zeroedDataIdx - bitIdx).CopyTo(data.Slice(bitIdx + 1));
    }

    public static void mMpswd_decode_bit_code(Span<byte> data)
    {
        int method = data[1] & 0x0F;

        if (method > 12)
        {
            Common.mMpswd_bit_shift(data, -method * 3);
            Common.mMpswd_bit_reverse(data);
            Common.mMpswd_bit_arrange_reverse(data);
        }
        else if (method > 8)
        {
            Common.mMpswd_bit_shift(data, method * 5);
            Common.mMpswd_bit_arrange_reverse(data);
        }
        else if (method > 4)
        {
            Common.mMpswd_bit_reverse(data);
            Common.mMpswd_bit_shift(data, method * 5);
        }
        else
        {
            Common.mMpswd_bit_arrange_reverse(data);
            Common.mMpswd_bit_shift(data, -method * 3);
        }
    }

    public static void mMpswd_decode_RSA_cipher(Span<byte> data)
    {
        Span<byte> outputBuffer = stackalloc byte[data.Length];
        data.CopyTo(outputBuffer);

        var rsa = new RsaKeyCode(data);

        int primeProduct = rsa.Prime1 * rsa.Prime2;
        int lessProduct = (rsa.Prime1 - 1) * (rsa.Prime2 - 1);

        int modCount = 0;
        int loopEndValue;
        int modValue;

        do
        {
            modCount++;
            loopEndValue = (modCount * lessProduct + 1) % rsa.Prime3;
            modValue = (modCount * lessProduct + 1) / rsa.Prime3;
        } while (loopEndValue != 0);

        for (int i = 0; i < 8; i++)
        {
            int value = data[rsa.IndexTable[i]];
            value |= ((data[23] >> i) << 8) & 0x100;
            int currentValue = value;

            for (int x = 0; x < modValue - 1; x++)
            {
                value = value * currentValue % primeProduct;
            }

            outputBuffer[rsa.IndexTable[i]] = (byte)value;
        }

        for (int i = 0; i < 24; i++)
        {
            data[i] = outputBuffer[i];
        }
    }

    public static void mMpswd_chg_8bits_code(Span<byte> storedLocation, ReadOnlySpan<byte> password)
    {
        int passwordIndex = 0;
        int storedLocationIndex = 0;

        int storedValue = 0;
        int count = 0;
        int shiftRightValue = 0;
        int shiftLeftValue = 0;

        while (true)
        {
            storedValue |= (((password[passwordIndex] >> shiftRightValue) & 1) << shiftLeftValue) & 0xFF;
            shiftRightValue++;
            shiftLeftValue++;

            if (shiftLeftValue > 7)
            {
                count++;
                storedLocation[storedLocationIndex++] = (byte)storedValue;
                shiftLeftValue = 0;
                if (count >= 24)
                {
                    return;
                }
                storedValue = 0;
            }
            if (shiftRightValue > 5)
            {
                shiftRightValue = 0;
                passwordIndex++;
            }
        }
    }

    public static void mMpswd_decode_substitution_cipher(Span<byte> data)
    {
        for (int i = 0; i < 24; i++)
        {
            for (int x = 0; x < 256; x++)
            {
                if (data[i] == Common.mMpswd_chg_code_table[x])
                {
                    data[i] = (byte)x;
                    break;
                }
            }
        }
    }

    // Main Code \\
    public static void Decode(Span<byte> input, Span<byte> output, bool englishPasswords = false)
    {
        if (input.Length != 32)
        {
            throw new ArgumentException("Input must be 32 bytes long", nameof(input));
        }

        var passwordAlphabet = englishPasswords
            ? Common.usable_to_fontnum_new_translation
            : Common.usable_to_fontnum_new;

        Common.mMpswd_chg_password_font_code(input, passwordAlphabet);
        mMpswd_chg_8bits_code(output, input);
        Common.mMpswd_transposition_cipher(output, true, 1);
        mMpswd_decode_bit_shuffle(output, true);
        mMpswd_decode_bit_code(output);
        mMpswd_decode_RSA_cipher(output);
        mMpswd_decode_bit_shuffle(output, false);
        Common.mMpswd_transposition_cipher(output, false, 0);
        mMpswd_decode_substitution_cipher(output);
    }

    public static void Decode(string password, Span<byte> output, bool englishPasswords = false)
    {
        var passwordRunes = password.EnumerateRunes().ToArray();

        if (passwordRunes.Length != 32)
        {
            return;
        }

        Span<byte> passwordBytes = stackalloc byte[32];

        for (int i = 0; i < 32; i++)
        {
            var rune = passwordRunes[i];
            int idx = Common.AFe_CharList.IndexOf(rune);
            if (idx < 0)
            {
                throw new Exception($"The password contains an invalid character: '{rune}'");
            }
            passwordBytes[i] = (byte)idx;
        }

        Decode(passwordBytes, output, englishPasswords);
    }
}
