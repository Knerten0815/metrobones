using Microsoft.AspNetCore.Components;
using Metrobones.Services;

namespace Metrobones.Layout;

public abstract class MetronomePageBase : ComponentBase, IAsyncDisposable
{
    [Inject]
    protected Metronome Met { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if (Met.IsRunning)
            await Met.Stop();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Met.BeatCallback += OnStateChanged;
            Met.StopCallback += OnStateChanged;
            await Met.Initialize();
        }
    }

    protected virtual async void OnStateChanged()
    {
        await InvokeAsync(StateHasChanged);
    }

    public virtual async ValueTask DisposeAsync()
    {
        Met.StopCallback -= OnStateChanged;
        Met.BeatCallback -= OnStateChanged;
        await Met.Stop();
    }
}
