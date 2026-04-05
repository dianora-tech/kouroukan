import type { BaseEntity } from '~/types/common'

// ── Forfait ──────────────────────────────────────────────
export interface Forfait extends BaseEntity {
  nom: string
  type: string
  montantMensuel: number
  montantAnnuel: number
  dateEffet: string
  statut: string
}

export interface CreateForfaitPayload {
  nom: string
  type: string
  montantMensuel: number
  montantAnnuel: number
  dateEffet: string
}

export interface UpdateForfaitPayload {
  id: number
  nom?: string
  type?: string
  montantMensuel?: number
  montantAnnuel?: number
  dateEffet?: string
  statut?: string
}

export interface UpdateTarifPayload {
  forfaitId: number
  montantMensuel: number
  montantAnnuel: number
}

// ── Abonnement ───────────────────────────────────────────
export interface Abonnement extends BaseEntity {
  etablissementId: number
  forfaitId: number
  forfaitNom: string
  etablissementNom: string
  dateDebut: string
  dateFin: string
  statut: string
  montant: number
}

export interface CreateAbonnementPayload {
  etablissementId: number
  forfaitId: number
  dateDebut: string
  dateFin: string
}

export interface UpdateAbonnementPayload {
  id: number
  dateDebut?: string
  dateFin?: string
  statut?: string
}

// ── Geste Commercial ─────────────────────────────────────
export interface GesteCommercial extends BaseEntity {
  beneficiaire: string
  type: string
  montant: number
  raison: string
}

export interface CreateGesteCommercialPayload {
  beneficiaire: string
  type: string
  montant: number
  raison: string
}

export interface UpdateGesteCommercialPayload {
  id: number
  beneficiaire?: string
  type?: string
  montant?: number
  raison?: string
}

// ── Etablissement (admin) ────────────────────────────────
export interface EtablissementAdmin extends BaseEntity {
  name: string
  email: string | null
  phoneNumber: string | null
  address: string | null
  regionCode: string | null
  prefectureCode: string | null
  sousPrefectureCode: string | null
  regionName: string | null
  prefectureName: string | null
  sousPrefectureName: string | null
  modules: string[]
  isActive: boolean
  userCount: number
  directeurNom: string | null
}

export interface UpdateEtablissementAdminPayload {
  id: number
  name?: string
  email?: string
  phoneNumber?: string
  address?: string
  regionCode?: string
  prefectureCode?: string
  sousPrefectureCode?: string
}

// ── Contenu IA ───────────────────────────────────────────
export interface ContenuIA extends BaseEntity {
  rubrique: string
  titre: string
  contenu: string
  auteur: string
}

export interface CreateContenuIAPayload {
  rubrique: string
  titre: string
  contenu: string
}

export interface UpdateContenuIAPayload {
  id: number
  rubrique?: string
  titre?: string
  contenu?: string
}

// ── SMS Config (NimbaSMS) ────────────────────────────────
export interface SmsConfig {
  serviceId: string
  secretToken: string
  senderName: string
  solde: number
  smsRestants: number
  coutUnitaire: number
}

export interface UpdateSmsConfigPayload {
  serviceId: string
  secretToken?: string
  senderName: string
}

export interface SendTestSmsPayload {
  to: string
  message: string
}

export interface SmsEnvoi extends BaseEntity {
  destinataire: string
  message: string
  statut: string
  date: string
  cout: number
}

// ── Email Config ─────────────────────────────────────────
export interface EmailConfig {
  host: string
  port: number
  username: string
  password: string
  expediteurNom: string
  expediteurEmail: string
  tls: boolean
}

export interface UpdateEmailConfigPayload {
  host: string
  port: number
  username: string
  password?: string
  expediteurNom: string
  expediteurEmail: string
  tls: boolean
}

export interface SendTestEmailPayload {
  email: string
}

// ── Compte Mobile Money (admin) ──────────────────────────
export interface CompteAdmin extends BaseEntity {
  operateur: string
  numero: string
  solde: number
  statut: string
}

export interface CreateCompteAdminPayload {
  operateur: string
  numero: string
}

export interface UpdateCompteAdminPayload {
  id: number
  operateur?: string
  numero?: string
  statut?: string
}

export interface TransactionAdmin extends BaseEntity {
  operateur: string
  type: string
  montant: number
  reference: string
  date: string
  statut: string
}

// ── Filters ──────────────────────────────────────────────
export interface AdminFilters {
  search?: string
  type?: string
  statut?: string
  rubrique?: string
}
