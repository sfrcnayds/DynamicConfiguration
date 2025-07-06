namespace DynamicConfiguration.Library.Interfaces
{
    public interface IConfigurationReader
    {
        T GetValue<T>(string key);
    }
}