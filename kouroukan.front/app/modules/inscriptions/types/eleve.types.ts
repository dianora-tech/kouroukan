import type { BaseEntity } from '~/types/common'

export interface Eleve extends BaseEntity {
  firstName: string
  lastName: string
  dateNaissance: string
  lieuNaissance: string
  genre: string
  nationalite: string
  adresse: string | null
  photoUrl: string | null
  numeroMatricule: string
  niveauClasseId: number
  niveauClasseName?: string
  classeId: number | null
  classeName?: string
  parentId: number | null
  parentNom?: string
  statutInscription: string
}

export interface CreateElevePayload {
  firstName: string
  lastName: string
  dateNaissance: string
  lieuNaissance: string
  genre: string
  nationalite: string
  adresse?: string
  photoUrl?: string
  numeroMatricule: string
  niveauClasseId: number
  classeId?: number
  parentId?: number
  statutInscription: string
}

export interface UpdateElevePayload {
  id: number
  firstName?: string
  lastName?: string
  dateNaissance?: string
  lieuNaissance?: string
  genre?: string
  nationalite?: string
  adresse?: string | null
  photoUrl?: string | null
  numeroMatricule?: string
  niveauClasseId?: number
  classeId?: number | null
  parentId?: number | null
  statutInscription?: string
}

export interface EleveFilters {
  search?: string
  niveauClasseId?: number
  classeId?: number
  statutInscription?: string
  genre?: string
  dateFrom?: string
  dateTo?: string
}

export const STATUTS_INSCRIPTION_ELEVE = [
  'Prospect',
  'PreInscrit',
  'Inscrit',
  'Radie',
] as const

export const GENRES = ['M', 'F'] as const
