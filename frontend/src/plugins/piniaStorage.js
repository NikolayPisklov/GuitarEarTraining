import { defineStore } from 'pinia'

export const usePiniaStore = defineStore('user', {
    state: () => ({
        firstName: null,
        lastName: null,
        isUser: null
    }),
    persist: true
})