using static DnmEplusPassword.Library.Internal.ByteCollectionExtensions;

namespace DnmEplusPassword.Library.Unused;

internal static class ZuruChecker
{
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

    private static int GetStringByteValue(ReadOnlySpan<char> input)
    {
        Span<byte> bytes = stackalloc byte[input.Length];
        input.EncodeTo(bytes);
        return bytes.Sum();
    }
}
