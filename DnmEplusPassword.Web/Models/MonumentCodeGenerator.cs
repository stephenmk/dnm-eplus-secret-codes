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
        try
        {
            var password = Encoder.Encode
            (
                codeType: CodeType.Monument,
                hitRateIndex: 0,
                recipientTown: Recipient.TownName.PadRight(6, ' '),
                recipient: Recipient.Name.PadRight(6, ' '),
                sender: Price.Value.ToString().PadRight(6, ' '),
                itemId: (ushort)(Decoration.Id % 15),
                extraData: ((PlacementAcre.Row & 7) << 3) | (PlacementAcre.Col & 7),
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
