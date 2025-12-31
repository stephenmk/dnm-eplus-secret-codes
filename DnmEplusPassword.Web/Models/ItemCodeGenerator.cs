using static System.Globalization.NumberStyles;
using DnmEplusPassword.Library;

namespace DnmEplusPassword.Web.Models;

[ValidatableType]
public sealed class ItemCodeGenerator
{
    public Name RecipientTownName { get; set; } = new();
    public Name RecipientName { get; set; } = new();
    public Name SenderName { get; set; } = new();
    public Item Item { get; set; } = new();
    public Language Language { get; set; } = new();

    public SecretCode? GenerateSecretCode()
    {
        if (!ushort.TryParse(Item.Id, HexNumber, null, out var itemId))
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
