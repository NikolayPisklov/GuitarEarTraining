<template>
  <section class="mx-auto flex min-h-[calc(100vh-9rem)] w-full max-w-sm flex-col justify-center">
    <div class="mb-7 text-center">
      <h1 class="text-2xl font-semibold tracking-normal text-zinc-950">
        Войдите в аккаунт
      </h1>
      <p class="mt-2 text-sm leading-6 text-zinc-600">
        Введите свои данные для входа в аккаунт.
      </p>
    </div>

    <form
      class="space-y-5 rounded-lg border border-zinc-200 bg-white p-6 shadow-sm"
      @submit.prevent="handleLogIn"
    >
      <div>
        <label for="email" class="mb-2 block text-sm font-medium text-zinc-800">
          Почта
        </label>
        <p v-if="logInError" class="text-red-500 mb-2">
          {{ logInError }}
        </p>
        <input
          id="email"
          v-model="email"
          type="email"
          autocomplete="email"
          :disabled="isLoading"
          required
          placeholder="Введите почту"
          class="h-11 w-full rounded-md border border-zinc-300 bg-white px-3 text-sm text-zinc-950 outline-none transition focus:border-amber-500 focus:ring-2 focus:ring-amber-200 disabled:cursor-not-allowed disabled:bg-zinc-100"
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
          :disabled="isLoading"
          required
          placeholder="Введите пароль"
          class="h-11 w-full rounded-md border border-zinc-300 bg-white px-3 text-sm text-zinc-950 outline-none transition focus:border-amber-500 focus:ring-2 focus:ring-amber-200 disabled:cursor-not-allowed disabled:bg-zinc-100"
        />
      </div>

      <button
        type="submit"
        :disabled="isLoading"
        class="h-11 w-full rounded-md bg-zinc-950 px-4 text-sm font-semibold text-white transition hover:bg-zinc-800 focus:outline-none focus:ring-2 focus:ring-zinc-400 focus:ring-offset-2 disabled:cursor-not-allowed disabled:bg-zinc-400"
      >
        {{ isLoading ? 'Входим...' : 'Войти' }}
      </button>
    </form>
  </section>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import {login} from '../services/auth'
import { getUserName } from '../services/profile'
import { isUserWithName } from '../services/profile'
import { usePiniaStore } from '../plugins/piniaStorage'

const router = useRouter()
const email = ref('')
const password = ref('')
const isLoading = ref(false)
const session = usePiniaStore()

const logInError = ref('')

async function handleLogIn() {
  try{
    isLoading.value = true
    await login(email.value, password.value)
    isLoading.value = false
    if(!await isUserWithName()){
      router.push('/userNameForm')
    }
    else{
      const response = await getUserName()
      session.firstName = response.firstName
      session.lastName = response.lastName
      session.isUser = true
      router.push('/home')
    }
  }
  catch(error){
    isLoading.value = false
    if(error.response.status === 401){
      logInError.value = 'Неверый логин или пароль.'
    }
    else{
      console.log(error.response)
    }
  }
}
</script>