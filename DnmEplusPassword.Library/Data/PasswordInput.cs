using static DnmEplusPassword.Library.Internal.UnicodeNormalizer;

namespace DnmEplusPassword.Library.Data;

public readonly ref struct PasswordInput
{
    public required readonly CodeType CodeType { get; init; }
    public required readonly ReadOnlySpan<char> RecipientTown { get; init => field = value.Normalize(); }
    public required readonly ReadOnlySpan<char> Recipient { get; init => field = value.Normalize(); }
    public required readonly ReadOnlySpan<char> Sender { get; init => field = value.Normalize(); }
    public required readonly ushort ItemId { get; init; }

    public readonly byte RowAcre { get; init; }
    public readonly byte ColAcre { get; init; }
    public readonly int ExtraData => CodeType switch
    {
        CodeType.Monument => (RowAcre << 3) | ColAcre,
        _ => 0
    };

    public readonly HitRate HitRate
    {
        get => CodeType switch
        {
            CodeType.Magazine => field,
            _ => HitRate.OneHundredPercent
        };
        init => field = value;
    }

    internal readonly byte NpcCode => CodeType switch
    {
        CodeType.NPC or CodeType.New_NPC => 0x00,
        _ => 0xFF
    };
}
