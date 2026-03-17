import type { BaseEntity } from '~/types/common'

export interface Classe extends BaseEntity {
  name: string
  niveauClasseId: number
  niveauClasseName?: string
  niveauClasseCode?: string
  capacite: number
  anneeScolaireId: number
  anneeScolaireLibelle?: string
  enseignantPrincipalId: number | null
  enseignantPrincipalNom?: string
  effectif: number
}

export interface CreateClassePayload {
  name: string
  niveauClasseId: number
  capacite: number
  anneeScolaireId: number
  enseignantPrincipalId?: number
  effectif: number
}

export interface UpdateClassePayload {
  id: number
  name?: string
  niveauClasseId?: number
  capacite?: number
  anneeScolaireId?: number
  enseignantPrincipalId?: number | null
  effectif?: number
}

export interface ClasseFilters {
  search?: string
  niveauClasseId?: number
  anneeScolaireId?: number
}
