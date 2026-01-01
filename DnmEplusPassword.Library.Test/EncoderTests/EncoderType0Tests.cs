using DnmEplusPassword.Library.Data;

namespace DnmEplusPassword.Library.Test.EncoderTests;

[TestClass]
public sealed class EncoderType0Tests
{
    [TestMethod]
    public void TestFamicomPunchout()
    {
        var input = new PasswordInput
        {
            CodeType = CodeType.Famicom,
            RecipientTown = "コーヒーまめ",
            Recipient = "クーパー",
            Sender = "にんてんどう",
            ItemId = 0x1E04, // パンチアウト
        };
        var password = Encoder.Encode(input, englishPasswords: false);
        Assert.AreEqual("ぎいのわれのかぬてもさめとさづで", password.Line1);
        Assert.AreEqual("とことうぶいぶけらぐぶをたりろね", password.Line2);
    }

    [TestMethod]
    public void TestFamicomCluCluLand()
    {
        var input = new PasswordInput
        {
            CodeType = CodeType.Famicom,
            RecipientTown = "コーヒーまめ",
            Recipient = "クーパー",
            Sender = string.Empty,
            ItemId = 0x1E0C, // クルクルランドＤ
        };
        var password = Encoder.Encode(input, englishPasswords: false);
        Assert.AreEqual("ぶななむううわすはとうそらりぎぐ", password.Line1);
        Assert.AreEqual("くめぶびべらろそむはたぶぐこださ", password.Line2);
    }
}
