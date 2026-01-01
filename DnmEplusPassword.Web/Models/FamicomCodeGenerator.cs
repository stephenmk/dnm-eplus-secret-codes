using DnmEplusPassword.Library;
using DnmEplusPassword.Library.Data;

namespace DnmEplusPassword.Web.Models;

[ValidatableType]
public sealed class FamicomCodeGenerator
{
    public Name RecipientTownName { get; set; } = new() { MaxLength = 6 };
    public Name RecipientName { get; set; } = new() { MaxLength = 6 };
    public Item Item { get; set; } = new() { Type = Item.ItemType.Famicom };
    public Language Language { get; set; } = new();

    public SecretCode GenerateSecretCode()
    {
        var passwordInput = new PasswordInput
        {
            CodeType = CodeType.Famicom,
            RecipientTown = RecipientTownName.Value,
            Recipient = RecipientName.Value,
            ItemId = Item.HexId,
        };

        var password = Encoder.Encode(passwordInput, Language.IsEnglish);

        return new()
        {
            Line1 = password.Item1,
            Line2 = password.Item2,
        };
    }
}
