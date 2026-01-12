<template>
    <v-container>
        <div>
            <div class="text-h2 mb-6">Github Repositories</div>
            <v-text-field label="Search Repositories" prepend-inner-icon="mdi-magnify" v-model="repoInput"/>
        </div>
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
                    v-for="repo in filteredRepositories"
                    :key="repo.id"
                    class="mb-4"
                    cols="12"
                    md="6"
                >
                    <v-card 
                        class="h-100 d-flex flex-column pa-4"
                        elevation="2"
                    >
                    <v-card-title class="d-flex align-center">
                        <span class="text-h6 flex-grow-1 text-truncate">{{ repo.name }}</span>
                            <v-chip
                                v-if="getStatusOfRepo(repo)"
                                :color="getStatusOfRepo(repo).color" 
                                variant="flat"
                                size="small"
                            >
                                {{ getStatusOfRepo(repo).label }}
                            </v-chip>
                    </v-card-title>
                    <v-card-subtitle>
                        <a :href="repo.htmlUrl" target="_blank" rel="noopener noreferrer">
                            {{ repo.htmlUrl }}
                        </a>
                    </v-card-subtitle>
                    <v-divider class="my-2" />
                    <v-card-text class="flex-grow-1">{{ repo.description }}</v-card-text>
                    <v-card-actions class="d-flex justify-space-between">
                        <div>
                            <v-btn 
                                variant="outlined"
                                append-icon="$next"
                                spaced="end"
                                :to="`/repositories/${repo.name}?owner=${repo.owner.login}`"
                            >
                                <span>View</span>
                            </v-btn>
                        </div>
                        <v-chip-group>
                            <v-chip variant="plain" text-color="white">
                                <template #prepend>
                                    <v-icon size="16" color="yellow" class="me-1 align-center">mdi-star</v-icon>
                                </template>
                                {{ repo.stargazersCount }}
                            </v-chip>
                            <v-chip prepend-icon="mdi-eye" variant="plain">{{ repo.watchersCount }}</v-chip>
                            <v-chip prepend-icon="mdi-clock" variant="plain">{{ convertUTCToLocal(repo.updatedAt) }}</v-chip>
                        </v-chip-group>
                    </v-card-actions>
                    </v-card>
                </v-col>
            </v-row>
            <v-pagination v-model="page" :length="50" rounded="0" />
        </template>
    </v-container>
</template>

<script setup>
    import { defineModel } from 'vue';

    definePageMeta({
        ssr: false
    });
    
    const { $api } = useNuxtApp();

    const repositories = ref([]);
    const page = ref(1);
    const perPage = ref(20);
    const loading = ref(true);
    const repoInput = ref("");

    async function fetchRepositories() {
        loading.value = true;
        const response = await $api.get(`/repositories?page=${page.value}&perPage=${perPage.value}`);
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

    const filteredRepositories = computed(() => {
        const query = repoInput.value.toLowerCase();

        if (query === "")
            return repositories.value;

        return repositories.value.filter(repo => repo.fullName.toLowerCase().includes(query));
    });

    function getStatusOfRepo(repo) {
        if (repo.archived)
            return { label: 'ARCHIVED', color: 'orange-darken-2'};

        const description = repo.description?.toLowerCase() || '';

        if (description.includes('deprecated'))
            return { label: 'DEPRECATED', color: 'red-darken-2'};
        else if (description.toLowerCase().includes('unmaintained'))
            return { label: 'UNMAINTAINED', color: 'grey-darken-2'}
        else
            return { label: 'MAINTAINED', color: 'green-darken-2' };
    }

    function convertUTCToLocal(datetime) {
        const localDate = new Date(datetime); // Automatically converts to local time

        return new Intl.DateTimeFormat(undefined, {
            dateStyle: 'medium',
            timeStyle: 'short',
        }).format(localDate);
    }
</script>