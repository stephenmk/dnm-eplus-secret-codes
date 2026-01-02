using System.Text;

namespace DnmEplusPassword.Library.Data;

public readonly ref struct PasswordInput()
{
    public required readonly CodeType CodeType { get; init; } // 3 bits
    public readonly HitRate HitRate { get; init; } = HitRate.OneHundredPercent; // 3 bits
    public readonly byte Checksum { get; init; } // 4 bits
    public readonly byte ExtraData { get; init; } // 6 bits

    public readonly byte RowAcre
    {
        get => (byte)((ExtraData >> 3) & 0b111);
        init => ExtraData |= (byte)(value << 3);
    }

    public readonly byte ColAcre
    {
        get => (byte)(ExtraData & 0b111);
        init => ExtraData |= value;
    }

    public readonly byte NpcCode { get; init; } = 0xFF; // 1 byte

    public readonly ReadOnlySpan<byte> Name1 { get; init; } // 6 bytes
    public readonly ReadOnlySpan<byte> Name2 { get; init; } // 6 bytes
    public readonly ReadOnlySpan<byte> Name3 { get; init; } // 6 bytes

    public readonly ReadOnlySpan<char> RecipientTown
    {
        get => Name1.DecodeToUnicodeText().TrimEnd();
        init => Name1 = value.EncodeToDnmText(6);
    }

    public readonly ReadOnlySpan<char> Recipient
    {
        get => Name2.DecodeToUnicodeText().TrimEnd();
        init => Name2 = value.EncodeToDnmText(6);
    }

    public readonly ReadOnlySpan<char> Sender
    {
        get => Name3.DecodeToUnicodeText().TrimEnd();
        init => Name3 = value.EncodeToDnmText(6);
    }

    public readonly int Price
    {
        get => int.Parse(Sender.ToString().Normalize(NormalizationForm.FormKC));
        init => Sender = value.ToString();
    }

    public readonly ushort ItemId { get; init; } // 2 bytes

    public readonly Monument Monument
    {
        get => (Monument)(ItemId & 0xFF);
        init => ItemId = (ushort)value;
    }

    public byte CalculateChecksum()
        => (byte)((NpcCode + Name1.Sum() + Name2.Sum() + Name3.Sum() + ItemId) & 0xF);

    public Span<byte> ToPlaintext()
    {
        Span<byte> output = new byte[24];

        var checksum = CalculateChecksum(); // 4 bits

        // First byte: 3 bits for code type, 3 bits for hit rate, and 2 bits for checksum.
        var byte0 = ((byte)CodeType << 5) | ((byte)HitRate << 2) | ((checksum >> 2) & 0b11);
        output[0] = (byte)byte0;

        // Second byte: 2 bits for checksum and 6 bits for extra data.
        var byte1 = ((checksum & 0b11) << 6) | (ExtraData & 0b0011_1111);
        output[1] = (byte)byte1;

        output[2] = NpcCode;
        Name1.CopyTo(output[3..]);
        Name2.CopyTo(output[9..]);
        Name3.CopyTo(output[15..]);
        output[21] = (byte)(ItemId >> 8);
        output[22] = (byte)(ItemId & 0xFF);

        return output;
    }
}
