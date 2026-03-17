<script setup lang="ts">
import type { NotificationFilters } from '../types/notification.types'
import { CANAUX_NOTIFICATION } from '../types/notification.types'

const emit = defineEmits<{
  (e: 'filter', filters: NotificationFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()

const filters = reactive<NotificationFilters>({
  search: '',
  canal: undefined,
  estEnvoyee: undefined,
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
  filters.canal = undefined
  filters.estEnvoyee = undefined
  filters.dateFrom = undefined
  filters.dateTo = undefined
  emit('reset')
}

const canalOptions = [
  { label: t('communication.notification.filters.allCanaux'), value: '' },
  ...CANAUX_NOTIFICATION.map(c => ({
    label: t(`communication.notification.canal.${c}`),
    value: c,
  })),
]

const statutEnvoiOptions = [
  { label: t('communication.notification.filters.allStatuts'), value: '' },
  { label: t('communication.notification.envoyee'), value: 'true' },
  { label: t('communication.notification.enAttente'), value: 'false' },
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
      v-model="filters.canal"
      :items="canalOptions"
      value-key="value"
      class="w-40"
      @update:model-value="onFilterChange"
    />

    <USelect
      v-model="filters.estEnvoyee"
      :items="statutEnvoiOptions"
      value-key="value"
      class="w-40"
      @update:model-value="onFilterChange"
    />

    <UInput
      v-model="filters.dateFrom"
      type="date"
      :placeholder="$t('communication.notification.filters.dateFrom')"
      class="w-40"
      @change="onFilterChange"
    />

    <UInput
      v-model="filters.dateTo"
      type="date"
      :placeholder="$t('communication.notification.filters.dateTo')"
      class="w-40"
      @change="onFilterChange"
    />

    <UButton variant="ghost" size="sm" icon="i-heroicons-x-mark" @click="resetFilters">
      {{ $t('communication.notification.filters.reset') }}
    </UButton>
  </div>
</template>
