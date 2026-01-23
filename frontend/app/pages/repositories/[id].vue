<template>
    <v-sheet elevation="8" rounded class="pa-6">
        <v-btn
            prepend-icon="mdi-arrow-left"
            :to="'/repositories'"
            variant="text"
            color="primary"
        >
            Back to Repositories
        </v-btn>

        <!-- Hero Section header-->
        <v-container>
            <v-row no-gutters>
                <v-col cols="8">
                    <v-sheet class="d-flex">
                        <!-- <v-avatar :image="repository.owner.avatarUrl" size="large" /> -->
                        <h1 class="">{{ repository.fullName }}</h1>
                    </v-sheet>
                    <!-- <v-card>
                        <v-card-title class="text-h3">
                            <v-avatar :image="repository.owner.avatarUrl" />
                            {{ repository.fullName }}
                        </v-card-title>
                        <v-card-subtitle class="text-h5">{{ repository.description }}</v-card-subtitle>
                        <v-card-actions>
                            <v-btn 
                                prepend-icon="mdi-github" 
                                text="Github" 
                                target="_blank"
                                :href="repository.htmlUrl"
                                rel="noopener noreferrer"
                            />
                        </v-card-actions>
                    </v-card> -->
                </v-col>
                <v-col cols="4">
                    <v-sheet>
                        <v-chip-group>
                            <v-chip prepend-icon="mdi-star">
                                {{ repository.stargazersCount }}
                            </v-chip>
                            <v-chip prepend-icon="mdi-source-fork">
                                {{ repository.forksCount }}
                            </v-chip>
                        </v-chip-group>
                    </v-sheet>
                </v-col>
            </v-row>
            <v-row>
                <v-col cols="12">
                    <p>{{ repository.description }}</p>
                </v-col>
            </v-row>
            <v-row>
                <v-col cols="4">
                    <div class="d-flex flex-direction-row justify-space-evenly">
                        <v-btn>GitHub</v-btn>
                        <v-btn>Star</v-btn>
                        <v-btn>Fork</v-btn>
                        <v-btn>Clone</v-btn>
                    </div>
                </v-col>
            </v-row>
        </v-container>
        <!-- <v-divider /> -->
        <v-container>
            <v-row>
                <v-col cols="12" md="8">
                    <v-tabs color="primary" v-model="tab">
                        <v-tab value="one">README</v-tab>
                        <v-tab value="two">Issues</v-tab>
                        <v-tab value="three">Insights</v-tab>
                    </v-tabs>
                    <v-divider />
                    <v-tabs-window v-model="tab">
                        <v-tabs-window-item value="one">
                            <v-card class="pa-5" style="height: 600px; overflow-y: auto;">
                                <MDCRenderer v-if="readmeMarkdown" :body="readmeMarkdown.body" :data="readmeMarkdown.data" />
                            </v-card>
                        </v-tabs-window-item>
                        <v-tabs-window-item value="two">
                            <v-sheet class="pa-5">
                                <v-row dense>
                                    <v-col 
                                        v-for="issue in paginatedItems"
                                        :key="issue.id"
                                        cols="12"
                                        md="6"
                                    >
                                        <v-card class="pa-4" elevation="2">
                                            <v-card-title>{{ issue.title }}</v-card-title>
                                            <v-card-subtitle>{{ issue.body }}</v-card-subtitle>
                                        </v-card>
                                    </v-col>
                                </v-row>
                                <v-pagination
                                    v-model="issuesPage"
                                    :length="pageCount"
                                    :total-visible="7"
                                    class="mt-6"
                                />
                            </v-sheet>
                        </v-tabs-window-item>
                        <v-tabs-window-item value="three">
                            <v-sheet class="pa-5" color="brown">Three</v-sheet>
                        </v-tabs-window-item>
                    </v-tabs-window>
                </v-col>
                <v-divider vertical />
                <v-col cols="12" md="4">
                    <v-container>
                        <v-card>
                            <v-card-title>MIT License</v-card-title>
                            <v-divider></v-divider>
                        </v-card>
                        <v-card>
                            <v-card-title>Languages</v-card-title>
                            <v-container v-for="language in languages.languages">
                                <span>{{ language.name }}</span>
                                <v-progress-linear :model-value="computeLengthOfLanguage(language)" height="3" />
                            </v-container>
                        </v-card>
                    </v-container>
                </v-col>
            </v-row>
        </v-container>
    </v-sheet>
</template>

<script setup>
    import { parseMarkdown } from '@nuxtjs/mdc/runtime';

    definePageMeta({
        ssr: false
    });

    const { $api } = useNuxtApp();
    const { path, query } = useRoute();
    const repoName = path?.split('/').at(2);

    const tab = ref('option-1');
    const readmeMarkdown = ref(null);

    const repository = ref({
        owner: {},
        license: {}
    });

    const issues = ref([]);
    const issuesPage = ref(1);
    const issuesPerPage = 10;

    const languages = ref({
        languages: [],
        totalBytes: 0
    });

    const pageCount = computed(() => {
        return Math.ceil(issues.value.length / issuesPerPage);
    });

    const paginatedItems = computed(() => {
        const start = (issuesPage.value - 1) * issuesPerPage;
        const value = issues.value.slice(start, start + issuesPerPage);

        return value;
    });

    function computeLengthOfLanguage(recievedLanguage) {
        const value = recievedLanguage.numberOfBytes / languages.value.totalBytes * 100;
        return value;
    }

    onMounted(async () => {
        const owner = query.owner;

        const repositoryResult = await $api.get(`/repositories/${owner}/${repoName}`);
        const readmeResult = await $api.get(`/repositories/${owner}/${repoName}/contents/README.md`);
        const languagesResult = await $api.get(`/repositories/${owner}/${repoName}/languages`);
        const issuesResult = await $api.get(`/repositories/${owner}/${repoName}/issues`);

        const totalNumberOfBytesOfLanguages = languagesResult.data
            .map(language => language.numberOfBytes)
            .reduce((acc, current) => acc + current );

        languages.value.languages = languagesResult.data;
        languages.value.totalBytes = totalNumberOfBytesOfLanguages;

        repository.value = repositoryResult.data;
        readmeMarkdown.value = await parseMarkdown(readmeResult.data);

        issues.value = issuesResult.data.filter(issue => issue.pullRequest == null);
    });
</script>