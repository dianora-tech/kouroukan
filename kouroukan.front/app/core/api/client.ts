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
      // Don't retry client errors (4xx) except 408 (timeout) and 429 (rate limit)
      if (status && status >= 400 && status < 500 && status !== 408 && status !== 429) {
        throw error
      }
      if (attempt === retries) throw error
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
    return {
      'Content-Type': 'application/json',
    }
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
        data: undefined as void,
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
