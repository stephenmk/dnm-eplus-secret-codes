using System.Text;

namespace DnmEplusPassword.Library;

public class Decoder
{
    public static void DecodeBitShuffle(Span<byte> data, bool keyIdx)
    {
        int count = keyIdx ? 0x17 : 0x16;
        int bitIdx = keyIdx ? 0x09 : 0x0D;

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
        var shuffleTable = Common.SelectIndexTable[data[bitIdx] & 3];
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

        zeroedData[..bitIdx].CopyTo(data);
        data[bitIdx] = tableIndex;
        Console.WriteLine($"zeroedDataIdx ({zeroedDataIdx}) - bitIdx ({bitIdx}) is {zeroedDataIdx - bitIdx}");
        zeroedData[bitIdx..zeroedDataIdx].CopyTo(data[(bitIdx + 1)..]);
    }

    public static void DecodeBitCode(Span<byte> data)
    {
        int method = data[1] & 0x0F;

        if (method > 12)
        {
            Common.BitShift(data, -method * 3);
            Common.BitReverse(data);
            Common.BitArrangeReverse(data);
        }
        else if (method > 8)
        {
            Common.BitShift(data, method * 5);
            Common.BitArrangeReverse(data);
        }
        else if (method > 4)
        {
            Common.BitReverse(data);
            Common.BitShift(data, method * 5);
        }
        else
        {
            Common.BitArrangeReverse(data);
            Common.BitShift(data, -method * 3);
        }
    }

    public static void DecodeRsaCipher(Span<byte> data)
    {
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

        Span<byte> outputBuffer = stackalloc byte[data.Length];
        data.CopyTo(outputBuffer);

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

    public static void ChangeEightBitsCode(Span<byte> storedLocation, ReadOnlySpan<byte> password)
    {
        int passwordIndex = 0;
        int storedLocationIndex = 0;

        int storedValue = 0;
        int shiftRightValue = 0;
        int shiftLeftValue = 0;

        while (true)
        {
            storedValue |= (((password[passwordIndex] >> shiftRightValue) & 1) << shiftLeftValue) & 0xFF;
            shiftRightValue++;
            shiftLeftValue++;

            if (shiftLeftValue > 7)
            {
                storedLocation[storedLocationIndex++] = (byte)storedValue;
                shiftLeftValue = 0;
                if (storedLocationIndex >= 24)
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

    public static void DecodeSubstitutionCipher(Span<byte> data)
    {
        for (int i = 0; i < 24; i++)
        {
            for (int x = 0; x < 256; x++)
            {
                if (data[i] == Common.ChangeCodeTable[x])
                {
                    data[i] = (byte)x;
                    break;
                }
            }
        }
    }

    // Main Code \\
    private const int pwLength = 32;

    public static PasswordInput Decode(string password, bool englishPasswords = false)
    {
        Span<Rune> runes = stackalloc Rune[pwLength];
        int runeCount = FillRunes(password, runes);
        if (runeCount != pwLength)
        {
            throw new ArgumentException($"Password must contain {pwLength} characters", nameof(password));
        }

        Span<byte> passwordBytes = stackalloc byte[runes.Length];
        for (int i = 0; i < pwLength; i++)
        {
            if (Common.UnicodeCharacterCodepointDictionary.TryGetValue(runes[i], out var idx))
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

        var townName = data.Slice(3, 6).ToPasswordLine().TrimEnd();
        var playerName = data.Slice(9, 6).ToPasswordLine().TrimEnd();
        var senderString = data.Slice(15, 6).ToPasswordLine().TrimEnd();

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
            ? Common.TranslatedCharacterCodepoints
            : Common.CharacterCodepoints;

        Common.ChangePasswordFontCode(input, characterCodepoints);
        ChangeEightBitsCode(output, input);
        Common.TranspositionCipher(output, true, 1);
        DecodeBitShuffle(output, true);
        DecodeBitCode(output);
        DecodeRsaCipher(output);
        DecodeBitShuffle(output, false);
        Common.TranspositionCipher(output, false, 0);
        DecodeSubstitutionCipher(output);
    }

    private static int FillRunes(ReadOnlySpan<char> source, Span<Rune> dest)
    {
        int i = 0;
        foreach (var rune in source.EnumerateRunes())
        {
            if (i == dest.Length)
            {
                break;
            }
            dest[i++] = rune;
        }
        return i;
    }
}
