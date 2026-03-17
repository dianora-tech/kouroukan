import { vi } from 'vitest'
import type { ApiResponse, PaginatedResult } from '~/core/api/types'

/**
 * Cree une reponse API paginee mock.
 */
export function mockPaginatedResponse<T>(
  items: T[],
  page = 1,
  pageSize = 20,
): ApiResponse<PaginatedResult<T>> {
  return {
    success: true,
    data: {
      items,
      totalCount: items.length,
      pageNumber: page,
      pageSize,
      totalPages: Math.ceil(items.length / pageSize),
      hasNextPage: false,
      hasPreviousPage: page > 1,
    },
    message: null,
    errors: null,
  }
}

/**
 * Cree une reponse API simple mock.
 */
export function mockApiResponse<T>(data: T, success = true): ApiResponse<T> {
  return {
    success,
    data,
    message: success ? null : 'Erreur',
    errors: success ? null : ['Erreur'],
  }
}

/**
 * Cree un mock de l'apiClient avec toutes les methodes.
 */
export function createApiClientMock() {
  return {
    get: vi.fn(),
    getPaginated: vi.fn(),
    post: vi.fn(),
    put: vi.fn(),
    delete: vi.fn(),
  }
}

/**
 * Entite de base avec les champs d'audit.
 */
export function baseEntity(overrides: Record<string, unknown> = {}) {
  return {
    id: 1,
    createdAt: '2025-01-01T00:00:00Z',
    updatedAt: null,
    createdBy: 'admin',
    updatedBy: null,
    ...overrides,
  }
}
