using System.Text;
using static DnmEplusPassword.Library.Internal.UnicodeNormalizer;

namespace DnmEplusPassword.Library.Data;

public readonly ref struct PasswordInput()
{
    // First 2 bytes (16 bits) are composed of the code type, hit rate, checksum, and extra data.
    public required readonly CodeType CodeType { get; init; } // 3 bits
    public readonly HitRate HitRate { get; init; } = HitRate.OneHundredPercent; // 3 bits
    public readonly byte CheckSum { get; init; } // 4 bits
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

    public required readonly ReadOnlySpan<char> RecipientTown { get; init => field = value.DnmNormalize(); } // 6 bytes
    public required readonly ReadOnlySpan<char> Recipient { get; init => field = value.DnmNormalize(); } // 6 bytes
    public readonly ReadOnlySpan<char> Sender { get; init => field = value.DnmNormalize(); } // 6 bytes

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
}
