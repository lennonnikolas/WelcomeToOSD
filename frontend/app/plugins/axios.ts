import axios from 'axios'

export default defineNuxtPlugin(() => {
  const api = axios.create({
    baseURL: 'http://localhost:5147'
  })

  return {
    provide: { api }
  }
})
