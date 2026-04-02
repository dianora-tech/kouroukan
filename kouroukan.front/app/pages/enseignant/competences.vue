<script setup lang="ts">
import { useEnseignantCompetence } from '~/modules/enseignant/composables/useEnseignantCompetence'
import type { Competence } from '~/modules/enseignant/types/enseignant.types'

definePageMeta({ layout: 'default' })

const { t } = useI18n()

const {
  items: competences,
  loading,
  saving,
  fetchAll,
  create,
  remove,
} = useEnseignantCompetence()

const showForm = ref(false)
const newCompetence = reactive({ matiere: '', cycle: 'college' })

const cycleLabel: Record<string, string> = {
  primaire: 'Primaire',
  college: 'College',
  lycee: 'Lycee',
}

const cycleColor: Record<string, 'success' | 'info' | 'warning'> = {
  primaire: 'success',
  college: 'info',
  lycee: 'warning',
}

onMounted(() => {
  fetchAll()
})

async function addCompetence() {
  if (!newCompetence.matiere) return
  const result = await create({
    matiere: newCompetence.matiere,
    cycle: newCompetence.cycle,
  })
  if (result) {
    newCompetence.matiere = ''
    showForm.value = false
  }
}

async function removeCompetence(id: number) {
  await remove(id)
}
</script>

<template>
  <div class="space-y-6">
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <UBreadcrumb
          :items="[
            { label: $t('enseignant.title'), to: '/enseignant' },
            { label: $t('enseignant.competences.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('enseignant.competences.title') }}
        </h1>
      </div>
      <UButton
        color="primary"
        icon="i-heroicons-plus"
        @click="showForm = true"
      >
        {{ $t('enseignant.competences.ajouter') }}
      </UButton>
    </div>

    <!-- Loading state -->
    <div
      v-if="loading"
      class="flex items-center justify-center py-12"
    >
      <UIcon
        name="i-heroicons-arrow-path"
        class="h-8 w-8 animate-spin text-gray-400"
      />
    </div>

    <!-- Table -->
    <div
      v-else
      class="rounded-xl border border-gray-200 bg-white dark:border-gray-700 dark:bg-gray-800"
    >
      <table class="w-full text-left text-sm">
        <thead class="border-b border-gray-200 dark:border-gray-700">
          <tr>
            <th class="px-4 py-3 font-medium text-gray-500">
              {{ $t('enseignant.competences.matiere') }}
            </th>
            <th class="px-4 py-3 font-medium text-gray-500">
              {{ $t('enseignant.competences.cycle') }}
            </th>
            <th class="px-4 py-3 font-medium text-gray-500 w-24" />
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-100 dark:divide-gray-700">
          <tr
            v-for="comp in competences"
            :key="comp.id"
          >
            <td class="px-4 py-3 text-gray-900 dark:text-white">
              {{ comp.matiere }}
            </td>
            <td class="px-4 py-3">
              <UBadge
                :color="cycleColor[comp.cycle] ?? 'info'"
                variant="subtle"
                size="sm"
              >
                {{ cycleLabel[comp.cycle] ?? comp.cycle }}
              </UBadge>
            </td>
            <td class="px-4 py-3">
              <UButton
                variant="ghost"
                size="xs"
                color="error"
                icon="i-heroicons-trash"
                :loading="saving"
                @click="removeCompetence(comp.id)"
              />
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Add form dialog -->
    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ $t('enseignant.competences.ajouter') }}
        </h3>
      </template>
      <template #body>
        <div class="space-y-4 p-4">
          <UFormField :label="$t('enseignant.competences.matiere')">
            <UInput
              v-model="newCompetence.matiere"
              class="w-full"
              :placeholder="$t('enseignant.competences.matierePlaceholder')"
            />
          </UFormField>
          <UFormField :label="$t('enseignant.competences.cycle')">
            <USelect
              v-model="newCompetence.cycle"
              class="w-full"
              :items="[
                { label: 'Primaire', value: 'primaire' },
                { label: 'College', value: 'college' },
                { label: 'Lycee', value: 'lycee' },
              ]"
            />
          </UFormField>
          <div class="flex justify-end gap-2">
            <UButton
              variant="outline"
              @click="showForm = false"
            >
              {{ $t('common.cancel') }}
            </UButton>
            <UButton
              color="primary"
              :loading="saving"
              @click="addCompetence"
            >
              {{ $t('common.save') }}
            </UButton>
          </div>
        </div>
      </template>
    </USlideover>
  </div>
</template>
