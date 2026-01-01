using static DnmEplusPassword.Library.Internal.UnicodeNormalizer;

namespace DnmEplusPassword.Library.Data;

public readonly ref struct PasswordInput()
{
    public required readonly CodeType CodeType { get; init; }
    public readonly HitRate HitRate { get; init; } = HitRate.OneHundredPercent;
    public readonly byte NpcCode { get; init; } = 0xFF;

    private readonly byte _extraData;
    public readonly byte ExtraData
    {
        get => _extraData;
        init => _extraData = value;
    }

    public readonly byte RowAcre
    {
        get => (byte)((_extraData >> 3) & 0b111);
        init => _extraData |= (byte)(value << 3);
    }

    public readonly byte ColAcre
    {
        get => (byte)(_extraData & 0b111);
        init => _extraData |= value;
    }

    public required readonly ReadOnlySpan<char> RecipientTown { get; init => field = value.Normalize(); }
    public required readonly ReadOnlySpan<char> Recipient { get; init => field = value.Normalize(); }
    public required readonly ReadOnlySpan<char> Sender { get; init => field = value.Normalize(); }
    public required readonly ushort ItemId { get; init; }
}
