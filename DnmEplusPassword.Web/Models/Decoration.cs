using DnmEplusPassword.Library.Data;
using static DnmEplusPassword.Library.Data.Monument;

namespace DnmEplusPassword.Web.Models;

public sealed record Decoration
{
    public Monument Id { get; set; } = ValidDecorations.Keys.First();

    public static IReadOnlyDictionary<Monument, string> ValidDecorations { get; } = new Dictionary<Monument, string>()
    {
        {ParkClock,   "Park Clock"},
        {GasLamp,     "Gas Lamp"},
        {Windpump,    "Windpump"},
        {FlowerClock, "Flower Clock"},
        {Heliport,    "Heliport"},
        {WindTurbine, "Wind Turbine"},
        {PipeStack,   "Pipe Stack"},
        {Stonehenge,  "Stonehenge"},
        {Egg,         "Egg"},
        {Footprints,  "Footprints"},
        {Geoglyph,    "Geoglyph"},
        {Mushroom,    "Mushroom"},
        {Signpost,    "Signpost"},
        {Well,        "Well"},
        {Fountain,    "Fountain"},
    };
}
