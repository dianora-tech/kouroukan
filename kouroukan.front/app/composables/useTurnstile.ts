/**
 * Composable pour intégrer Cloudflare Turnstile (bot detection).
 *
 * Mode « managed » : invisible pour la majorité des utilisateurs,
 * challenge interactif uniquement si Cloudflare a un doute.
 *
 * @see https://developers.cloudflare.com/turnstile/
 */
export function useTurnstile() {
  const config = useRuntimeConfig()
  const siteKey = config.public.turnstileSiteKey as string

  const token = ref<string | null>(null)
  const error = ref<string | null>(null)
  const isReady = ref(false)

  let widgetId: string | null = null

  /**
   * Charge le script Turnstile si pas déjà présent dans le DOM.
   */
  function loadScript(): Promise<void> {
    return new Promise((resolve, reject) => {
      if (window.turnstile) {
        resolve()
        return
      }

      const existing = document.querySelector('script[src*="turnstile"]')
      if (existing) {
        existing.addEventListener('load', () => resolve())
        existing.addEventListener('error', () => reject(new Error('Turnstile script failed to load')))
        return
      }

      const script = document.createElement('script')
      script.src = 'https://challenges.cloudflare.com/turnstile/v0/api.js?render=explicit'
      script.async = true
      script.defer = true
      script.onload = () => resolve()
      script.onerror = () => reject(new Error('Turnstile script failed to load'))
      document.head.appendChild(script)
    })
  }

  /**
   * Rend le widget Turnstile dans l'élément cible.
   * @param container — sélecteur CSS ou HTMLElement
   */
  async function render(container: string | HTMLElement): Promise<void> {
    if (!siteKey) {
      // Pas de clé configurée → skip silencieusement (dev local)
      isReady.value = true
      return
    }

    try {
      await loadScript()
    }
    catch (e) {
      error.value = (e as Error).message
      return
    }

    // Attendre que window.turnstile soit disponible
    await new Promise<void>((resolve) => {
      const check = () => {
        if (window.turnstile) {
          resolve()
        }
        else {
          setTimeout(check, 50)
        }
      }
      check()
    })

    widgetId = window.turnstile.render(container, {
      'sitekey': siteKey,
      'callback': (t: string) => {
        token.value = t
        isReady.value = true
      },
      'error-callback': () => {
        error.value = 'Turnstile verification failed'
        isReady.value = false
      },
      'expired-callback': () => {
        token.value = null
        isReady.value = false
      },
      'appearance': 'interaction-only',
      'retry': 'auto',
      'retry-interval': 3000,
    })
  }

  /**
   * Réinitialise le widget pour obtenir un nouveau token.
   * Utile après un échec de soumission du formulaire.
   */
  function reset(): void {
    token.value = null
    error.value = null
    isReady.value = false

    if (widgetId !== null && window.turnstile) {
      window.turnstile.reset(widgetId)
    }
  }

  /**
   * Supprime le widget du DOM.
   */
  function remove(): void {
    if (widgetId !== null && window.turnstile) {
      window.turnstile.remove(widgetId)
      widgetId = null
    }
    token.value = null
    error.value = null
    isReady.value = false
  }

  // Cleanup automatique au démontage du composant
  onUnmounted(() => {
    remove()
  })

  return {
    /** Token Turnstile à envoyer au backend */
    token: readonly(token),
    /** Message d'erreur éventuel */
    error: readonly(error),
    /** true quand un token valide est disponible */
    isReady: readonly(isReady),
    /** Rend le widget dans un conteneur */
    render,
    /** Réinitialise le widget (après erreur de soumission) */
    reset,
    /** Supprime le widget */
    remove,
  }
}

/**
 * Déclaration globale du SDK Turnstile.
 */
declare global {
  interface Window {
    turnstile: {
      render: (container: string | HTMLElement, options: Record<string, unknown>) => string
      reset: (widgetId: string) => void
      remove: (widgetId: string) => void
    }
  }
}
