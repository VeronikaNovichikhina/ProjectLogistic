using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Presentation.Shared;

public class ClientBaseComponent : ComponentBase
{
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;

    protected bool Loading { get; set; }
    protected string ErrorMessage { get; set; } = string.Empty;
    protected string SuccessMessage { get; set; } = string.Empty;

    protected async Task HandleAsync(Func<Task> operation, string successMessage = "")
    {
        try
        {
            Loading = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
            await operation();

            if (!string.IsNullOrEmpty(successMessage))
            {
                SuccessMessage = successMessage;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка: {ex.Message}";
        }
        finally
        {
            Loading = false;
            StateHasChanged();
        }
    }

    protected void NavigateTo(string url) => Navigation.NavigateTo(url);
    protected void GoBack() => Navigation.NavigateTo("/clientList");
}
