<template>
    <v-container>
        <div class="text-h2 text-center mb-6">Github Repositories</div>
        <template v-if="loading">
            <v-row v-if="loading">
                <v-col
                    v-for="n in perPage"
                    :key="n"
                    cols="12"
                    md="6"
                    class="mb-4"
                >
                    <v-skeleton-loader type="card" elevation="2" class="h-100" />
                </v-col>
            </v-row>
        </template>
        <template v-else>
            <v-row>
                <v-col
                    v-for="repo in repositories"
                    :key="repo.id"
                    class="mb-4"
                    cols="12"
                    md="6"
                >
                    <v-card 
                        class="h-100 d-flex flex-column pa-4"
                        elevation="2"
                    >
                    <v-card-title class="text-h5">{{ repo.name }}</v-card-title>
                    <v-card-subtitle>
                        <a :href="repo.htmlUrl" target="_blank" rel="noopener noreferrer">
                            {{ repo.htmlUrl }}
                        </a>
                    </v-card-subtitle>
                    <v-divider class="my-2" />
                    <v-card-text class="flex-grow-1">{{ repo.description }}</v-card-text>
                    <v-card-actions>
                        <v-btn 
                            variant="outlined"
                            append-icon="$next"
                            spaced="end"
                            :to="`/repositories/${repo.name}?owner=${repo.owner.login}`"
                        >
                            <span>View</span>
                        </v-btn>
                    </v-card-actions>
                    </v-card>
                </v-col>
            </v-row>
            <v-pagination
            v-model="page"
            :length="50"
            rounded="0"
            ></v-pagination>
        </template>
    </v-container>
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