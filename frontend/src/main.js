import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { createPinia } from 'pinia'
import './style.css'
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'
import { getCsrfToken } from './services/csrfToken.js'

const pinia = createPinia()
pinia.use(piniaPluginPersistedstate)

const app = createApp(App)

app.use(pinia)

await getCsrfToken()

app.use(router)
app.mount('#app')
