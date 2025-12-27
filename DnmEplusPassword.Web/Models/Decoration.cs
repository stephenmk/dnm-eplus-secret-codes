namespace DnmEplusPassword.Web.Models;

public sealed record Decoration
{
    public int Id { get; set; } = ValidDecorations.Keys.First();

    private static readonly string[] DecorationNames =
    [
        "Park Clock",
        "Gas Lamp",
        "Windpump",
        "Flower Clock",
        "Heliport",
        "Wind Turbine",
        "Pipe Stack",
        "Stonehenge",
        "Egg",
        "Footprints",
        "Geoglyph",
        "Mushroom",
        "Signpost",
        "Well",
        "Fountain",
    ];

    public static IReadOnlyDictionary<int, string> ValidDecorations { get; } =
        DecorationNames
            .Select(static (name, idx) => new KeyValuePair<int, string>(idx, name))
            .ToDictionary();
}
