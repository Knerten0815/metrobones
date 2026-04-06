globalThis.registerServiceWorkerUpdateHandler = (dotNetRef) => {
    if ('serviceWorker' in navigator) {
        navigator.serviceWorker.ready.then(reg => {
            reg.addEventListener('updatefound', () => {
                const newWorker = reg.installing;
                newWorker.addEventListener('statechange', () => {
                    if (newWorker.state === 'installed' &&
                        navigator.serviceWorker.controller) {
                        dotNetRef.invokeMethodAsync('OnUpdateAvailable');
                    }
                });
            });
        });
    }
}

globalThis.reloadApp = () => location.reload();