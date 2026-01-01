using DnmEplusPassword.Library.Data;

namespace DnmEplusPassword.Library.Test.EncoderTests;

[TestClass]
public sealed class EncoderType3Tests
{
    [TestMethod]
    public void TestMagazineWarioWoods()
    {
        var input = new PasswordInput
        {
            CodeType = CodeType.Magazine,
            RecipientTown = "にんてんどう",
            Recipient = "ニンテンドウ",
            Sender = "あいうえお",
            ItemId = 0x1E20, // ワリオのもり
            HitRate = HitRate.OneHundredPercent,
        };
        var password = Encoder.Encode(input, englishPasswords: false);
        Assert.AreEqual("すねわとつくずよぞろれはごゆつよ", password.Line1);
        Assert.AreEqual("へずんすわじぜにのごはせはみへひ", password.Line2);
    }

    [TestMethod]
    public void TestMagazineFroggyChair()
    {
        var input = new PasswordInput
        {
            CodeType = CodeType.Magazine,
            RecipientTown = "Stephe",
            Recipient = "nKraus",
            Sender = string.Empty,
            ItemId = 0x10A4, // カエルのチェア
            HitRate = HitRate.OneHundredPercent,
        };
        var password = Encoder.Encode(input, englishPasswords: false);
        Assert.AreEqual("んじそちべかづぞづせえさかでせる", password.Line1);
        Assert.AreEqual("やぎさけろひむあぢめこれえぢとお", password.Line2);
    }
}
