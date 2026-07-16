<template>
  <section class="mx-auto flex min-h-[calc(100vh-9rem)] w-full max-w-3xl flex-col justify-center">
    <div class="mb-8 text-center">
      <h1 class="text-2xl font-semibold tracking-normal text-zinc-950">
        Тренировка бэндов
      </h1>
      <p class="mt-3 text-sm leading-6 text-zinc-600">
        С помощью этого упражнения можно натренировать свой слух на попадание в бэнды
      </p>
    </div>

    <fieldset class="mx-auto mb-6 w-full max-w-md">
      <legend class="mb-2 text-sm font-medium text-zinc-950">
        Интервал бенда
      </legend>
      <div class="flex gap-6">
        <label class="flex cursor-pointer items-center gap-2 text-sm text-zinc-950">
          <input
            v-model="bendInterval"
            type="radio"
            value="tone"
            class="h-4 w-4 accent-amber-500"
          >
          Бенд на тон
        </label>
        <label class="flex cursor-pointer items-center gap-2 text-sm text-zinc-950">
          <input
            v-model="bendInterval"
            type="radio"
            value="semitone"
            class="h-4 w-4 accent-amber-500"
          >
          Бенд на пол тона
        </label>
      </div>
    </fieldset>

    <div class="mx-auto mb-4 w-full max-w-md">
      <label for="audio-input" class="mb-1.5 block text-sm font-medium text-zinc-950">
        Устройство записи
      </label>
      <select
        id="audio-input"
        v-model="selectedAudioInputId"
        class="block h-11 w-full rounded-md border border-zinc-300 bg-white px-3 text-sm text-zinc-950 shadow-sm focus:border-amber-500 focus:outline-none focus:ring-2 focus:ring-amber-300 disabled:cursor-not-allowed disabled:bg-zinc-100 disabled:text-zinc-500"
        :disabled="isLoadingAudioInputs || isRecording || audioInputDevices.length === 0"
      >
        <option v-if="isLoadingAudioInputs" value="">
          Ищем устройства…
        </option>
        <option v-else-if="audioInputDevices.length === 0" value="">
          Устройства не найдены
        </option>
        <option
          v-for="(device, index) in audioInputDevices"
          :key="device.deviceId"
          :value="device.deviceId"
        >
          {{ device.label || `Устройство записи ${index + 1}` }}
        </option>
      </select>
      <p v-if="audioDevicesError" class="mt-2 text-sm text-red-600" role="alert">
        {{ audioDevicesError }}
      </p>
    </div>

    <div class="flex justify-center">
      <button
        type="button"
        class="inline-flex h-11 items-center rounded-md border border-zinc-300 bg-white px-5 text-sm font-semibold text-zinc-950 shadow-sm transition hover:border-zinc-400 hover:bg-zinc-50 focus:outline-none focus:ring-2 focus:ring-amber-300"
        :disabled="!selectedAudioInputId || isLoadingAudioInputs"
        @click="onStartRecordingButtonClick"
      >
        {{ isRecording ? 'Остановить запись' : 'Начать запись' }}
      </button>
    </div>
  </section>
</template>

<script setup>
import { onBeforeUnmount, onMounted, ref } from 'vue'
import { submitHalfStepBend, submitWholeStepBend } from '../services/bends'

const SILENCE_THRESHOLD = 0.005
const SILENCE_FRAME_DURATION_MS = 20
const SILENCE_PADDING_MS = 100

const audioInputDevices = ref([])
const selectedAudioInputId = ref('')
const bendInterval = ref('tone')
const isLoadingAudioInputs = ref(false)
const isRecording = ref(false)
const audioDevicesError = ref('')
const recordedSamples = ref(new Float32Array())
const recordedSampleRate = ref(null)

let activeStream = null
let audioContext = null
let mediaStreamSource = null
let recorderNode = null
let silentGainNode = null
let recordedChunks = []
let resolveRecorderFlush = null

async function refreshAudioInputs() {
  if (!navigator.mediaDevices?.enumerateDevices) {
    audioDevicesError.value = 'Ваш браузер не поддерживает выбор устройства записи.'
    return
  }

  const devices = await navigator.mediaDevices.enumerateDevices()
  audioInputDevices.value = devices.filter(device => device.kind === 'audioinput')

  const selectedDeviceStillExists = audioInputDevices.value.some(
    device => device.deviceId === selectedAudioInputId.value,
  )

  if (!selectedDeviceStillExists) {
    selectedAudioInputId.value = audioInputDevices.value[0]?.deviceId ?? ''
  }
}

async function loadAudioInputs() {
  if (!navigator.mediaDevices?.getUserMedia) {
    audioDevicesError.value = 'Ваш браузер не поддерживает доступ к устройствам записи.'
    return
  }

  isLoadingAudioInputs.value = true
  audioDevicesError.value = ''
  let permissionStream = null

  try {
    // После выдачи разрешения браузер раскрывает названия всех доступных входов.
    permissionStream = await navigator.mediaDevices.getUserMedia({ audio: true })
    await refreshAudioInputs()
  } catch (error) {
    audioDevicesError.value = error?.name === 'NotAllowedError'
      ? 'Разрешите доступ к микрофону, чтобы выбрать устройство записи.'
      : 'Не удалось получить список устройств записи.'
  } finally {
    permissionStream?.getTracks().forEach(track => track.stop())
    isLoadingAudioInputs.value = false
  }
}

function mergeChunks(chunks) {
  const samplesCount = chunks.reduce((total, chunk) => total + chunk.length, 0)
  const samples = new Float32Array(samplesCount)
  let offset = 0

  for (const chunk of chunks) {
    samples.set(chunk, offset)
    offset += chunk.length
  }

  return samples
}

function trimEdgeSilence(samples, sampleRate) {
  const frameSize = Math.max(1, Math.round(sampleRate * SILENCE_FRAME_DURATION_MS / 1000))
  const frameCount = Math.ceil(samples.length / frameSize)
  const paddingFrames = Math.ceil(SILENCE_PADDING_MS / SILENCE_FRAME_DURATION_MS)
  let firstSoundFrame = -1
  let lastSoundFrame = -1

  for (let frameIndex = 0; frameIndex < frameCount; frameIndex += 1) {
    const start = frameIndex * frameSize
    const end = Math.min(start + frameSize, samples.length)
    let sumOfSquares = 0

    for (let sampleIndex = start; sampleIndex < end; sampleIndex += 1) {
      sumOfSquares += samples[sampleIndex] ** 2
    }

    const rms = Math.sqrt(sumOfSquares / (end - start))

    if (rms >= SILENCE_THRESHOLD) {
      if (firstSoundFrame === -1) {
        firstSoundFrame = frameIndex
      }
      lastSoundFrame = frameIndex
    }
  }

  if (firstSoundFrame === -1) {
    return new Float32Array()
  }

  // Внутренние паузы сохраняем, обрезаем только края и оставляем запас перед первой нотой.
  const firstFrameToKeep = Math.max(0, firstSoundFrame - paddingFrames)
  const lastFrameToKeep = Math.min(frameCount - 1, lastSoundFrame + paddingFrames)
  const start = firstFrameToKeep * frameSize
  const end = Math.min(samples.length, (lastFrameToKeep + 1) * frameSize)

  return samples.slice(start, end)
}

async function flushRecorder() {
  if (!recorderNode) return

  await new Promise((resolve) => {
    let timeoutId = null
    const finishFlush = () => {
      clearTimeout(timeoutId)
      resolveRecorderFlush = null
      resolve()
    }

    resolveRecorderFlush = finishFlush
    timeoutId = window.setTimeout(finishFlush, 250)
    recorderNode.port.postMessage({ type: 'flush' })
  })
}

async function stopRecording(prepareSamples = true) {
  mediaStreamSource?.disconnect()

  if (prepareSamples) {
    await flushRecorder()
  }

  recorderNode?.disconnect()
  silentGainNode?.disconnect()
  activeStream?.getTracks().forEach(track => track.stop())

  const sampleRate = audioContext?.sampleRate ?? recordedSampleRate.value

  if (audioContext) {
    await audioContext.close()
  }

  activeStream = null
  audioContext = null
  mediaStreamSource = null
  recorderNode = null
  silentGainNode = null
  isRecording.value = false

  if (!prepareSamples) {
    recordedChunks = []
    return
  }

  const allSamples = mergeChunks(recordedChunks)
  recordedSamples.value = trimEdgeSilence(allSamples, sampleRate)
  recordedSampleRate.value = sampleRate
  recordedChunks = []
  if(bendInterval.value === 'semitone'){
    const proccessResult = await submitHalfStepBend(recordedSamples.value, recordedSampleRate.value)
  }
  else{
    const proccessResult = await submitWholeStepBend(recordedSamples.value, recordedSampleRate.value)
  }
}

async function onStartRecordingButtonClick() {
  if (isRecording.value) {
    await stopRecording()
    return
  }

  if (!selectedAudioInputId.value) return

  audioDevicesError.value = ''
  recordedSamples.value = new Float32Array()
  recordedChunks = []

  try {
    activeStream = await navigator.mediaDevices.getUserMedia({
      audio: {
        deviceId: { exact: selectedAudioInputId.value },
        channelCount: 1,
        echoCancellation: false,
        noiseSuppression: false,
        autoGainControl: false,
      },
    })

    const AudioContext = window.AudioContext || window.webkitAudioContext
    audioContext = new AudioContext()
    recordedSampleRate.value = audioContext.sampleRate
    await audioContext.audioWorklet.addModule(
      new URL('../audio/sample-recorder.worklet.js', import.meta.url),
    )

    mediaStreamSource = audioContext.createMediaStreamSource(activeStream)
    recorderNode = new AudioWorkletNode(audioContext, 'sample-recorder', {
      numberOfInputs: 1,
      numberOfOutputs: 1,
      outputChannelCount: [1],
    })
    silentGainNode = audioContext.createGain()
    silentGainNode.gain.value = 0

    recorderNode.port.onmessage = (event) => {
      if (event.data?.type === 'samples') {
        recordedChunks.push(event.data.samples)
      } else if (event.data?.type === 'flushed') {
        resolveRecorderFlush?.()
      }
    }

    mediaStreamSource
      .connect(recorderNode)
      .connect(silentGainNode)
      .connect(audioContext.destination)

    await audioContext.resume()
    isRecording.value = true
  } catch (error) {
    await stopRecording(false)
    audioDevicesError.value = 'Не удалось начать запись с выбранного устройства.'
  }
}

onMounted(() => {
  loadAudioInputs()
  navigator.mediaDevices?.addEventListener?.('devicechange', refreshAudioInputs)
})

onBeforeUnmount(() => {
  navigator.mediaDevices?.removeEventListener?.('devicechange', refreshAudioInputs)
  stopRecording(false)
})
</script>
