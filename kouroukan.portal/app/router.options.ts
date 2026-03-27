import type { RouterConfig } from '@nuxt/schema'

export default <RouterConfig>{
  scrollBehavior(_to, _from, savedPosition) {
    // Toujours scroller en haut de page lors d'une navigation
    if (savedPosition) {
      return savedPosition
    }
    return { top: 0, left: 0 }
  },
}
