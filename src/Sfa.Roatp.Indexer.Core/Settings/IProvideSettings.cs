namespace Sfa.Roatp.Indexer.Core.Settings
{
    public interface IProvideSettings
    {
        string GetSetting(string settingKey);
    }
}