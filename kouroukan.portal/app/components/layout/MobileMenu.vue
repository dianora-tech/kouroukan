<template>
  <Teleport to="body">
    <Transition
      enter-active-class="transition-opacity duration-300"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition-opacity duration-300"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div v-if="open" class="fixed inset-0 z-50 bg-black/50" @click="close" />
    </Transition>

    <Transition
      enter-active-class="transition-transform duration-300"
      enter-from-class="translate-x-full"
      enter-to-class="translate-x-0"
      leave-active-class="transition-transform duration-300"
      leave-from-class="translate-x-0"
      leave-to-class="translate-x-full"
    >
      <div
        v-if="open"
        class="fixed top-0 right-0 bottom-0 z-50 w-80 max-w-full bg-white dark:bg-gray-900 shadow-xl flex flex-col"
      >
        <!-- Header -->
        <div class="flex items-center justify-between p-4 border-b border-gray-200 dark:border-gray-800">
          <span class="font-bold text-lg text-gray-900 dark:text-white">Kouroukan</span>
          <button class="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800" @click="close">
            <UIcon name="i-heroicons-x-mark" class="w-5 h-5 text-gray-500" />
          </button>
        </div>

        <!-- Links -->
        <nav class="flex-1 overflow-y-auto p-4 space-y-1">
          <NuxtLink
            v-for="link in links"
            :key="link.to"
            :to="localePath(link.to)"
            class="block px-4 py-3 rounded-lg text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-800 font-medium"
            @click="close"
          >
            {{ $t(link.label) }}
          </NuxtLink>
        </nav>

        <!-- CTA -->
        <div class="p-4 border-t border-gray-200 dark:border-gray-800 space-y-3">
          <LayoutLanguageSwitcher :scrolled="true" />
          <UButton
            :to="localePath('/inscription')"
            color="primary"
            block
            @click="close"
          >
            {{ $t('nav.register') }}
          </UButton>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
const localePath = useLocalePath()

const props = defineProps<{
  open: boolean
  links: Array<{ to: string; label: string }>
}>()

const emit = defineEmits<{
  'update:open': [value: boolean]
}>()

function close() {
  emit('update:open', false)
}
</script>
