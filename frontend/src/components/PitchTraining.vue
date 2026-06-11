<template>
  <SampleExercise
    v-if="isExerciseStarted"
    :exercise-id="1"
    :samples="samples"
    :answer-options="answerOptions"
    @back="isExerciseStarted = false"
  />

  <section
    v-else
    class="mx-auto flex min-h-[calc(100vh-9rem)] w-full max-w-3xl flex-col justify-center"
  >
    <div class="mb-8 text-center">
      <h1 class="text-2xl font-semibold tracking-normal text-zinc-950">
        Тренировка питча
      </h1>
      <p class="mt-3 text-sm leading-6 text-zinc-600">
        Упражнение помогает научиться различать уровень питча между двумя нотами.
        Определить вторая нота выше или ниже.
      </p>
    </div>

    <div class="flex justify-center">
      <button
        type="button"
        class="inline-flex h-11 items-center rounded-md border border-amber-600 bg-amber-600 px-5 text-sm font-semibold text-white shadow-sm transition hover:border-amber-700 hover:bg-amber-700 focus:outline-none focus:ring-2 focus:ring-amber-300"
        @click="onStartExerciseButtonClick" 
      >
        Начать упражнение
      </button>
    </div>
  </section>
</template>

<script setup>
import { ref } from 'vue'
import SampleExercise from './SampleExercise.vue'
import { getAnswerOptionsForPitch, getPitchSamples } from '../services/pitch.js'

const isExerciseStarted = ref(false)
const samples = ref([])
const answerOptions = ref([])

async function onStartExerciseButtonClick(){
  try
  {
    const samplesResponse = await getPitchSamples()
    const answerOptionsResponse = await getAnswerOptionsForPitch()
    samples.value = samplesResponse
    answerOptions.value = answerOptionsResponse
    isExerciseStarted.value = true
  }
  catch(error){
    console.log(error.response)
  }
}
</script>
