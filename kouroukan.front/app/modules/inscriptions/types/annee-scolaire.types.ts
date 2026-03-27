import type { BaseEntity } from '~/types/common'

export type AnneeScolaireStatut = 'preparation' | 'active' | 'cloturee' | 'archivee'
export type TypePeriode = 'trimestre' | 'semestre'

export interface AnneeScolaire extends BaseEntity {
  code: string | null
  libelle: string
  dateDebut: string
  dateFin: string
  dateRentree: string | null
  description: string | null
  statut: AnneeScolaireStatut
  nombrePeriodes: number
  typePeriode: TypePeriode
}

export interface CreateAnneeScolairePayload {
  code?: string
  libelle: string
  dateDebut: string
  dateFin: string
  dateRentree?: string
  description?: string
  statut?: AnneeScolaireStatut
  nombrePeriodes?: number
  typePeriode?: TypePeriode
}

export interface UpdateAnneeScolairePayload {
  id: number
  code?: string
  libelle?: string
  dateDebut?: string
  dateFin?: string
  dateRentree?: string
  description?: string
  statut?: AnneeScolaireStatut
  nombrePeriodes?: number
  typePeriode?: TypePeriode
}

export interface AnneeScolaireFilters {
  search?: string
  statut?: AnneeScolaireStatut
  dateFrom?: string
  dateTo?: string
}
