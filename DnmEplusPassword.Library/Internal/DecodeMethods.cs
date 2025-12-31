using static DnmEplusPassword.Library.Internal.CommonMethods;
using static DnmEplusPassword.Library.Internal.Constants;

namespace DnmEplusPassword.Library.Internal;

internal static class DecodeMethods
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
        var shuffleTable = SelectIndexTable[data[bitIdx] & 3];
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
            BitShift(data, -method * 3);
            BitReverse(data);
            BitArrangeReverse(data);
        }
        else if (method > 8)
        {
            BitShift(data, method * 5);
            BitArrangeReverse(data);
        }
        else if (method > 4)
        {
            BitReverse(data);
            BitShift(data, method * 5);
        }
        else
        {
            BitArrangeReverse(data);
            BitShift(data, -method * 3);
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
                if (data[i] == ChangeCodeTable[x])
                {
                    data[i] = (byte)x;
                    break;
                }
            }
        }
    }
}
