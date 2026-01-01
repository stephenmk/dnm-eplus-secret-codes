namespace DnmEplusPassword.Library.Internal;

internal static class ByteCollectionExtensions
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

    public static string DecodeToUnicodeText(this ReadOnlySpan<byte> bytes)
    {
        Span<char> unicodeChars = stackalloc char[bytes.Length];
        for (int i = 0; i < bytes.Length; i++)
        {
            unicodeChars[i] = UnicodeCharacterCodepoints[bytes[i]];
        }
        return new string(unicodeChars);
    }

    public static Span<byte> EncodeToNameBytes(this ReadOnlySpan<char> unicodeText, int size)
    {
        Span<byte> nameBytes = new byte[size];
        var normalizedText = unicodeText.DnmNormalize();
        if (normalizedText.Length > size)
        {
            throw new ArgumentException($"Normalized text must not contain more than {size} characters", nameof(unicodeText));
        }
        normalizedText.EncodeTo(nameBytes);
        return nameBytes;
    }

    private static void EncodeTo(this ReadOnlySpan<char> unicodeText, Span<byte> bytes)
    {
        int i = 0;
        foreach (var unicodeChar in unicodeText)
        {
            if (i == bytes.Length)
            {
                throw new ArgumentException($"Length of input text '{unicodeText}' exceeds maximum size = {bytes.Length}", nameof(unicodeText));
            }
            if (UnicodeCharacterCodepointDictionary.TryGetValue(unicodeChar, out var @byte))
            {
                bytes[i++] = @byte;
            }
            else
            {
                throw new ArgumentException($"Invalid character: '{unicodeChar}'", nameof(unicodeText));
            }
        }
        if (i == bytes.Length)
        {
            return;
        }
        // Fill the rest of the output with spaces.
        if (!UnicodeCharacterCodepointDictionary.TryGetValue('　', out var spaceByte))
        {
            throw new ArgumentException($"Invalid character: '　'", nameof(unicodeText));
        }
        while (i < bytes.Length)
        {
            bytes[i++] = spaceByte;
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
    private static readonly IReadOnlyList<char> UnicodeCharacterCodepoints =
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
    private static readonly IReadOnlyDictionary<char, byte> UnicodeCharacterCodepointDictionary =
        UnicodeCharacterCodepoints
            .Select(static (chr, idx) => new KeyValuePair<char, byte>(chr, (byte)idx))
            .ToDictionary();
}
