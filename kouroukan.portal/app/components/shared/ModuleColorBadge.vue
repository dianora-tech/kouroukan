<template>
  <span
    class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-medium"
    :style="{ backgroundColor: `${color}15`, color }"
  >
    <UIcon v-if="icon" :name="icon" class="w-3.5 h-3.5" />
    {{ label }}
  </span>
</template>

<script setup lang="ts">
import { MODULE_COLORS } from '~/utils/constants'

const props = defineProps<{
  module: string
  label: string
  icon?: string
}>()

const color = computed(() => MODULE_COLORS[props.module] || '#64748b')
</script>
