const EDGE_THRESHOLD = 80;
const SCROLL_SPEED = 12;
const TICK_MS = 16;

let scrollInterval = null;
let isTouchDrag = false;
let fixElementFlag = false;

// --- Pointer (touch/mobile) ---

function onPointerMove(e) {
    e.preventDefault();
    isTouchDrag = true;
    fixDraggedElement();
    handlePosition(e.clientY);
}

function onPointerUp() {
    stopScrolling();
    stopPointerTracking();
    releaseDraggedElement();
}

function onTouchMove(e) {
    e.preventDefault(); // blocks native scroll during drag
}

function fixDraggedElement() {
    if(fixElementFlag) return;
    fixElementFlag = true;
    const dragged = document.querySelector('.dragging-track-item');
    if (!dragged) return;
    const rect = dragged.getBoundingClientRect();
    dragged.style.position = 'fixed';
    dragged.style.top = `${rect.top}px`;
    dragged.style.left = `${rect.left}px`;
    dragged.style.width = `${rect.width}px`;
    dragged.style.zIndex = '1000';
    dragged.style.transform = 'none';
}

function releaseDraggedElement() {
    fixElementFlag = false;
    const dragged = document.querySelector('.dragging-track-item');
    if (!dragged) return;
    dragged.style.position = '';
    dragged.style.top = '';
    dragged.style.left = '';
    dragged.style.width = '';
    dragged.style.zIndex = '';
    dragged.style.transform = '';
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