using System.ComponentModel.DataAnnotations;

namespace DnmEplusPassword.Web.ComponentModels;

public sealed record Code
{
    [StringLength(maximumLength: 16, MinimumLength = 16, ErrorMessage = "Must contain exactly 16 characters.")]
    public string Line1 { get; set; } = string.Empty;

    [StringLength(maximumLength: 16, MinimumLength = 16, ErrorMessage = "Must contain exactly 16 characters.")]
    public string Line2 { get; set; } = string.Empty;
}
