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
            Sender = "1", // Price
            ItemId = (ushort)Monument.Egg,
            RowAcre = 2,
            ColAcre = 4,
        };
        var password = Encoder.Encode(input, englishPasswords: false);
        Assert.AreEqual("すまぜんふねちそそこずぎやむぐら", password.Item1);
        Assert.AreEqual("みなまえざほにぎごべぢぜびけもじ", password.Item2);
    }

    [TestMethod]
    public void TestMonumentStonehenge()
    {
        var input = new PasswordInput
        {
            CodeType = CodeType.Monument,
            RecipientTown = "コーヒーまめ",
            Recipient = "クーパー",
            Sender = "1234", // Price
            ItemId = (ushort)Monument.Stonehenge,
            RowAcre = 3,
            ColAcre = 3,
        };
        var password = Encoder.Encode(input, englishPasswords: false);
        Assert.AreEqual("ゆれびのほそかぜれくごびしおげあ", password.Item1);
        Assert.AreEqual("いぎきほしみやほようんほとづぶか", password.Item2);
    }
}
