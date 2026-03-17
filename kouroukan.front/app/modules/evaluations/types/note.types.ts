import type { BaseEntity } from '~/types/common'

export interface Note extends BaseEntity {
  evaluationId: number
  evaluationTypeName?: string
  eleveId: number
  eleveNom?: string
  valeur: number
  commentaire: string | null
  dateSaisie: string
  matiereName?: string
  classeName?: string
  noteMaximale?: number
}

export interface CreateNotePayload {
  evaluationId: number
  eleveId: number
  valeur: number
  commentaire?: string
  dateSaisie: string
}

export interface UpdateNotePayload {
  id: number
  evaluationId?: number
  eleveId?: number
  valeur?: number
  commentaire?: string | null
  dateSaisie?: string
}

export interface NoteFilters {
  search?: string
  evaluationId?: number
  eleveId?: number
  classeId?: number
  matiereId?: number
  trimestre?: number
  anneeScolaireId?: number
  dateFrom?: string
  dateTo?: string
}
