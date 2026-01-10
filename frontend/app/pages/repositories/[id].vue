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
        <v-toolbar color="primary" density="comfortable">
            <template #prepend>

                <v-avatar size="40">
                <v-img :src="repository.owner.avatarUrl" />
                </v-avatar>
            </template>

            <v-toolbar-title>
                {{ repository.fullName }}
            </v-toolbar-title>

            <v-spacer />

            <v-btn
                icon
                :href="repository.htmlUrl"
                target="_blank"
                rel="noopener noreferrer"
            >
                <v-icon>mdi-open-in-new</v-icon>
            </v-btn>
        </v-toolbar>
         <div class="d-flex flex-row">
            <v-tabs
                v-model="tab"
                color="primary"
                direction="vertical"
            >
                <v-tab prepend-icon="mdi-account" text="Description" value="option-1"></v-tab>
                <v-tab prepend-icon="mdi-lock" text="Clone/Fork" value="option-2"></v-tab>
                <v-tab prepend-icon="mdi-access-point" text="Option 3" value="option-3"></v-tab>
            </v-tabs>
            <v-tabs-window v-model="tab">
                <v-tabs-window-item value="option-1">
                    <v-card flat>
                        <v-card-text>
                            Description: {{ repository.description }}
                            <v-list>
                                <v-list-item prepend-icon="mdi-account">
                                    <span>Owner: {{ repository.owner.login ?? 'N/A'  }}</span>
                                </v-list-item>
                                <v-list-item prepend-icon="mdi-source-branch">
                                    <span>Default Branch: {{ repository.defaultBranch }}</span>
                                </v-list-item>
                                <v-list-item :prepend-icon=languageIcon>
                                    <span>Language: {{ repository.language }}</span>
                                </v-list-item>
                                <v-list-item prepend-icon="mdi-license">
                                    <span>License: {{ repository.license.name }}</span>
                                </v-list-item>
                            </v-list>
                        </v-card-text>
                    </v-card>
                </v-tabs-window-item>

                <v-tabs-window-item value="option-2">
                    <v-card flat>
                        <v-card-text>
                            <v-list>
                                <v-list-item prepend-icon="mdi-ssh">
                                    <span>{{ repository.sshUrl ?? 'N/A'  }}</span>
                                </v-list-item>
                                <v-list-item prepend-icon="mdi-web">
                                    <span>{{ repository.cloneUrl }}</span>
                                </v-list-item>
                                <v-list-item prepend-icon="mdi-console">
                                    <span>gh repo clone {{ repository.owner.login }}/{{ repository.name }}</span>
                                </v-list-item>
                            </v-list>
                        </v-card-text>
                    </v-card>
                </v-tabs-window-item>

                <v-tabs-window-item value="option-3">
                <v-card flat>
                    <v-card-text>
                    <p>
                        Fusce a quam. Phasellus nec sem in justo pellentesque facilisis. Nam eget dui. Proin viverra, ligula sit amet ultrices semper, ligula arcu tristique sapien, a accumsan nisi mauris ac eros. In dui magna, posuere eget, vestibulum et, tempor auctor, justo.
                    </p>

                    <p class="mb-0">
                        Cras sagittis. Phasellus nec sem in justo pellentesque facilisis. Proin sapien ipsum, porta a, auctor quis, euismod ut, mi. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nam at tortor in tellus interdum sagittis.
                    </p>
                    </v-card-text>
                </v-card>
                </v-tabs-window-item>
            </v-tabs-window>
         </div>
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
        console.log('result', result.data);
        repository.value = result.data;        
    });
</script>