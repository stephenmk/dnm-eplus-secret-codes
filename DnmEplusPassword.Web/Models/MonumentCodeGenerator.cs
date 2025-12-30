using DnmEplusPassword.Library;

namespace DnmEplusPassword.Web.Models;

[ValidatableType]
public sealed class MonumentCodeGenerator
{
    public Recipient Recipient { get; set; } = new();
    public Decoration Decoration { get; set; } = new();
    public Price Price { get; set; } = new();
    public PlacementAcre PlacementAcre { get; set; } = new();
    public Language Language { get; set; } = new();

    public SecretCode? GenerateSecretCode()
    {
        var passwordInput = new PasswordInput
        {
            CodeType = CodeType.Monument,
            RecipientTown = Recipient.TownName,
            Recipient = Recipient.Name,
            Sender = Price.Value.ToString(),
            ItemId = (ushort)(Decoration.Id % 15),
            RowAcre = PlacementAcre.Row,
            ColAcre = PlacementAcre.Col,
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
