import type { ApiResponse, PaginatedResult, PaginationParams } from './types'
import { useAuthStore } from '~/core/stores/auth.store'
import { useOfflineStore } from '~/core/stores/offline.store'

const MAX_RETRIES = 3
const RETRY_DELAY_MS = 1000

function delay(ms: number): Promise<void> {
  return new Promise(resolve => setTimeout(resolve, ms))
}

function buildQueryString(params: PaginationParams): string {
  const query = new URLSearchParams()
  if (params.page) query.set('page', String(params.page))
  if (params.pageSize) query.set('pageSize', String(params.pageSize))
  if (params.search) query.set('search', params.search)
  if (params.orderBy) query.set('orderBy', params.orderBy)
  if (params.orderDirection) query.set('orderDirection', params.orderDirection)
  const str = query.toString()
  return str ? `?${str}` : ''
}

/**
 * Extrait le message d'erreur lisible depuis une FetchError ou une erreur quelconque.
 * Le backend retourne toujours un ApiResponse avec un champ `message`.
 */
export function extractErrorMessage(error: unknown): string {
  // FetchError de $fetch — le body JSON est dans error.data
  const data = (error as { data?: { message?: string, errors?: string[] } })?.data
  if (data?.message) return data.message
  if (data?.errors?.length) return data.errors.join(', ')

  // Fallback : message HTTP standard
  const statusMessage = (error as { statusMessage?: string })?.statusMessage
  if (statusMessage) return statusMessage

  // Fallback : Error standard
  if (error instanceof Error && error.message) return error.message

  return 'Une erreur inattendue est survenue.'
}

/**
 * Affiche un toast d'erreur via useToast() de Nuxt UI.
 */
function showErrorToast(error: unknown): void {
  try {
    const toast = useToast()
    toast.add({
      title: extractErrorMessage(error),
      color: 'error',
      icon: 'i-heroicons-exclamation-triangle',
    })
  }
  catch {
    // toast non disponible (SSR)
  }
}

/**
 * Tente de rafraichir le token via /api/auth/refresh.
 * Retourne true si le refresh a reussi, false sinon.
 */
let refreshPromise: Promise<boolean> | null = null

async function tryRefreshToken(): Promise<boolean> {
  // Eviter plusieurs refresh simultanes
  if (refreshPromise) return refreshPromise

  refreshPromise = (async () => {
    try {
      const auth = useAuthStore()
      const response = await $fetch<{
        success: boolean
        data: { accessToken: string, refreshToken: string }
      }>('/api/auth/refresh', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: { refreshToken: auth.accessToken ?? '' },
      })

      if (response?.success && response.data?.accessToken) {
        auth.accessToken = response.data.accessToken
        try {
          const tokenCookie = useCookie('auth.token')
          tokenCookie.value = response.data.accessToken
        }
        catch {
          // cookie non disponible
        }
        return true
      }
      return false
    }
    catch {
      return false
    }
    finally {
      refreshPromise = null
    }
  })()

  return refreshPromise
}

async function fetchWithRetry<T>(
  url: string,
  options: Parameters<typeof $fetch>[1],
  retries = MAX_RETRIES,
): Promise<T> {
  for (let attempt = 0; attempt <= retries; attempt++) {
    try {
      return await $fetch<T>(url, options)
    }
    catch (error: unknown) {
      const status = (error as { statusCode?: number })?.statusCode

      // Sur 401, tenter un refresh du token (une seule fois)
      if (status === 401 && attempt === 0) {
        const refreshed = await tryRefreshToken()
        if (refreshed) {
          // Re-essayer avec le nouveau token
          const auth = useAuthStore()
          const newHeaders = {
            ...(options?.headers as Record<string, string> ?? {}),
            Authorization: `Bearer ${auth.accessToken}`,
          }
          try {
            return await $fetch<T>(url, { ...options, headers: newHeaders })
          }
          catch (retryError) {
            showErrorToast(retryError)
            throw retryError
          }
        }
        // Refresh echoue → deconnecter l'utilisateur
        try {
          const auth = useAuthStore()
          auth.$reset()
          await navigateTo('/connexion', { replace: true })
        }
        catch {
          // ignore
        }
        showErrorToast(error)
        throw error
      }

      // Don't retry client errors (4xx) except 408 (timeout) and 429 (rate limit)
      if (status && status >= 400 && status < 500 && status !== 408 && status !== 429) {
        showErrorToast(error)
        throw error
      }
      if (attempt === retries) {
        showErrorToast(error)
        throw error
      }
      await delay(RETRY_DELAY_MS * (attempt + 1))
    }
  }
  throw new Error('Unreachable')
}

function handleCguError(response: ApiResponse<unknown>): void {
  if (response.code === 'CGU_NOT_ACCEPTED') {
    navigateTo('/support/cgu')
  }
}

export class ApiClient {
  private getHeaders(): Record<string, string> {
    const headers: Record<string, string> = {
      'Content-Type': 'application/json',
    }

    let token: string | null = null

    // Lire le token depuis le store Pinia (persiste dans localStorage)
    try {
      const auth = useAuthStore()
      if (auth.accessToken) {
        token = auth.accessToken
      }
    }
    catch {
      // Store non disponible
    }

    if (token) {
      headers.Authorization = `Bearer ${token}`
    }

    return headers
  }

  async get<T>(url: string): Promise<ApiResponse<T>> {
    const offline = useOfflineStore()

    if (!offline.isOnline) {
      throw new Error('offline')
    }

    const response = await fetchWithRetry<ApiResponse<T>>(url, {
      method: 'GET',
      headers: this.getHeaders(),
    })

    handleCguError(response)
    return response
  }

  async getPaginated<T>(url: string, params: PaginationParams = {}): Promise<ApiResponse<PaginatedResult<T>>> {
    const fullUrl = `${url}${buildQueryString(params)}`
    return this.get<PaginatedResult<T>>(fullUrl)
  }

  async post<T>(url: string, body?: unknown): Promise<ApiResponse<T>> {
    const offline = useOfflineStore()

    if (!offline.isOnline) {
      offline.enqueue({ method: 'POST', url, body })
      return {
        success: true,
        data: null as T,
        message: 'queued_for_sync',
        errors: null,
      }
    }

    const response = await fetchWithRetry<ApiResponse<T>>(url, {
      method: 'POST',
      headers: this.getHeaders(),
      body,
    })

    handleCguError(response)
    return response
  }

  async put<T>(url: string, body?: unknown): Promise<ApiResponse<T>> {
    const offline = useOfflineStore()

    if (!offline.isOnline) {
      offline.enqueue({ method: 'PUT', url, body })
      return {
        success: true,
        data: null as T,
        message: 'queued_for_sync',
        errors: null,
      }
    }

    const response = await fetchWithRetry<ApiResponse<T>>(url, {
      method: 'PUT',
      headers: this.getHeaders(),
      body,
    })

    handleCguError(response)
    return response
  }

  async delete(url: string): Promise<ApiResponse<void>> {
    const offline = useOfflineStore()

    if (!offline.isOnline) {
      offline.enqueue({ method: 'DELETE', url })
      return {
        success: true,
        data: undefined as unknown as void,
        message: 'queued_for_sync',
        errors: null,
      }
    }

    const response = await fetchWithRetry<ApiResponse<void>>(url, {
      method: 'DELETE',
      headers: this.getHeaders(),
    })

    handleCguError(response)
    return response
  }
}

/** Singleton instance */
export const apiClient = new ApiClient()
