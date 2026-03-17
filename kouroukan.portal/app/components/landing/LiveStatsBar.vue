<template>
  <section ref="sectionRef" class="py-12 bg-white dark:bg-gray-900 border-y border-gray-100 dark:border-gray-800">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="text-center mb-8">
        <div class="inline-flex items-center gap-2 mb-2">
          <span class="relative flex h-2.5 w-2.5">
            <span v-if="stats.isLive" class="animate-ping absolute inline-flex h-full w-full rounded-full bg-green-400 opacity-75" />
            <span class="relative inline-flex rounded-full h-2.5 w-2.5" :class="stats.isLive ? 'bg-green-500' : 'bg-gray-400'" />
          </span>
          <span class="text-sm font-medium text-gray-500">
            {{ stats.isLive ? $t('liveStats.title') : $t('liveStats.fallback') }}
          </span>
        </div>
        <p class="text-xs text-gray-400">{{ $t('liveStats.subtitle') }}</p>
      </div>

      <div class="grid grid-cols-3 sm:grid-cols-4 lg:grid-cols-7 gap-4">
        <div v-for="item in displayItems" :key="item.key" class="text-center">
          <div class="text-xl sm:text-2xl font-bold text-gray-900 dark:text-white">
            {{ formatCompact(item.value) }}
          </div>
          <div class="text-xs text-gray-500 mt-1">{{ $t(`liveStats.${item.key}`) }}</div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
const { stats, fetchStats } = usePortalStats()
const sectionRef = ref<HTMLElement | null>(null)

const displayItems = computed(() => [
  { key: 'schools', value: stats.value.schools },
  { key: 'directors', value: stats.value.directors },
  { key: 'teachers', value: stats.value.teachers },
  { key: 'parents', value: stats.value.parents },
  { key: 'students', value: stats.value.students },
  { key: 'transactions', value: stats.value.transactions },
  { key: 'documents', value: stats.value.documents }
])

function formatCompact(n: number): string {
  if (n >= 1000) {
    return `${(n / 1000).toFixed(n >= 10000 ? 0 : 1)}k`
  }
  return n.toString()
}

onMounted(() => {
  if (!sectionRef.value) return

  const observer = new IntersectionObserver(
    ([entry]) => {
      if (entry.isIntersecting) {
        fetchStats()
        observer.disconnect()
      }
    },
    { threshold: 0.2 }
  )

  observer.observe(sectionRef.value)
})
</script>
