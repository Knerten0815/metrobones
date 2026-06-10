let dotNetReference = null;
let mediaSessionAudio = null;
let audioCtx = null;
let schedulerHandle = null;
let nextBeatTime = 0;
let tempo = 120;        // I avoid naming it BPM, because Beats Per Minute is misleading. As BPM is actually quarter notes per minute. See clicksPerSecond()
let beatsPerBar = 4;
let noteValue = 4;
let isRunning = false;
let currentBeat = 0;
let beatAccents = [1, 0, 0, 0];
let subdivisions = -1;
// agogics:
let startTempo = -1;
let endTempo = -1;
let beatCount = -1;
let agogicCurrentBeat = 0;

// sound
let volume = 1.0;
let onbeat_freq = 1500;
let offbeat_freq = 1000;
let waveform = "sine";

const LOOKAHEAD_SEC = 0.1;
const SCHEDULER_INTERVAL_MS = 25;
const CLICK_DURATION_SEC = 0.025;


function beatLengthInSeconds() {
    let beatLength = 60 / (tempo * (noteValue / 4));  // Base interval calculation

    // Subdivisions:
    if (subdivisions > 0) {
        beatLength = (beatLength * beatsPerBar) / subdivisions;
        beatCount = (beatCount / beatsPerBar) * subdivisions;   // Adjust for agogics
    }

    // Agogics:
    if(startTempo > 0 && endTempo > 0 && beatCount > 0) {
        beatLength = CalculateAgogicBeatLength();
        agogicCurrentBeat++;

        // Stop agogic scheduling after the last beat
        if(agogicCurrentBeat >= beatCount) {
            startTempo = -1;
            endTempo = -1;
            beatCount = -1;
            agogicCurrentBeat = 0;
        }
    }

    return beatLength;
}


function CalculateAgogicBeatLength() {
    const scale = noteValue / 4;
    const scaledStart = startTempo * scale;
    const scaledEnd = endTempo * scale;
    const k = 60 * beatCount / (scaledEnd - scaledStart);
    tempo = scaledStart + (scaledEnd - scaledStart) * agogicCurrentBeat / beatCount;
    const nextTempo   = scaledStart + (scaledEnd - scaledStart) * (agogicCurrentBeat + 1) / beatCount;
    return k * Math.log(nextTempo / tempo);
}


function scheduleClick(time) {
    if (subdivisions > 0) {
        currentBeat = (currentBeat % subdivisions) + 1;
    }
    else {
        currentBeat = (currentBeat % beatsPerBar) + 1;
    }

    if (beatAccents[currentBeat - 1] != 2) {
        createClickOscillator(beatAccents[currentBeat - 1] === 1, time);
    }

    // --- visual callback scheduling ---
    const delayMs = (time - audioCtx.currentTime) * 1000;

    // Fire 30ms early to compensate for interop latency
    const visualDelayMs = Math.max(0, delayMs - 30);

    setTimeout(() => {
        if (dotNetReference) {
            dotNetReference.invokeMethodAsync('OnBeat', currentBeat, tempo);
        }
    }, visualDelayMs);
}


function createClickOscillator(isAccented, time) {
    const osc = audioCtx.createOscillator();
    const gain = audioCtx.createGain();

    osc.connect(gain);
    gain.connect(audioCtx.destination);

    osc.type = waveform;
    if (isAccented) {
        osc.frequency.value = onbeat_freq;
    } else {
        osc.frequency.value = offbeat_freq;
    }

    // Envelope: full volume at beat time, fade out to avoid a pop on cutoff.
    // Exponential ramp cannot target exactly 0.
    gain.gain.setValueAtTime(volume, time);
    gain.gain.exponentialRampToValueAtTime(0.0001, time + CLICK_DURATION_SEC);

    osc.start(time);
    osc.stop(time + CLICK_DURATION_SEC);
}


function scheduler() {
    while (nextBeatTime < audioCtx.currentTime + LOOKAHEAD_SEC) {
        scheduleClick(nextBeatTime);
        nextBeatTime += beatLengthInSeconds(); // Advance from last scheduled time, not from now — prevents drift
    }
}


function start() {
    if (isRunning) return;

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
    nextBeatTime = audioCtx.currentTime + 0.15;

    schedulerHandle = setInterval(scheduler, SCHEDULER_INTERVAL_MS);
    startMediaSession();
    isRunning = true;
}


function stop() {
    if (!isRunning) return;
    clearInterval(schedulerHandle);
    schedulerHandle = null;
    currentBeat = 0;
    stopMediaSession();
    isRunning = false;
}


function setBpm(newTempo, bpb, noteVal, newBeatAccents, newSubdivisions=-1, resetBeat = false, agogicEndTempo = -1, agogicBeatCount = -1) {
    tempo = newTempo;
    beatsPerBar = bpb;
    subdivisions = newSubdivisions;
    noteValue = noteVal;
    beatAccents = newBeatAccents;
    startTempo = newTempo;
    endTempo = agogicEndTempo;
    beatCount = agogicBeatCount;
    agogicCurrentBeat = 0;

    if (resetBeat) 
    {
        currentBeat = 0;
    }
}


function setClickSound(vol, sound, onbeatFrequency, offbeatFrequency) {
    volume = vol;
    waveform = sound;
    onbeat_freq = onbeatFrequency;
    offbeat_freq = offbeatFrequency;
}


function getIsRunning() {
    return isRunning;
}


function resumeAudio() {
    if (audioCtx?.state === 'suspended') {
        audioCtx.resume();
    }
}


function initialize(ref) {
    dotNetReference = ref;

    mediaSessionAudio = new Audio('silent.mp3');
    mediaSessionAudio.loop = true;

    if (!('mediaSession' in navigator)) return;

    navigator.mediaSession.setPositionState({
        duration: Infinity
    });

    navigator.mediaSession.setActionHandler('play', () => {
        dotNetReference.invokeMethodAsync('OnMediaSessionPlay');
    });
    navigator.mediaSession.setActionHandler('pause', () => {
        dotNetReference.invokeMethodAsync('OnMediaSessionStop');
    });
    navigator.mediaSession.setActionHandler('stop', () => {
        dotNetReference.invokeMethodAsync('OnMediaSessionStop');
    });
    navigator.mediaSession.setActionHandler('previoustrack', null);
    navigator.mediaSession.setActionHandler('nexttrack', null);
}


function startMediaSession() {
    mediaSessionAudio?.play();

    if (!('mediaSession' in navigator)) return;

    navigator.mediaSession.metadata = new MediaMetadata({
        title: `Playing Click`,
        artist: 'Metrobones',
        artwork: [
            { src: 'favicon.png', sizes: '32x32', type: 'image/png' },
            { src: 'icon-192.png', sizes: '192x192', type: 'image/png' },
            { src: 'icon-512.png', sizes: '512x512', type: 'image/png' }
        ]
    });

    navigator.mediaSession.playbackState = 'playing';
}


function stopMediaSession() {
    mediaSessionAudio?.pause();

    if (!('mediaSession' in navigator)) return;
    navigator.mediaSession.playbackState = 'none';
}


// Expose public API to Blazor's string-based JS interop
globalThis.metronome = { start, stop, setBpm, getIsRunning, initialize, setClickSound};
