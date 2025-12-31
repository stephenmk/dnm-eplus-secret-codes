using static DnmEplusPassword.Library.Internal.Constants;

namespace DnmEplusPassword.Library.Internal;

public static class ZuruChecker
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

    private static int GetStringByteValue(string input)
        => StringToAFByteArray(input).Sum(x => x);

    private static byte[] StringToAFByteArray(string input)
    {
        var inputRunes = input.EnumerateRunes().ToArray();
        var output = new byte[inputRunes.Length];

        for (int i = 0; i < input.Length; i++)
        {
            var rune = inputRunes[i];
            if (UnicodeCharacterCodepointDictionary.TryGetValue(rune, out var idx))
            {
                output[i] = idx;
            }
            else
            {
                throw new ArgumentException($"Invalid character: {rune}", nameof(input));
            }
        }

        return output;
    }
}
