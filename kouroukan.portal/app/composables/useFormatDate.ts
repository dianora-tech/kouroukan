/**
 * Composable for locale-aware date formatting (Portal).
 * Uses the active i18n locale to format dates consistently.
 */

const LOCALE_MAP: Record<string, string> = {
  fr: 'fr-GN',
  en: 'en-US',
}

export function useFormatDate() {
  const { locale } = useI18n()

  function getLocale(): string {
    return LOCALE_MAP[locale.value] || locale.value
  }

  function formatDate(dateStr: string | null | undefined): string {
    if (!dateStr) return '—'
    try {
      const date = new Date(dateStr)
      if (isNaN(date.getTime())) return '—'
      return new Intl.DateTimeFormat(getLocale(), {
        day: 'numeric',
        month: 'long',
        year: 'numeric',
      }).format(date)
    }
    catch {
      return dateStr
    }
  }

  function formatDateShort(dateStr: string | null | undefined): string {
    if (!dateStr) return '—'
    try {
      const date = new Date(dateStr)
      if (isNaN(date.getTime())) return '—'
      return new Intl.DateTimeFormat(getLocale(), {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
      }).format(date)
    }
    catch {
      return dateStr
    }
  }

  return { formatDate, formatDateShort }
}
