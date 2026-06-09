<template>
  <section class="mx-auto flex min-h-[calc(100vh-9rem)] w-full max-w-sm flex-col justify-center">
    <div class="mb-7 text-center">
      <h1 class="text-2xl font-semibold tracking-normal text-zinc-950">
        Почти готово...
      </h1>
      <p class="mt-2 text-sm leading-6 text-zinc-600">
        Введите имя и фамилию для завершения создания аккаунта.
      </p>
    </div>
    <form
      class="space-y-5 rounded-lg border border-zinc-200 bg-white p-6 shadow-sm" 
      @submit.prevent="handleSubmit"
    >
      <div>
        <label for="first-name" class="mb-2 block text-sm font-medium text-zinc-800">
          Имя
        </label>
        <input
          id="first-name"
          v-model="firstName"
          type="text"
          autocomplete="given-name"
          required
          class="h-11 w-full rounded-md border border-zinc-300 bg-white px-3 text-sm text-zinc-950 outline-none transition focus:border-amber-500 focus:ring-2 focus:ring-amber-200"
        />
      </div>

      <div>
        <label for="last-name" class="mb-2 block text-sm font-medium text-zinc-800">
          Фамилия
        </label>
        <input
          id="last-name"
          v-model="lastName"
          type="text"
          autocomplete="family-name"
          required
          class="h-11 w-full rounded-md border border-zinc-300 bg-white px-3 text-sm text-zinc-950 outline-none transition focus:border-amber-500 focus:ring-2 focus:ring-amber-200"
        />
      </div>

      <button
        type="submit"
        class="h-11 w-full rounded-md bg-zinc-950 px-4 text-sm font-semibold text-white transition hover:bg-zinc-800 focus:outline-none focus:ring-2 focus:ring-zinc-400 focus:ring-offset-2"
      >
        Продолжить
      </button>
    </form>
  </section>
</template>

<script setup>
import { ref } from 'vue'
import { updateUserFullName } from '../services/profile';
import { useRouter } from 'vue-router';
import { usePiniaStore } from '../plugins/piniaStorage';

const router = useRouter()
const firstName = ref('')
const lastName = ref('')
const session = usePiniaStore()

async function handleSubmit() {
  try{
    await updateUserFullName(firstName.value, lastName.value)
    session.firstName = firstName.value
    session.lastName = lastName.value
    session.isUser = true
    router.push('/home')
  }
  catch(error){
    console.log(error)
  }
}
</script>
