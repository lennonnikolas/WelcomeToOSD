<script setup>
import { onMounted } from 'vue'
const { $api } = useNuxtApp()

definePageMeta({
  ssr: false
})

onMounted(async () => {
  const response = await $api.get('/repositories?query=license:apache-2.0 is:public archived:false')
  console.log(response.data)
})
</script>

<template>
  <div>GitHub Results</div>
</template>