import type { BaseEntity } from '~/types/common'

export interface Bulletin extends BaseEntity {
  eleveId: number
  eleveNom?: string
  classeId: number
  classeName?: string
  trimestre: number
  anneeScolaireId: number
  anneeScolaireLibelle?: string
  moyenneGenerale: number
  rang: number | null
  appreciation: string | null
  estPublie: boolean
  dateGeneration: string
  cheminFichierPdf: string | null
}

export interface CreateBulletinPayload {
  eleveId: number
  classeId: number
  trimestre: number
  anneeScolaireId: number
  moyenneGenerale: number
  rang?: number
  appreciation?: string
  estPublie: boolean
}

export interface UpdateBulletinPayload {
  id: number
  eleveId?: number
  classeId?: number
  trimestre?: number
  anneeScolaireId?: number
  moyenneGenerale?: number
  rang?: number | null
  appreciation?: string | null
  estPublie?: boolean
}

export interface BulletinFilters {
  search?: string
  classeId?: number
  trimestre?: number
  anneeScolaireId?: number
  estPublie?: boolean
  dateFrom?: string
  dateTo?: string
}

export const TRIMESTRES_BULLETIN = [1, 2, 3] as const

export const MENTIONS = [
  'TresBien',
  'Bien',
  'AssezBien',
  'Passable',
  'Insuffisant',
  'TresInsuffisant',
] as const
