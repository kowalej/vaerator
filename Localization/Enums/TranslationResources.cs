namespace Localization.Enums
{
    /// <summary>
    /// ****** AN ENUM MUST BE ADDED FOR EACH RESX FILE FOR IT TO GET PICKED UP ******
    /// Each enum description should reflect corresponding .resx file name.
    /// </summary>
    public enum TranslationResources {[EnumDescription("SharedResources")] SHARED,
                                        [EnumDescription("ErrorResources")] ERRORS,
                                        [EnumDescription("MainMenuResources")] MAINMENU
    };
}
