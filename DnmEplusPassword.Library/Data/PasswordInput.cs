using static DnmEplusPassword.Library.Internal.UnicodeNormalizer;

namespace DnmEplusPassword.Library.Data;

public readonly ref struct PasswordInput()
{
    public required readonly CodeType CodeType { get; init; }
    public readonly HitRate HitRate { get; init; } = HitRate.OneHundredPercent;
    public readonly byte CheckSum { get; init; }
    public readonly byte ExtraData { get; init; }

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

    public readonly byte NpcCode { get; init; } = 0xFF;

    public required readonly ReadOnlySpan<char> RecipientTown { get; init => field = value.Normalize(); }
    public required readonly ReadOnlySpan<char> Recipient { get; init => field = value.Normalize(); }
    public required readonly ReadOnlySpan<char> Sender { get; init => field = value.Normalize(); }
    public required readonly ushort ItemId { get; init; }
}
