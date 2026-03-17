import type { BaseEntity } from '~/types/common'

export interface DossierAdmission extends BaseEntity {
  eleveId: number
  eleveNom?: string
  anneeScolaireId: number
  anneeScolaireLibelle?: string
  statutDossier: string
  etapeActuelle: string
  dateDemande: string
  dateDecision: string | null
  motifRefus: string | null
  scoringInterne: number | null
  commentaires: string | null
  responsableAdmissionId: number | null
  responsableAdmissionNom?: string
  typeId: number
  typeName: string
}

export interface CreateDossierAdmissionPayload {
  eleveId: number
  anneeScolaireId: number
  statutDossier: string
  etapeActuelle: string
  dateDemande: string
  dateDecision?: string
  motifRefus?: string
  scoringInterne?: number
  commentaires?: string
  responsableAdmissionId?: number
  typeId: number
}

export interface UpdateDossierAdmissionPayload {
  id: number
  eleveId?: number
  anneeScolaireId?: number
  statutDossier?: string
  etapeActuelle?: string
  dateDemande?: string
  dateDecision?: string | null
  motifRefus?: string | null
  scoringInterne?: number | null
  commentaires?: string | null
  responsableAdmissionId?: number | null
  typeId?: number
}

export interface DossierAdmissionFilters {
  search?: string
  typeId?: number
  statutDossier?: string
  anneeScolaireId?: number
  dateFrom?: string
  dateTo?: string
}

export interface DossierAdmissionType {
  id: number
  name: string
}

export const STATUTS_DOSSIER = [
  'Prospect',
  'PreInscrit',
  'EnEtude',
  'Convoque',
  'Admis',
  'Refuse',
  'ListeAttente',
] as const

export const ETAPES_WORKFLOW = [
  'DepotDossier',
  'VerificationPieces',
  'EtudeInterne',
  'Convocation',
  'TestAdmission',
  'DecisionFinale',
] as const
