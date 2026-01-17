<template>
  <NuxtLayout>
    <v-app :theme="theme">
      <v-app-bar title="CodeCommons" app>
        <v-spacer />
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
          <v-list-item title="Home" />
          <v-list-item title="Repositories" />
          <v-list-item title="Contribute" />
          <v-list-item title="Learn more" />
          <v-list-item title="About Us" />
        </v-list>
      </v-navigation-drawer>

      <v-main app>
        <v-container class="flex-grow-1">
          <NuxtPage />
          
        </v-container>
      </v-main>
      <v-btn
        class="fab"
        icon="mdi-arrow-up"
        size="large"
        @click="scrollToTop"
      />

      <!-- TODO: Fix this footer from not being sticky or figure something else out -->
      <!-- <v-footer class="text-end" app>
        <div>CodeCommons - Copyright 2026</div>
      </v-footer> -->
    </v-app>
  </NuxtLayout>
  
</template>

<script setup>
  import { ref } from 'vue';

  const theme = ref('light');

  function scrollToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }

  function onThemeClick () {
    theme.value = theme.value === 'light' ? 'dark' : 'light'
  }
</script>

<style>
  .fab {
    position: fixed !important;
    bottom: 24px;
    right: 24px;
    z-index: 999;
  }

</style>