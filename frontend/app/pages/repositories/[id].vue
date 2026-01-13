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
            <v-row class="d-flex">
                <v-col>
                    <v-card>
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
                    </v-card>
                </v-col>
                <v-col>
                    <v-chip-group>
                        <v-chip prepend-icon="mdi-star">
                            {{ repository.stargazersCount }}
                        </v-chip>
                        <v-chip prepend-icon="mdi-source-fork">
                            {{ repository.forksCount }}
                        </v-chip>
                    </v-chip-group>
                </v-col>
            </v-row>
        </v-container>
        <v-divider />
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
                            <v-sheet class="pa-5" color="orange">Two</v-sheet>
                        </v-tabs-window-item>
                        <v-tabs-window-item value="three">
                            <v-sheet class="pa-5" color="brown">Three</v-sheet>
                        </v-tabs-window-item>
                    </v-tabs-window>
                </v-col>
                <v-col cols="12" md="4">
                    <v-container>
                        <v-card>
                            <v-card-title>MIT License</v-card-title>
                            <v-divider></v-divider>
                        </v-card>
                        <v-card>
                            <v-card-title>Languages</v-card-title>
                            <div v-for="language in languages.languages">
                                <span>{{ language.name }}</span>
                                <v-progress-linear :model-value="computeLengthOfLanguage(language)" height="3" />
                            </div>
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
    const tab = ref('option-1');
    const repoName = path?.split('/').at(2);

    const repository = ref({
        owner: {},
        license: {}
    });

    const languages = ref({
        languages: [],
        totalBytes: 0
    });

    const readmeMarkdown = ref(null);

    function computeLengthOfLanguage(recievedLanguage) {
        console.log('recievedLanguage', recievedLanguage);
        const value = recievedLanguage.numberOfBytes / languages.value.totalBytes * 100;
        console.log('value', value);
        return value;
    }

    const languageIcon = computed(() => {
        //handle edge cases
        let lowerCaseLanguage = repository.value?.language?.toLowerCase() ?? 'javascript';
        
        if (lowerCaseLanguage === 'c++')
            lowerCaseLanguage = 'cpp';
            
        return `mdi-language-${lowerCaseLanguage}`;
    });

    onMounted(async () => {
        const repositoryResult = await $api.get(`/repositories/${query.owner}/${repoName}`);
        const readmeResult = await $api.get(`/repositories/${query.owner}/${repoName}/contents/README.md`);
        const languagesResult = await $api.get(`/repositories/${query.owner}/${repoName}/languages`);

        const totalNumberOfBytesOfLanguages = languagesResult.data.map(language => language.numberOfBytes).reduce((acc, current) => acc + current );
        languages.value.languages = languagesResult.data;
        languages.value.totalBytes = totalNumberOfBytesOfLanguages;

        console.log('languages', languages.value);

        repository.value = repositoryResult.data;
        readmeMarkdown.value = await parseMarkdown(readmeResult.data);
    });
</script>