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
            Price = 1,
            Monument = Monument.Egg,
            RowAcre = 2,
            ColAcre = 4,
        };
        var password = Encoder.Encode(input, englishPasswords: false);
        Assert.AreEqual("すまぜんふねちそそこずぎやむぐら", password.Line1);
        Assert.AreEqual("みなまえざほにぎごべぢぜびけもじ", password.Line2);
    }

    [TestMethod]
    public void TestMonumentStonehenge()
    {
        var input = new PasswordInput
        {
            CodeType = CodeType.Monument,
            RecipientTown = "コーヒーまめ",
            Recipient = "クーパー",
            Price = 1234,
            Monument = Monument.Stonehenge,
            RowAcre = 3,
            ColAcre = 3,
        };
        var password = Encoder.Encode(input, englishPasswords: false);
        Assert.AreEqual("ゆれびのほそかぜれくごびしおげあ", password.Line1);
        Assert.AreEqual("いぎきほしみやほようんほとづぶか", password.Line2);
    }
}
