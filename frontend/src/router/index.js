import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/Home.vue'
import Register from '../components/Register.vue'
import LogIn from '../components/LogIn.vue'
import UserNameForm from '../components/UserNameForm.vue'

const routes = [
  {
    path: '/',
    name: 'register',
    component: Register
  },
  {
    path: '/login',
    name: 'login',
    component: LogIn
  },
  {
    path: '/home',
    name: 'home',
    component: Home
  },
  {
    path: '/userNameForm',
    name: 'userNameForm',
    component: UserNameForm
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
