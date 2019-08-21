using System.Threading.Tasks;

namespace DynamicApp.Client
{
    public interface ISettings
    {
        Task<string> GetValue(string key);
        Task AddOrUpdateValue(string key, string value);
        Task Remove(string key);
        string GetBaseUrl();
        Task<string> GetIdentifier();
        Task Clear();

        Task SetClaims(Claims claims);
        Task<Claims> GetClaims();
        Task ClearClaims();
    }
}
