namespace DnmEplusPassword.Library;

public class Decoder
{
    public static void mMpswd_decode_bit_shuffle(ref byte[] data, bool keyIdx)
    {
        int count = keyIdx ? 0x17 : 0x16; // Count
        int bitIdx = keyIdx ? 0x09 : 0x0D; // Bit index

        byte tableIndex = data[bitIdx];
        byte[] shuffledData = new byte[23]; // Exclude the r31 byte

        for (int i = 0, idx = 0; i < 23; i++)
        {
            if (i == bitIdx)
            {
                idx++; // Skip r31 byte
            }
            shuffledData[i] = data[idx++];
        }

        var zeroedData = new byte[23];
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

        zeroedData.Take(bitIdx).ToArray().CopyTo(data, 0);
        data[bitIdx] = tableIndex;
        zeroedData.Skip(bitIdx).Take(zeroedDataIdx - bitIdx).ToArray().CopyTo(data, bitIdx + 1);
    }

    public static void mMpswd_decode_bit_code(ref byte[] data)
    {
        int method = data[1] & 0x0F;

        if (method > 12)
        {
            Common.mMpswd_bit_shift(ref data, -method * 3);
            Common.mMpswd_bit_reverse(ref data);
            Common.mMpswd_bit_arrange_reverse(ref data);
        }
        else if (method > 8)
        {
            Common.mMpswd_bit_shift(ref data, method * 5);
            Common.mMpswd_bit_arrange_reverse(ref data);
        }
        else if (method > 4)
        {
            Common.mMpswd_bit_reverse(ref data);
            Common.mMpswd_bit_shift(ref data, method * 5);
        }
        else
        {
            Common.mMpswd_bit_arrange_reverse(ref data);
            Common.mMpswd_bit_shift(ref data, -method * 3);
        }
    }

    public static void mMpswd_decode_RSA_cipher(ref byte[] data)
    {
        byte[] outputBuffer = [.. data];

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

    public static void mMpswd_chg_8bits_code(ref byte[] storedLocation, byte[] password)
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

    public static void mMpswd_decode_substitution_cipher(ref byte[] data)
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
    public static byte[] Decode(byte[] input, bool englishPasswords)
    {
        if (input.Length != 32)
        {
            throw new ArgumentException("Input must be 32 bytes long", nameof(input));
        }

        byte[] passwordData = new byte[24];

        Common.mMpswd_chg_password_font_code(ref input, englishPasswords ? Common.usable_to_fontnum_new_translation : Common.usable_to_fontnum_new);
        mMpswd_chg_8bits_code(ref passwordData, input);
        Common.mMpswd_transposition_cipher(ref passwordData, true, 1);
        mMpswd_decode_bit_shuffle(ref passwordData, true);
        mMpswd_decode_bit_code(ref passwordData);
        mMpswd_decode_RSA_cipher(ref passwordData);
        mMpswd_decode_bit_shuffle(ref passwordData, false);
        Common.mMpswd_transposition_cipher(ref passwordData, false, 0);
        mMpswd_decode_substitution_cipher(ref passwordData);

        return passwordData;
    }

    public static byte[]? Decode(string password, bool englishPasswords = false)
    {
        if (password.Length == 32)
        {
            byte[] data = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                var character = password.Substring(i, 1);
                int idx = Common.AFe_CharList.IndexOf(character);
                if (idx < 0)
                {
                    throw new Exception($"The password contains an invalid character: '{character}'");
                }
                data[i] = (byte)idx;
            }
            return Decode(data, englishPasswords);
        }
        return null;
    }
}
