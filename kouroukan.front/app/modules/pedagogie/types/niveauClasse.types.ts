import type { BaseEntity } from '~/types/common'

export interface NiveauClasse extends BaseEntity {
  name: string
  code: string
  ordre: number
  cycleEtude: string
  ageOfficielEntree: number | null
  ministereTutelle: string | null
  examenSortie: string | null
  tauxHoraireEnseignant: number | null
}

export interface CreateNiveauClassePayload {
  name: string
  code: string
  ordre: number
  cycleEtude: string
  ageOfficielEntree?: number
  ministereTutelle?: string
  examenSortie?: string
  tauxHoraireEnseignant?: number
}

export interface UpdateNiveauClassePayload {
  id: number
  name?: string
  code?: string
  ordre?: number
  cycleEtude?: string
  ageOfficielEntree?: number | null
  ministereTutelle?: string | null
  examenSortie?: string | null
  tauxHoraireEnseignant?: number | null
}

export interface NiveauClasseFilters {
  search?: string
  cycleEtude?: string
  ministereTutelle?: string
}

export const CYCLES_ETUDE = [
  'Prescolaire',
  'Primaire',
  'College',
  'Lycee',
  'ETFP_PostPrimaire',
  'ETFP_TypeA',
  'ETFP_TypeB',
  'ENF',
  'Universite',
] as const

export const MINISTERES_TUTELLE = [
  'MENA',
  'METFP-ET',
  'MESRS',
] as const

export const EXAMENS_SORTIE = [
  'CEE',
  'BEPC',
  'BU',
  'CQP',
  'BEP',
  'CAP',
  'BTS',
  'Licence',
  'Master',
  'Doctorat',
] as const
