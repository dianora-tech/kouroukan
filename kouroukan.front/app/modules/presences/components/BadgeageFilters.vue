<script setup lang="ts">
import type { BadgeageFilters } from '../types/badgeage.types'
import { POINTS_ACCES, METHODES_BADGEAGE } from '../types/badgeage.types'
import { useBadgeage } from '../composables/useBadgeage'

const emit = defineEmits<{
  (e: 'filter', filters: BadgeageFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()
const { typeOptions } = useBadgeage()

const filters = reactive<BadgeageFilters>({
  search: '',
  typeId: undefined,
  pointAcces: undefined,
  methodeBadgeage: undefined,
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
  filters.pointAcces = undefined
  filters.methodeBadgeage = undefined
  filters.dateFrom = undefined
  filters.dateTo = undefined
  emit('reset')
}

const allTypeOptions = computed(() => [
  { label: t('presences.badgeage.filters.allTypes'), value: '' },
  ...typeOptions.value,
])

const pointAccesOptions = [
  { label: t('presences.badgeage.filters.allPointsAcces'), value: '' },
  ...POINTS_ACCES.map(p => ({
    label: t(`presences.badgeage.pointAcces.${p}`),
    value: p,
  })),
]

const methodeOptions = [
  { label: t('presences.badgeage.filters.allMethodes'), value: '' },
  ...METHODES_BADGEAGE.map(m => ({
    label: t(`presences.badgeage.methode.${m}`),
    value: m,
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
      v-model="filters.pointAcces"
      :items="pointAccesOptions"
      value-key="value"
      class="w-44"
      @update:model-value="onFilterChange"
    />

    <USelect
      v-model="filters.methodeBadgeage"
      :items="methodeOptions"
      value-key="value"
      class="w-36"
      @update:model-value="onFilterChange"
    />

    <UInput
      v-model="filters.dateFrom"
      type="date"
      :placeholder="t('presences.badgeage.filters.dateFrom')"
      class="w-40"
      @change="onFilterChange"
    />

    <UInput
      v-model="filters.dateTo"
      type="date"
      :placeholder="t('presences.badgeage.filters.dateTo')"
      class="w-40"
      @change="onFilterChange"
    />

    <UButton variant="ghost" size="sm" icon="i-heroicons-x-mark" @click="resetFilters">
      {{ $t('presences.badgeage.filters.reset') }}
    </UButton>
  </div>
</template>
