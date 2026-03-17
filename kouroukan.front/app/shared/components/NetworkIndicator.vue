<script setup lang="ts">
import { useOfflineStore } from '~/core/stores/offline.store'

const offline = useOfflineStore()
const { t } = useI18n()

onMounted(() => {
  offline.initNetworkListener()
})
</script>

<template>
  <div class="flex items-center gap-1.5">
    <span
      class="h-2.5 w-2.5 rounded-full"
      :class="offline.isOnline ? 'bg-green-500' : 'bg-red-500'"
    />
    <span class="text-xs text-gray-500 dark:text-gray-400">
      {{ offline.isOnline ? $t('network.online') : $t('network.offline') }}
    </span>
  </div>
</template>
