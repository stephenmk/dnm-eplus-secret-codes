using DnmEplusPassword.Library.Data;
using static DnmEplusPassword.Library.Data.HitRate;

namespace DnmEplusPassword.Web.Models;

public sealed record SuccessRate
{
    public HitRate Id { get; set; } = ValidRates.Keys.First();

    public static IReadOnlyDictionary<HitRate, string> ValidRates { get; } = new Dictionary<HitRate, string>()
    {
        {OneHundredPercent, "100%"},
        {EightyPercent,      "80%"},
        {SixtyPercent,       "60%"},
        {ThirtyPercent,      "30%"},
        {ZeroPercent,         "0%"},
    };
}
