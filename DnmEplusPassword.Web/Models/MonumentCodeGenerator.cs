using DnmEplusPassword.Library;
using DnmEplusPassword.Library.Data;

namespace DnmEplusPassword.Web.Models;

[ValidatableType]
public sealed class MonumentCodeGenerator
{
    public Name RecipientTownName { get; set; } = new() { MaxLength = 6 };
    public Name RecipientName { get; set; } = new() { MaxLength = 6 };
    public Decoration Decoration { get; set; } = new();
    public Price Price { get; set; } = new();
    public PlacementAcre PlacementAcre { get; set; } = new();
    public Language Language { get; set; } = new();

    public PasswordOutput GenerateSecretCode()
    {
        var passwordInput = new PasswordInput
        {
            CodeType = CodeType.Monument,
            RecipientTown = RecipientTownName.Value,
            Recipient = RecipientName.Value,
            Price = Price.Value,
            Monument = Decoration.Id,
            RowAcre = PlacementAcre.Row,
            ColAcre = PlacementAcre.Col,
        };

        return Encoder.Encode(passwordInput, Language.IsEnglish);
    }
}
