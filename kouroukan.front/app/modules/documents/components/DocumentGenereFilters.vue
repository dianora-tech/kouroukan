<script setup lang="ts">
import type { DocumentGenereFilters } from '../types/document-genere.types'
import { STATUTS_SIGNATURE_DOCUMENT } from '../types/document-genere.types'
import { useDocumentGenere } from '../composables/useDocumentGenere'

const emit = defineEmits<{
  (e: 'filter', filters: DocumentGenereFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()
const { typeOptions } = useDocumentGenere()

const filters = reactive<DocumentGenereFilters>({
  search: '',
  typeId: undefined,
  statutSignature: undefined,
  dateFrom: undefined,
  dateTo: undefined,
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
  filters.statutSignature = undefined
  filters.dateFrom = undefined
  filters.dateTo = undefined
  emit('reset')
}

const allTypeOptions = computed(() => [
  { label: t('documents.documentGenere.filters.allTypes'), value: '' },
  ...typeOptions.value,
])

const statutOptions = [
  { label: t('documents.documentGenere.filters.allStatuts'), value: '' },
  ...STATUTS_SIGNATURE_DOCUMENT.map(s => ({
    label: t(`documents.documentGenere.statut.${s}`),
    value: s,
  })),
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
      v-model="filters.statutSignature"
      :items="statutOptions"
      value-key="value"
      class="w-40"
      @update:model-value="onFilterChange"
    />

    <UButton
      variant="ghost"
      size="sm"
      icon="i-heroicons-x-mark"
      @click="resetFilters"
    >
      {{ $t('documents.documentGenere.filters.reset') }}
    </UButton>
  </div>
</template>
