namespace DnmEplusPassword.Library;

/// <remarks>
/// The hit is RNG based and the player "wins" if hit < hitRate.
/// </remarks>
public enum HitRate : byte
{
    EightyPercent = 0x00,
    SixtyPercent = 0x01,
    ThirtyPercent = 0x02,
    ZeroPercent = 0x03,
    OneHundredPercent = 0x04,
}
