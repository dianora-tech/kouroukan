<script setup lang="ts" generic="T extends Record<string, unknown>">
import type { PaginatedResult } from '~/core/api/types'

export interface Column<R = T> {
  key: string
  label: string
  sortable?: boolean
  class?: string
  render?: (row: R) => string
}

const props = withDefaults(defineProps<{
  columns: Column[]
  data: PaginatedResult<T> | null
  loading?: boolean
  searchable?: boolean
}>(), {
  loading: false,
  searchable: false,
})

const emit = defineEmits<{
  (e: 'page-change', page: number): void
  (e: 'sort', key: string, direction: 'asc' | 'desc'): void
}>()

const { t } = useI18n()

const searchQuery = ref('')

const filteredItems = computed(() => {
  if (!props.data?.items) return []
  if (!props.searchable || !searchQuery.value.trim()) return props.data.items

  const term = searchQuery.value.trim().toLowerCase()
  return props.data.items.filter((row) => {
    return props.columns.some((col) => {
      const value = col.render ? col.render(row) : row[col.key]
      if (value == null) return false
      return String(value).toLowerCase().includes(term)
    })
  })
})

const currentSort = ref<{ key: string, direction: 'asc' | 'desc' } | null>(null)

function handleSort(key: string): void {
  if (!props.columns.find(c => c.key === key)?.sortable) return

  const direction = currentSort.value?.key === key && currentSort.value.direction === 'asc'
    ? 'desc'
    : 'asc'

  currentSort.value = { key, direction }
  emit('sort', key, direction)
}

function getCellValue(row: T, key: string): unknown {
  return row[key]
}
</script>

<template>
  <div class="overflow-hidden rounded-lg border border-gray-200 dark:border-gray-700">
    <!-- Search input -->
    <div
      v-if="searchable"
      class="border-b border-gray-200 bg-white px-4 py-3 dark:border-gray-700 dark:bg-gray-800"
    >
      <UInput
        v-model="searchQuery"
        :placeholder="$t('table.search')"
        icon="i-heroicons-magnifying-glass"
        size="sm"
        class="max-w-xs"
      />
    </div>
    <div class="overflow-x-auto">
      <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
        <thead class="bg-gray-50 dark:bg-gray-800">
          <tr>
            <th
              v-for="col in columns"
              :key="col.key"
              class="px-4 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500 dark:text-gray-400"
              :class="[col.class, { 'cursor-pointer select-none': col.sortable }]"
              @click="handleSort(col.key)"
            >
              <div class="flex items-center gap-1">
                {{ col.label }}
                <template v-if="col.sortable && currentSort?.key === col.key">
                  <UIcon
                    :name="currentSort.direction === 'asc' ? 'i-heroicons-chevron-up' : 'i-heroicons-chevron-down'"
                    class="h-3 w-3"
                  />
                </template>
              </div>
            </th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 bg-white dark:divide-gray-700 dark:bg-gray-900">
          <!-- Loading -->
          <tr v-if="loading">
            <td
              :colspan="columns.length"
              class="px-4 py-8 text-center"
            >
              <UIcon
                name="i-heroicons-arrow-path"
                class="mx-auto h-6 w-6 animate-spin text-gray-400"
              />
            </td>
          </tr>

          <!-- Empty -->
          <tr v-else-if="!filteredItems.length">
            <td
              :colspan="columns.length"
              class="px-4 py-8 text-center text-sm text-gray-500"
            >
              {{ $t('table.empty') }}
            </td>
          </tr>

          <!-- Rows -->
          <tr
            v-for="(row, idx) in filteredItems"
            v-else
            :key="idx"
            class="transition-colors hover:bg-gray-50 dark:hover:bg-gray-800"
          >
            <td
              v-for="col in columns"
              :key="col.key"
              class="whitespace-nowrap px-4 py-3 text-sm text-gray-900 dark:text-gray-100"
              :class="col.class"
            >
              <slot
                :name="`cell-${col.key}`"
                :row="row"
                :value="getCellValue(row, col.key)"
              >
                {{ col.render ? col.render(row) : getCellValue(row, col.key) }}
              </slot>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Pagination -->
    <div
      v-if="data && data.totalPages > 1"
      class="flex items-center justify-between border-t border-gray-200 bg-white px-4 py-3 dark:border-gray-700 dark:bg-gray-800"
    >
      <span class="text-sm text-gray-500 dark:text-gray-400">
        {{ $t('table.showing', { from: (data.pageNumber - 1) * data.pageSize + 1, to: Math.min(data.pageNumber * data.pageSize, data.totalCount), total: data.totalCount }) }}
      </span>
      <div class="flex gap-1">
        <UButton
          variant="outline"
          size="xs"
          :disabled="!data.hasPreviousPage"
          @click="emit('page-change', data.pageNumber - 1)"
        >
          {{ $t('table.previous') }}
        </UButton>
        <UButton
          variant="outline"
          size="xs"
          :disabled="!data.hasNextPage"
          @click="emit('page-change', data.pageNumber + 1)"
        >
          {{ $t('table.next') }}
        </UButton>
      </div>
    </div>
  </div>
</template>
