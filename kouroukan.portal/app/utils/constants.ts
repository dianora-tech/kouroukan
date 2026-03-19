import type { Module, PricingPlan, Testimonial, GeoRegion, GeoPrefecture, GeoSousPrefecture } from './types'

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
  { name: 'Boké', code: 'BOK' },
  { name: 'Faranah', code: 'FAR' },
  { name: 'Kankan', code: 'KAN' },
  { name: 'Kindia', code: 'KIN' },
  { name: 'Labé', code: 'LAB' },
  { name: 'Mamou', code: 'MAM' },
  { name: 'Nzérékoré', code: 'NZR' }
]

// Prefectures par region.
// Pour Conakry (CKY) : les "prefectures" sont des communes — pas de sous-prefectures.
export const GUINEA_PREFECTURES: Record<string, GeoPrefecture[]> = {
  CKY: [
    { name: 'Kaloum', code: 'KLM' },
    { name: 'Dixinn', code: 'DXN' },
    { name: 'Matam', code: 'MTM' },
    { name: 'Ratoma', code: 'RTM' },
    { name: 'Matoto', code: 'MTO' }
  ],
  BOK: [
    { name: 'Boké', code: 'BOK' },
    { name: 'Boffa', code: 'BFA' },
    { name: 'Fria', code: 'FRI' },
    { name: 'Gaoual', code: 'GAO' },
    { name: 'Koundara', code: 'KND' }
  ],
  FAR: [
    { name: 'Faranah', code: 'FAR' },
    { name: 'Dabola', code: 'DAB' },
    { name: 'Dinguiraye', code: 'DIN' },
    { name: 'Kissidougou', code: 'KIS' }
  ],
  KAN: [
    { name: 'Kankan', code: 'KAN' },
    { name: 'Kérouané', code: 'KER' },
    { name: 'Kouroussa', code: 'KOU' },
    { name: 'Mandiana', code: 'MAN' },
    { name: 'Siguiri', code: 'SIG' }
  ],
  KIN: [
    { name: 'Kindia', code: 'KIN' },
    { name: 'Coyah', code: 'COY' },
    { name: 'Dubréka', code: 'DUB' },
    { name: 'Forécariah', code: 'FOR' },
    { name: 'Télimélé', code: 'TEL' }
  ],
  LAB: [
    { name: 'Labé', code: 'LAB' },
    { name: 'Koubia', code: 'KBA' },
    { name: 'Lélouma', code: 'LEL' },
    { name: 'Mali', code: 'MAL' },
    { name: 'Tougué', code: 'TOU' }
  ],
  MAM: [
    { name: 'Mamou', code: 'MAM' },
    { name: 'Dalaba', code: 'DAL' },
    { name: 'Pita', code: 'PIT' }
  ],
  NZR: [
    { name: 'Nzérékoré', code: 'NZR' },
    { name: 'Beyla', code: 'BEY' },
    { name: 'Guéckédou', code: 'GUE' },
    { name: 'Lola', code: 'LOL' },
    { name: 'Macenta', code: 'MAC' },
    { name: 'Yomou', code: 'YOM' }
  ]
}

// Sous-prefectures par prefecture.
// Conakry n'a pas de sous-prefectures (zones speciales = communes uniquement).
export const GUINEA_SOUS_PREFECTURES: Record<string, GeoSousPrefecture[]> = {
  // ── Boké ──
  BOK: [
    { name: 'Boké Centre', code: 'BKC' }, { name: 'Binari', code: 'BNR' },
    { name: 'Camayenne', code: 'CMY' }, { name: 'Kanfarande', code: 'KNF' },
    { name: 'Kolaboui', code: 'KLB' }, { name: 'Mankountan', code: 'MNK' },
    { name: 'Sangarédi', code: 'SGR' }, { name: 'Tanéné', code: 'TNE' }
  ],
  BFA: [
    { name: 'Boffa Centre', code: 'BFC' }, { name: 'Baguinet', code: 'BGN' },
    { name: 'Douprou', code: 'DPR' }, { name: 'Koba', code: 'KBA' },
    { name: 'Lisso', code: 'LSO' }
  ],
  FRI: [
    { name: 'Fria Centre', code: 'FRC' }, { name: 'Banguignyi', code: 'BGY' },
    { name: 'Tormelin', code: 'TRM' }
  ],
  GAO: [
    { name: 'Gaoual Centre', code: 'GAC' }, { name: 'Foulamory', code: 'FLM' },
    { name: 'Koumbia', code: 'KMB' }, { name: 'Touba', code: 'TBA' },
    { name: 'Wendou-Mbour', code: 'WMB' }
  ],
  KND: [
    { name: 'Koundara Centre', code: 'KDC' }, { name: 'Beyla', code: 'BYK' },
    { name: 'Foulamori', code: 'FLK' }, { name: 'Guingan', code: 'GNG' },
    { name: 'Manda', code: 'MDA' }
  ],
  // ── Faranah ──
  FAR: [
    { name: 'Faranah Centre', code: 'FAC' }, { name: 'Banian', code: 'BNN' },
    { name: 'Beindou', code: 'BND' }, { name: 'Gbangbadou', code: 'GBD' },
    { name: 'Hérémakono', code: 'HRM' }, { name: 'Kobikoro', code: 'KBK' },
    { name: 'Sandénia', code: 'SDN' }, { name: 'Songoyah', code: 'SGY' },
    { name: 'Tiro', code: 'TRO' }
  ],
  DAB: [
    { name: 'Dabola Centre', code: 'DAC' }, { name: 'Arfamoussaya', code: 'ARM' },
    { name: 'Banko', code: 'BNK' }, { name: 'Binia', code: 'BNA' },
    { name: 'Bissikrima', code: 'BSK' }, { name: 'Dogomet', code: 'DGM' },
    { name: 'Ndéma', code: 'NDM' }
  ],
  DIN: [
    { name: 'Dinguiraye Centre', code: 'DNC' }, { name: 'Banora', code: 'DNB' },
    { name: 'Dialakoro', code: 'DLK' }, { name: 'Diontou', code: 'DNT' },
    { name: 'Gagnakali', code: 'GNK' }, { name: 'Kalinko', code: 'KLN' },
    { name: 'Lansanaya', code: 'LNS' }, { name: 'Mory', code: 'MRY' }
  ],
  KIS: [
    { name: 'Kissidougou Centre', code: 'KSC' }, { name: 'Albadaria', code: 'ALB' },
    { name: 'Bardou', code: 'BDU' }, { name: 'Firawa', code: 'FRW' },
    { name: 'Gbessoba', code: 'GBS' }, { name: 'Kondiadou', code: 'KDD' },
    { name: 'Koundou', code: 'KDO' }, { name: 'Sangardo', code: 'SGD' },
    { name: 'Yende-Millimou', code: 'YNM' }
  ],
  // ── Kankan ──
  KAN: [
    { name: 'Kankan Centre', code: 'KNC' }, { name: 'Baranama', code: 'BRN' },
    { name: 'Cissela', code: 'CSL' }, { name: 'Djankana', code: 'DJN' },
    { name: 'Fatako', code: 'FTK' }, { name: 'Koumana', code: 'KMN' },
    { name: 'Missamana', code: 'MSM' }, { name: 'Morodou', code: 'MRD' },
    { name: 'Tinti-Oulel', code: 'TNO' }
  ],
  KER: [
    { name: 'Kérouané Centre', code: 'KRC' }, { name: 'Damaro', code: 'DMR' },
    { name: 'Banankoro', code: 'BNK' }, { name: 'Komodou', code: 'KMD' },
    { name: 'Koumandou', code: 'KMU' }, { name: 'Sengbédou', code: 'SGB' }
  ],
  KOU: [
    { name: 'Kouroussa Centre', code: 'KOC' }, { name: 'Babila', code: 'BBL' },
    { name: 'Baro', code: 'BRO' }, { name: 'Doura', code: 'DRA' },
    { name: 'Gbéredou', code: 'GBR' }, { name: 'Sanguiana', code: 'SGA' }
  ],
  MAN: [
    { name: 'Mandiana Centre', code: 'MDC' }, { name: 'Bate-Nafadji', code: 'BTN' },
    { name: 'Franwalia', code: 'FRN' }, { name: 'Karifamoriah', code: 'KRF' },
    { name: 'Niantanina', code: 'NTN' }, { name: 'Saranko', code: 'SRK' },
    { name: 'Yankasso', code: 'YNK' }
  ],
  SIG: [
    { name: 'Siguiri Centre', code: 'SGC' }, { name: 'Doko', code: 'DKO' },
    { name: 'Kintinian', code: 'KNT' }, { name: 'Niagassola', code: 'NGS' },
    { name: 'Norassoba', code: 'NRS' }, { name: 'Sanguiana', code: 'SGS' },
    { name: 'Won', code: 'WON' }
  ],
  // ── Kindia ──
  KIN: [
    { name: 'Kindia Centre', code: 'KIC' }, { name: 'Bangouya', code: 'BGY' },
    { name: 'Damakania', code: 'DMK' }, { name: 'Friguiagbé', code: 'FRG' },
    { name: 'Kollet', code: 'KLL' }, { name: 'Mambia', code: 'MMB' }
  ],
  COY: [
    { name: 'Coyah Centre', code: 'COC' }, { name: 'Mafanco', code: 'MFC' },
    { name: 'Manéah', code: 'MNH' }, { name: 'Wonkifong', code: 'WKF' }
  ],
  DUB: [
    { name: 'Dubréka Centre', code: 'DBC' }, { name: 'Badi', code: 'BDI' },
    { name: 'Khorira', code: 'KHR' }, { name: 'Tanéné', code: 'TNN' }
  ],
  FOR: [
    { name: 'Forécariah Centre', code: 'FOC' }, { name: 'Benty', code: 'BTY' },
    { name: 'Farmoriah', code: 'FRM' }, { name: 'Kakossa', code: 'KKS' },
    { name: 'Kaback', code: 'KBK' }, { name: 'Maferenya', code: 'MFR' },
    { name: 'Sansale', code: 'SNS' }
  ],
  TEL: [
    { name: 'Télimélé Centre', code: 'TLC' }, { name: 'Ditinn', code: 'DTN' },
    { name: 'Gongoret', code: 'GGR' }, { name: 'Kollet', code: 'KLT' },
    { name: 'Lansanaya', code: 'LNA' }, { name: 'Mombeyah', code: 'MMY' }
  ],
  // ── Labé ──
  LAB: [
    { name: 'Labé Centre', code: 'LAC' }, { name: 'Alpha-Yaya', code: 'ALY' },
    { name: 'Beindou', code: 'BDL' }, { name: 'Daara', code: 'DAR' },
    { name: 'Dionfo', code: 'DNF' }, { name: 'Hafia', code: 'HAF' },
    { name: 'Kouramangui', code: 'KRM' }, { name: 'Noussy', code: 'NSY' },
    { name: 'Sannou', code: 'SNN' }, { name: 'Tountouroun', code: 'TNT' }
  ],
  KBA: [
    { name: 'Koubia Centre', code: 'KBC' }, { name: 'Hidayatou', code: 'HDY' },
    { name: 'Kindoye', code: 'KDY' }, { name: 'Missira', code: 'MSR' },
    { name: 'Timbi-Madina', code: 'TBM' }
  ],
  LEL: [
    { name: 'Lélouma Centre', code: 'LLC' }, { name: 'Diari', code: 'DAI' },
    { name: 'Guémé-Barambem', code: 'GMB' }, { name: 'Kébali', code: 'KBL' },
    { name: 'Linsan-Koura', code: 'LNK' }, { name: 'Manda', code: 'MDL' }
  ],
  MAL: [
    { name: 'Mali Centre', code: 'MLC' }, { name: 'Donghol-Touma', code: 'DGT' },
    { name: 'Fougou', code: 'FGO' }, { name: 'Gayah', code: 'GYH' },
    { name: 'Kansangui', code: 'KSG' }, { name: 'Laboya', code: 'LBY' },
    { name: 'Madina-Wora', code: 'MDW' }
  ],
  TOU: [
    { name: 'Tougué Centre', code: 'TGC' }, { name: 'Bouliwel', code: 'BLW' },
    { name: 'Dopili', code: 'DPL' }, { name: 'Fatako', code: 'FTO' },
    { name: 'Koba-Tarikol', code: 'KBT' }, { name: 'Kolangui', code: 'KLG' },
    { name: 'Linsan', code: 'LNT' }, { name: 'Loudou', code: 'LDD' }
  ],
  // ── Mamou ──
  MAM: [
    { name: 'Mamou Centre', code: 'MMC' }, { name: 'Bouliwel', code: 'BWT' },
    { name: 'Dinka', code: 'DNK' }, { name: 'Dounet', code: 'DNE' },
    { name: 'Gongoret', code: 'GGM' }, { name: 'Pelel', code: 'PPL' },
    { name: 'Porédaka', code: 'PRD' }, { name: 'Saramoussaya', code: 'SRM' },
    { name: 'Soyah', code: 'SYH' }, { name: 'Timbi-Touni', code: 'TBT' }
  ],
  DAL: [
    { name: 'Dalaba Centre', code: 'DLC' }, { name: 'Diari', code: 'DLD' },
    { name: 'Ditinn', code: 'DLT' }, { name: 'Kalinko', code: 'KLD' },
    { name: 'Koba', code: 'KBD' }, { name: 'Maci', code: 'MCI' },
    { name: 'Minthin', code: 'MTH' }, { name: 'Sannou', code: 'SNL' }
  ],
  PIT: [
    { name: 'Pita Centre', code: 'PTC' }, { name: 'Bantingnel', code: 'BNL' },
    { name: 'Bourouwal-Tappé', code: 'BTP' }, { name: 'Donghol-Sigon', code: 'DGS' },
    { name: 'Kéneya', code: 'KNY' }, { name: 'Kollet', code: 'KLP' },
    { name: 'Maci', code: 'MCP' }, { name: 'Pelel', code: 'PLP' },
    { name: 'Sannou', code: 'SNP' }
  ],
  // ── Nzérékoré ──
  NZR: [
    { name: 'Nzérékoré Centre', code: 'NZC' }, { name: 'Bossou', code: 'BSS' },
    { name: 'Bounouma', code: 'BNM' }, { name: 'Gouécké', code: 'GCK' },
    { name: 'Lainé', code: 'LNE' }, { name: 'Womé', code: 'WME' }
  ],
  BEY: [
    { name: 'Beyla Centre', code: 'BYC' }, { name: 'Blama', code: 'BLM' },
    { name: 'Daro', code: 'DRO' }, { name: 'Diassodou', code: 'DSS' },
    { name: 'Fouala', code: 'FLA' }, { name: 'Koyama', code: 'KYM' },
    { name: 'Sinko', code: 'SNK' }
  ],
  GUE: [
    { name: 'Guéckédou Centre', code: 'GCC' }, { name: 'Fangamadou', code: 'FGM' },
    { name: 'Kassadou', code: 'KSD' }, { name: 'Nongoa', code: 'NGA' },
    { name: 'Termessadou', code: 'TSD' }, { name: 'Yende-Millimou', code: 'YML' }
  ],
  LOL: [
    { name: 'Lola Centre', code: 'LOC' }, { name: 'Bossou', code: 'BSL' },
    { name: 'Foumbadou', code: 'FMB' }, { name: 'Kouankan', code: 'KNK' },
    { name: 'Koyama', code: 'KYL' }, { name: "N'Zoo", code: 'NZO' },
    { name: 'Tounkarata', code: 'TNK' }
  ],
  MAC: [
    { name: 'Macenta Centre', code: 'MCC' }, { name: 'Banie', code: 'BNE' },
    { name: 'Bofossou', code: 'BFS' }, { name: 'Gbakedou', code: 'GBK' },
    { name: 'Kouankan', code: 'KNM' }, { name: 'Panziazou', code: 'PNZ' },
    { name: 'Vasseredou', code: 'VSR' }, { name: 'Wonde', code: 'WND' }
  ],
  YOM: [
    { name: 'Yomou Centre', code: 'YMC' }, { name: 'Bolodou', code: 'BLD' },
    { name: 'Daro', code: 'DRY' }, { name: 'Diécké', code: 'DKE' },
    { name: 'Gama-Bérema', code: 'GMR' }, { name: 'Koule', code: 'KLE' }
  ]
}

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
