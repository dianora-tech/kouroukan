import type { BaseEntity } from '~/types/common'

export interface Facture extends BaseEntity {
  eleveId: number
  eleveNom?: string
  parentId: number | null
  parentNom?: string
  anneeScolaireId: number
  anneeScolaireLibelle?: string
  montantTotal: number
  montantPaye: number
  solde: number
  dateEmission: string
  dateEcheance: string
  statutFacture: string
  numeroFacture: string
  typeId: number
  typeName: string
}

export interface CreateFacturePayload {
  eleveId: number
  parentId?: number | null
  anneeScolaireId: number
  montantTotal: number
  montantPaye?: number
  dateEmission: string
  dateEcheance: string
  statutFacture: string
  numeroFacture: string
  typeId: number
}

export interface UpdateFacturePayload {
  id: number
  eleveId?: number
  parentId?: number | null
  anneeScolaireId?: number
  montantTotal?: number
  montantPaye?: number
  dateEmission?: string
  dateEcheance?: string
  statutFacture?: string
  typeId?: number
}

export interface FactureFilters {
  search?: string
  typeId?: number
  anneeScolaireId?: number
  statutFacture?: string
  dateFrom?: string
  dateTo?: string
}

export interface FactureType {
  id: number
  name: string
}

export const STATUTS_FACTURE = [
  'Emise',
  'PartPaye',
  'Payee',
  'Echue',
  'Annulee',
] as const
