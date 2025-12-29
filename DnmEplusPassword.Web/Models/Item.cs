using System.ComponentModel.DataAnnotations;

namespace DnmEplusPassword.Web.Models;

public sealed record Item
{
    [StringLength(maximumLength: 4, MinimumLength = 4, ErrorMessage = "Item ID should contain 4 characters.")]
    [RegularExpression(@"^[0-9A-Fa-f]+$", ErrorMessage = "Item ID should only contain numbers and letters A through F.")]
    public string Id { get; set; } = string.Empty;
}
