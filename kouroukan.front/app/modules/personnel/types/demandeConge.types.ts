import type { BaseEntity } from '~/types/common'

export interface DemandeConge extends BaseEntity {
  enseignantId: number
  enseignantNom?: string
  dateDebut: string
  dateFin: string
  motif: string
  statutDemande: string
  pieceJointeUrl: string | null
  commentaireValidateur: string | null
  validateurId: number | null
  validateurNom?: string
  dateValidation: string | null
  impactPaie: boolean
  typeId: number
  typeName: string
}

export interface CreateDemandeCongePayload {
  enseignantId: number
  dateDebut: string
  dateFin: string
  motif: string
  statutDemande: string
  pieceJointeUrl?: string
  impactPaie: boolean
  typeId: number
}

export interface UpdateDemandeCongePayload {
  id: number
  enseignantId?: number
  dateDebut?: string
  dateFin?: string
  motif?: string
  statutDemande?: string
  pieceJointeUrl?: string
  commentaireValidateur?: string
  validateurId?: number
  dateValidation?: string
  impactPaie?: boolean
  typeId?: number
}

export interface DemandeCongeFilters {
  search?: string
  typeId?: number
  statutDemande?: string
  enseignantId?: number
  dateFrom?: string
  dateTo?: string
}

export interface DemandeCongeType {
  id: number
  name: string
}

export const STATUTS_DEMANDE_CONGE = [
  'Soumise',
  'ApprouveeN1',
  'ApprouveeDirection',
  'Refusee',
] as const
