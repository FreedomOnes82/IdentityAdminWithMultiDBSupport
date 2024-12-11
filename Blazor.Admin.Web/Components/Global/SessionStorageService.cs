using Microsoft.JSInterop;
using System.Text.Json;

namespace Blazor.Admin.Web.Components.Global
{
    public class SessionStorageService
    {
        private readonly IJSRuntime _jsRuntime;

        public SessionStorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task SetItemAsync<T>(string key, T value)
        {
            string serializedValue = JsonSerializer.Serialize(value);
            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", key, serializedValue);
        }

        public async Task<T> GetItemAsync<T>(string key)
        {
            string serializedValue = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", key);
            return serializedValue != null ? JsonSerializer.Deserialize<T>(serializedValue) : default(T);
        }

        public async Task RemoveItemAsync(string key)
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", key);
        }
    }
}
