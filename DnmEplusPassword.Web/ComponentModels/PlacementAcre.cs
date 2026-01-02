namespace DnmEplusPassword.Web.ComponentModels;

public sealed record PlacementAcre
{
    public byte Row { get; set; } = ValidRows.First();
    public byte Col { get; set; } = ValidCols.First();

    public static IReadOnlyList<byte> ValidRows { get; } = [1, 2, 3, 4, 5, 6];
    public static IReadOnlyList<byte> ValidCols { get; } = [1, 2, 3, 4, 5];
}
