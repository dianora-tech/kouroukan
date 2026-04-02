<script setup lang="ts">
definePageMeta({
  layout: 'default',
})

const { t } = useI18n()
const messages = ref<{ role: 'user' | 'assistant', content: string }[]>([])
const inputText = ref('')
const thinking = ref(false)

async function sendMessage(): Promise<void> {
  if (!inputText.value.trim()) return
  const userMsg = inputText.value.trim()
  inputText.value = ''
  messages.value.push({ role: 'user', content: userMsg })
  thinking.value = true
  // Placeholder — no backend yet
  await new Promise(r => setTimeout(r, 800))
  thinking.value = false
}
</script>

<template>
  <div class="flex h-full flex-col space-y-4">
    <div>
      <UBreadcrumb
        :items="[
          { label: $t('nav.support'), to: '/support' },
          { label: $t('nav.supportAideIA') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('supportPages.aideIA.title') }}
      </h1>
      <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
        {{ $t('supportPages.aideIA.subtitle') }}
      </p>
    </div>

    <!-- Chat area -->
    <div class="flex min-h-64 flex-1 flex-col rounded-xl border border-gray-200 bg-white dark:border-gray-700 dark:bg-gray-900">
      <div class="flex-1 overflow-y-auto p-4">
        <div
          v-if="messages.length === 0"
          class="flex h-full items-center justify-center"
        >
          <div class="text-center text-gray-400 dark:text-gray-500">
            <UIcon
              name="i-heroicons-cpu-chip"
              class="mx-auto mb-2 h-12 w-12"
            />
            <p>{{ $t('supportPages.aideIA.emptyState') }}</p>
          </div>
        </div>
        <div
          v-else
          class="space-y-4"
        >
          <div
            v-for="(msg, i) in messages"
            :key="i"
            :class="msg.role === 'user' ? 'flex justify-end' : 'flex justify-start'"
          >
            <div
              :class="[
                'max-w-lg rounded-lg px-4 py-2 text-sm',
                msg.role === 'user'
                  ? 'bg-indigo-600 text-white'
                  : 'bg-gray-100 text-gray-900 dark:bg-gray-800 dark:text-white',
              ]"
            >
              {{ msg.content }}
            </div>
          </div>
          <div
            v-if="thinking"
            class="flex justify-start"
          >
            <div class="rounded-lg bg-gray-100 px-4 py-2 text-sm text-gray-500 dark:bg-gray-800 dark:text-gray-400">
              {{ $t('supportPages.aideIA.thinking') }}
            </div>
          </div>
        </div>
      </div>

      <!-- Input -->
      <div class="border-t border-gray-200 p-4 dark:border-gray-700">
        <div class="flex gap-2">
          <UInput
            v-model="inputText"
            :placeholder="$t('supportPages.aideIA.placeholder')"
            class="flex-1"
            @keyup.enter="sendMessage"
          />
          <UButton
            color="primary"
            icon="i-heroicons-paper-airplane"
            :loading="thinking"
            :disabled="!inputText.trim()"
            @click="sendMessage"
          >
            {{ $t('supportPages.aideIA.send') }}
          </UButton>
        </div>
      </div>
    </div>
  </div>
</template>
