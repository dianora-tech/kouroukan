<template>
  <section ref="sectionRef" class="py-20 bg-green-600">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="text-center mb-12">
        <h2 class="text-3xl font-bold text-white">{{ $t('stats.title') }}</h2>
        <p class="mt-2 text-green-100">{{ $t('stats.subtitle') }}</p>
      </div>

      <div class="grid grid-cols-2 lg:grid-cols-4 gap-8">
        <div v-for="stat in statItems" :key="stat.key" class="text-center">
          <div class="text-4xl sm:text-5xl font-bold text-white">
            {{ formatNumber(stat.current) }}{{ stat.suffix }}
          </div>
          <div class="mt-2 text-green-100 text-sm font-medium">{{ $t(`stats.${stat.key}`) }}</div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
const sectionRef = ref<HTMLElement | null>(null)
const animated = ref(false)

const statItems = reactive([
  { key: 'schools', target: 500, current: 0, suffix: '+' },
  { key: 'students', target: 50000, current: 0, suffix: '+' },
  { key: 'regions', target: 8, current: 0, suffix: '' },
  { key: 'satisfaction', target: 98, current: 0, suffix: '%' }
])

function formatNumber(n: number): string {
  if (n >= 1000) {
    return new Intl.NumberFormat('fr-GN').format(Math.round(n))
  }
  return Math.round(n).toString()
}

function animateCounters() {
  if (animated.value) return
  animated.value = true

  const duration = 2000
  const start = performance.now()

  function tick(now: number) {
    const progress = Math.min((now - start) / duration, 1)
    const eased = 1 - Math.pow(1 - progress, 3) // easeOutCubic

    for (const stat of statItems) {
      stat.current = stat.target * eased
    }

    if (progress < 1) {
      requestAnimationFrame(tick)
    }
  }

  requestAnimationFrame(tick)
}

onMounted(() => {
  if (!sectionRef.value) return

  const observer = new IntersectionObserver(
    ([entry]) => {
      if (entry.isIntersecting) {
        animateCounters()
        observer.disconnect()
      }
    },
    { threshold: 0.3 }
  )

  observer.observe(sectionRef.value)
})
</script>
