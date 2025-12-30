namespace DnmEplusPassword.Library.Test;

[TestClass]
public sealed class EncoderTests
{
    [TestMethod]
    public void TestEggMonument()
    {
        var input = new PasswordInput
        {
            CodeType = CodeType.Monument,
            RecipientTown = "コーヒーまめ",
            Recipient = "クーパー",
            Sender = "1", // Price
            ItemId = 8, // Egg
            RowAcre = 2,
            ColAcre = 4,
        };
        var password = Encoder.Encode(input, englishPasswords: false);
        Assert.AreEqual("すまぜんふねちそそこずぎやむぐら", password.Item1);
        Assert.AreEqual("みなまえざほにぎごべぢぜびけもじ", password.Item2);
    }
}
