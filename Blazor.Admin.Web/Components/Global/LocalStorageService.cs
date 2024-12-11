using Microsoft.JSInterop;
using System.Text.Json;

namespace Blazor.Admin.Web.Components.Global
{
    public class LocalStorageService
    {
        private readonly IJSRuntime _jsRuntime;

        public LocalStorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task SetItemAsync<T>(string key, T value)
        {
            string serializedValue = JsonSerializer.Serialize(value);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, serializedValue);
        }

        public async Task<T> GetItemAsync<T>(string key)
        {
            string serializedValue = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
            return serializedValue != null ? JsonSerializer.Deserialize<T>(serializedValue) : default(T);
        }

        public async Task RemoveItemAsync(string key)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }
    }
}
