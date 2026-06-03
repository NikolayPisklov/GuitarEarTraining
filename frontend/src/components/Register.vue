<template>
  <section class="mx-auto flex min-h-[calc(100vh-9rem)] w-full max-w-sm flex-col justify-center">
    <div class="mb-7 text-center">
      <h1 class="text-2xl font-semibold tracking-normal text-zinc-950">
        Регистрация
      </h1>
      <p class="mt-2 text-sm leading-6 text-zinc-600">
        Создайте аккаунт, чтобы начать тренировки.
      </p>
    </div>

    <form class="space-y-5 rounded-lg border border-zinc-200 bg-white p-6 shadow-sm" @submit.prevent>
      <div>
        <label for="email" class="mb-2 block text-sm font-medium text-zinc-800">
          Почта
        </label>
        <input
          id="email"
          v-model="email"
          type="email"
          autocomplete="email"
          required
          placeholder="you@example.com"
          class="h-11 w-full rounded-md border border-zinc-300 bg-white px-3 text-sm text-zinc-950 outline-none transition focus:border-amber-500 focus:ring-2 focus:ring-amber-200"
        />
      </div>

      <div>
        <label for="password" class="mb-2 block text-sm font-medium text-zinc-800">
          Пароль
        </label>
        <input
          id="password"
          v-model="password"
          type="password"
          autocomplete="new-password"
          required
          placeholder="Не меньше 8 символов"
          class="h-11 w-full rounded-md border border-zinc-300 bg-white px-3 text-sm text-zinc-950 outline-none transition focus:border-amber-500 focus:ring-2 focus:ring-amber-200"
        />
      </div>

      <button
        type="submit"
        class="h-11 w-full rounded-md bg-zinc-950 px-4 text-sm font-semibold text-white transition hover:bg-zinc-800 focus:outline-none focus:ring-2 focus:ring-zinc-400 focus:ring-offset-2"
      >
        Зарегистрироваться
      </button>
    </form>
  </section>
</template>

<script setup>
import { ref } from 'vue'

const email = ref('')
const password = ref('')
</script>
