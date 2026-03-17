<script setup lang="ts">
import type { ServiceParentFilters } from '../types/service-parent.types'
import { PERIODICITES } from '../types/service-parent.types'
import { useServiceParent } from '../composables/useServiceParent'

const emit = defineEmits<{
  (e: 'filter', filters: ServiceParentFilters): void
  (e: 'reset'): void
}>()

const { t } = useI18n()
const { typeOptions } = useServiceParent()

const filters = reactive<ServiceParentFilters>({
  search: '',
  typeId: undefined,
  periodicite: undefined,
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
  filters.periodicite = undefined
  filters.estActif = undefined
  emit('reset')
}

const allTypeOptions = computed(() => [
  { label: t('servicesPremium.serviceParent.filters.allTypes'), value: '' },
  ...typeOptions.value,
])

const periodiciteOptions = [
  { label: t('servicesPremium.serviceParent.filters.allPeriodicites'), value: '' },
  ...PERIODICITES.map(p => ({
    label: t(`servicesPremium.serviceParent.periodicite.${p}`),
    value: p,
  })),
]

const statutOptions = [
  { label: t('servicesPremium.serviceParent.filters.allStatuts'), value: '' },
  { label: t('servicesPremium.serviceParent.actif'), value: 'true' },
  { label: t('servicesPremium.serviceParent.inactif'), value: 'false' },
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
      v-model="filters.periodicite"
      :items="periodiciteOptions"
      value-key="value"
      class="w-36"
      @update:model-value="onFilterChange"
    />

    <UButton variant="ghost" size="sm" icon="i-heroicons-x-mark" @click="resetFilters">
      {{ $t('servicesPremium.serviceParent.filters.reset') }}
    </UButton>
  </div>
</template>
