<script setup lang="ts">
import { useForfaitGating } from '~/composables/useForfaitGating'
import { useCommunicationStore } from '~/modules/famille/stores/communication.store'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const { formatDate } = useFormatDate()
const { isFeatureLocked } = useForfaitGating()
const isLocked = computed(() => isFeatureLocked('communication'))

const store = useCommunicationStore()
const loading = computed(() => store.loading)
const messages = computed(() => store.items)

async function onClickMessage(id: number) {
  await store.markAsRead(id)
}

onMounted(async () => {
  await store.fetchAll()
})
</script>

<template>
  <ForfaitRequiredOverlay
    v-if="isLocked"
    user-type="famille"
  />
  <div
    v-else
    class="space-y-6"
  >
    <div>
      <UBreadcrumb
        :items="[
          { label: $t('famille.title'), to: '/famille' },
          { label: $t('famille.communication.title') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('famille.communication.title') }}
      </h1>
    </div>

    <!-- Loading -->
    <div
      v-if="loading"
      class="flex justify-center py-12"
    >
      <UIcon
        name="i-heroicons-arrow-path"
        class="h-8 w-8 animate-spin text-gray-400"
      />
    </div>

    <!-- Empty state -->
    <div
      v-else-if="messages.length === 0"
      class="rounded-xl border border-gray-200 bg-white p-12 text-center dark:border-gray-700 dark:bg-gray-800"
    >
      <UIcon
        name="i-heroicons-chat-bubble-left-right"
        class="mx-auto h-12 w-12 text-gray-300"
      />
      <p class="mt-4 text-sm text-gray-500">
        {{ $t('common.noData') }}
      </p>
    </div>

    <div
      v-else
      class="space-y-3"
    >
      <div
        v-for="msg in messages"
        :key="msg.id"
        :class="[
          'cursor-pointer rounded-xl border p-4 transition-colors hover:bg-gray-50 dark:hover:bg-gray-700/50',
          msg.lu ? 'border-gray-200 bg-white dark:border-gray-700 dark:bg-gray-800' : 'border-indigo-200 bg-indigo-50/50 dark:border-indigo-700 dark:bg-indigo-900/10',
        ]"
        @click="onClickMessage(msg.id)"
      >
        <div class="flex items-start justify-between">
          <div class="flex items-center gap-2">
            <div
              v-if="!msg.lu"
              class="h-2 w-2 rounded-full bg-indigo-500"
            />
            <p :class="['text-sm', msg.lu ? 'text-gray-600 dark:text-gray-400' : 'font-semibold text-gray-900 dark:text-white']">
              {{ msg.expediteur }}
            </p>
          </div>
          <span class="text-xs text-gray-400">{{ formatDate(msg.date) }}</span>
        </div>
        <p :class="['mt-1 text-sm', msg.lu ? 'text-gray-900 dark:text-white' : 'font-semibold text-gray-900 dark:text-white']">
          {{ msg.objet }}
        </p>
        <p class="mt-1 text-xs text-gray-500 line-clamp-1">
          {{ msg.contenu }}
        </p>
      </div>
    </div>
  </div>
</template>
