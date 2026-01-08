<template>
    <v-container v-for="repo in repositories">
        <v-card elevation="24" :title="repo.name" :subtitle="repo.htmlUrl" :text="repo.description" />
    </v-container>
    <v-pagination rounded></v-pagination>
</template>

<script setup>
    definePageMeta({
        ssr: false
    });
    
    const { $api } = useNuxtApp();

    const repositories = ref([]);

    onMounted(async () => {
        const response = await $api.get('/repositories?query=license:apache-2.0 is:public archived:false');
        console.log(response.data);
        repositories.value = response.data;
    });
</script>