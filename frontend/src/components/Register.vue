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

    <form
      class="space-y-5 rounded-lg border border-zinc-200 bg-white p-6 shadow-sm"
      @submit.prevent="handleRegister"
    >
      <div>
        <label for="email" class="mb-2 block text-sm font-medium text-zinc-800">
          Почта
        </label>
        <p v-if="duplicateUserNameError" class="text-red-500 mb-2">
          {{ duplicateUserNameError }}
        </p>
        <input
          id="email"
          v-model="email"
          type="email"
          autocomplete="email"
          :disabled="isLoading"
          required
          placeholder="you@example.com"
          class="h-11 w-full rounded-md border border-zinc-300 bg-white px-3 text-sm text-zinc-950 outline-none transition focus:border-amber-500 focus:ring-2 focus:ring-amber-200 disabled:cursor-not-allowed disabled:bg-zinc-100"
        />
      </div>

      <div>
        <label for="password" class="mb-2 block text-sm font-medium text-zinc-800">
          Пароль
        </label>
        <ul v-if="passwordErrors.length > 0" class="text-red-500 list-disc pl-5 mb-2">
          <li 
            v-for="error in passwordErrors"
            :key="error"
          >
            {{ error }}
          </li>
        </ul>
        <input
          id="password"
          v-model="password"
          type="password"
          autocomplete="new-password"
          :disabled="isLoading"
          required
          placeholder="Не меньше 8 символов"
          class="h-11 w-full rounded-md border border-zinc-300 bg-white px-3 text-sm text-zinc-950 outline-none transition focus:border-amber-500 focus:ring-2 focus:ring-amber-200 disabled:cursor-not-allowed disabled:bg-zinc-100"
        />
      </div>

      <button
        type="submit"
        :disabled="isLoading"
        class="h-11 w-full rounded-md bg-zinc-950 px-4 text-sm font-semibold text-white transition hover:bg-zinc-800 focus:outline-none focus:ring-2 focus:ring-zinc-400 focus:ring-offset-2 disabled:cursor-not-allowed disabled:bg-zinc-400"
      >
        {{ isLoading ? 'Регистрируем...' : 'Зарегистрироваться' }}
      </button>
    </form>
  </section>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import {register} from '../services/auth'

const router = useRouter()
const email = ref('')
const password = ref('')
const isLoading = ref(false)

const passwordErrors = ref([])
const duplicateUserNameError = ref('')

async function handleRegister() {
  try{
    clearErrors()
    isLoading.value = true
    await register(email.value, password.value)
    isLoading.value = false
  }
  catch(error){
    isLoading.value = false
    if(error.response.status === 400){
      const errors = error.response.data.errors
      errorListHandler(errors)
    }
  }
}
function clearErrors(){
  duplicateUserNameError.value = ''
  passwordErrors.value = []
}
function errorListHandler(errors){
  if(errors.DuplicateUserName){
    duplicateUserNameError.value = 'Пользователь с такой почтой уже существует.'
  }
  if(errors.PasswordRequiresLower){
    passwordErrors.value.push('Пароль должен содержать латинскую букву нижнего регистра.')
  }
  if(errors.PasswordRequiresDigit){
    passwordErrors.value.push('Пароль должен содержать цифру.')
  }
  if(errors.PasswordRequiresNonAlphanumeric){
    passwordErrors.value.push('Пароль должен содержать специальный символ: *, ?, _ и т.п.')
  }
  if(errors.PasswordRequiresUpper){
    passwordErrors.value.push('Пароль должен содержать латинскую букву верхнего регистра.')
  }
  if(errors.PasswordTooShort){
    passwordErrors.value.push('Пароль должен состоять минимум из 6 символов.')
  }
}
</script>
