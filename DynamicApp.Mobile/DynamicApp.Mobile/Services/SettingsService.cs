using DynamicApp.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace DynamicApp.Mobile.Services
{
    public class SettingsService
        : ISettings
    {
        public async Task AddOrUpdateValue(string key, string value)
        {
            await Task.Run(() => Preferences.Set(key, value));
        }

        public async Task Clear()
        {
            await Task.Run(() => Preferences.Clear());
        }

        public async Task ClearClaims()
        {
            await Clear();
        }

        public string GetBaseUrl()
        {
            throw new NotImplementedException();
        }

        public Task<Claims> GetClaims()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetIdentifier()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetValue(string key)
        {
            return Task.FromResult(Preferences.Get(key, null));
        }

        public async Task Remove(string key)
        {
            await Task.Run(() => Preferences.Remove(key));
        }

        public Task SetClaims(Claims claims)
        {
            throw new NotImplementedException();
        }
    }
}
