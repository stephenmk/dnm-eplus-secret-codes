namespace DnmEplusPassword.Library;

public readonly ref struct PasswordInput
{
    public readonly CodeType CodeType { get; init; }
    public readonly int HitRateIndex { get; init; }
    public readonly ReadOnlySpan<char> RecipientTown { get; init; }
    public readonly ReadOnlySpan<char> Recipient { get; init; }
    public readonly ReadOnlySpan<char> Sender { get; init; }
    public readonly ushort ItemId { get; init; }
    public readonly byte RowAcre { get; init; }
    public readonly byte ColAcre { get; init; }
    public readonly int ExtraData => CodeType switch
    {
        CodeType.Monument => ((RowAcre & 7) << 3) | (ColAcre & 7),
        _ => 0
    };
}
