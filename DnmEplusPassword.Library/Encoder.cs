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
        byte[] output = new byte[24];

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

        // Copy Recipient Name
        for (int i = 0; i < 6; i++)
        {
            if (i >= recipientTown.Length)
            {
                output[3 + i] = 0x20; // Space Character value
            }
            else
            {
                var character = recipientTown.Substring(i, 1);
                int characterIndex = Common.AFe_CharList.IndexOf(character);
                if (characterIndex < 0)
                {
                    characterIndex = 0x20; // Set to space? TODO: Maybe we should return "invalid code" if this happens
                    Console.WriteLine("Encountered an invalid character in the Recipient's Name at string offset: " + i);
                }
                output[3 + i] = (byte)characterIndex;
            }
        }

        // Copy Recipient Town Name
        for (int i = 0; i < 6; i++)
        {
            if (i >= recipient.Length)
            {
                output[9 + i] = 0x20; // Space Character value
            }
            else
            {
                var character = recipient.Substring(i, 1);
                int characterIndex = Common.AFe_CharList.IndexOf(character);
                if (characterIndex < 0)
                {
                    characterIndex = 0x20; // Set to space? TODO: Maybe we should return "invalid code" if this happens
                    Console.WriteLine("Encountered an invalid character in the Recipient's Town Name at string offset: " + i);
                }
                output[9 + i] = (byte)characterIndex;
            }
        }

        // Copy Sender Name
        for (int i = 0; i < 6; i++)
        {
            if (i >= sender.Length)
            {
                output[15 + i] = 0x20; // Space Character value
            }
            else
            {
                var character = sender.Substring(i, 1);
                int characterIndex = Common.AFe_CharList.IndexOf(character);
                if (characterIndex < 0)
                {
                    characterIndex = 0x20; // Set to space? TODO: Maybe we should return "invalid code" if this happens
                    Console.WriteLine("Encountered an invalid character in the Sender's Name at string offset: " + i);
                }
                output[15 + i] = (byte)characterIndex;
            }
        }

        // Copy Item ID
        output[0x15] = (byte)(itemId >> 8);
        output[0x16] = (byte)itemId;

        // Add up byte totals of all characters in each string
        int checksum = 0;
        for (int i = 0; i < 6; i++)
        {
            checksum += output[3 + i];
        }

        for (int i = 0; i < 6; i++)
        {
            checksum += output[9 + i];
        }

        for (int i = 0; i < 6; i++)
        {
            checksum += output[15 + i];
        }

        checksum += itemId;
        checksum += npcCode;
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

        var parameters = Common.mMpswd_get_RSA_key_code(data);
        var prime1 = parameters.Item1;
        var prime2 = parameters.Item2;
        var prime3 = parameters.Item3;
        var indexTable = parameters.Item4;

        byte cipherValue = 0;
        int primeProduct = prime1 * prime2;

        for (int i = 0; i < 8; i++)
        {
            int value = data[indexTable[i]];
            int currentValue = value;

            for (int x = 0; x < prime3 - 1; x++)
            {
                value = value * currentValue % primeProduct;
            }

            buffer[indexTable[i]] = (byte)value;
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

#if DEBUG

    public static string Encode(
        CodeType codeType,
        int hitRateIndex,
        string recipientTown,
        string recipient,
        string sender,
        ushort itemId,
        int extraData)
    {
        byte[] PasswordData = mMpswd_make_passcode(codeType, hitRateIndex, recipientTown, recipient, sender, itemId, extraData);
        PrintByteBuffer("mMpswd_make_passcode", PasswordData);
        mMpswd_substitution_cipher(ref PasswordData);
        PrintByteBuffer("mMpswd_substitution_cipher", PasswordData);
        Common.mMpswd_transposition_cipher(ref PasswordData, true, 0);
        PrintByteBuffer("mMpswd_transposition_cipher", PasswordData);
        mMpswd_bit_shuffle(ref PasswordData, 0); // this doesn't change the last byte. Is that necessary? Doesn't seem to be.
        PrintByteBuffer("mMpswd_bit_shuffle", PasswordData);
        mMpswd_chg_RSA_cipher(ref PasswordData);
        PrintByteBuffer("mMpswd_chg_RSA_cipher", PasswordData);
        mMpswd_bit_mix_code(ref PasswordData); // the problem appears to be in the bit mix function.
        PrintByteBuffer("mMpswd_bit_mix_code", PasswordData);
        mMpswd_bit_shuffle(ref PasswordData, 1);
        PrintByteBuffer("mMpswd_bit_shuffle", PasswordData);
        Common.mMpswd_transposition_cipher(ref PasswordData, false, 1);
        PrintByteBuffer("mMpswd_transposition_cipher", PasswordData);
        byte[] Password = mMpswd_chg_6bits_code(PasswordData);
        PrintByteBuffer("mMpswd_chg_6bits_code", Password);
        mMpswd_chg_common_font_code(ref Password, false);
        PrintByteBuffer("mMpswd_chg_common_font_code", Password);

        // Construct password string
        string passwordString = "";
        for (int i = 0; i < 32; i++)
        {
            if (i == 16)
            {
                passwordString += "\r\n";
            }
            passwordString += Common.AFe_CharList[Password[i]];
        }

        return passwordString;
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

#else

    public static string Encode(
        CodeType codeType,
        int hitRateIndex,
        string recipientTown,
        string recipient,
        string sender,
        ushort itemId,
        int extraData,
        bool englishPasswords)
    {
        byte[] PasswordData = mMpswd_make_passcode(codeType, hitRateIndex, recipientTown, recipient, sender, itemId, extraData);
        mMpswd_substitution_cipher(ref PasswordData);
        Common.mMpswd_transposition_cipher(ref PasswordData, true, 0);
        mMpswd_bit_shuffle(ref PasswordData, 0);
        mMpswd_chg_RSA_cipher(ref PasswordData);
        mMpswd_bit_mix_code(ref PasswordData);
        mMpswd_bit_shuffle(ref PasswordData, 1);
        Common.mMpswd_transposition_cipher(ref PasswordData, false, 1);
        byte[] Password = mMpswd_chg_6bits_code(PasswordData);
        mMpswd_chg_common_font_code(ref Password, englishPasswords);

        // Construct password string
        string passwordString = "";
        for (int i = 0; i < 32; i++)
        {
            if (i == 16)
            {
                passwordString += "\r\n";
            }
            passwordString += Common.AFe_CharList[Password[i]];
        }

        return passwordString;
    }

#endif

}
