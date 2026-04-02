const STORAGE_KEY = 'kouroukan:onboarding:draft'

interface DraftStore {
  [stepKey: string]: Record<string, unknown>
}

function getDrafts(): DraftStore {
  if (import.meta.server) return {}
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    return raw ? JSON.parse(raw) : {}
  }
  catch {
    return {}
  }
}

function setDrafts(drafts: DraftStore) {
  if (import.meta.server) return
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(drafts))
  }
  catch {
    // localStorage unavailable
  }
}

/**
 * Composable to persist onboarding form drafts in localStorage.
 *
 * Usage in a step component:
 *   const { saveDraft, loadDraft, clearDraft } = useOnboardingDraft('step1')
 *
 *   onMounted(() => {
 *     const saved = loadDraft()
 *     if (saved) Object.assign(form, saved)
 *   })
 *
 *   watch(form, () => saveDraft(toRaw(form)), { deep: true })
 *
 *   function onSubmit() { clearDraft(); emit('submit', ...) }
 */
export function useOnboardingDraft(stepKey: string) {
  function saveDraft(data: Record<string, unknown>) {
    const drafts = getDrafts()
    drafts[stepKey] = JSON.parse(JSON.stringify(data))
    setDrafts(drafts)
  }

  function loadDraft<T = Record<string, unknown>>(): T | null {
    const drafts = getDrafts()
    return (drafts[stepKey] as T) ?? null
  }

  function clearDraft() {
    const drafts = getDrafts()
    const { [stepKey]: _, ...rest } = drafts
    setDrafts(rest)
  }

  return { saveDraft, loadDraft, clearDraft }
}

/** Clear ALL onboarding drafts (call after onboarding completion) */
export function clearAllOnboardingDrafts() {
  if (import.meta.server) return
  try {
    localStorage.removeItem(STORAGE_KEY)
  }
  catch {
    // ignore
  }
}
