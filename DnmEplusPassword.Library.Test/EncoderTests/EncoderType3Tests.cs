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
        Assert.AreEqual("すねわとつくずよぞろれはごゆつよ", password.Item1);
        Assert.AreEqual("へずんすわじぜにのごはせはみへひ", password.Item2);
    }
}
