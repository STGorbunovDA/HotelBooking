using Microsoft.JSInterop;

namespace HotelBookingBlazor.Extensions
{
    public static class JsRuntimeExtensions
    {
        public static async Task AlertAsync(this IJSRuntime jSRuntime, string message) =>
            await jSRuntime.InvokeVoidAsync("window.alert", message);

        public static async Task<bool> ConfirmAsync(this IJSRuntime jSRuntime, string message) =>
            await jSRuntime.InvokeAsync<bool>("window.confirm", message);

        public static async Task<string?> PromptAsync(this IJSRuntime jSRuntime, string message) =>
            await jSRuntime.InvokeAsync<string?>("window.prompt", message);
    }
}
