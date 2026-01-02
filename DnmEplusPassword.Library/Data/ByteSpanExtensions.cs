namespace DnmEplusPassword.Library.Data;

internal static class ByteSpanExtensions
{
    public static int Sum(this ReadOnlySpan<byte> bytes)
    {
        int sum = 0;
        foreach (var b in bytes)
        {
            sum += b;
        }
        return sum;
    }
}
