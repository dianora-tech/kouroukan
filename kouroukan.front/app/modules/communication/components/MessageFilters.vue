<script setup lang="ts">
import type { MessageFilters } from '../types/message.types'
import { TYPES_MESSAGE } from '../types/message.types'

const emit = defineEmits<{
  (e: 'filter', filters: MessageFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()

const filters = reactive<MessageFilters>({
  search: '',
  estLu: undefined,
  groupeDestinataire: undefined,
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
  filters.estLu = undefined
  filters.groupeDestinataire = undefined
  filters.dateFrom = undefined
  filters.dateTo = undefined
  emit('reset')
}

const statutLuOptions = [
  { label: t('communication.message.filters.allStatuts'), value: '' },
  { label: t('communication.message.lu'), value: 'true' },
  { label: t('communication.message.nonLu'), value: 'false' },
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
      v-model="filters.estLu"
      :items="statutLuOptions"
      value-key="value"
      class="w-40"
      @update:model-value="onFilterChange"
    />

    <UInput
      v-model="filters.dateFrom"
      type="date"
      :placeholder="$t('communication.message.filters.dateFrom')"
      class="w-40"
      @change="onFilterChange"
    />

    <UInput
      v-model="filters.dateTo"
      type="date"
      :placeholder="$t('communication.message.filters.dateTo')"
      class="w-40"
      @change="onFilterChange"
    />

    <UButton
      variant="ghost"
      size="sm"
      icon="i-heroicons-x-mark"
      @click="resetFilters"
    >
      {{ $t('communication.message.filters.reset') }}
    </UButton>
  </div>
</template>
