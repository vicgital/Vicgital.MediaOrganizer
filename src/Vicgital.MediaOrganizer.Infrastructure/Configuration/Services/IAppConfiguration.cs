namespace Vicgital.MediaOrganizer.Infrastructure.Configuration.Services
{
    public interface IAppConfiguration
    {
        string GetValue(string key);
        string GetValue(string key, string defaultValue);
        int GetValue(string key, int defaultValue);
    }
}
