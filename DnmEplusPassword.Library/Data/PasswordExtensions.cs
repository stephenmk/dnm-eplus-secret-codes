using System.Text;

namespace DnmEplusPassword.Library.Data;

/// <summary>
/// This class contains methods for converting Unicode text to and from the "Dōbutsu no Mori" character set (range 0x00 to 0xFF inclusive).
/// </summary>
internal static class PasswordExtensions
{
    public static string DecodeToUnicodeText(this IReadOnlyList<byte> dnmText)
    {
        Span<char> unicodeText = stackalloc char[dnmText.Count];
        for (int i = 0; i < dnmText.Count; i++)
        {
            unicodeText[i] = DnmCharToUnicodeChar[dnmText[i]];
        }
        return new string(unicodeText);
    }

    public static Span<byte> EncodeToDnmText(this string unicodeText, int size)
    {
        var normalizedText = Normalize(unicodeText);
        if (normalizedText.Length > size)
        {
            throw new ArgumentException($"Text must not contain more than {size} characters", nameof(unicodeText));
        }
        var dnmText = new byte[size];
        EncodeToDnmText(normalizedText, dnmText);
        return dnmText;
    }

    private static void EncodeToDnmText(ReadOnlySpan<char> unicodeText, Span<byte> dnmText)
    {
        int i = 0;
        foreach (var unicodeChar in unicodeText)
        {
            if (i == dnmText.Length)
            {
                throw new ArgumentException($"Length of input text '{unicodeText}' exceeds maximum size = {dnmText.Length}", nameof(unicodeText));
            }
            if (UnicodeCharToDnmChar.TryGetValue(unicodeChar, out var dnmChar))
            {
                dnmText[i++] = dnmChar;
            }
            else
            {
                throw new ArgumentException($"Invalid character: '{unicodeChar}'", nameof(unicodeText));
            }
        }
        if (i == dnmText.Length)
        {
            return;
        }
        // Fill the rest of the output with spaces.
        var dnmSpaceChar = UnicodeCharToDnmChar['　'];
        while (i < dnmText.Length)
        {
            dnmText[i++] = dnmSpaceChar;
        }
    }

    /// <summary>
    /// DnM's 16x16 usable character block mapped to the corresponding unicode characters.
    /// </summary>
    /// <remarks>
    /// The index represents the byte code of the DnM character.
    /// So for example, 'あ' is 0x00 (first index) and 'ぽ' is 0xFF (last index).
    /// Characters ①②③④ cannot be entered by the in-game user.
    /// </remarks>
    private static readonly IReadOnlyList<char> DnmCharToUnicodeChar =
    [
        'あ', 'い', 'う', 'え', 'お', 'か', 'き', 'く', 'け', 'こ', 'さ', 'し', 'す', 'せ', 'そ', 'た',
        'ち', 'つ', 'て', 'と', 'な', 'に', 'ぬ', 'ね', 'の', 'は', 'ひ', 'ふ', 'へ', 'ほ', 'ま', 'み',
        '　', '！', '＂', 'む', 'め', '％', '＆', '＇', '（', '）', '〜', '心', '，', '－', '．', '楽',
        '０', '１', '２', '３', '４', '５', '６', '７', '８', '９', '：', '水', '＜', '①', '＞', '？',
        '＠', 'Ａ', 'Ｂ', 'Ｃ', 'Ｄ', 'Ｅ', 'Ｆ', 'Ｇ', 'Ｈ', 'Ｉ', 'Ｊ', 'Ｋ', 'Ｌ', 'Ｍ', 'Ｎ', 'Ｏ',
        'Ｐ', 'Ｑ', 'Ｒ', 'Ｓ', 'Ｔ', 'Ｕ', 'Ｖ', 'Ｗ', 'Ｘ', 'Ｙ', 'Ｚ', 'も', '怒', 'や', 'ゆ', '＿',
        'よ', 'ａ', 'ｂ', 'ｃ', 'ｄ', 'ｅ', 'ｆ', 'ｇ', 'ｈ', 'ｉ', 'ｊ', 'ｋ', 'ｌ', 'ｍ', 'ｎ', 'ｏ',
        'ｐ', 'ｑ', 'ｒ', 'ｓ', 'ｔ', 'ｕ', 'ｖ', 'ｗ', 'ｘ', 'ｙ', 'ｚ', 'ら', 'り', 'る', 'れ', '②',
        '③', '。', '「', '」', '、', '・', 'ヲ', 'ァ', 'ィ', 'ゥ', 'ェ', 'ォ', 'ャ', 'ュ', 'ョ', 'ッ',
        'ー', 'ア', 'イ', 'ウ', 'エ', 'オ', 'カ', 'キ', 'ク', 'ケ', 'コ', 'サ', 'シ', 'ス', 'セ', 'ソ',
        'タ', 'チ', 'ツ', 'テ', 'ト', 'ナ', 'ニ', 'ヌ', 'ネ', 'ノ', 'ハ', 'ヒ', 'フ', 'ヘ', 'ホ', 'マ',
        'ミ', 'ム', 'メ', 'モ', 'ヤ', 'ユ', 'ヨ', 'ラ', 'リ', 'ル', 'レ', 'ロ', 'ワ', 'ン', 'ヴ', '顔',
        'ろ', 'わ', 'を', 'ん', 'ぁ', 'ぃ', 'ぅ', 'ぇ', 'ぉ', 'ゃ', 'ゅ', 'ょ', 'っ', '④', 'ガ', 'ギ',
        'グ', 'ゲ', 'ゴ', 'ザ', 'ジ', 'ズ', 'ゼ', 'ゾ', 'ダ', 'ヂ', 'ヅ', 'デ', 'ド', 'バ', 'ビ', 'ブ',
        'ベ', 'ボ', 'パ', 'ピ', 'プ', 'ペ', 'ポ', 'が', 'ぎ', 'ぐ', 'げ', 'ご', 'ざ', 'じ', 'ず', 'ぜ',
        'ぞ', 'だ', 'ぢ', 'づ', 'で', 'ど', 'ば', 'び', 'ぶ', 'べ', 'ぼ', 'ぱ', 'ぴ', 'ぷ', 'ぺ', 'ぽ',
    ];

    /// <remarks>
    /// IReadOnlyList doesn't have an IndexOf method, so we'll convert the list to a dictionary for that functionality.
    /// </remarks>
    private static readonly IReadOnlyDictionary<char, byte> UnicodeCharToDnmChar =
        DnmCharToUnicodeChar
            .Select(static (chr, idx) => new KeyValuePair<char, byte>(chr, (byte)idx))
            .ToDictionary();

    private static ReadOnlySpan<char> Normalize(ReadOnlySpan<char> input)
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
