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
                            Stars
                        </v-chip>
                        <v-chip prepend-icon="mdi-source-fork">
                            Forks
                        </v-chip>
                    </v-chip-group>
                </v-col>
            </v-row>
        </v-container>
        <v-sheet>
            Hello
        </v-sheet>
    </v-sheet>
</template>

<script setup>
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

    const languageIcon = computed(() => {
        //handle edge cases
        let lowerCaseLanguage = repository.value?.language?.toLowerCase() ?? 'javascript';
        
        if (lowerCaseLanguage === 'c++')
            lowerCaseLanguage = 'cpp';
            
        return `mdi-language-${lowerCaseLanguage}`;
    });

    onMounted(async () => {
        const result = await $api.get(`/repositories/${query.owner}/${repoName}`);
        repository.value = result.data;        
    });
</script>