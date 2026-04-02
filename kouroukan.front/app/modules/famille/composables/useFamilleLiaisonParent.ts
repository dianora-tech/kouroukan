import { useLiaisonParentStore } from '../stores/liaison-parent.store'

export function useFamilleLiaisonParent() {
  const store = useLiaisonParentStore()
  const toast = useToast()
  const { t } = useI18n()

  const items = computed(() => store.items)
  const loading = computed(() => store.loading)
  const isEmpty = computed(() => store.isEmpty)

  async function fetchAll(): Promise<void> {
    try {
      await store.fetchAll()
    }
    catch {
      toast.add({ title: t('famille.enfants.fetchError'), color: 'error' })
    }
  }

  return {
    items,
    loading,
    isEmpty,
    fetchAll,
  }
}
