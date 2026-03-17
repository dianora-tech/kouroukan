import type { PortalStats } from '~/utils/types'
import { FALLBACK_STATS } from '~/utils/constants'

const CACHE_KEY = 'kouroukan_portal_stats'
const CACHE_TTL = 60 * 60 * 1000 // 1 hour

export function usePortalStats() {
  const stats = ref<PortalStats>(FALLBACK_STATS)
  const loading = ref(false)

  async function fetchStats() {
    // Check localStorage cache first
    try {
      const cached = localStorage.getItem(CACHE_KEY)
      if (cached) {
        const { data, timestamp } = JSON.parse(cached)
        if (Date.now() - timestamp < CACHE_TTL) {
          stats.value = data
          return
        }
      }
    } catch {
      // Cache read failed
    }

    loading.value = true
    try {
      const data = await $fetch<PortalStats>('/api/portal/stats')
      stats.value = { ...data, isLive: true }
      // Cache the result
      try {
        localStorage.setItem(CACHE_KEY, JSON.stringify({
          data: stats.value,
          timestamp: Date.now()
        }))
      } catch {
        // Cache write failed
      }
    } catch {
      stats.value = FALLBACK_STATS
    } finally {
      loading.value = false
    }
  }

  return { stats, loading, fetchStats }
}
