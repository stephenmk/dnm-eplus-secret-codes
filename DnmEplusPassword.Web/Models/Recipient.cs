using System.ComponentModel.DataAnnotations;

namespace DnmEplusPassword.Web.Models;

public sealed record Recipient
{
    [Required]
    [StringLength(maximumLength: 6, ErrorMessage = "Maximum name length is 6 characters")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(maximumLength: 6, ErrorMessage = "Maximum town name length is 6 characters")]
    public string TownName { get; set; } = string.Empty;
}
