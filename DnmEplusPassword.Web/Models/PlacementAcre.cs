namespace DnmEplusPassword.Web.Models;

public sealed record PlacementAcre
{
    public int Row { get; set; } = ValidRows.First();
    public int Col { get; set; } = ValidCols.First();

    public static IReadOnlyList<int> ValidRows { get; } = [1, 2, 3, 4, 5, 6];
    public static IReadOnlyList<int> ValidCols { get; } = [1, 2, 3, 4, 5];
}
