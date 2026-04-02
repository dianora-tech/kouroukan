<script setup lang="ts">
import type { SignatureFilters } from '../types/signature.types'
import { STATUTS_SIGNATURE, NIVEAUX_SIGNATURE } from '../types/signature.types'
import { useSignature } from '../composables/useSignature'

const emit = defineEmits<{
  (e: 'filter', filters: SignatureFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()
const { typeOptions } = useSignature()

const filters = reactive<SignatureFilters>({
  search: '',
  typeId: undefined,
  statutSignature: undefined,
  niveauSignature: undefined,
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
  filters.niveauSignature = undefined
  filters.dateFrom = undefined
  filters.dateTo = undefined
  emit('reset')
}

const allTypeOptions = computed(() => [
  { label: t('documents.signature.filters.allTypes'), value: '' },
  ...typeOptions.value,
])

const statutOptions = [
  { label: t('documents.signature.filters.allStatuts'), value: '' },
  ...STATUTS_SIGNATURE.map(s => ({
    label: t(`documents.signature.statut.${s}`),
    value: s,
  })),
]

const niveauOptions = [
  { label: t('documents.signature.filters.allNiveaux'), value: '' },
  ...NIVEAUX_SIGNATURE.map(n => ({
    label: t(`documents.signature.niveau.${n}`),
    value: n,
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

    <USelect
      v-model="filters.niveauSignature"
      :items="niveauOptions"
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
      {{ $t('documents.signature.filters.reset') }}
    </UButton>
  </div>
</template>
