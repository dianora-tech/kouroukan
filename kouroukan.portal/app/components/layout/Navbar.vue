<template>
  <header
    class="fixed top-0 left-0 right-0 z-50 transition-all duration-300"
    :class="scrolled ? 'bg-white/95 dark:bg-gray-900/95 backdrop-blur-sm shadow-sm' : 'bg-transparent'"
  >
    <nav class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex items-center justify-between h-16">
        <!-- Logo -->
        <NuxtLink to="/" class="flex items-center gap-2">
          <div class="w-9 h-9 bg-green-600 rounded-lg flex items-center justify-center">
            <span class="text-white font-bold text-lg">K</span>
          </div>
          <span class="font-bold text-xl" :class="scrolled ? 'text-gray-900 dark:text-white' : 'text-white'">
            Kouroukan
          </span>
        </NuxtLink>

        <!-- Desktop Nav -->
        <div class="hidden lg:flex items-center gap-6">
          <NuxtLink
            v-for="link in navLinks"
            :key="link.to"
            :to="localePath(link.to)"
            class="text-sm font-medium transition-colors"
            :class="scrolled ? 'text-gray-600 hover:text-green-600 dark:text-gray-300' : 'text-white/80 hover:text-white'"
          >
            {{ $t(link.label) }}
          </NuxtLink>
        </div>

        <!-- Actions -->
        <div class="hidden lg:flex items-center gap-3">
          <LayoutLanguageSwitcher :scrolled="scrolled" />
          <UButton
            :href="`${appUrl}/connexion`"
            variant="outline"
            size="sm"
            external
          >
            {{ $t('nav.login') }}
          </UButton>
          <UButton
            :to="localePath('/inscription')"
            color="primary"
            size="sm"
          >
            {{ $t('nav.register') }}
          </UButton>
        </div>

        <!-- Mobile hamburger -->
        <button
          class="lg:hidden p-2 rounded-lg"
          :class="scrolled ? 'text-gray-600' : 'text-white'"
          @click="mobileMenuOpen = true"
        >
          <UIcon name="i-heroicons-bars-3" class="w-6 h-6" />
        </button>
      </div>
    </nav>

    <LayoutMobileMenu v-model:open="mobileMenuOpen" :links="navLinks" />
  </header>
</template>

<script setup lang="ts">
const localePath = useLocalePath()
const { appUrl } = useRuntimeConfig().public

const scrolled = ref(false)
const mobileMenuOpen = ref(false)

const navLinks = [
  { to: '/fonctionnalites', label: 'nav.features' },
  { to: '/tarifs', label: 'nav.pricing' },
  { to: '/systeme-educatif', label: 'nav.education' },
  { to: '/aide', label: 'nav.help' },
  { to: '/blog', label: 'nav.blog' },
  { to: '/contact', label: 'nav.contact' }
]

function onScroll() {
  scrolled.value = window.scrollY > 50
}

onMounted(() => {
  window.addEventListener('scroll', onScroll, { passive: true })
  onScroll()
})

onUnmounted(() => {
  window.removeEventListener('scroll', onScroll)
})
</script>
