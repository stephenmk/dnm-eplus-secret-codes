using System.ComponentModel.DataAnnotations;

namespace DnmEplusPassword.Web.Models;

public sealed record Sender
{
    [StringLength(maximumLength: 6, MinimumLength = 1, ErrorMessage = "Must contain at least 1 character.")]
    [RegularExpression($@"^[{Constants.ValidNameCharacters}]+$", ErrorMessage = "Name contains invalid characters.")]
    public string Name { get; set; } = string.Empty;
}
