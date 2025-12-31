namespace DnmEplusPassword.Library.Test;

[TestClass]
public sealed class EncoderTests
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
