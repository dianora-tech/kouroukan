// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2025-01-01',

  future: {
    compatibilityVersion: 4,
  },

  modules: [
    '@nuxt/ui',
    '@sidebase/nuxt-auth',
    'nuxt-security',
    '@nuxtjs/i18n',
    '@pinia/nuxt',
    'pinia-plugin-persistedstate/nuxt',
    '@vite-pwa/nuxt',
    '@nuxt/image',
  ],

  // --------------- Auth (sidebase) ---------------
  auth: {
    baseURL: '/api/auth',
    provider: {
      type: 'local',
      endpoints: {
        signIn: { path: '/login', method: 'post' },
        signOut: { path: '/logout', method: 'post' },
        getSession: { path: '/me', method: 'get' },
      },
      token: {
        signInResponseTokenPointer: '/data/accessToken',
        headerName: 'Authorization',
        type: 'Bearer',
        maxAgeInSeconds: 60 * 60 * 24, // 24h
      },
      session: {
        dataType: {
          id: 'number',
          firstName: 'string',
          lastName: 'string',
          email: 'string',
          roles: 'string[]',
          permissions: 'string[]',
          cguVersion: 'string | null',
          cguAcceptedAt: 'string | null',
        },
        dataResponsePointer: '/data',
      },
    },
    session: {
      enableRefreshOnWindowFocus: false,
      enableRefreshPeriodically: false, // On gere le refresh manuellement via apiClient
      enableOnNuxtReady: false, // Ne pas declencher getSession automatiquement au ready
    },
    globalAppMiddleware: {
      isEnabled: false, // We handle this manually in guards.ts
    },
  },

  // --------------- Security ---------------
  // TODO: Re-enable nuxt-security once hydration payload compatibility is resolved
  security: {
    headers: false,
    rateLimiter: false,
    requestSizeLimiter: false,
    xssValidator: false,
    corsHandler: false,
    allowedMethodsRestricter: false,
    csrf: false,
  },

  // --------------- i18n ---------------
  i18n: {
    locales: [
      { code: 'fr', name: 'Français', file: 'fr.json' },
      { code: 'en', name: 'English', file: 'en.json' },
    ],
    defaultLocale: 'fr',
    strategy: 'prefix_except_default',
    lazy: true,
    langDir: '../i18n/',
    detectBrowserLanguage: {
      useCookie: true,
      cookieKey: 'kouroukan_lang',
      cookieSecure: false,
      cookieDomain: '',
      cookieMaxAge: 365 * 24 * 60 * 60,
      fallbackLocale: 'fr',
      alwaysRedirect: true,
      redirectOn: 'all',
    },
  },

  // --------------- Pinia ---------------
  pinia: {
    storesDirs: ['./app/core/stores/**', './app/modules/**/stores/**'],
  },

  // --------------- Auto-imports ---------------
  imports: {
    dirs: ['./app/modules/**/composables/**'],
  },

  // --------------- Components ---------------
  components: {
    dirs: [
      { path: '~/components', pathPrefix: false },
      '~/shared/components',
      '~/modules/**/components',
    ],
  },

  // --------------- PWA ---------------
  pwa: {
    registerType: 'autoUpdate',
    manifest: {
      name: 'Kouroukan',
      short_name: 'Kouroukan',
      description: 'Plateforme de gestion d\'établissement scolaire',
      theme_color: '#16a34a',
      background_color: '#ffffff',
      display: 'standalone',
      orientation: 'portrait',
      icons: [
        { src: '/icons/icon-192x192.png', sizes: '192x192', type: 'image/png' },
        { src: '/icons/icon-512x512.png', sizes: '512x512', type: 'image/png' },
      ],
    },
    workbox: {
      navigateFallback: '/',
      globPatterns: ['**/*.{js,css,html,ico,png,svg,woff2}'],
      runtimeCaching: [
        {
          urlPattern: /^\/api\/.*$/,
          handler: 'NetworkFirst',
          options: {
            cacheName: 'api-cache',
            expiration: { maxEntries: 100, maxAgeSeconds: 60 * 60 },
          },
        },
      ],
    },
  },

  // --------------- Image ---------------
  image: {
    quality: 70,
    formats: ['webp', 'avif'],
    domains: ['localhost', 'minio'],
  },

  // --------------- Icon ---------------
  icon: {
    localApiEndpoint: '/_nuxt_icon',
  },

  // --------------- Nitro (Proxy API) ---------------
  nitro: {
    routeRules: {
      '/api/**': {
        proxy: `${process.env.API_BASE_URL || 'http://localhost:5000'}/api/**`,
      },
    },
  },

  // --------------- Runtime Config ---------------
  runtimeConfig: {
    public: {
      apiBaseUrl: process.env.API_BASE_URL || 'http://localhost:5000',
      appName: 'Kouroukan',
      cguVersion: process.env.CGU_VERSION || '1.0.0',
    },
  },

  // --------------- App ---------------
  app: {
    head: {
      title: 'Kouroukan',
      meta: [
        { charset: 'utf-8' },
        { name: 'viewport', content: 'width=device-width, initial-scale=1' },
        { name: 'description', content: 'Plateforme de gestion d\'établissement scolaire' },
        { name: 'theme-color', content: '#16a34a' },
      ],
      link: [
        { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' },
      ],
    },
  },

  // --------------- Dev ---------------
  devtools: { enabled: true },

  typescript: {
    strict: true,
    typeCheck: process.env.NUXT_TYPECHECK !== 'false',
  },

  css: ['~/assets/css/main.css'],
})
