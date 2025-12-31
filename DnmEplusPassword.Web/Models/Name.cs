using System.ComponentModel.DataAnnotations;

namespace DnmEplusPassword.Web.Models;

public sealed record Name
{
    private const string ValidNameCharacters =
        """
        あいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみ !"むめ%&'()~♥,-.♪0123456789:🌢<+>?@ABCDEFGHIJKLMNOPQRSTUVWXYZも💢やゆ_よabcdefghijklmnopqrstuvwxyzらりるれ�□。｢｣、･ヲァィゥェォャュョッーアイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワンヴ☺ろわをんぁぃぅぇぉゃゅょっ⏎ガギグゲゴザジズゼゾダヂヅデドバビブベボパピプペポがぎぐげござじずぜぞだぢづでどばびぶべぼぱぴぷぺぽ
        """;

    public required int MaxLength { get; init; }

    [Required(ErrorMessage = "Must contain at least 1 character.")]
    [RegularExpression($@"^[{ValidNameCharacters}]+$", ErrorMessage = "Name contains invalid characters.")]
    public string Value
    {
        get => _value;
        set => _value = _value is null
            ? string.Empty
            : value.EnumerateRunes().ToList() is var runes && runes.Count <= MaxLength
                ? value
                : string.Join(string.Empty, runes.Take(MaxLength).Select(r => r.ToString()));
    }

    private string _value = string.Empty;
}
