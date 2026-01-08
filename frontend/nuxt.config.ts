import vuetify, { transformAssetUrls } from 'vite-plugin-vuetify';

// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },
  // modules: ['@nuxt/eslint', '@nuxt/hints', '@nuxt/test-utils'],
  plugins: ['~/plugins/axios'],
  build: {
    transpile: ['vuetify']
  },
  vite: {
    plugins: [
      vuetify({ autoImport: true })
    ],
    vue: {
      template: {
        transformAssetUrls
      }
    }
  }
})
