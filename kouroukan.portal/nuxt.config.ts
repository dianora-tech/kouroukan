export default defineNuxtConfig({
  compatibilityDate: '2025-01-01',

  future: {
    compatibilityVersion: 4
  },

  modules: [
    '@nuxt/ui',
    '@nuxt/content',
    '@nuxtjs/i18n',
    '@nuxtjs/seo',
    '@nuxt/image',
    '@vite-pwa/nuxt'
  ],

  css: ['~/assets/css/main.css'],

  app: {
    head: {
      htmlAttrs: { lang: 'fr' },
      meta: [
        { name: 'theme-color', content: '#16a34a' },
        { name: 'viewport', content: 'width=device-width, initial-scale=1' }
      ],
      link: [
        { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' }
      ]
    }
  },

  site: {
    url: process.env.SITE_URL || 'https://www.kouroukan.gn',
    name: 'Kouroukan',
    description: 'Plateforme de gestion d\'etablissement scolaire. Inscriptions, notes, paiements Mobile Money, bulletins, emplois du temps — tout en une plateforme. Concu pour la Guinee.',
    defaultLocale: 'fr'
  },

  ogImage: { enabled: false },

  i18n: {
    locales: [
      { code: 'fr', name: 'Francais', file: 'fr.json' },
      { code: 'en', name: 'English', file: 'en.json' }
    ],
    defaultLocale: 'fr',
    strategy: 'prefix_except_default',
    lazy: true,
    langDir: '../i18n/',
    bundle: {
      optimizeTranslationDirective: false
    }
  },

  content: {
    build: {
      markdown: {
        highlight: {
          theme: 'github-light'
        }
      }
    }
  },

  image: {
    quality: 70,
    formats: ['webp', 'avif']
  },

  pwa: {
    registerType: 'autoUpdate',
    manifest: {
      name: 'Kouroukan',
      short_name: 'Kouroukan',
      theme_color: '#16a34a',
      background_color: '#ffffff',
      display: 'standalone',
      icons: [
        { src: '/icons/icon-192x192.png', sizes: '192x192', type: 'image/png' },
        { src: '/icons/icon-512x512.png', sizes: '512x512', type: 'image/png' }
      ]
    },
    workbox: {
      navigateFallback: '/offline'
    }
  },

  nitro: {
    prerender: {
      routes: ['/'],
      crawlLinks: true
    },
    routeRules: {
      '/api/**': {
        proxy: `${process.env.API_BASE_URL || 'http://localhost:5000'}/api/**`
      }
    }
  },

  runtimeConfig: {
    public: {
      appUrl: process.env.APP_URL || 'http://localhost',
    },
  },

  routeRules: {
    '/inscription': { ssr: true },
    '/aide/**': { isr: true }
  }
})
