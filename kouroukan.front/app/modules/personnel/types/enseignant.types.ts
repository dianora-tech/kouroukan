import type { BaseEntity } from '~/types/common'

export interface Enseignant extends BaseEntity {
  matricule: string
  specialite: string
  dateEmbauche: string
  modeRemuneration: string
  montantForfait: number | null
  telephone: string
  email: string | null
  statutEnseignant: string
  soldeCongesAnnuel: number
  typeId: number
  typeName: string
}

export interface CreateEnseignantPayload {
  matricule: string
  specialite: string
  dateEmbauche: string
  modeRemuneration: string
  montantForfait?: number
  telephone: string
  email?: string
  statutEnseignant: string
  soldeCongesAnnuel: number
  typeId: number
}

export interface UpdateEnseignantPayload {
  id: number
  matricule?: string
  specialite?: string
  dateEmbauche?: string
  modeRemuneration?: string
  montantForfait?: number
  telephone?: string
  email?: string
  statutEnseignant?: string
  soldeCongesAnnuel?: number
  typeId?: number
}

export interface EnseignantFilters {
  search?: string
  typeId?: number
  statutEnseignant?: string
  modeRemuneration?: string
  dateFrom?: string
  dateTo?: string
}

export interface EnseignantType {
  id: number
  name: string
}

export const STATUTS_ENSEIGNANT = [
  'Actif',
  'EnConge',
  'Suspendu',
  'Inactif',
] as const

export const MODES_REMUNERATION = [
  'Forfait',
  'Heures',
  'Mixte',
] as const
