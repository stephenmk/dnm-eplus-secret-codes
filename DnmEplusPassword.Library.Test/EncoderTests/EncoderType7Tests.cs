using DnmEplusPassword.Library.Data;

namespace DnmEplusPassword.Library.Test.EncoderTests;

[TestClass]
public sealed class EncoderType7Tests
{
    [TestMethod]
    public void TestMonumentEgg()
    {
        var input = new PasswordInput
        {
            CodeType = CodeType.Monument,
            RecipientTown = "コーヒーまめ",
            Recipient = "クーパー",
            Sender = "１", // Price
            ItemId = (ushort)Monument.Egg,
            RowAcre = 2,
            ColAcre = 4,
        };
        var password = Encoder.Encode(input, englishPasswords: false);
        Assert.AreEqual("すまぜんふねちそそこずぎやむぐら", password.Item1);
        Assert.AreEqual("みなまえざほにぎごべぢぜびけもじ", password.Item2);
    }
}
