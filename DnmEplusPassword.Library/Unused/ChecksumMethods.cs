namespace DnmEplusPassword.Library.Unused;

internal static class ChecksumMethods
{
    public static bool VerifyChecksum(ReadOnlySpan<byte> passwordData)
    {
        int calculatedChecksum = GetPasswordChecksum(passwordData);
        int storedChecksum = ((passwordData[0] & 3) << 2) | ((passwordData[1] & 0xC0) >> 6);

        Console.WriteLine($"Calculated Checksum: 0x{calculatedChecksum:X}\nStored Checksum: 0x{storedChecksum:X}");

        return calculatedChecksum == storedChecksum;
    }

    private static int GetPasswordChecksum(ReadOnlySpan<byte> passwordData)
    {
        int checksum = 0;

        for (int i = 0x03; i < 0x15; i++)
        {
            checksum += passwordData[i];
        }

        checksum += (passwordData[0x15] << 8) | passwordData[16];
        checksum += passwordData[2];

        return (((checksum >> 2) & 3) << 2) | (((checksum << 6) & 0xC0) >> 6);
    }
}
