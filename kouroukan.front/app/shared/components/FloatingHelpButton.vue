<script setup lang="ts">
const { t } = useI18n()
const route = useRoute()
const panelOpen = ref(false)

const hiddenRoutes = ['/connexion', '/inscription']
const isVisible = computed(() =>
  !hiddenRoutes.some(r => route.path === r || route.path.endsWith(r)),
)

function toggle(): void {
  panelOpen.value = !panelOpen.value
}

function close(): void {
  panelOpen.value = false
}

const items = computed(() => [
  {
    label: t('help.askAI'),
    icon: 'i-heroicons-cpu-chip',
    to: '/support/aide-ia',
  },
  {
    label: t('help.browseHelp'),
    icon: 'i-heroicons-book-open',
    to: '/support/aide',
  },
  {
    label: t('help.reportIssue'),
    icon: 'i-heroicons-ticket',
    to: '/support/tickets',
  },
])

// Close on click outside
const panelRef = ref<HTMLElement | null>(null)

function handleClickOutside(event: MouseEvent): void {
  if (panelRef.value && !panelRef.value.contains(event.target as Node)) {
    close()
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside, true)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside, true)
})
</script>

<template>
  <div v-if="isVisible" ref="panelRef" class="fixed bottom-4 right-4 z-50">
    <!-- Panel -->
    <Transition
      enter-active-class="transition duration-200 ease-out"
      enter-from-class="translate-y-2 opacity-0"
      enter-to-class="translate-y-0 opacity-100"
      leave-active-class="transition duration-150 ease-in"
      leave-from-class="translate-y-0 opacity-100"
      leave-to-class="translate-y-2 opacity-0"
    >
      <div
        v-if="panelOpen"
        class="absolute bottom-14 right-0 mb-2 w-56 rounded-xl border border-gray-200 bg-white p-2 shadow-lg dark:border-gray-700 dark:bg-gray-800"
      >
        <NuxtLink
          v-for="item in items"
          :key="item.to"
          :to="item.to"
          class="flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium text-gray-700 transition-colors hover:bg-gray-100 dark:text-gray-200 dark:hover:bg-gray-700"
          @click="close"
        >
          <UIcon :name="item.icon" class="h-5 w-5 text-indigo-500" />
          {{ item.label }}
        </NuxtLink>
      </div>
    </Transition>

    <!-- FAB -->
    <button
      class="flex h-12 w-12 items-center justify-center rounded-full bg-indigo-600 text-white shadow-lg transition-transform hover:scale-105 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
      :aria-label="$t('help.title')"
      @click.stop="toggle"
    >
      <UIcon
        :name="panelOpen ? 'i-heroicons-x-mark' : 'i-heroicons-question-mark-circle'"
        class="h-6 w-6"
      />
    </button>
  </div>
</template>
