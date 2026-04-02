import { apiClient } from '~/core/api/client'

interface GeoItem {
  name: string
  code: string
}

const regionsCache = ref<GeoItem[]>([])
const prefecturesCache = new Map<string, GeoItem[]>()
const sousPrefecturesCache = new Map<string, GeoItem[]>()

export function useGeo() {
  const loadingRegions = ref(false)
  const loadingPrefectures = ref(false)
  const loadingSousPrefectures = ref(false)

  async function fetchRegions(): Promise<GeoItem[]> {
    if (regionsCache.value.length > 0) return regionsCache.value
    loadingRegions.value = true
    try {
      const response = await apiClient.get<GeoItem[]>('/api/geo/regions')
      if (response.success && response.data) {
        regionsCache.value = response.data
        return response.data
      }
    }
    catch { /* fallback */ }
    finally { loadingRegions.value = false }
    return []
  }

  async function fetchPrefectures(regionCode: string): Promise<GeoItem[]> {
    if (!regionCode) return []
    const cached = prefecturesCache.get(regionCode)
    if (cached) return cached
    loadingPrefectures.value = true
    try {
      const response = await apiClient.get<GeoItem[]>(`/api/geo/prefectures?regionCode=${regionCode}`)
      if (response.success && response.data) {
        prefecturesCache.set(regionCode, response.data)
        return response.data
      }
    }
    catch { /* fallback */ }
    finally { loadingPrefectures.value = false }
    return []
  }

  async function fetchSousPrefectures(prefectureCode: string): Promise<GeoItem[]> {
    if (!prefectureCode) return []
    const cached = sousPrefecturesCache.get(prefectureCode)
    if (cached) return cached
    loadingSousPrefectures.value = true
    try {
      const response = await apiClient.get<GeoItem[]>(`/api/geo/sous-prefectures?prefectureCode=${prefectureCode}`)
      if (response.success && response.data) {
        sousPrefecturesCache.set(prefectureCode, response.data)
        return response.data
      }
    }
    catch { /* fallback */ }
    finally { loadingSousPrefectures.value = false }
    return []
  }

  return {
    regions: regionsCache,
    loadingRegions,
    loadingPrefectures,
    loadingSousPrefectures,
    fetchRegions,
    fetchPrefectures,
    fetchSousPrefectures,
  }
}
