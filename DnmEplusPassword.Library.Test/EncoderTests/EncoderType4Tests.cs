using DnmEplusPassword.Library.Data;

namespace DnmEplusPassword.Library.Test.EncoderTests;

[TestClass]
public sealed class EncoderType4Tests
{
    [TestMethod]
    public void TestUserFestiveFlag()
    {
        var input = new PasswordInput
        {
            CodeType = CodeType.User,
            RecipientTown = "ああああ",
            Recipient = "いいいい",
            Sender = "クーパー",
            ItemId = 0x32BC, // おまつりのはた
        };
        var password = Encoder.Encode(input, englishPasswords: false);
        Assert.AreEqual("ゆとやいどだせでねそめにめめつめ", password.Item1);
        Assert.AreEqual("のつはゆうりむいどずあとれえずか", password.Item2);
    }

    [TestMethod]
    public void TestUserSandbag()
    {
        var input = new PasswordInput
        {
            CodeType = CodeType.User,
            RecipientTown = "コーヒーまめ",
            Recipient = "クーパー",
            Sender = "ああああ",
            ItemId = 0x3394, // サンドバッグ
        };
        var password = Encoder.Encode(input, englishPasswords: false);
        Assert.AreEqual("さふぶゆろじをねぐくよぬがねみみ", password.Item1);
        Assert.AreEqual("ぐべごすねぬげをなづぞほえこひか", password.Item2);
    }
}
