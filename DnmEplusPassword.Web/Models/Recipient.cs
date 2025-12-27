namespace DnmEplusPassword.Web.Models;

public sealed record Recipient
{
    public string Name { get; set; } = string.Empty;
    public string TownName { get; set; } = string.Empty;
}
