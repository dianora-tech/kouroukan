<template>
  <div>
    <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-2">{{ $t('inscription.step0.title') }}</h2>
    <p class="text-gray-500 mb-8">{{ $t('inscription.step0.subtitle') }}</p>

    <div class="grid grid-cols-1 gap-4">
      <button
        v-for="type in accountTypes"
        :key="type.key"
        type="button"
        class="relative rounded-xl border-2 p-6 text-left transition-all"
        :class="modelValue === type.key
          ? 'border-green-500 bg-green-50 dark:bg-green-900/20'
          : 'border-gray-200 dark:border-gray-700 hover:border-gray-300 dark:hover:border-gray-600'"
        @click="$emit('update:modelValue', type.key)"
      >
        <div class="flex items-start gap-4">
          <div class="w-12 h-12 rounded-xl flex items-center justify-center shrink-0" :class="type.bgClass">
            <UIcon :name="type.icon" class="w-6 h-6" :class="type.iconClass" />
          </div>
          <div class="flex-1">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">
              {{ $t(`inscription.step0.types.${type.key}.name`) }}
            </h3>
            <p class="text-sm text-gray-500 mt-1">
              {{ $t(`inscription.step0.types.${type.key}.description`) }}
            </p>
            <p class="text-sm font-medium text-green-600 dark:text-green-400 mt-2">
              {{ $t(`inscription.step0.types.${type.key}.price`) }}
            </p>
          </div>
          <div class="shrink-0 mt-1">
            <div
              class="w-5 h-5 rounded-full border-2 flex items-center justify-center"
              :class="modelValue === type.key
                ? 'border-green-500 bg-green-500'
                : 'border-gray-300 dark:border-gray-600'"
            >
              <UIcon v-if="modelValue === type.key" name="i-heroicons-check" class="w-3 h-3 text-white" />
            </div>
          </div>
        </div>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { AccountType } from '~/utils/types'

defineProps<{
  modelValue: AccountType | ''
}>()

defineEmits<{
  'update:modelValue': [value: AccountType]
}>()

const accountTypes = [
  {
    key: 'etablissement' as AccountType,
    icon: 'i-heroicons-building-office-2',
    bgClass: 'bg-green-100 dark:bg-green-900/30',
    iconClass: 'text-green-600'
  },
  {
    key: 'enseignant' as AccountType,
    icon: 'i-heroicons-academic-cap',
    bgClass: 'bg-blue-100 dark:bg-blue-900/30',
    iconClass: 'text-blue-600'
  },
  {
    key: 'parent' as AccountType,
    icon: 'i-heroicons-users',
    bgClass: 'bg-purple-100 dark:bg-purple-900/30',
    iconClass: 'text-purple-600'
  }
]
</script>
