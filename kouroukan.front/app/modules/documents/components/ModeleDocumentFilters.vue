<script setup lang="ts">
import type { ModeleDocumentFilters } from '../types/modele-document.types'
import { useModeleDocument } from '../composables/useModeleDocument'

const emit = defineEmits<{
  (e: 'filter', filters: ModeleDocumentFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()
const { typeOptions } = useModeleDocument()

const filters = reactive<ModeleDocumentFilters>({
  search: '',
  typeId: undefined,
  estActif: undefined,
})

let searchTimeout: ReturnType<typeof setTimeout> | null = null

function onSearchInput(): void {
  if (searchTimeout) clearTimeout(searchTimeout)
  searchTimeout = setTimeout(() => {
    emit('filter', { ...filters })
  }, 300)
}

function onFilterChange(): void {
  emit('filter', { ...filters })
}

function resetFilters(): void {
  filters.search = ''
  filters.typeId = undefined
  filters.estActif = undefined
  emit('reset')
}

const allTypeOptions = computed(() => [
  { label: t('documents.modeleDocument.filters.allTypes'), value: '' },
  ...typeOptions.value,
])

const statutOptions = [
  { label: t('documents.modeleDocument.filters.allStatuts'), value: '' },
  { label: t('documents.modeleDocument.actif'), value: 'true' },
  { label: t('documents.modeleDocument.inactif'), value: 'false' },
]
</script>

<template>
  <div class="flex flex-wrap items-end gap-3">
    <div class="min-w-[200px] flex-1">
      <UInput
        v-model="filters.search"
        :placeholder="$t('actions.search')"
        icon="i-heroicons-magnifying-glass"
        class="w-full"
        @input="onSearchInput"
      />
    </div>

    <USelect
      v-model="filters.typeId"
      :items="allTypeOptions"
      value-key="value"
      class="w-48"
      @update:model-value="onFilterChange"
    />

    <USelect
      v-model="filters.estActif"
      :items="statutOptions"
      value-key="value"
      class="w-40"
      @update:model-value="onFilterChange"
    />

    <UButton variant="ghost" size="sm" icon="i-heroicons-x-mark" @click="resetFilters">
      {{ $t('documents.modeleDocument.filters.reset') }}
    </UButton>
  </div>
</template>
