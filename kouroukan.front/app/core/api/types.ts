export interface ApiResponse<T> {
  success: boolean
  data: T
  message: string | null
  errors: string[] | null
  code?: string // e.g. 'CGU_NOT_ACCEPTED'
}

export interface PaginatedResult<T> {
  items: T[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

export interface PaginationParams {
  page?: number
  pageSize?: number
  search?: string
  orderBy?: string
  orderDirection?: 'asc' | 'desc'
}
