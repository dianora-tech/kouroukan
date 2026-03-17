/**
 * TEMPLATE DE TEST POUR STORE PINIA MODULE
 *
 * Ce fichier sert de modele pour tester chaque store de module.
 * Copier et adapter pour chaque entite de chaque module.
 *
 * Fichiers a creer (un par store) :
 *
 * ─── inscriptions ───
 * tests/unit/stores/inscriptions/eleve.store.spec.ts
 * tests/unit/stores/inscriptions/dossier-admission.store.spec.ts
 * tests/unit/stores/inscriptions/inscription.store.spec.ts (deja fait)
 * tests/unit/stores/inscriptions/annee-scolaire.store.spec.ts
 *
 * ─── pedagogie ───
 * tests/unit/stores/pedagogie/niveau-classe.store.spec.ts
 * tests/unit/stores/pedagogie/classe.store.spec.ts
 * tests/unit/stores/pedagogie/matiere.store.spec.ts
 * tests/unit/stores/pedagogie/salle.store.spec.ts
 * tests/unit/stores/pedagogie/seance.store.spec.ts
 * tests/unit/stores/pedagogie/cahier-textes.store.spec.ts
 *
 * ─── evaluations ───
 * tests/unit/stores/evaluations/evaluation.store.spec.ts
 * tests/unit/stores/evaluations/note.store.spec.ts
 * tests/unit/stores/evaluations/bulletin.store.spec.ts
 *
 * ─── presences ───
 * tests/unit/stores/presences/appel.store.spec.ts
 * tests/unit/stores/presences/absence.store.spec.ts
 * tests/unit/stores/presences/badgeage.store.spec.ts
 *
 * ─── finances ───
 * tests/unit/stores/finances/facture.store.spec.ts
 * tests/unit/stores/finances/paiement.store.spec.ts
 * tests/unit/stores/finances/depense.store.spec.ts
 * tests/unit/stores/finances/remuneration.store.spec.ts
 *
 * ─── personnel ───
 * tests/unit/stores/personnel/enseignant.store.spec.ts
 * tests/unit/stores/personnel/demande-conge.store.spec.ts
 *
 * ─── communication ───
 * tests/unit/stores/communication/message.store.spec.ts
 * tests/unit/stores/communication/notification.store.spec.ts
 * tests/unit/stores/communication/annonce.store.spec.ts
 *
 * ─── bde ───
 * tests/unit/stores/bde/association.store.spec.ts
 * tests/unit/stores/bde/evenement.store.spec.ts
 * tests/unit/stores/bde/membre-bde.store.spec.ts
 * tests/unit/stores/bde/depense-bde.store.spec.ts
 *
 * ─── documents ───
 * tests/unit/stores/documents/modele-document.store.spec.ts
 * tests/unit/stores/documents/document-genere.store.spec.ts
 * tests/unit/stores/documents/signature.store.spec.ts
 *
 * ─── services-premium ───
 * tests/unit/stores/services-premium/service-parent.store.spec.ts
 * tests/unit/stores/services-premium/souscription.store.spec.ts
 *
 * ─── support ───
 * tests/unit/stores/support/ticket.store.spec.ts
 * tests/unit/stores/support/suggestion.store.spec.ts
 * tests/unit/stores/support/article-aide.store.spec.ts
 * tests/unit/stores/support/conversation-ia.store.spec.ts
 *
 * ═══════════════════════════════════════
 * PATTERN STANDARD A SUIVRE :
 * ═══════════════════════════════════════
 *
 * import { describe, it, expect, vi, beforeEach } from 'vitest'
 * import { use{Entity}Store } from '~/modules/{module}/stores/{entity}.store'
 * import { mockPaginatedResponse, mockApiResponse } from '../../helpers/api-mock'
 *
 * vi.mock('~/core/api/client', () => ({
 *   apiClient: {
 *     get: vi.fn(),
 *     getPaginated: vi.fn(),
 *     post: vi.fn(),
 *     put: vi.fn(),
 *     delete: vi.fn(),
 *   },
 * }))
 *
 * describe('use{Entity}Store', () => {
 *   let store: ReturnType<typeof use{Entity}Store>
 *
 *   beforeEach(() => { vi.clearAllMocks(); store = use{Entity}Store() })
 *
 *   describe('etat initial', () => {
 *     it('demarre vide', () => { ... })
 *   })
 *
 *   describe('fetchAll()', () => {
 *     it('recupere les items et met a jour pagination', async () => { ... })
 *     it('passe loading a true pendant le chargement', async () => { ... })
 *   })
 *
 *   describe('fetchById()', () => {
 *     it('recupere un item par id', async () => { ... })
 *     it('retourne null si non trouve', async () => { ... })
 *   })
 *
 *   describe('create()', () => {
 *     it('cree un item et rafraichit la liste', async () => { ... })
 *     it('passe saving a true pendant la sauvegarde', async () => { ... })
 *   })
 *
 *   describe('update()', () => {
 *     it('met a jour un item', async () => { ... })
 *   })
 *
 *   describe('remove()', () => {
 *     it('supprime un item', async () => { ... })
 *   })
 *
 *   describe('setFilters() / resetFilters()', () => {
 *     it('met a jour les filtres et remet page a 1', () => { ... })
 *     it('reset les filtres', () => { ... })
 *   })
 * })
 */

export {}
