using static System.Globalization.NumberStyles;
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
        if (!ushort.TryParse(Item.Id, HexNumber, null, out var itemId))
        {
            return null;
        }
        var passwordInput = new PasswordInput
        {
            CodeType = CodeType.User,
            HitRateIndex = 1,
            RecipientTown = Recipient.TownName,
            Recipient = Recipient.Name,
            Sender = Recipient.Name,
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
