<template>
  <section class="mx-auto w-full max-w-3xl">
    <div class="rounded-lg border border-zinc-200 bg-white p-5 shadow-sm">
      <div
        v-if="isFinished"
        class="flex min-h-40 flex-col items-center justify-center rounded-md border border-zinc-200 bg-zinc-50 px-4 text-center text-lg font-semibold text-zinc-950"
      >
        <p>Поздравляю, ты завершил упражнение</p>
        <p
          v-if="score !== null"
          :class="[
            'mt-3 text-3xl font-bold',
            score > 75 ? 'text-green-600' : 'text-red-600',
          ]"
        >
          {{ score }}%
        </p>
      </div>

      <template v-else>
        <div class="mb-5">
        <audio
          v-if="currentSample"
          :src="currentSampleSrc"
          controls
          preload="metadata"
          class="w-full"
        />

        <div
          v-else
          class="flex min-h-16 items-center justify-center rounded-md border border-dashed border-zinc-300 bg-zinc-50 px-4 text-center text-sm text-zinc-500"
        >
          Сэмплы для упражнения пока не загружены.
        </div>
      </div>

      <div class="grid gap-3 sm:grid-cols-3">
        <button
          v-for="answerOption in answerOptions"
          :key="answerOption.id"
          type="button"
          :class="getAnswerButtonClass(answerOption)"
          :disabled="isAnswerLocked"
          @click="selectAnswer(answerOption)"
        >
          {{ answerTranslations[answerOption.title] }}
        </button>
      </div>
      </template>

      <div class="mt-5 flex justify-center">
        <button
          type="button"
          class="inline-flex h-9 items-center rounded-md border border-zinc-200 bg-white px-3 text-sm font-medium text-zinc-800 transition hover:border-amber-500 hover:bg-amber-50 focus:outline-none focus:ring-2 focus:ring-amber-300"
          @click="$emit('back')"
        >
          Вернуться к упражнению
        </button>
      </div>
    </div>
  </section>
</template>

<script setup>
import { computed, ref, watch } from 'vue'
import { addExerciseAttempt, getLatestScore } from '../services/exerciseResult'

const answerTranslations = {
    higher: "Выше",
    lower: "Ниже",
    same: "Такая же"
}

const props = defineProps({
  samples: {
    type: Array,
    default: () => [],
  },
  answerOptions: {
    type: Array,
    default: () => [],
  },
  exerciseId: {
    type: Number,
    required: true,
  },
})

const emit = defineEmits(['answer-selected', 'back'])

const currentSampleIndex = ref(0)
const isFinished = ref(false)
const isAnswerLocked = ref(false)
const selectedAnswerId = ref(null)
const selectedAnswerIsCorrect = ref(false)
const userAnswers = ref([])
const score = ref(null)
const backendApiUrl = __BACKEND_API_URL__

const currentSample = computed(() => props.samples[currentSampleIndex.value])

const currentSampleSrc = computed(() => {
  const samplePath = currentSample.value?.samplePath
  if (!samplePath) {
    return ''
  }
  const normalizedPath = samplePath.replaceAll('\\', '/')
  if (normalizedPath.startsWith('http')) {
    return normalizedPath
  }
  return `${backendApiUrl.replace(/\/$/, '')}/${normalizedPath.replace(/^\//, '')}`
})

function getAnswerButtonClass(answerOption) {
  const defaultClass = 'inline-flex min-h-12 items-center justify-center rounded-md border border-zinc-200 bg-white px-4 py-2 text-center text-sm font-semibold text-zinc-950 transition hover:border-amber-500 hover:bg-amber-50 focus:outline-none focus:ring-2 focus:ring-amber-300 disabled:cursor-not-allowed'

  if (isAnswerLocked.value && selectedAnswerId.value !== answerOption.id) {
    return `${defaultClass} opacity-45 grayscale`
  }

  if (selectedAnswerId.value !== answerOption.id) {
    return defaultClass
  }

  return selectedAnswerIsCorrect.value
    ? 'inline-flex min-h-12 items-center justify-center rounded-md border border-green-600 bg-green-600 px-4 py-2 text-center text-sm font-semibold text-white transition focus:outline-none focus:ring-2 focus:ring-green-300 disabled:cursor-not-allowed'
    : 'inline-flex min-h-12 items-center justify-center rounded-md border border-red-600 bg-red-600 px-4 py-2 text-center text-sm font-semibold text-white transition focus:outline-none focus:ring-2 focus:ring-red-300 disabled:cursor-not-allowed'
}

function selectAnswer(answerOption) {
  if (isAnswerLocked.value || !currentSample.value) {
    return
  }

  const isCorrect = answerOption.id === currentSample.value.answerId

  selectedAnswerId.value = answerOption.id
  selectedAnswerIsCorrect.value = isCorrect
  isAnswerLocked.value = true
  userAnswers.value.push(isCorrect)

  emit('answer-selected', {
    answerOption,
    sample: currentSample.value,
    isCorrect,
  })

  setTimeout(showNextSample, 3000)
}

function showNextSample() {
  selectedAnswerId.value = null
  selectedAnswerIsCorrect.value = false
  isAnswerLocked.value = false

  if (currentSampleIndex.value >= props.samples.length - 1) {
    isFinished.value = true
    onExerciseAttemptEnd(props.exerciseId)
    return
  }

  currentSampleIndex.value += 1
}

async function onExerciseAttemptEnd(exerciseId) {
  try{
    await addExerciseAttempt(userAnswers.value, exerciseId) 
    score.value = await getLatestScore(exerciseId)
  }
  catch(error){
    console.log(error.response)
  }
}

watch(
  () => props.samples.length,
  (samplesCount) => {
    if (currentSampleIndex.value >= samplesCount) {
      currentSampleIndex.value = 0
    }

    isFinished.value = false
    isAnswerLocked.value = false
    selectedAnswerId.value = null
    selectedAnswerIsCorrect.value = false
    userAnswers.value = []
    score.value = null
  },
)
</script>
