export interface BaseEntity {
  id: number
  createdAt: string
  updatedAt: string | null
  createdBy: string | null
  updatedBy: string | null
}

export interface SelectOption {
  label: string
  value: string | number
}

export interface BreadcrumbItem {
  label: string
  to?: string
  icon?: string
}
