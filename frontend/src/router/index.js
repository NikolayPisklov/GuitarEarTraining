import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/Home.vue'
import Register from '../components/Register.vue'

const routes = [
  {
    path: '/',
    name: 'register',
    component: Register,
  },
  {
    path: '/home',
    name: 'home',
    component: Home,
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
