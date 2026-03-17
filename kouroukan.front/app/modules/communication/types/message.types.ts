import type { BaseEntity } from '~/types/common'

export interface Message extends BaseEntity {
  expediteurId: number
  expediteurNom?: string
  destinataireId: number | null
  destinataireNom?: string
  sujet: string
  contenu: string
  estLu: boolean
  dateLecture: string | null
  groupeDestinataire: string | null
  typeId: number
  typeName?: string
}

export interface CreateMessagePayload {
  expediteurId: number
  destinataireId?: number
  sujet: string
  contenu: string
  groupeDestinataire?: string
  typeId: number
}

export interface UpdateMessagePayload {
  id: number
  destinataireId?: number | null
  sujet?: string
  contenu?: string
  estLu?: boolean
  dateLecture?: string | null
  groupeDestinataire?: string | null
  typeId?: number
}

export interface MessageFilters {
  search?: string
  typeId?: number
  estLu?: boolean
  expediteurId?: number
  destinataireId?: number
  groupeDestinataire?: string
  dateFrom?: string
  dateTo?: string
}

export const TYPES_MESSAGE = [
  'Personnel',
  'Groupe',
  'Diffusion',
] as const
