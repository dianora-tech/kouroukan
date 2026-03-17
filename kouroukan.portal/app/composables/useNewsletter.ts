export function useNewsletter() {
  const loading = ref(false)
  const success = ref(false)
  const error = ref(false)

  async function subscribe(email: string) {
    loading.value = true
    error.value = false
    try {
      await $fetch('/api/newsletter/subscribe', { method: 'POST', body: { email } })
      success.value = true
    } catch {
      const stored = JSON.parse(localStorage.getItem('kouroukan_newsletter') || '[]')
      stored.push({ email, date: new Date().toISOString() })
      localStorage.setItem('kouroukan_newsletter', JSON.stringify(stored))
      success.value = true
    } finally {
      loading.value = false
    }
  }

  return { subscribe, loading, success, error }
}
