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
        return Library.Decoder.Decode(output, englishPasswords: false);
    }
}
