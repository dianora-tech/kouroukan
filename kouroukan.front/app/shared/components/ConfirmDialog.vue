<script setup lang="ts">
const props = withDefaults(defineProps<{
  open: boolean
  title: string
  description?: string
  confirmLabel?: string
  cancelLabel?: string
  variant?: 'danger' | 'warning' | 'info'
  loading?: boolean
}>(), {
  variant: 'danger',
  loading: false,
})

const emit = defineEmits<{
  (e: 'confirm'): void
  (e: 'cancel'): void
  (e: 'update:open', value: boolean): void
}>()

const isOpen = computed({
  get: () => props.open,
  set: (val: boolean) => emit('update:open', val),
})

const iconMap = {
  danger: 'i-heroicons-exclamation-triangle',
  warning: 'i-heroicons-exclamation-circle',
  info: 'i-heroicons-information-circle',
}

const colorMap = {
  danger: 'error' as const,
  warning: 'warning' as const,
  info: 'info' as const,
}

function handleConfirm(): void {
  emit('confirm')
}

function handleCancel(): void {
  emit('cancel')
  emit('update:open', false)
}
</script>

<template>
  <Teleport to="body">
    <div v-if="isOpen" class="fixed inset-0 z-50 flex items-center justify-center">
      <!-- Backdrop -->
      <div class="absolute inset-0 bg-black/50" @click="handleCancel" />
      <!-- Dialog -->
      <div class="relative z-10 mx-4 w-full max-w-md rounded-xl bg-white p-6 shadow-xl dark:bg-gray-800">
        <div class="flex items-start gap-4">
          <UIcon
            :name="iconMap[variant]"
            class="mt-0.5 h-6 w-6 shrink-0"
            :class="{
              'text-red-500': variant === 'danger',
              'text-amber-500': variant === 'warning',
              'text-blue-500': variant === 'info',
            }"
          />
          <div>
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">
              {{ title }}
            </h3>
            <p v-if="description" class="mt-2 text-sm text-gray-500 dark:text-gray-400">
              {{ description }}
            </p>
          </div>
        </div>
        <div class="mt-6 flex justify-end gap-3">
          <UButton variant="outline" @click="handleCancel">
            {{ cancelLabel || $t('actions.cancel') }}
          </UButton>
          <UButton
            :color="colorMap[variant]"
            :loading="loading"
            @click="handleConfirm"
          >
            {{ confirmLabel || $t('actions.confirm') }}
          </UButton>
        </div>
      </div>
    </div>
  </Teleport>
</template>
