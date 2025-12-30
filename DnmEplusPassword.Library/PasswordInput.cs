namespace DnmEplusPassword.Library;

public readonly ref struct PasswordInput
{
    public readonly CodeType CodeType { get; init; }
    public readonly ReadOnlySpan<char> RecipientTown { get; init; }
    public readonly ReadOnlySpan<char> Recipient { get; init; }
    public readonly ReadOnlySpan<char> Sender { get; init; }
    public readonly ushort ItemId { get; init; }
    public readonly byte RowAcre { get; init; }
    public readonly byte ColAcre { get; init; }

    public readonly HitRate HitRate
    {
        get => CodeType switch
        {
            CodeType.Magazine => field,
            _ => HitRate.OneHundredPercent
        };
        init;
    }

    public readonly int ExtraData => CodeType switch
    {
        CodeType.Monument => (RowAcre << 3) | ColAcre,
        _ => 0
    };

    public readonly byte NpcCode => CodeType switch
    {
        CodeType.NPC or CodeType.New_NPC => 0x00,
        _ => 0xFF
    };
}
