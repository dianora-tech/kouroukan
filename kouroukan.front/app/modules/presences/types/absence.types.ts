import type { BaseEntity } from '~/types/common'

export interface Absence extends BaseEntity {
  eleveId: number
  eleveNom?: string
  appelId: number | null
  dateAbsence: string
  heureDebut: string | null
  heureFin: string | null
  estJustifiee: boolean
  motifJustification: string | null
  pieceJointeUrl: string | null
  typeId: number
  typeName: string
}

export interface CreateAbsencePayload {
  eleveId: number
  appelId?: number | null
  dateAbsence: string
  heureDebut?: string | null
  heureFin?: string | null
  estJustifiee: boolean
  motifJustification?: string | null
  pieceJointeUrl?: string | null
  typeId: number
}

export interface UpdateAbsencePayload {
  id: number
  eleveId?: number
  appelId?: number | null
  dateAbsence?: string
  heureDebut?: string | null
  heureFin?: string | null
  estJustifiee?: boolean
  motifJustification?: string | null
  pieceJointeUrl?: string | null
  typeId?: number
}

export interface AbsenceFilters {
  search?: string
  typeId?: number
  eleveId?: number
  estJustifiee?: boolean
  dateFrom?: string
  dateTo?: string
}

export interface AbsenceType {
  id: number
  name: string
}

export const TYPES_ABSENCE = [
  'NonJustifiee',
  'Maladie',
  'EvenementFamilial',
  'Retard',
] as const
