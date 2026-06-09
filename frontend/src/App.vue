<template>
  <div class="min-h-screen bg-stone-50 text-zinc-950">
    <header class="border-b border-zinc-200 bg-white">
      <div class="relative mx-auto flex h-16 max-w-6xl items-center justify-center px-5">
        <RouterLink
          to="/"
          class="text-lg font-semibold tracking-normal text-zinc-950 transition hover:text-amber-700"
        >
          Guitar Trainer
        </RouterLink>

        <div class="absolute right-5 flex items-center gap-3">
          <RouterLink
            v-if="!session.isUser"
            to="/"
            class="inline-flex h-9 items-center rounded-md border border-zinc-200 bg-white px-3 text-sm font-medium text-zinc-800 transition hover:border-amber-500 hover:bg-amber-50 focus:outline-none focus:ring-2 focus:ring-amber-300"
          >
            Регистрация
          </RouterLink>

          <RouterLink
            v-if="!session.isUser"
            to="/login"
            class="inline-flex h-9 items-center rounded-md border border-zinc-200 bg-white px-3 text-sm font-medium text-zinc-800 transition hover:border-amber-500 hover:bg-amber-50 focus:outline-none focus:ring-2 focus:ring-amber-300"
          >
            Войти в аккаунт
          </RouterLink>

          <RouterLink
            v-if="session.isUser"
            to="/home"
            class="inline-flex h-9 items-center rounded-md border border-zinc-200 bg-white px-3 text-sm font-medium text-zinc-800 transition hover:border-amber-500 hover:bg-amber-50 focus:outline-none focus:ring-2 focus:ring-amber-300"
          >
            Главная
          </RouterLink>

          <button
            v-if="session.isUser" @click="logoutButtonClick"
            class="inline-flex h-9 items-center rounded-md border border-zinc-200 bg-white px-3 text-sm font-medium text-zinc-800 transition hover:border-amber-500 hover:bg-amber-50 focus:outline-none focus:ring-2 focus:ring-amber-300"
          >
            Выйти
          </button>
        </div>
      </div>
    </header>

    <main class="mx-auto min-h-[calc(100vh-4rem)] max-w-6xl px-5 py-10">
      <RouterView />
    </main>
  </div>
</template>

<script setup>
  import { usePiniaStore } from './plugins/piniaStorage';
  import { logout } from './services/auth';
  import { useRouter } from 'vue-router';

  const session = usePiniaStore()
  const router = useRouter()

  async function logoutButtonClick(){
    try{
      await logout()
      session.$reset()
      router.push('/login')
    }
    catch(error){
      console.log(error.response)
    }
  }
</script>
