import type { BaseEntity } from '~/types/common'

export interface ModeleDocument extends BaseEntity {
  name: string
  description: string | null
  code: string
  contenu: string
  logoUrl: string | null
  couleurPrimaire: string | null
  textePiedPage: string | null
  estActif: boolean
  typeId: number
  typeName: string
}

export interface CreateModeleDocumentPayload {
  name: string
  description?: string | null
  code: string
  contenu: string
  logoUrl?: string | null
  couleurPrimaire?: string | null
  textePiedPage?: string | null
  estActif: boolean
  typeId: number
}

export interface UpdateModeleDocumentPayload {
  id: number
  name?: string
  description?: string | null
  code?: string
  contenu?: string
  logoUrl?: string | null
  couleurPrimaire?: string | null
  textePiedPage?: string | null
  estActif?: boolean
  typeId?: number
}

export interface ModeleDocumentFilters {
  search?: string
  typeId?: number
  estActif?: boolean
}

export interface ModeleDocumentType {
  id: number
  name: string
}

export const TYPES_MODELE_DOCUMENT = [
  'Bulletin de notes',
  'Releve de notes',
  'Attestation de scolarite',
  'Attestation de reussite',
  'Attestation de reussite CEE',
  'Attestation de reussite BEPC',
  'Attestation de reussite BU',
  'Certificat de fin d\'etudes',
  'Justificatif de depense',
  'Contrat d\'inscription',
  'Recu de paiement',
  'Convocation',
  'Attestation de travail',
  'Demande de conge',
] as const
