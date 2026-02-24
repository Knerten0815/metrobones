let dotNetReference = null;
let audioCtx = null;
let schedulerHandle = null;
let nextBeatTime = 0;
let bpm = 120;
let beatsPerBar = 4;
let noteValue = 4;
let isRunning = false;
let currentBeat = 0;

const LOOKAHEAD_SEC = 0.1;
const SCHEDULER_INTERVAL_MS = 25;
const CLICK_FREQUENCY_HZ = 1000;
const CLICK_DURATION_SEC = 0.025;


function scheduleClick(time) {
    currentBeat = (currentBeat % beatsPerBar) + 1;

    const osc = audioCtx.createOscillator();
    const gain = audioCtx.createGain();

    osc.connect(gain);
    gain.connect(audioCtx.destination);

    osc.type = 'sine';
    if (currentBeat === 1) {
        osc.frequency.value = CLICK_FREQUENCY_HZ * 1.5;
    } else {
        osc.frequency.value = CLICK_FREQUENCY_HZ;
    }

    // Envelope: full volume at beat time, fade out to avoid a pop on cutoff.
    // Exponential ramp cannot target exactly 0.
    gain.gain.setValueAtTime(1.0, time);
    gain.gain.exponentialRampToValueAtTime(0.0001, time + CLICK_DURATION_SEC);

    osc.start(time);
    osc.stop(time + CLICK_DURATION_SEC);

    // --- visual callback scheduling ---
    const delayMs = (time - audioCtx.currentTime) * 1000;

    // Fire 30ms early to compensate for interop latency
    const visualDelayMs = Math.max(0, delayMs - 30);

    setTimeout(() => {
        if (dotNetReference) {
            dotNetReference.invokeMethodAsync('OnBeat', currentBeat);
        }
    }, visualDelayMs);
}


function scheduler() {
    const intervalSec = 60 / bpm;
    while (nextBeatTime < audioCtx.currentTime + LOOKAHEAD_SEC) {
        scheduleClick(nextBeatTime);
        nextBeatTime += intervalSec; // Advance from last scheduled time, not from now — prevents drift
    }
}


function start(newBpm, bpb, noteVal) {
    if (isRunning) return;

    bpm = newBpm || 120;
    beatsPerBar = bpb || 4;
    noteValue = noteVal || 4;

    if (!audioCtx) {
        audioCtx = new AudioContext();
    }

    // AudioContext starts suspended on iOS/Android until activated by a user gesture.
    // Since start() is always called from a button tap, resume() is safe here.
    if (audioCtx.state === 'suspended') {
        audioCtx.resume();
    }

    // Small offset so the first beat isn't scheduled in the past
    // by the time the audio engine processes it.
    nextBeatTime = audioCtx.currentTime + 0.05;

    schedulerHandle = setInterval(scheduler, SCHEDULER_INTERVAL_MS);
    isRunning = true;
}


function stop() {
    if (!isRunning) return;
    clearInterval(schedulerHandle);
    schedulerHandle = null;
    isRunning = false;
    currentBeat = 0;
}


function setBpm(newBpm, bpb, noteVal) {
    bpm = newBpm;
    beatsPerBar = bpb;
    noteValue = noteVal;
}


function getIsRunning() {
    return isRunning;
}


function resumeAudio() {
    if (audioCtx?.state === 'suspended') {
        audioCtx.resume();
    }
}

function setDotNetReference(ref) {
    dotNetReference = ref;
}

// Expose public API to Blazor's string-based JS interop
globalThis.metronome = { start, stop, setBpm, getIsRunning, setDotNetReference };