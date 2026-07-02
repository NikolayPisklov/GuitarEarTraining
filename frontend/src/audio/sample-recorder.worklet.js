const BUFFER_SIZE = 4096

class SampleRecorderProcessor extends AudioWorkletProcessor {
  constructor() {
    super()
    this.buffer = new Float32Array(BUFFER_SIZE)
    this.offset = 0

    this.port.onmessage = (event) => {
      if (event.data?.type === 'flush') {
        this.flush()
        this.port.postMessage({ type: 'flushed' })
      }
    }
  }

  flush() {
    if (this.offset === 0) return

    const samples = this.buffer.slice(0, this.offset)
    this.port.postMessage({ type: 'samples', samples }, [samples.buffer])
    this.offset = 0
  }

  process(inputs, outputs) {
    const input = inputs[0]?.[0]

    if (input) {
      let inputOffset = 0

      while (inputOffset < input.length) {
        const samplesToCopy = Math.min(
          input.length - inputOffset,
          this.buffer.length - this.offset,
        )

        this.buffer.set(input.subarray(inputOffset, inputOffset + samplesToCopy), this.offset)
        this.offset += samplesToCopy
        inputOffset += samplesToCopy

        if (this.offset === this.buffer.length) {
          this.flush()
        }
      }
    }

    // Узел подключён к выходу только для стабильной работы аудиографа — звук не воспроизводим.
    for (const output of outputs[0] ?? []) {
      output.fill(0)
    }

    return true
  }
}

registerProcessor('sample-recorder', SampleRecorderProcessor)
