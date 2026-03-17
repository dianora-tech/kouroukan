import type { BaseEntity } from '~/types/common'

export interface AnneeScolaire extends BaseEntity {
  libelle: string
  dateDebut: string
  dateFin: string
  estActive: boolean
}

export interface CreateAnneeScolairePayload {
  libelle: string
  dateDebut: string
  dateFin: string
  estActive: boolean
}

export interface UpdateAnneeScolairePayload {
  id: number
  libelle?: string
  dateDebut?: string
  dateFin?: string
  estActive?: boolean
}

export interface AnneeScolaireFilters {
  search?: string
  estActive?: boolean
  dateFrom?: string
  dateTo?: string
}
