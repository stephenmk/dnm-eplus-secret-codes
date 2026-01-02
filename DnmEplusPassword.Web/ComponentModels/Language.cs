namespace DnmEplusPassword.Web.ComponentModels;

public sealed record Language
{
    public LanguageName Name { get; set; } = LanguageName.English;
    public bool IsEnglish => Name == LanguageName.English;
}

public enum LanguageName
{
    English,
    Japanese,
}
