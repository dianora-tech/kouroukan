/**
 * Composable for locale-aware date formatting.
 * Uses the active i18n locale to format dates consistently across the app.
 *
 * Usage:
 *   const { formatDate, formatDateTime, formatDateShort, formatTime } = useFormatDate()
 *   formatDate('2025-09-15')       // fr → "15 septembre 2025"  |  en → "September 15, 2025"
 *   formatDateShort('2025-09-15')  // fr → "15/09/2025"         |  en → "09/15/2025"
 *   formatDateTime('2025-09-15T08:00:00') // includes time
 *   formatTime('2025-09-15T08:00:00')     // fr → "08:00"       |  en → "8:00 AM"
 */

// Map i18n locale codes to BCP 47 locale identifiers
const LOCALE_MAP: Record<string, string> = {
  fr: 'fr-GN',
  en: 'en-US',
}

export function useFormatDate() {
  const { locale } = useI18n()

  function getLocale(): string {
    return LOCALE_MAP[locale.value] || locale.value
  }

  /**
   * Format a date string as a full readable date.
   * e.g. "15 septembre 2025" (fr) / "September 15, 2025" (en)
   */
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

  /**
   * Format a date string as a short date.
   * e.g. "15/09/2025" (fr) / "09/15/2025" (en)
   */
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

  /**
   * Format a date string with date + time.
   * e.g. "15 septembre 2025 à 08:00" (fr) / "September 15, 2025, 8:00 AM" (en)
   */
  function formatDateTime(dateStr: string | null | undefined): string {
    if (!dateStr) return '—'
    try {
      const date = new Date(dateStr)
      if (isNaN(date.getTime())) return '—'
      return new Intl.DateTimeFormat(getLocale(), {
        day: 'numeric',
        month: 'long',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
      }).format(date)
    }
    catch {
      return dateStr
    }
  }

  /**
   * Format a date string to show only time.
   * e.g. "08:00" (fr) / "8:00 AM" (en)
   */
  function formatTime(dateStr: string | null | undefined): string {
    if (!dateStr) return '—'
    try {
      const date = new Date(dateStr)
      if (isNaN(date.getTime())) return '—'
      return new Intl.DateTimeFormat(getLocale(), {
        hour: '2-digit',
        minute: '2-digit',
      }).format(date)
    }
    catch {
      return dateStr
    }
  }

  /**
   * Format a date string as medium (day + short month + year).
   * e.g. "15 sept. 2025" (fr) / "Sep 15, 2025" (en)
   */
  function formatDateMedium(dateStr: string | null | undefined): string {
    if (!dateStr) return '—'
    try {
      const date = new Date(dateStr)
      if (isNaN(date.getTime())) return '—'
      return new Intl.DateTimeFormat(getLocale(), {
        day: 'numeric',
        month: 'short',
        year: 'numeric',
      }).format(date)
    }
    catch {
      return dateStr
    }
  }

  return {
    formatDate,
    formatDateShort,
    formatDateTime,
    formatTime,
    formatDateMedium,
  }
}
