import type { BaseEntity } from '~/types/common'

export interface Evaluation extends BaseEntity {
  matiereId: number
  matiereName?: string
  classeId: number
  classeName?: string
  enseignantId: number
  enseignantNom?: string
  dateEvaluation: string
  coefficient: number
  noteMaximale: number
  trimestre: number
  anneeScolaireId: number
  anneeScolaireLibelle?: string
  typeId: number
  typeName: string
}

export interface CreateEvaluationPayload {
  matiereId: number
  classeId: number
  enseignantId: number
  dateEvaluation: string
  coefficient: number
  noteMaximale: number
  trimestre: number
  anneeScolaireId: number
  typeId: number
}

export interface UpdateEvaluationPayload {
  id: number
  matiereId?: number
  classeId?: number
  enseignantId?: number
  dateEvaluation?: string
  coefficient?: number
  noteMaximale?: number
  trimestre?: number
  anneeScolaireId?: number
  typeId?: number
}

export interface EvaluationFilters {
  search?: string
  typeId?: number
  matiereId?: number
  classeId?: number
  enseignantId?: number
  trimestre?: number
  anneeScolaireId?: number
  dateFrom?: string
  dateTo?: string
}

export interface EvaluationType {
  id: number
  name: string
}

export const TRIMESTRES = [1, 2, 3] as const

export const TYPES_EVALUATION = [
  'DevoirSurveille',
  'Interrogation',
  'ExamenTrimestriel',
  'ExamenSemestriel',
  'ExamenFinal',
  'TPTD',
  'CEE',
  'BEPC',
  'BaccalaureatUnique',
  'ConcoursDentree',
] as const
