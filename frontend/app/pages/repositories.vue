<template>
    <div>
        <div class="text-h2 text-center g-4">Github Repositories</div>
        <template v-if="loading">
            <div
                v-for="n in 20"
                :key="n"
                class="d-flex flex-column ga-2 py-4 my-2"
            >
                <v-skeleton-loader
                    v-for="n in perPage"
                    :key="n"
                    :loading="loading"
                    type="card"
                    elevation="24"
                />
            </div>
        </template>
        <template v-else>
            <v-card v-for="repo in repositories" 
                class="d-flex flex-column ga-2 py-4 my-2"
                elevation="24"
                hover
            >
            <v-card-title>{{ repo.name }}</v-card-title>
            <v-card-subtitle>
                <a :href="repo.htmlUrl" target="_blank" rel="noopener noreferrer">
                    {{ repo.htmlUrl }}
                </a>
            </v-card-subtitle>
            <v-card-text>{{ repo.description }}</v-card-text>
            </v-card>
            <v-pagination
            v-model="page"
            :length="50"
            rounded="0"
            ></v-pagination>
        </template>
    </div>
</template>

<script setup>
    definePageMeta({
        ssr: false
    });
    
    const { $api } = useNuxtApp();

    const repositories = ref([]);
    const page = ref(1);
    const perPage = ref(20);
    const loading = ref(true);

    async function fetchRepositories() {
        loading.value = true;
        const response = await $api.get(`/repositories?page=${page.value}&perPage=${perPage.value}&query=license:apache-2.0 is:public archived:false`);
        console.log(response.data);
        repositories.value = response.data;
        loading.value = false;
    }

    onMounted(async () => {
        loading.value = true;
        await fetchRepositories();
        loading.value = false;
    });

    watch(page, (newValue) => {
        window.scrollTo({ top: 0, behavior: 'smooth' });
        page.value = newValue;
        fetchRepositories();
    });
</script>