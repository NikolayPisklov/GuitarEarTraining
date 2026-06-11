import { createRouter, createWebHistory } from 'vue-router'
import Home from '../components/Home.vue'
import Register from '../components/Register.vue'
import LogIn from '../components/LogIn.vue'
import UserNameForm from '../components/UserNameForm.vue'
import PitchTraining from '../components/PitchTraining.vue'

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
    path: '/pitch-training',
    name: 'pitchTraining',
    component: PitchTraining
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
