using System.ComponentModel.DataAnnotations;

namespace DnmEplusPassword.Web.ComponentModels;

public sealed record Name
{
    public required int MaxLength { get; init; }

    [Required(ErrorMessage = "Must contain at least 1 character.")]
    public string Value
    {
        get => _value;
        set => _value = _value is null
            ? string.Empty
            : value.EnumerateRunes().ToList() is var runes && runes.Count <= MaxLength
                ? value
                : runes.Take(MaxLength).FastToString();
    }

    private string _value = string.Empty;
}
