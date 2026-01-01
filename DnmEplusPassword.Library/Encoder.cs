using DnmEplusPassword.Library.Data;
using static DnmEplusPassword.Library.Internal.ByteCollectionExtensions;
using static DnmEplusPassword.Library.Internal.CommonMethods;
using static DnmEplusPassword.Library.Internal.EncodeMethods;

namespace DnmEplusPassword.Library;

public static class Encoder
{
    public static PasswordOutput Encode(in PasswordInput input, bool englishPasswords)
    {
        Span<byte> passcode = stackalloc byte[24];

        MakePasscode(input, passcode);
        SubstitutionCipher(passcode);
        TranspositionCipher(passcode, true, 0);
        BitShuffle(passcode, 0);
        ChangeRsaCipher(passcode);
        BitMixCode(passcode);
        BitShuffle(passcode, 1);
        TranspositionCipher(passcode, false, 1);

        Span<byte> password = new byte[32];
        ChangeSixBitsCode(passcode, password);
        ChangeCommonFontCode(password, englishPasswords);

        return new PasswordOutput(password);
    }

    private static void MakePasscode(in PasswordInput input, Span<byte> output)
    {
        int checksum = input.CalculateChecksum();

        // First byte: 3 bits for code type, 3 bits for hit rate, and 2 bits for checksum.
        var byte0 = ((byte)input.CodeType << 5) | ((byte)input.HitRate << 2) | ((checksum >> 2) & 0b11);
        output[0] = (byte)byte0;

        // Second byte: 2 bits for checksum and 6 bits for extra data.
        var byte1 = ((checksum & 0b11) << 6) | (input.ExtraData & 0b0011_1111);
        output[1] = (byte)byte1;

        output[2] = input.NpcCode;
        input.Name1.CopyTo(output[3..]);
        input.Name2.CopyTo(output[9..]);
        input.Name3.CopyTo(output[15..]);
        output[21] = (byte)(input.ItemId >> 8);
        output[22] = (byte)(input.ItemId & 0b1111_1111);

        // Zero the final byte just in case. Stack-allocated arrays aren't initialized to zeroes.
        output[23] = 0x00;
    }
}
