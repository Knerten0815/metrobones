using Microsoft.AspNetCore.Components;
using Metrobones.Services;

namespace Metrobones.Layout;

public abstract class MetronomePageBase : ComponentBase, IAsyncDisposable
{
    [Inject]
    protected Metronome Met { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        if (Met.IsRunning)
            await Met.Stop();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Met.StopCallback += OnStateChange;
            await Met.Initialize();
        }
    }

    protected virtual async void OnStateChange()
    {
        await InvokeAsync(StateHasChanged);
    }

    public virtual async ValueTask DisposeAsync()
    {
        Met.StopCallback -= OnStateChange;
        await Met.Stop();
    }
}
