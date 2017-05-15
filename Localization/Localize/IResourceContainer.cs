namespace Localization.Localize
{
    public interface IResourceContainer
    {
        bool RefreshCulture();
        string GetString(string key, Enums.TranslationResourcesFiles type);
        string GetString(string key, string type);
    }
}
