import type { BaseEntity } from '~/types/common'

export interface Depense extends BaseEntity {
  montant: number
  motifDepense: string
  categorie: string
  beneficiaireNom: string
  beneficiaireTelephone: string | null
  beneficiaireNIF: string | null
  statutDepense: string
  dateDemande: string
  dateValidation: string | null
  validateurId: number | null
  validateurNom?: string
  pieceJointeUrl: string | null
  numeroJustificatif: string
  typeId: number
  typeName: string
}

export interface CreateDepensePayload {
  montant: number
  motifDepense: string
  categorie: string
  beneficiaireNom: string
  beneficiaireTelephone?: string | null
  beneficiaireNIF?: string | null
  statutDepense: string
  dateDemande: string
  pieceJointeUrl?: string | null
  numeroJustificatif: string
  typeId: number
}

export interface UpdateDepensePayload {
  id: number
  montant?: number
  motifDepense?: string
  categorie?: string
  beneficiaireNom?: string
  beneficiaireTelephone?: string | null
  beneficiaireNIF?: string | null
  statutDepense?: string
  dateDemande?: string
  pieceJointeUrl?: string | null
  typeId?: number
}

export interface DepenseFilters {
  search?: string
  typeId?: number
  categorie?: string
  statutDepense?: string
  dateFrom?: string
  dateTo?: string
}

export interface DepenseType {
  id: number
  name: string
}

export const STATUTS_DEPENSE = [
  'Demande',
  'ValideN1',
  'ValideFinance',
  'ValideDirection',
  'Executee',
  'Archivee',
] as const

export const CATEGORIES_DEPENSE = [
  'Personnel',
  'Fournitures',
  'Maintenance',
  'Evenements',
  'BDE',
  'Equipements',
] as const
