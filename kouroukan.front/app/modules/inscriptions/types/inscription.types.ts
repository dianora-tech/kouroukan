import type { BaseEntity } from '~/types/common'

export interface Inscription extends BaseEntity {
  eleveId: number
  eleveNom?: string
  classeId: number
  classeName?: string
  anneeScolaireId: number
  anneeScolaireLibelle?: string
  dateInscription: string
  montantInscription: number
  estPaye: boolean
  estRedoublant: boolean
  typeEtablissement: string | null
  serieBac: string | null
  statutInscription: string
  typeId: number
  typeName: string
}

export interface CreateInscriptionPayload {
  eleveId: number
  classeId: number
  anneeScolaireId: number
  dateInscription: string
  montantInscription: number
  estPaye: boolean
  estRedoublant: boolean
  typeEtablissement?: string
  serieBac?: string
  statutInscription: string
  typeId: number
}

export interface UpdateInscriptionPayload {
  id: number
  eleveId?: number
  classeId?: number
  anneeScolaireId?: number
  dateInscription?: string
  montantInscription?: number
  estPaye?: boolean
  estRedoublant?: boolean
  typeEtablissement?: string | null
  serieBac?: string | null
  statutInscription?: string
  typeId?: number
}

export interface InscriptionFilters {
  search?: string
  typeId?: number
  classeId?: number
  anneeScolaireId?: number
  statutInscription?: string
  estPaye?: boolean
  estRedoublant?: boolean
  typeEtablissement?: string
  dateFrom?: string
  dateTo?: string
}

export interface InscriptionType {
  id: number
  name: string
}

export const STATUTS_INSCRIPTION = [
  'EnAttente',
  'Validee',
  'Annulee',
] as const

export const TYPES_ETABLISSEMENT = [
  'Public',
  'PriveLaique',
  'PriveFrancoArabe',
  'Communautaire',
  'PriveCatholique',
  'PriveProtestant',
] as const

export const SERIES_BAC = ['SE', 'SM', 'SS'] as const
