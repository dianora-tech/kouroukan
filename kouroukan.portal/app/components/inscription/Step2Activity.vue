<template>
  <div>
    <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-2">{{ $t('inscription.step2.title') }}</h2>
    <p class="text-gray-500 mb-6">{{ $t('inscription.step2.subtitle') }}</p>

    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
      <button
        v-for="mod in MODULE_LIST"
        :key="mod.slug"
        type="button"
        class="p-4 rounded-xl border-2 text-left transition-all hover:shadow-md"
        :class="isSelected(mod.slug)
          ? 'border-green-600 bg-green-50 dark:bg-green-900/10'
          : 'border-gray-200 dark:border-gray-800'"
        @click="toggle(mod.slug)"
      >
        <div class="flex items-center gap-3">
          <div
            class="w-10 h-10 rounded-lg flex items-center justify-center"
            :style="{ backgroundColor: `${mod.color}15` }"
          >
            <UIcon :name="mod.icon" class="w-5 h-5" :style="{ color: mod.color }" />
          </div>
          <div class="flex-1 min-w-0">
            <p class="font-semibold text-sm text-gray-900 dark:text-white">{{ $t(mod.name) }}</p>
          </div>
          <UIcon
            v-if="isSelected(mod.slug)"
            name="i-heroicons-check-circle-solid"
            class="w-6 h-6 text-green-600 shrink-0"
          />
        </div>
        <p class="mt-2 text-xs text-gray-500 line-clamp-2">{{ $t(mod.description) }}</p>
      </button>
    </div>

    <p v-if="showError" class="mt-4 text-sm text-red-600">{{ $t('inscription.step2.minSelect') }}</p>
  </div>
</template>

<script setup lang="ts">
import { MODULE_LIST } from '~/utils/constants'

const props = defineProps<{
  data: { modules: string[] }
}>()

const showError = ref(false)

function isSelected(slug: string): boolean {
  return props.data.modules.includes(slug)
}

function toggle(slug: string) {
  const idx = props.data.modules.indexOf(slug)
  if (idx >= 0) {
    props.data.modules.splice(idx, 1)
  } else {
    props.data.modules.push(slug)
  }
  showError.value = false
}
</script>
