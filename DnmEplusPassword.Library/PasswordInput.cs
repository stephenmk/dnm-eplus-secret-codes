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

    /// <remarks>
    /// Valid indices are 0 - 4. Hit rates are: { 80.0f, 60.0f, 30.0f, 0.0f, 100.0f }.
    /// The hit is RNG based and the player "wins" if hit < hitRate.
    /// </remarks>
    public readonly byte HitRateIndex
    {
        get => CodeType switch
        {
            CodeType.Magazine => field,
            _ => 4
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
