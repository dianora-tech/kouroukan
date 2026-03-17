<template>
  <Transition
    enter-active-class="transition-transform duration-300"
    enter-from-class="translate-y-full"
    enter-to-class="translate-y-0"
    leave-active-class="transition-transform duration-300"
    leave-from-class="translate-y-0"
    leave-to-class="translate-y-full"
  >
    <div
      v-if="visible"
      class="fixed bottom-0 left-0 right-0 z-40 bg-white dark:bg-gray-900 border-t border-gray-200 dark:border-gray-800 shadow-lg"
    >
      <div class="max-w-7xl mx-auto px-4 py-4 flex flex-col sm:flex-row items-center justify-between gap-4">
        <p class="text-sm text-gray-600 dark:text-gray-400">
          {{ $t('cookies.message') }}
          <NuxtLink :to="localePath('/legal/confidentialite')" class="text-green-600 underline">
            {{ $t('cookies.learnMore') }}
          </NuxtLink>
        </p>
        <div class="flex items-center gap-2">
          <UButton variant="ghost" size="sm" @click="decline">{{ $t('cookies.decline') }}</UButton>
          <UButton color="primary" size="sm" @click="accept">{{ $t('cookies.accept') }}</UButton>
        </div>
      </div>
    </div>
  </Transition>
</template>

<script setup lang="ts">
const localePath = useLocalePath()
const visible = ref(false)

function accept() {
  localStorage.setItem('kouroukan_cookies', 'accepted')
  visible.value = false
}

function decline() {
  localStorage.setItem('kouroukan_cookies', 'declined')
  visible.value = false
}

onMounted(() => {
  if (!localStorage.getItem('kouroukan_cookies')) {
    visible.value = true
  }
})
</script>
