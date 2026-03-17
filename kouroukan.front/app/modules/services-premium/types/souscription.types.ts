import type { BaseEntity } from '~/types/common'

export interface Souscription extends BaseEntity {
  serviceParentId: number
  serviceParentNom?: string
  parentId: number
  parentNom?: string
  dateDebut: string
  dateFin: string | null
  statutSouscription: string
  montantPaye: number
  renouvellementAuto: boolean
  dateProchainRenouvellement: string | null
}

export interface CreateSouscriptionPayload {
  serviceParentId: number
  parentId: number
  dateDebut: string
  dateFin?: string | null
  statutSouscription: string
  montantPaye: number
  renouvellementAuto: boolean
  dateProchainRenouvellement?: string | null
}

export interface UpdateSouscriptionPayload {
  id: number
  serviceParentId?: number
  parentId?: number
  dateDebut?: string
  dateFin?: string | null
  statutSouscription?: string
  montantPaye?: number
  renouvellementAuto?: boolean
  dateProchainRenouvellement?: string | null
}

export interface SouscriptionFilters {
  search?: string
  serviceParentId?: number
  statutSouscription?: string
  dateFrom?: string
  dateTo?: string
}

export const STATUTS_SOUSCRIPTION = [
  'Active',
  'Expiree',
  'Resiliee',
  'Essai',
] as const
