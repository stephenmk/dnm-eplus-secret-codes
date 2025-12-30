namespace DnmEplusPassword.Library;

/// <remarks>
/// The hit is RNG based and the player "wins" if hit < hitRate.
/// </remarks>
public enum HitRate : byte
{
    EightyPercent = 0,
    SixtyPercent = 1,
    ThirtyPercent = 2,
    ZeroPercent = 3,
    OneHundredPercent = 4,
}
