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
        try
        {
            var password = Encoder.Encode
            (
                codeType: CodeType.User,
                hitRateIndex: 1,
                recipientTown: Recipient.TownName,
                recipient: Recipient.Name,
                sender: Recipient.Name,
                itemId: ushort.Parse(Item.Id, HexNumber),
                extraData: 0,
                englishPasswords: Language.IsEnglish
            );
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
