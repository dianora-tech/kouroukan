import type { BaseEntity } from '~/types/common'

export interface MembreBDE extends BaseEntity {
  name: string
  description: string | null
  associationId: number
  associationNom?: string
  eleveId: number
  eleveNom?: string
  roleBDE: string
  dateAdhesion: string
  montantCotisation: number | null
  estActif: boolean
}

export interface CreateMembreBDEPayload {
  name: string
  description?: string
  associationId: number
  eleveId: number
  roleBDE: string
  dateAdhesion: string
  montantCotisation?: number | null
  estActif: boolean
}

export interface UpdateMembreBDEPayload {
  id: number
  name?: string
  description?: string
  associationId?: number
  eleveId?: number
  roleBDE?: string
  dateAdhesion?: string
  montantCotisation?: number | null
  estActif?: boolean
}

export interface MembreBDEFilters {
  search?: string
  associationId?: number
  roleBDE?: string
  estActif?: boolean
}

export const ROLES_BDE = [
  'President',
  'Tresorier',
  'Secretaire',
  'RespPole',
  'Membre',
] as const
