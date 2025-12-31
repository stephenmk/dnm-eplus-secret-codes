using DnmEplusPassword.Library;

namespace DnmEplusPassword.Web.Models;

[ValidatableType]
public sealed class MagazineCodeGenerator
{
    public Name MagazineName { get; set; } = new() { MaxLength = 12 };
    public Item Item { get; set; } = new() { Type = Item.ItemType.Universal };
    public SuccessRate SuccessRate { get; set; } = new();
    public Language Language { get; set; } = new();

    public SecretCode? GenerateSecretCode()
    {
        if (Item.HexId is not ushort itemId)
        {
            return null;
        }

        var nameRunes = MagazineName.Value.EnumerateRunes();
        var name1 = nameRunes.Take(6).FastToString();
        var name2 = nameRunes.Skip(6).FastToString();

        var passwordInput = new PasswordInput
        {
            CodeType = CodeType.Magazine,
            RecipientTown = name1,
            Recipient = name2,
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
