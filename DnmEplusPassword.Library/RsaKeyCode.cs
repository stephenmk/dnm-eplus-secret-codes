using System.Collections.Immutable;

namespace DnmEplusPassword.Library;

internal readonly ref struct RsaKeyCode
{
    public readonly int Prime1 { get; }
    public readonly int Prime2 { get; }
    public readonly int Prime3 { get; }
    public readonly ImmutableArray<int> IndexTable { get; }

    public RsaKeyCode(byte[] data)
    {
        int bit10 = data[3] & 3;
        int bit32 = (data[3] >> 2) & 3;

        if (bit10 == 3)
        {
            bit10 = (bit10 ^ bit32) & 3;
            if (bit10 == 3)
            {
                bit10 = 0;
            }
        }

        if (bit32 == 3)
        {
            bit32 = (bit10 + 1) & 3;
            if (bit32 == 3)
            {
                bit32 = 1;
            }
        }

        if (bit10 == bit32)
        {
            bit32 = (bit10 + 1) & 3;
            if (bit32 == 3)
            {
                bit32 = 1;
            }
        }

        int tableIdx = ((data[3] >> 2) & 0x3C) >> 2;

        Prime1 = Common.mMpswd_prime_number[bit10];
        Prime2 = Common.mMpswd_prime_number[bit32];
        Prime3 = Common.mMpswd_prime_number[data[0xC]];
        IndexTable = Common.mMpswd_select_idx_table[tableIdx];
    }
}
