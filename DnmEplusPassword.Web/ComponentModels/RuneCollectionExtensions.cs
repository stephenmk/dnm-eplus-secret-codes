using System.Text;

namespace DnmEplusPassword.Web.ComponentModels;

public static class RuneCollectionExtensions
{
    public static string FastToString(this IEnumerable<Rune> runes)
    {
        int length = 0;
        foreach (var rune in runes)
        {
            length += rune.Utf16SequenceLength;
        }
        return string.Create(length, state: runes, static (output, state) =>
        {
            int charsWritten = 0;
            foreach (var rune in state)
            {
                charsWritten += rune.EncodeToUtf16(output[charsWritten..]);
            }
        });
    }
}
