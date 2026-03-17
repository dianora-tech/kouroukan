import type { BaseEntity } from '~/types/common'

export interface Seance extends BaseEntity {
  matiereId: number
  matiereName?: string
  matiereCode?: string
  classeId: number
  classeName?: string
  enseignantId: number
  enseignantNom?: string
  salleId: number
  salleName?: string
  jourSemaine: number
  heureDebut: string
  heureFin: string
  anneeScolaireId: number
  anneeScolaireLibelle?: string
}

export interface CreateSeancePayload {
  matiereId: number
  classeId: number
  enseignantId: number
  salleId: number
  jourSemaine: number
  heureDebut: string
  heureFin: string
  anneeScolaireId: number
}

export interface UpdateSeancePayload {
  id: number
  matiereId?: number
  classeId?: number
  enseignantId?: number
  salleId?: number
  jourSemaine?: number
  heureDebut?: string
  heureFin?: string
  anneeScolaireId?: number
}

export interface SeanceFilters {
  search?: string
  classeId?: number
  enseignantId?: number
  jourSemaine?: number
  anneeScolaireId?: number
}

export const JOURS_SEMAINE = [
  { value: 1, label: 'Lundi' },
  { value: 2, label: 'Mardi' },
  { value: 3, label: 'Mercredi' },
  { value: 4, label: 'Jeudi' },
  { value: 5, label: 'Vendredi' },
  { value: 6, label: 'Samedi' },
] as const
