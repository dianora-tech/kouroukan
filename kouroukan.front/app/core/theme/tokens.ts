export interface ThemeTokens {
  primary: string
  secondary: string
  accent: string
  moduleColors: Record<string, string>
}

export const MODULE_COLORS = {
  'inscriptions': '#16a34a',
  'pedagogie': '#2563eb',
  'evaluations': '#7c3aed',
  'presences': '#0891b2',
  'finances': '#dc2626',
  'personnel': '#ea580c',
  'communication': '#0d9488',
  'bde': '#d946ef',
  'documents': '#64748b',
  'services-premium': '#f59e0b',
  'support': '#6366f1',
} as const

export const defaultTheme: ThemeTokens = {
  primary: '#16a34a',
  secondary: '#dc2626',
  accent: '#f59e0b',
  moduleColors: { ...MODULE_COLORS },
}

export type ModuleSlug = keyof typeof MODULE_COLORS
