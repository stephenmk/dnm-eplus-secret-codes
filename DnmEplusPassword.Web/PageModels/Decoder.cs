using System.Text.RegularExpressions;
using DnmEplusPassword.Library.Data;
using DnmEplusPassword.Web.ComponentModels;

namespace DnmEplusPassword.Web.PageModels;

[ValidatableType]
public sealed class Decoder
{
    public Code Code { get; set; } = new();

    public PasswordInput Decode()
    {
        var output = new PasswordOutput(Code.Line1, Code.Line2);
        var regex = new Regex(@"^[あ-ん]+$");
        var japanesePassword = regex.IsMatch(Code.Line1) && regex.IsMatch(Code.Line2);
        return Library.Decoder.Decode(output, !japanesePassword);
    }
}
