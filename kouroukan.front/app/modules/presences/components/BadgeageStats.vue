<script setup lang="ts">
import type { Badgeage } from '../types/badgeage.types'

const props = defineProps<{
  items: Badgeage[]
  totalCount: number
}>()

const { t } = useI18n()

const totalNFC = computed(() =>
  props.items.filter(b => b.methodeBadgeage === 'NFC').length,
)

const totalQRCode = computed(() =>
  props.items.filter(b => b.methodeBadgeage === 'QRCode').length,
)

const totalManuel = computed(() =>
  props.items.filter(b => b.methodeBadgeage === 'Manuel').length,
)

const stats = computed(() => [
  {
    label: t('presences.badgeage.stats.total'),
    value: String(props.totalCount),
    icon: 'i-heroicons-identification',
    color: 'text-cyan-600 dark:text-cyan-400',
    bgColor: 'bg-cyan-50 dark:bg-cyan-900/20',
  },
  {
    label: t('presences.badgeage.stats.nfc'),
    value: String(totalNFC.value),
    icon: 'i-heroicons-wifi',
    color: 'text-blue-600 dark:text-blue-400',
    bgColor: 'bg-blue-50 dark:bg-blue-900/20',
  },
  {
    label: t('presences.badgeage.stats.qrcode'),
    value: String(totalQRCode.value),
    icon: 'i-heroicons-qr-code',
    color: 'text-purple-600 dark:text-purple-400',
    bgColor: 'bg-purple-50 dark:bg-purple-900/20',
  },
  {
    label: t('presences.badgeage.stats.manuel'),
    value: String(totalManuel.value),
    icon: 'i-heroicons-hand-raised',
    color: 'text-amber-600 dark:text-amber-400',
    bgColor: 'bg-amber-50 dark:bg-amber-900/20',
  },
])
</script>

<template>
  <div class="grid grid-cols-2 gap-4 lg:grid-cols-4">
    <div
      v-for="stat in stats"
      :key="stat.label"
      class="rounded-lg border border-gray-200 bg-white p-4 dark:border-gray-700 dark:bg-gray-900"
    >
      <div class="flex items-center gap-3">
        <div class="rounded-lg p-2" :class="stat.bgColor">
          <UIcon :name="stat.icon" class="h-5 w-5" :class="stat.color" />
        </div>
        <div>
          <p class="text-2xl font-bold text-gray-900 dark:text-white">
            {{ stat.value }}
          </p>
          <p class="text-sm text-gray-500 dark:text-gray-400">
            {{ stat.label }}
          </p>
        </div>
      </div>
    </div>
  </div>
</template>
