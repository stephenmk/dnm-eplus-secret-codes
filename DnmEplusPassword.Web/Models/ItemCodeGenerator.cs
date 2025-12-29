using DnmEplusPassword.Library;

namespace DnmEplusPassword.Web.Models;

[ValidatableType]
public sealed class ItemCodeGenerator
{
    public Recipient Recipient { get; set; } = new();
    public Sender Sender { get; set; } = new();
    public Item Item { get; set; } = new();
    public Language Language { get; set; } = new();

    public SecretCode? GenerateSecretCode()
    {
        return null;
    }
}
