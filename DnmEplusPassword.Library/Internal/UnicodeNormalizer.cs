using System.Text;

namespace DnmEplusPassword.Library.Internal;

internal static class UnicodeNormalizer
{
    public static ReadOnlySpan<char> DnmNormalize(this ReadOnlySpan<char> input)
    {
        Span<char> characters = input.Length < 128
            ? stackalloc char[input.Length]
            : new char[input.Length];

        int length = 0;
        foreach (var rune in input.EnumerateRunes())
        {
            characters[length++] = Normalize(rune);
        }
        return new string(characters[..length]);
    }

    private static char Normalize(Rune rune)
        => rune.Value switch
        {
            ' ' => '　',
            '!' => '！',
            '"' or '”' => '＂',
            '%' => '％',
            '&' => '＆',
            '\'' or '’' => '＇',
            '(' => '（',
            ')' => '）',
            '~' => '〜',
            '♡' or '♥' or '❤' => '心',
            ',' => '，',
            '-' => '－',
            '.' => '．',
            '♪' => '楽',
            '0' => '０',
            '1' => '１',
            '2' => '２',
            '3' => '３',
            '4' => '４',
            '5' => '５',
            '6' => '６',
            '7' => '７',
            '8' => '８',
            '9' => '９',
            ':' => '：',
            0x1F322 or 0x1F4A7 => '水', // 🌢 and 💧
            '<' => '＜',
            '>' => '＞',
            '?' => '？',
            '@' => '＠',
            'A' => 'Ａ',
            'B' => 'Ｂ',
            'C' => 'Ｃ',
            'D' => 'Ｄ',
            'E' => 'Ｅ',
            'F' => 'Ｆ',
            'G' => 'Ｇ',
            'H' => 'Ｈ',
            'I' => 'Ｉ',
            'J' => 'Ｊ',
            'K' => 'Ｋ',
            'L' => 'Ｌ',
            'M' => 'Ｍ',
            'N' => 'Ｎ',
            'O' => 'Ｏ',
            'P' => 'Ｐ',
            'Q' => 'Ｑ',
            'R' => 'Ｒ',
            'S' => 'Ｓ',
            'T' => 'Ｔ',
            'U' => 'Ｕ',
            'V' => 'Ｖ',
            'W' => 'Ｗ',
            'X' => 'Ｘ',
            'Y' => 'Ｙ',
            'Z' => 'Ｚ',
            0x1F4A2 => '怒', // 💢
            '_' => '＿',
            'a' => 'ａ',
            'b' => 'ｂ',
            'c' => 'ｃ',
            'd' => 'ｄ',
            'e' => 'ｅ',
            'f' => 'ｆ',
            'g' => 'ｇ',
            'h' => 'ｈ',
            'i' => 'ｉ',
            'j' => 'ｊ',
            'k' => 'ｋ',
            'l' => 'ｌ',
            'm' => 'ｍ',
            'n' => 'ｎ',
            'o' => 'ｏ',
            'p' => 'ｐ',
            'q' => 'ｑ',
            'r' => 'ｒ',
            's' => 'ｓ',
            't' => 'ｔ',
            'u' => 'ｕ',
            'v' => 'ｖ',
            'w' => 'ｗ',
            'x' => 'ｘ',
            'y' => 'ｙ',
            'z' => 'ｚ',
            '｢' => '「',
            '｣' => '」',
            '･' => '・',
            '☺' => '顔',
            >= char.MinValue and <= char.MaxValue => (char)rune.Value,
            _ => throw new ArgumentException($"Input text contains unsupported character: '{rune}'")
        };
}
