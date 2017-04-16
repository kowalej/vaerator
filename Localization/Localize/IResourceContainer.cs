namespace Localization.Localize
{
    public interface IResourceContainer
    {
        bool RefreshCulture();
        string GetString(string key, Enums.TranslationResources type);
        string GetString(string key, string type);
    }
}
