<script setup lang="ts">
import type { BulletinFilters } from '../types/bulletin.types'
import { TRIMESTRES_BULLETIN } from '../types/bulletin.types'

const emit = defineEmits<{
  (e: 'filter', filters: BulletinFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()

const filters = reactive<BulletinFilters>({
  search: '',
  classeId: undefined,
  trimestre: undefined,
  estPublie: undefined,
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
  filters.classeId = undefined
  filters.trimestre = undefined
  filters.estPublie = undefined
  filters.dateFrom = undefined
  filters.dateTo = undefined
  emit('reset')
}

const trimestreOptions = [
  { label: t('evaluations.bulletin.filters.allTrimestres'), value: '' },
  ...TRIMESTRES_BULLETIN.map(tr => ({
    label: t(`evaluations.bulletin.trimestre.${tr}`),
    value: tr,
  })),
]

const publicationOptions = [
  { label: t('evaluations.bulletin.filters.allPublications'), value: '' },
  { label: t('evaluations.bulletin.publie'), value: 'true' },
  { label: t('evaluations.bulletin.nonPublie'), value: 'false' },
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
      v-model="filters.trimestre"
      :items="trimestreOptions"
      value-key="value"
      class="w-40"
      @update:model-value="onFilterChange"
    />

    <USelect
      v-model="filters.estPublie"
      :items="publicationOptions"
      value-key="value"
      class="w-40"
      @update:model-value="onFilterChange"
    />

    <UInput
      v-model="filters.dateFrom"
      type="date"
      :placeholder="$t('evaluations.bulletin.filters.dateFrom')"
      class="w-40"
      @change="onFilterChange"
    />

    <UInput
      v-model="filters.dateTo"
      type="date"
      :placeholder="$t('evaluations.bulletin.filters.dateTo')"
      class="w-40"
      @change="onFilterChange"
    />

    <UButton
      variant="ghost"
      size="sm"
      icon="i-heroicons-x-mark"
      @click="resetFilters"
    >
      {{ $t('evaluations.bulletin.filters.reset') }}
    </UButton>
  </div>
</template>
