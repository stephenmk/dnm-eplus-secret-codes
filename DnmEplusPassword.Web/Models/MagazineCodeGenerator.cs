
using DnmEplusPassword.Library;

namespace DnmEplusPassword.Web.Models;

[ValidatableType]
public sealed class MagazineCodeGenerator
{
    public Name SenderTownName { get; set; } = new();
    public Name SenderName { get; set; } = new();
    public Item Item { get; set; } = new();
    public SuccessRate SuccessRate { get; set; } = new();
    public Language Language { get; set; } = new();

    public SecretCode? GenerateSecretCode()
    {
        if (Item.HexId is not ushort itemId)
        {
            return null;
        }
        var passwordInput = new PasswordInput
        {
            CodeType = CodeType.Magazine,
            RecipientTown = SenderTownName.Value,
            Recipient = SenderName.Value,
            Sender = "あいうえお",
            ItemId = itemId,
            HitRate = SuccessRate.Id,
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
