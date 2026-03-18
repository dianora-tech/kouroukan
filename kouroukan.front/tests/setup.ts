import { vi } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'
import { ref, computed, reactive, watch, watchEffect, onMounted, onUnmounted, onBeforeMount, onBeforeUnmount, nextTick, toRef, toRefs, unref, isRef, shallowRef, triggerRef, defineProps, defineEmits, withDefaults } from 'vue'

// ─── Vue Composition API auto-imports ───
vi.stubGlobal('ref', ref)
vi.stubGlobal('computed', computed)
vi.stubGlobal('reactive', reactive)
vi.stubGlobal('watch', watch)
vi.stubGlobal('watchEffect', watchEffect)
vi.stubGlobal('onMounted', onMounted)
vi.stubGlobal('onUnmounted', onUnmounted)
vi.stubGlobal('onBeforeMount', onBeforeMount)
vi.stubGlobal('onBeforeUnmount', onBeforeUnmount)
vi.stubGlobal('nextTick', nextTick)
vi.stubGlobal('toRef', toRef)
vi.stubGlobal('toRefs', toRefs)
vi.stubGlobal('unref', unref)
vi.stubGlobal('isRef', isRef)
vi.stubGlobal('shallowRef', shallowRef)
vi.stubGlobal('triggerRef', triggerRef)

// ─── Nuxt auto-imports mock ───
vi.stubGlobal('navigateTo', vi.fn())
vi.stubGlobal('useRuntimeConfig', () => ({
  public: {
    cguVersion: '1.0.0',
    apiBaseUrl: 'http://localhost:5000',
  },
}))
vi.stubGlobal('useFetch', vi.fn())
vi.stubGlobal('$fetch', vi.fn())
vi.stubGlobal('useI18n', () => ({
  t: (key: string) => key,
  locale: { value: 'fr' },
}))
vi.stubGlobal('useRoute', () => ({
  path: '/dashboard',
  params: {},
  query: {},
}))
vi.stubGlobal('useRouter', () => ({
  push: vi.fn(),
  replace: vi.fn(),
  back: vi.fn(),
}))
vi.stubGlobal('useToast', () => ({
  add: vi.fn(),
}))
vi.stubGlobal('useNuxtApp', () => ({
  $i18n: { t: (key: string) => key },
}))
vi.stubGlobal('useAuth', () => ({
  getToken: vi.fn().mockReturnValue(null),
  setToken: vi.fn(),
  signIn: vi.fn(),
  signOut: vi.fn(),
  status: ref('unauthenticated'),
}))
vi.stubGlobal('useState', vi.fn().mockImplementation((_key: string, init?: () => unknown) => {
  return ref(init !== undefined ? init() : null)
}))

// ─── Pinia persistence plugin mock ───
vi.stubGlobal('piniaPluginPersistedstate', {
  localStorage: () => ({
    getItem: vi.fn(),
    setItem: vi.fn(),
    removeItem: vi.fn(),
  }),
})

// ─── crypto.randomUUID mock ───
if (typeof globalThis.crypto === 'undefined') {
  Object.defineProperty(globalThis, 'crypto', {
    value: {
      randomUUID: () => '00000000-0000-0000-0000-000000000000',
    },
  })
}

// ─── Reset Pinia before each test ───
beforeEach(() => {
  setActivePinia(createPinia())
})
