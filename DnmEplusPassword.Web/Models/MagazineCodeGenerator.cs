using DnmEplusPassword.Library;
using DnmEplusPassword.Library.Data;

namespace DnmEplusPassword.Web.Models;

[ValidatableType]
public sealed class MagazineCodeGenerator
{
    public Name MagazineName { get; set; } = new() { MaxLength = 12 };
    public Item Item { get; set; } = new() { Type = Item.ItemType.Universal };
    public SuccessRate SuccessRate { get; set; } = new();
    public Language Language { get; set; } = new();

    public PasswordOutput GenerateSecretCode()
    {
        var nameRunes = MagazineName.Value.EnumerateRunes();
        var name1 = nameRunes.Take(6).FastToString();
        var name2 = nameRunes.Skip(6).FastToString();

        var passwordInput = new PasswordInput
        {
            CodeType = CodeType.Magazine,
            RecipientTown = name1,
            Recipient = name2,
            Sender = string.Empty,
            ItemId = Item.HexId,
            HitRate = SuccessRate.Id,
        };

        return Encoder.Encode(passwordInput, Language.IsEnglish);
    }
}
