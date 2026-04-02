<script setup lang="ts">
import { useOfflineStore } from '~/core/stores/offline.store'

const offline = useOfflineStore()
</script>

<template>
  <div
    v-if="offline.hasPendingSync"
    class="flex items-center gap-1.5"
  >
    <UIcon
      :name="offline.syncInProgress ? 'i-heroicons-arrow-path' : 'i-heroicons-cloud-arrow-up'"
      class="h-4 w-4 text-amber-500"
      :class="{ 'animate-spin': offline.syncInProgress }"
    />
    <span class="text-xs text-amber-600 dark:text-amber-400">
      {{ $t('sync.pending', { count: offline.pendingCount }) }}
    </span>
  </div>
</template>
