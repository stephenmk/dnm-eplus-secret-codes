using System.ComponentModel.DataAnnotations;

namespace DnmEplusPassword.Web.Models;

public sealed record Price
{
    [Range(1, 999999, ErrorMessage="Price must be between 1 and 999,999")]
    public int Value { get; set; } = 1;
}
