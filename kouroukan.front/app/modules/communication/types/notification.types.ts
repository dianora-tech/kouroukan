import type { BaseEntity } from '~/types/common'

export interface Notification extends BaseEntity {
  destinatairesIds: string
  contenu: string
  canal: string
  estEnvoyee: boolean
  dateEnvoi: string | null
  lienAction: string | null
  typeId: number
  typeName?: string
}

export interface CreateNotificationPayload {
  destinatairesIds: string
  contenu: string
  canal: string
  lienAction?: string
  typeId: number
}

export interface UpdateNotificationPayload {
  id: number
  destinatairesIds?: string
  contenu?: string
  canal?: string
  estEnvoyee?: boolean
  dateEnvoi?: string | null
  lienAction?: string | null
  typeId?: number
}

export interface NotificationFilters {
  search?: string
  typeId?: number
  canal?: string
  estEnvoyee?: boolean
  dateFrom?: string
  dateTo?: string
}

export const CANAUX_NOTIFICATION = [
  'Push',
  'SMS',
  'Email',
  'InApp',
] as const

export const TYPES_NOTIFICATION = [
  'Absence',
  'Note',
  'Paiement',
  'Evenement',
  'Alerte',
  'Systeme',
] as const
