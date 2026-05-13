const EDGE_THRESHOLD = 80;
const SCROLL_SPEED = 12;
const TICK_MS = 16;

let scrollInterval = null;
let isTouchDrag = false;

// --- Pointer (touch/mobile) ---

function onPointerMove(e) {
    e.preventDefault();
    isTouchDrag = true;
    handlePosition(e.clientY);
}

function onPointerUp() {
    stopScrolling();
    stopPointerTracking();
}

function onTouchMove(e) {
    e.preventDefault(); // blocks native scroll during drag
}

function startPointerTracking() {
    window.addEventListener('pointermove', onPointerMove);
    window.addEventListener('pointerup', onPointerUp, { once: true });
    window.addEventListener('dragover', onDragOver);
    window.addEventListener('dragend', onDragEnd, { once: true });
    // Prevent native touch scroll while drag-scrolling via JS
    window.addEventListener('touchmove', onTouchMove, { passive: false });
}

function stopPointerTracking() {
    window.removeEventListener('pointermove', onPointerMove);
    window.removeEventListener('pointerup', onPointerUp);
    window.removeEventListener('dragover', onDragOver);
    window.removeEventListener('dragend', onDragEnd);
    window.removeEventListener('touchmove', onTouchMove);
}

// --- Drag (desktop) ---

function onDragOver(e) {
    e.preventDefault(); // required or dragover won't fire continuously
    isTouchDrag = false;
    handlePosition(e.clientY);
}

function onDragEnd() {
    stopScrolling();
    stopPointerTracking();
}

// --- Shared ---

function handlePosition(y) {
    const vh = window.innerHeight;

    if (y < EDGE_THRESHOLD) {
        const speed = Math.round(SCROLL_SPEED * (1 - y / EDGE_THRESHOLD));
        startScrolling(-speed);
    } else if (y > vh - EDGE_THRESHOLD) {
        const speed = Math.round(SCROLL_SPEED * (1 - (vh - y) / EDGE_THRESHOLD));
        startScrolling(speed);
    } else {
        stopScrolling();
    }
}

function startScrolling(deltaY) {
    stopScrolling();
    scrollInterval = setInterval(function () {
        const maxScroll = document.body.scrollHeight - window.innerHeight;
        const current = window.scrollY;
        const clamped = Math.min(Math.max(current + deltaY, 0), maxScroll) - current;

        if (clamped === 0) return;

        window.scrollBy({ top: clamped, behavior: 'instant' });

        if (isTouchDrag) {
            const dragged = document.querySelector('.dragging-track-item');
            if (dragged) {
                const matrix = new DOMMatrix(getComputedStyle(dragged).transform);
                dragged.style.transform = `translate(${matrix.m41}px, ${matrix.m42 + clamped}px)`;
            }
        }
    }, TICK_MS);
}

function stopScrolling() {
    if (scrollInterval) {
        clearInterval(scrollInterval);
        scrollInterval = null;
    }
}

function dispose() {
    stopScrolling();
    stopPointerTracking();
}

globalThis.DragScroll = { startPointerTracking, dispose };