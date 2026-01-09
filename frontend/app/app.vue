<template>
  <NuxtLayout>
    <v-app :theme="theme">
      <v-app-bar title="CodeCommons" app>
        <v-spacer></v-spacer>
        <v-btn 
          :prepend-icon="theme === 'light' ? 'mdi-weather-sunny': 'mdi-weather-night'"
          text="Toggle Theme"
          slim
          @click="onThemeClick"
        >
        </v-btn>
      </v-app-bar>
      <v-navigation-drawer app>
        <v-list>
          <v-list-item title="Navigation drawer"></v-list-item>
        </v-list>
      </v-navigation-drawer>

      <v-main app>
        <v-container class="flex-grow-1">
          <NuxtPage />
        </v-container>
      </v-main>

      <!-- TODO: Fix this footer from not being sticky or figure something else out -->
      <!-- <v-footer class="text-end" app>
        <div>CodeCommons - Copyright 2026</div>
      </v-footer> -->
    </v-app>
  </NuxtLayout>
</template>

<script setup>
  import { ref } from 'vue'

  const theme = ref('light')

  function onThemeClick () {
    theme.value = theme.value === 'light' ? 'dark' : 'light'
  }
</script>