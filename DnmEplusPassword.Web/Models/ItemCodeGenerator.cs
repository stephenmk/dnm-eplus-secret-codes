using DnmEplusPassword.Library;

namespace DnmEplusPassword.Web.Models;

[ValidatableType]
public sealed class ItemCodeGenerator
{
    public Name RecipientTownName { get; set; } = new() { MaxLength = 6 };
    public Name RecipientName { get; set; } = new() { MaxLength = 6 };
    public Name SenderName { get; set; } = new() { MaxLength = 6 };
    public Item Item { get; set; } = new() { Type = Item.ItemType.User };
    public Language Language { get; set; } = new();

    public SecretCode? GenerateSecretCode()
    {
        if (Item.HexId is not ushort itemId)
        {
            return null;
        }
        var passwordInput = new PasswordInput
        {
            CodeType = CodeType.User,
            RecipientTown = RecipientTownName.Value,
            Recipient = RecipientName.Value,
            Sender = SenderName.Value,
            ItemId = itemId,
        };
        try
        {
            var password = Encoder.Encode(passwordInput, Language.IsEnglish);
            return new()
            {
                Line1 = password.Item1,
                Line2 = password.Item2,
            };
        }
        catch
        {
            return null;
        }
    }
}
