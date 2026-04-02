import { useAuthStore } from '~/core/stores/auth.store'
import type { PermissionKey } from '~/core/auth/rbac'
import { useForfaitGating } from '~/composables/useForfaitGating'

export interface NavItem {
  slug: string
  label: string
  icon: string
  to: string
  permission?: PermissionKey
  color?: string
  locked?: boolean
  children?: Array<{
    label: string
    to: string
    icon: string
  }>
}

export function useNavigation() {
  const { t } = useI18n()
  const auth = useAuthStore()
  const localePath = useLocalePath()
  const { hasForfait, fetchForfaitStatus } = useForfaitGating()

  // Fetch forfait status on first use
  fetchForfaitStatus()

  // ── Admin Navigation ──
  const adminNav = computed<NavItem[]>(() => [
    {
      slug: 'admin-dashboard',
      label: t('nav.admin.dashboard'),
      icon: 'i-heroicons-chart-bar-square',
      to: localePath('/admin'),
    },
    {
      slug: 'admin-etablissements',
      label: t('nav.admin.etablissements'),
      icon: 'i-heroicons-building-office-2',
      to: localePath('/admin/etablissements'),
    },
    {
      slug: 'admin-enseignants',
      label: t('nav.admin.enseignants'),
      icon: 'i-heroicons-user-group',
      to: localePath('/admin/enseignants'),
    },
    {
      slug: 'admin-parents',
      label: t('nav.admin.parents'),
      icon: 'i-heroicons-users',
      to: localePath('/admin/parents'),
    },
    {
      slug: 'admin-annees-scolaires',
      label: t('nav.admin.anneesScolaires'),
      icon: 'i-heroicons-calendar-days',
      to: localePath('/admin/annees-scolaires'),
    },
    {
      slug: 'admin-niveaux-classes',
      label: t('nav.admin.niveauxClasses'),
      icon: 'i-heroicons-rectangle-group',
      to: localePath('/admin/niveaux-classes'),
    },
    {
      slug: 'admin-matieres',
      label: t('nav.admin.matieres'),
      icon: 'i-heroicons-book-open',
      to: localePath('/admin/matieres'),
      children: [
        { label: t('admin.matiere.title'), to: localePath('/admin/matieres'), icon: 'i-heroicons-book-open' },
        { label: t('admin.categorie.title'), to: localePath('/admin/categories-matieres'), icon: 'i-heroicons-tag' },
      ],
    },
    {
      slug: 'admin-forfaits',
      label: t('nav.admin.forfaits'),
      icon: 'i-heroicons-credit-card',
      to: localePath('/admin/forfaits'),
    },
    {
      slug: 'admin-sms',
      label: t('nav.admin.sms'),
      icon: 'i-heroicons-chat-bubble-bottom-center-text',
      to: localePath('/admin/sms'),
    },
    {
      slug: 'admin-email',
      label: t('nav.admin.email'),
      icon: 'i-heroicons-envelope',
      to: localePath('/admin/email'),
    },
    {
      slug: 'admin-paiements',
      label: t('nav.admin.paiements'),
      icon: 'i-heroicons-banknotes',
      to: localePath('/admin/paiements'),
    },
    {
      slug: 'admin-contenu-ia',
      label: t('nav.admin.contenuIA'),
      icon: 'i-heroicons-cpu-chip',
      to: localePath('/admin/contenu-ia'),
    },
    {
      slug: 'admin-utilisateurs',
      label: t('nav.admin.utilisateurs'),
      icon: 'i-heroicons-user-circle',
      to: localePath('/admin/utilisateurs'),
    },
  ])

  // ── Enseignant Navigation ──
  const enseignantNav = computed<NavItem[]>(() => [
    {
      slug: 'enseignant-dashboard',
      label: t('nav.enseignant.dashboard'),
      icon: 'i-heroicons-home',
      to: localePath('/enseignant'),
    },
    {
      slug: 'enseignant-profil',
      label: t('nav.enseignant.profil'),
      icon: 'i-heroicons-user-circle',
      to: localePath('/enseignant/profil'),
    },
    {
      slug: 'enseignant-competences',
      label: t('nav.enseignant.competences'),
      icon: 'i-heroicons-academic-cap',
      to: localePath('/enseignant/competences'),
    },
    {
      slug: 'enseignant-etablissements',
      label: t('nav.enseignant.etablissements'),
      icon: 'i-heroicons-building-office-2',
      to: localePath('/enseignant/etablissements'),
      locked: !hasForfait.value,
    },
    {
      slug: 'enseignant-emploi-du-temps',
      label: t('nav.enseignant.emploiDuTemps'),
      icon: 'i-heroicons-calendar',
      to: localePath('/enseignant/emploi-du-temps'),
      locked: !hasForfait.value,
    },
    {
      slug: 'enseignant-heures',
      label: t('nav.enseignant.heures'),
      icon: 'i-heroicons-clock',
      to: localePath('/enseignant/heures'),
      locked: !hasForfait.value,
    },
    {
      slug: 'enseignant-bulletins',
      label: t('nav.enseignant.bulletins'),
      icon: 'i-heroicons-document-text',
      to: localePath('/enseignant/bulletins'),
      locked: !hasForfait.value,
    },
    {
      slug: 'enseignant-forfait',
      label: t('nav.enseignant.forfait'),
      icon: 'i-heroicons-credit-card',
      to: localePath('/enseignant/forfait'),
    },
  ])

  // ── Famille Navigation ──
  const familleNav = computed<NavItem[]>(() => [
    {
      slug: 'famille-dashboard',
      label: t('nav.famille.dashboard'),
      icon: 'i-heroicons-home',
      to: localePath('/famille'),
    },
    {
      slug: 'famille-enfants',
      label: t('nav.famille.enfants'),
      icon: 'i-heroicons-users',
      to: localePath('/famille/enfants'),
    },
    {
      slug: 'famille-notes',
      label: t('nav.famille.notes'),
      icon: 'i-heroicons-clipboard-document-check',
      to: localePath('/famille/notes'),
      locked: !hasForfait.value,
    },
    {
      slug: 'famille-emploi-du-temps',
      label: t('nav.famille.emploiDuTemps'),
      icon: 'i-heroicons-calendar',
      to: localePath('/famille/emploi-du-temps'),
      locked: !hasForfait.value,
    },
    {
      slug: 'famille-absences',
      label: t('nav.famille.absences'),
      icon: 'i-heroicons-clock',
      to: localePath('/famille/absences'),
      locked: !hasForfait.value,
    },
    {
      slug: 'famille-communication',
      label: t('nav.famille.communication'),
      icon: 'i-heroicons-chat-bubble-left-right',
      to: localePath('/famille/communication'),
      locked: !hasForfait.value,
    },
    {
      slug: 'famille-paiements',
      label: t('nav.famille.paiements'),
      icon: 'i-heroicons-banknotes',
      to: localePath('/famille/paiements'),
      locked: !hasForfait.value,
    },
    {
      slug: 'famille-documents',
      label: t('nav.famille.documents'),
      icon: 'i-heroicons-document-text',
      to: localePath('/famille/documents'),
      locked: !hasForfait.value,
    },
    {
      slug: 'famille-options',
      label: t('nav.famille.options'),
      icon: 'i-heroicons-squares-plus',
      to: localePath('/famille/options'),
    },
    {
      slug: 'famille-forfait',
      label: t('nav.famille.forfait'),
      icon: 'i-heroicons-credit-card',
      to: localePath('/famille/forfait'),
    },
  ])

  // ── Établissement Navigation ──
  const etablissementNav = computed<NavItem[]>(() => {
    const items: NavItem[] = [
      {
        slug: 'dashboard',
        label: t('nav.dashboard'),
        icon: 'i-heroicons-home',
        to: localePath('/'),
      },
    ]

    const modules = [
      { slug: 'inscriptions', label: 'nav.inscriptions', icon: 'i-heroicons-user-plus', permission: 'inscriptions:read' as PermissionKey },
      { slug: 'pedagogie', label: 'nav.pedagogie', icon: 'i-heroicons-academic-cap', permission: 'pedagogie:read' as PermissionKey },
      { slug: 'evaluations', label: 'nav.evaluations', icon: 'i-heroicons-clipboard-document-check', permission: 'evaluations:read' as PermissionKey },
      { slug: 'presences', label: 'nav.presences', icon: 'i-heroicons-clock', permission: 'presences:read' as PermissionKey },
      { slug: 'finances', label: 'nav.finances', icon: 'i-heroicons-banknotes', permission: 'finances:read' as PermissionKey },
      { slug: 'personnel', label: 'nav.personnel', icon: 'i-heroicons-user-group', permission: 'personnel:read' as PermissionKey },
      { slug: 'communication', label: 'nav.communication', icon: 'i-heroicons-chat-bubble-left-right', permission: 'communication:read' as PermissionKey },
      { slug: 'bde', label: 'nav.bde', icon: 'i-heroicons-sparkles', permission: 'bde:read' as PermissionKey },
      { slug: 'documents', label: 'nav.documents', icon: 'i-heroicons-document-text', permission: 'documents:read' as PermissionKey },
      { slug: 'services-premium', label: 'nav.servicesPremium', icon: 'i-heroicons-star', permission: 'services-premium:read' as PermissionKey },
    ]

    for (const mod of modules) {
      if (auth.hasPermission(mod.permission)) {
        items.push({
          slug: mod.slug,
          label: t(mod.label),
          icon: mod.icon,
          to: localePath(`/${mod.slug}`),
          permission: mod.permission,
        })
      }
    }

    // Support
    if (auth.hasPermission('support:read')) {
      items.push({
        slug: 'support',
        label: t('nav.support'),
        icon: 'i-heroicons-lifebuoy',
        to: localePath('/support'),
        permission: 'support:read',
        children: [
          { label: t('nav.supportTickets'), to: localePath('/support/tickets'), icon: 'i-heroicons-ticket' },
          { label: t('nav.supportSuggestions'), to: localePath('/support/suggestions'), icon: 'i-heroicons-light-bulb' },
          { label: t('nav.supportAide'), to: localePath('/support/aide'), icon: 'i-heroicons-book-open' },
          { label: t('nav.supportAideIA'), to: localePath('/support/aide-ia'), icon: 'i-heroicons-cpu-chip' },
        ],
      })
    }

    // Paramètres
    if (auth.hasPermission('settings:manage') || auth.hasPermission('users:manage')) {
      items.push({
        slug: 'parametres',
        label: t('nav.settings'),
        icon: 'i-heroicons-cog-6-tooth',
        to: localePath('/parametres'),
        children: [
          { label: t('nav.establishment'), to: localePath('/parametres/etablissement'), icon: 'i-heroicons-building-office-2' },
          ...(auth.hasPermission('users:manage')
            ? [{ label: t('nav.users'), to: localePath('/parametres/utilisateurs'), icon: 'i-heroicons-users' }]
            : []),
        ],
      })
    }

    return items
  })

  // ── Determine which nav to use ──
  const currentSpace = computed<'admin' | 'enseignant' | 'famille' | 'etablissement'>(() => {
    if (auth.hasRole('super_admin')) return 'admin'
    if (auth.hasRole('enseignant')) return 'enseignant'
    if (auth.hasRole('parent') || auth.hasRole('eleve')) return 'famille'
    return 'etablissement'
  })

  const navItems = computed<NavItem[]>(() => {
    switch (currentSpace.value) {
      case 'admin': return adminNav.value
      case 'enseignant': return enseignantNav.value
      case 'famille': return familleNav.value
      default: return etablissementNav.value
    }
  })

  const spaceLabel = computed(() => {
    switch (currentSpace.value) {
      case 'admin': return 'Kouroukan Admin'
      case 'enseignant': return 'Kouroukan Enseignant'
      case 'famille': return 'Kouroukan Famille'
      default: return 'Kouroukan'
    }
  })

  const showCompanySwitcher = computed(() => currentSpace.value === 'etablissement')

  return {
    navItems,
    currentSpace,
    spaceLabel,
    showCompanySwitcher,
  }
}
