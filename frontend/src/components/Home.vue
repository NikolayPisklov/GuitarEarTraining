<template>
  <section class="mx-auto flex min-h-[calc(100vh-9rem)] w-full max-w-3xl flex-col justify-center">
    <div class="mb-8 text-center">
      <h1 class="text-2xl font-semibold tracking-normal text-zinc-950">
        Выберите тренировку
      </h1>
      <p class="mt-2 text-sm leading-6 text-zinc-600">
        Мок главной страницы для зарегистрированного пользователя.
      </p>
    </div>

    <div class="grid gap-4 sm:grid-cols-2">
      <template v-for="item in trainingItems" :key="item.label">
        <RouterLink
          v-if="item.to"
          :to="item.to"
          class="flex min-h-32 items-center justify-center rounded-lg border border-zinc-200 bg-white p-5 text-center text-base font-semibold text-zinc-950 shadow-sm transition hover:border-amber-500 hover:bg-amber-50 focus:outline-none focus:ring-2 focus:ring-amber-300"
        >
          {{ item.label }}
        </RouterLink>

        <button
          v-else
          type="button"
          class="flex min-h-32 items-center justify-center rounded-lg border border-zinc-200 bg-white p-5 text-center text-base font-semibold text-zinc-950 shadow-sm transition hover:border-amber-500 hover:bg-amber-50 focus:outline-none focus:ring-2 focus:ring-amber-300"
        >
          {{ item.label }}
        </button>
      </template>
    </div>
  </section>
</template>

<script setup>
const trainingItems = [
  {
    label: 'Тренировка питча',
    to: '/pitch-training',
  },
  {
    label: 'Бэнды',
  },
  {
    label: 'Рассинхрон',
  },
  {
    label: 'Подбор на слух',
  },
]
</script>
