namespace DnmEplusPassword.Web.Models;

public sealed record Decoration
{
    public int Id { get; set; } = ValidDecorations.Keys.First();

    public static IReadOnlyDictionary<int, string> ValidDecorations { get; } = new Dictionary<int, string>()
    {
        {00, "Park Clock"},
        {01, "Gas Lamp"},
        {02, "Windpump"},
        {03, "Flower Clock"},
        {04, "Heliport"},
        {05, "Wind Turbine"},
        {06, "Pipe Stack"},
        {07, "Stonehenge"},
        {08, "Egg"},
        {09, "Footprints"},
        {10, "Geoglyph"},
        {11, "Mushroom"},
        {12, "Signpost"},
        {13, "Well"},
        {14, "Fountain"},
    };
}
