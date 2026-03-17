import type { Module, PricingPlan, Testimonial, GeoRegion } from './types'

export const MODULE_COLORS: Record<string, string> = {
  inscriptions: '#16a34a',
  pedagogie: '#2563eb',
  evaluations: '#7c3aed',
  presences: '#0891b2',
  finances: '#dc2626',
  personnel: '#ea580c',
  communication: '#0d9488',
  bde: '#d946ef',
  documents: '#64748b',
  'services-premium': '#f59e0b',
  support: '#6366f1'
}

export const MODULE_LIST: Module[] = [
  {
    slug: 'inscriptions',
    name: 'modules.inscriptions.name',
    icon: 'i-heroicons-user-plus',
    color: MODULE_COLORS.inscriptions,
    description: 'modules.inscriptions.description',
    longDescription: 'modules.inscriptions.longDescription'
  },
  {
    slug: 'pedagogie',
    name: 'modules.pedagogie.name',
    icon: 'i-heroicons-academic-cap',
    color: MODULE_COLORS.pedagogie,
    description: 'modules.pedagogie.description',
    longDescription: 'modules.pedagogie.longDescription'
  },
  {
    slug: 'evaluations',
    name: 'modules.evaluations.name',
    icon: 'i-heroicons-clipboard-document-check',
    color: MODULE_COLORS.evaluations,
    description: 'modules.evaluations.description',
    longDescription: 'modules.evaluations.longDescription'
  },
  {
    slug: 'presences',
    name: 'modules.presences.name',
    icon: 'i-heroicons-hand-raised',
    color: MODULE_COLORS.presences,
    description: 'modules.presences.description',
    longDescription: 'modules.presences.longDescription'
  },
  {
    slug: 'finances',
    name: 'modules.finances.name',
    icon: 'i-heroicons-banknotes',
    color: MODULE_COLORS.finances,
    description: 'modules.finances.description',
    longDescription: 'modules.finances.longDescription'
  },
  {
    slug: 'personnel',
    name: 'modules.personnel.name',
    icon: 'i-heroicons-user-group',
    color: MODULE_COLORS.personnel,
    description: 'modules.personnel.description',
    longDescription: 'modules.personnel.longDescription'
  },
  {
    slug: 'communication',
    name: 'modules.communication.name',
    icon: 'i-heroicons-chat-bubble-left-right',
    color: MODULE_COLORS.communication,
    description: 'modules.communication.description',
    longDescription: 'modules.communication.longDescription'
  },
  {
    slug: 'bde',
    name: 'modules.bde.name',
    icon: 'i-heroicons-trophy',
    color: MODULE_COLORS.bde,
    description: 'modules.bde.description',
    longDescription: 'modules.bde.longDescription'
  },
  {
    slug: 'documents',
    name: 'modules.documents.name',
    icon: 'i-heroicons-document-text',
    color: MODULE_COLORS.documents,
    description: 'modules.documents.description',
    longDescription: 'modules.documents.longDescription'
  },
  {
    slug: 'services-premium',
    name: 'modules.services-premium.name',
    icon: 'i-heroicons-star',
    color: MODULE_COLORS['services-premium'],
    description: 'modules.services-premium.description',
    longDescription: 'modules.services-premium.longDescription'
  },
  {
    slug: 'support',
    name: 'modules.support.name',
    icon: 'i-heroicons-lifebuoy',
    color: MODULE_COLORS.support,
    description: 'modules.support.description',
    longDescription: 'modules.support.longDescription'
  }
]

export const PRICING_PLANS: PricingPlan[] = [
  {
    key: 'starter',
    name: 'pricing.plans.starter.name',
    price: 'pricing.plans.starter.price',
    description: 'pricing.plans.starter.description',
    features: Array.from({ length: 6 }, (_, i) => `pricing.plans.starter.features[${i}]`)
  },
  {
    key: 'standard',
    name: 'pricing.plans.standard.name',
    price: 'pricing.plans.standard.price',
    description: 'pricing.plans.standard.description',
    features: Array.from({ length: 6 }, (_, i) => `pricing.plans.standard.features[${i}]`),
    recommended: true
  },
  {
    key: 'premium',
    name: 'pricing.plans.premium.name',
    price: 'pricing.plans.premium.price',
    description: 'pricing.plans.premium.description',
    features: Array.from({ length: 6 }, (_, i) => `pricing.plans.premium.features[${i}]`)
  }
]

export const TESTIMONIALS: Testimonial[] = [
  { key: 'director1', name: '', role: '', quote: '', category: 'directors' },
  { key: 'teacher1', name: '', role: '', quote: '', category: 'teachers' },
  { key: 'parent1', name: '', role: '', quote: '', category: 'parents' },
  { key: 'director2', name: '', role: '', quote: '', category: 'directors' },
  { key: 'admin1', name: '', role: '', quote: '', category: 'staff' }
]

export const PAIN_POINTS = ['paper', 'visibility', 'payments', 'communication'] as const

export const VALUE_PROPOSITIONS = ['academic', 'payments', 'mobile', 'offline', 'documents', 'security'] as const

export const GUINEA_REGIONS: GeoRegion[] = [
  { name: 'Conakry', code: 'CKY' },
  { name: 'Boke', code: 'BOK' },
  { name: 'Faranah', code: 'FAR' },
  { name: 'Kankan', code: 'KAN' },
  { name: 'Kindia', code: 'KIN' },
  { name: 'Labe', code: 'LAB' },
  { name: 'Mamou', code: 'MAM' },
  { name: 'Nzerekore', code: 'NZR' }
]

export const EDUCATION_CYCLES = [
  { key: 'preschool', levels: 'PS, MS, GS', age: '3-5', duration: '3 ans', diploma: '—', ministry: 'MENA' },
  { key: 'primary', levels: 'CP1, CP2, CE1, CE2, CM1, CM2', age: '6-11', duration: '6 ans', diploma: 'CEE', ministry: 'MENA' },
  { key: 'college', levels: '7eme, 8eme, 9eme, 10eme', age: '12-15', duration: '4 ans', diploma: 'BEPC', ministry: 'MENA' },
  { key: 'highschool', levels: '11eme, 12eme, Terminale', age: '16-18', duration: '3 ans', diploma: 'BU (SE/SM/SS)', ministry: 'MENA' },
  { key: 'university', levels: 'L1, L2, L3, M1, M2', age: '19+', duration: '3-7 ans', diploma: 'Licence/Master/Doctorat', ministry: 'MESRS' },
  { key: 'etfp', levels: 'Post-primaire, Type A, Type B', age: 'Variable', duration: '1-3 ans', diploma: 'CQP/BEP/CAP/BTS', ministry: 'METFP-ET' }
]

export const MODULE_SLUGS = MODULE_LIST.map(m => m.slug)

export const FALLBACK_STATS = {
  schools: 150,
  directors: 150,
  teachers: 800,
  parents: 5000,
  students: 12000,
  transactions: 25000,
  documents: 45000,
  isLive: false
}
