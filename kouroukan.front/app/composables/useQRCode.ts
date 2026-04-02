import { apiClient } from '~/core/api/client'

interface QRCodeData {
  qrCodeUrl: string
  code: string
}

interface ResolvedUser {
  id: number
  nom: string
  prenom: string
  role: string
  email: string
}

export function useQRCode() {
  const loading = ref(false)
  const qrData = ref<QRCodeData | null>(null)
  const resolvedUser = ref<ResolvedUser | null>(null)
  const toast = useToast()
  const { t } = useI18n()

  async function getMyQR(): Promise<QRCodeData | null> {
    loading.value = true
    try {
      const response = await apiClient.get<QRCodeData>('/api/auth/qr/me')
      if (response.success && response.data) {
        qrData.value = response.data
        return response.data
      }
      return null
    }
    catch {
      toast.add({ title: t('common.fetchError', 'Erreur lors du chargement du QR code'), color: 'error' })
      return null
    }
    finally {
      loading.value = false
    }
  }

  async function resolveQR(code: string): Promise<ResolvedUser | null> {
    loading.value = true
    try {
      const response = await apiClient.get<ResolvedUser>(`/api/auth/qr/resolve/${code}`)
      if (response.success && response.data) {
        resolvedUser.value = response.data
        return response.data
      }
      return null
    }
    catch {
      toast.add({ title: t('common.fetchError', 'QR code invalide ou expiré'), color: 'error' })
      return null
    }
    finally {
      loading.value = false
    }
  }

  return {
    loading: computed(() => loading.value),
    qrData: computed(() => qrData.value),
    resolvedUser: computed(() => resolvedUser.value),
    getMyQR,
    resolveQR,
  }
}
