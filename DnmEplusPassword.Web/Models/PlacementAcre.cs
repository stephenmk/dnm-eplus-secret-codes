using System.Collections.Immutable;

namespace DnmEplusPassword.Web.Models;

public sealed record PlacementAcre
{
    public int Row { get; set; } = ValidRows.First();
    public int Col { get; set; } = ValidCols.First();

    public static readonly ImmutableArray<int> ValidRows = [1, 2, 3, 4, 5, 6];
    public static readonly ImmutableArray<int> ValidCols = [1, 2, 3, 4, 5];
}
