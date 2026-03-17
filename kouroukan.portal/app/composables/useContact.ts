export function useContact() {
  const loading = ref(false)
  const success = ref(false)
  const error = ref(false)

  async function submit(data: { name: string; email: string; phone?: string; subject: string; message: string }) {
    loading.value = true
    error.value = false
    try {
      await $fetch('/api/contact', { method: 'POST', body: data })
      success.value = true
    } catch {
      const stored = JSON.parse(localStorage.getItem('kouroukan_contacts') || '[]')
      stored.push({ ...data, date: new Date().toISOString() })
      localStorage.setItem('kouroukan_contacts', JSON.stringify(stored))
      success.value = true
    } finally {
      loading.value = false
    }
  }

  function reset() {
    success.value = false
    error.value = false
  }

  return { submit, loading, success, error, reset }
}
