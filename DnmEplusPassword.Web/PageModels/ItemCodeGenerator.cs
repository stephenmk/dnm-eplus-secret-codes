using DnmEplusPassword.Library;
using DnmEplusPassword.Library.Data;
using DnmEplusPassword.Web.ComponentModels;

namespace DnmEplusPassword.Web.PageModels;

[ValidatableType]
public sealed class ItemCodeGenerator
{
    public Name RecipientTownName { get; set; } = new() { MaxLength = 6 };
    public Name RecipientName { get; set; } = new() { MaxLength = 6 };
    public Name SenderName { get; set; } = new() { MaxLength = 6 };
    public Item Item { get; set; } = new() { Type = Item.ItemType.User };
    public Language Language { get; set; } = new();

    public PasswordOutput GenerateSecretCode()
    {
        var passwordInput = new PasswordInput
        {
            CodeType = CodeType.User,
            RecipientTown = RecipientTownName.Value,
            Recipient = RecipientName.Value,
            Sender = SenderName.Value,
            ItemId = Item.HexId,
        };

        return Encoder.Encode(passwordInput, Language.IsEnglish);
    }
}
