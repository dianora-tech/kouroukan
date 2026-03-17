import type { BaseEntity } from '~/types/common'

export interface Paiement extends BaseEntity {
  factureId: number
  factureNumero?: string
  montantPaye: number
  datePaiement: string
  moyenPaiement: string
  referenceMobileMoney: string | null
  statutPaiement: string
  caissierId: number | null
  caissierNom?: string
  numeroRecu: string
  typeId: number
  typeName: string
}

export interface CreatePaiementPayload {
  factureId: number
  montantPaye: number
  datePaiement: string
  moyenPaiement: string
  referenceMobileMoney?: string | null
  statutPaiement: string
  caissierId?: number | null
  numeroRecu: string
  typeId: number
}

export interface UpdatePaiementPayload {
  id: number
  factureId?: number
  montantPaye?: number
  datePaiement?: string
  moyenPaiement?: string
  referenceMobileMoney?: string | null
  statutPaiement?: string
  caissierId?: number | null
  typeId?: number
}

export interface PaiementFilters {
  search?: string
  typeId?: number
  moyenPaiement?: string
  statutPaiement?: string
  dateFrom?: string
  dateTo?: string
}

export interface PaiementType {
  id: number
  name: string
}

export const STATUTS_PAIEMENT = [
  'EnAttente',
  'Confirme',
  'Echec',
  'Rembourse',
] as const

export const MOYENS_PAIEMENT = [
  'OrangeMoney',
  'SoutraMoney',
  'MTNMoMo',
  'Especes',
] as const
