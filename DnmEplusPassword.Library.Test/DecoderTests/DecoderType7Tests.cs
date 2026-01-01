using DnmEplusPassword.Library.Data;

namespace DnmEplusPassword.Library.Test.DecoderTests;

[TestClass]
public sealed class DecoderType7Tests
{
    [TestMethod]
    public void TestMonumentEgg()
    {
        var password = "すまぜんふねちそそこずぎやむぐらみなまえざほにぎごべぢぜびけもじ";
        var input = Decoder.Decode(password, englishPasswords: false);

        Assert.AreEqual(CodeType.Monument, input.CodeType);
        Assert.AreEqual("コーヒーまめ", input.RecipientTown.ToString());
        Assert.AreEqual("クーパー", input.Recipient.ToString());
        Assert.AreEqual(1, input.Price);
        Assert.AreEqual(Monument.Egg, input.Monument);
        Assert.AreEqual(2, input.RowAcre);
        Assert.AreEqual(4, input.ColAcre);
    }
}
