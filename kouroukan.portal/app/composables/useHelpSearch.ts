export function useHelpSearch() {
  const query = ref('')
  const results = ref<Array<{ title: string; description: string; _path: string; category: string }>>([])
  const loading = ref(false)

  let debounceTimer: ReturnType<typeof setTimeout> | null = null

  async function search(q: string) {
    query.value = q

    if (debounceTimer) clearTimeout(debounceTimer)

    if (!q || q.length < 2) {
      results.value = []
      return
    }

    debounceTimer = setTimeout(async () => {
      loading.value = true
      try {
        const found = await queryCollection('aide').all()
        results.value = found
          .filter((a: Record<string, unknown>) => {
            const title = String(a.title || '').toLowerCase()
            const desc = String(a.description || '').toLowerCase()
            const search = q.toLowerCase()
            return title.includes(search) || desc.includes(search)
          })
          .map((a: Record<string, unknown>) => ({
            title: String(a.title || ''),
            description: String(a.description || ''),
            _path: String(a._path || ''),
            category: String(a.category || '')
          }))
      } catch {
        results.value = []
      } finally {
        loading.value = false
      }
    }, 300)
  }

  return { query, results, loading, search }
}
