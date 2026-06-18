<template>
  <section class="mx-auto flex min-h-[calc(100vh-9rem)] w-full max-w-3xl flex-col justify-center">
    <div v-if="isExerciseStarted" class="w-full">
      <div v-if="!isResultsVisible" class="mb-8 text-center">
        <p class="text-sm leading-6 text-zinc-600">
          Отрендерите запись бенда и загружайте сюда
        </p>
      </div>

      <form v-if="!isResultsVisible" class="mx-auto flex max-w-md flex-col gap-4" @submit.prevent="onSubmit">
        <input
          type="file"
          accept="audio/*"
          class="block w-full rounded-md border border-zinc-300 bg-white px-3 py-2 text-sm text-zinc-950 shadow-sm file:mr-4 file:rounded-md file:border-0 file:bg-amber-50 file:px-4 file:py-2 file:text-sm file:font-semibold file:text-amber-700 hover:file:bg-amber-100 focus:outline-none focus:ring-2 focus:ring-amber-300"
          @change="onFileChange"
        >

        <div class="flex justify-center gap-3">
          <button
            type="button"
            class="inline-flex h-11 items-center rounded-md border border-zinc-300 bg-white px-5 text-sm font-semibold text-zinc-950 shadow-sm transition hover:border-zinc-400 hover:bg-zinc-50 focus:outline-none focus:ring-2 focus:ring-amber-300"
            @click="goBack"
          >
            Назад
          </button>

          <button
            type="submit"
            class="inline-flex h-11 items-center rounded-md border border-amber-600 bg-amber-600 px-5 text-sm font-semibold text-white shadow-sm transition hover:border-amber-700 hover:bg-amber-700 focus:outline-none focus:ring-2 focus:ring-amber-300 disabled:cursor-not-allowed disabled:border-zinc-300 disabled:bg-zinc-200 disabled:text-zinc-500 disabled:hover:border-zinc-300 disabled:hover:bg-zinc-200"
            :disabled="!selectedFile || isSubmitting"
          >
            Отправить
          </button>
        </div>
      </form>

      <div
        v-if="isResultsVisible"
        class="mx-auto flex max-w-md justify-center rounded-md border border-zinc-200 bg-white p-6 text-sm font-medium text-zinc-950 shadow-sm"
      >
        <div v-if="isSubmitting" class="flex items-center gap-3 text-zinc-700">
          <span class="h-5 w-5 animate-spin rounded-full border-2 border-zinc-300 border-t-amber-600" />
          <span>Обрабатываем ваш файл</span>
        </div>

        <div v-else class="flex flex-col items-center gap-4">
          <div>Результаты</div>

          <button
            type="button"
            class="inline-flex h-11 items-center rounded-md border border-amber-600 bg-amber-600 px-5 text-sm font-semibold text-white shadow-sm transition hover:border-amber-700 hover:bg-amber-700 focus:outline-none focus:ring-2 focus:ring-amber-300"
            @click="onUploadAnotherFile"
          >
            Загрузить ещё один файл
          </button>
        </div>
      </div>
    </div>

    <div v-else>
      <div class="mb-8 text-center">
        <h1 class="text-2xl font-semibold tracking-normal text-zinc-950">
          Тренировка бэндов
        </h1>
        <p class="mt-3 text-sm leading-6 text-zinc-600">
          С помощью этого упражнения можно натренировать свой слух на попадание в бэнды
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
    </div>
  </section>
</template>

<script setup>
import { ref } from 'vue'
import { submitBendFile } from '../services/bends'

const isExerciseStarted = ref(false)
const selectedFile = ref(null)
const isSubmitting = ref(false)
const isResultsVisible = ref(false)

function onStartExerciseButtonClick() {
  isExerciseStarted.value = true
}

function onFileChange(event) {
  selectedFile.value = event.target.files[0] ?? null
  isResultsVisible.value = false
}

async function onSubmit() {
  if (!selectedFile.value || isSubmitting.value) return

  isResultsVisible.value = true
  isSubmitting.value = true

  try{
    await submitBendFile(selectedFile.value)
  }
  catch(error){
    console.log(error.response)
  }
  finally {
    isSubmitting.value = false
  }
}

function goBack() {
  selectedFile.value = null
  isSubmitting.value = false
  isResultsVisible.value = false
  isExerciseStarted.value = false
}

function onUploadAnotherFile() {
  selectedFile.value = null
  isResultsVisible.value = false
}
</script>
