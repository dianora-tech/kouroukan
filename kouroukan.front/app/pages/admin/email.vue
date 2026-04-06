<script setup lang="ts">
import { useEmailConfigStore } from '~/modules/admin/stores/email-config.store'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const toast = useToast()
const store = useEmailConfigStore()

const loading = computed(() => store.loading)
const saving = computed(() => store.saving)
const testSending = computed(() => store.testSending)

const smtpConfig = reactive({
  host: '',
  port: 587,
  username: '',
  password: '',
  expediteurNom: '',
  expediteurEmail: '',
  tls: true,
})

const testEmail = ref('')
const showSmtpPassword = ref(false)

onMounted(async () => {
  await store.fetch()
  if (store.config) {
    smtpConfig.host = store.config.host
    smtpConfig.port = store.config.port
    smtpConfig.username = store.config.username
    smtpConfig.expediteurNom = store.config.expediteurNom
    smtpConfig.expediteurEmail = store.config.expediteurEmail
    smtpConfig.tls = store.config.tls
  }
})

async function saveConfig(): Promise<void> {
  const success = await store.update({
    host: smtpConfig.host,
    port: smtpConfig.port,
    username: smtpConfig.username,
    password: smtpConfig.password || undefined,
    expediteurNom: smtpConfig.expediteurNom,
    expediteurEmail: smtpConfig.expediteurEmail,
    tls: smtpConfig.tls,
  })
  if (success) {
    toast.add({ title: t('admin.email.saveSuccess'), color: 'success' })
  }
  else {
    toast.add({ title: t('admin.email.saveError'), color: 'error' })
  }
}

async function sendTestEmail(): Promise<void> {
  if (!testEmail.value) return
  const success = await store.sendTest({ email: testEmail.value })
  if (success) {
    toast.add({ title: t('admin.email.testSuccess'), color: 'success' })
  }
  else {
    toast.add({ title: t('admin.email.testError'), color: 'error' })
  }
}
</script>

<template>
  <div class="space-y-6">
    <div>
      <UBreadcrumb
        :items="[
          { label: $t('admin.title'), to: '/admin' },
          { label: $t('admin.email.title') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('admin.email.title') }}
      </h1>
    </div>

    <!-- SMTP Config -->
    <div class="rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-800">
      <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
        {{ $t('admin.email.smtpConfig') }}
      </h2>
      <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <UFormField :label="$t('admin.email.host')">
          <UInput
            v-model="smtpConfig.host"
            placeholder="smtp.gmail.com"
          />
        </UFormField>
        <UFormField :label="$t('admin.email.port')">
          <UInput
            v-model.number="smtpConfig.port"
            type="number"
            placeholder="587"
          />
        </UFormField>
        <UFormField :label="$t('admin.email.username')">
          <UInput
            v-model="smtpConfig.username"
            placeholder="user@domain.com"
          />
        </UFormField>
        <UFormField :label="$t('admin.email.password')">
          <UInput
            v-model="smtpConfig.password"
            :type="showSmtpPassword ? 'text' : 'password'"
            :placeholder="$t('admin.email.passwordPlaceholder')"
          >
            <template #trailing>
              <UButton
                variant="ghost"
                size="xs"
                :icon="showSmtpPassword ? 'i-heroicons-eye-slash' : 'i-heroicons-eye'"
                @click="showSmtpPassword = !showSmtpPassword"
              />
            </template>
          </UInput>
        </UFormField>
        <UFormField :label="$t('admin.email.expediteurNom')">
          <UInput
            v-model="smtpConfig.expediteurNom"
            placeholder="Kouroukan"
          />
        </UFormField>
        <UFormField :label="$t('admin.email.expediteurEmail')">
          <UInput
            v-model="smtpConfig.expediteurEmail"
            type="email"
            placeholder="noreply@kouroukan.app"
          />
        </UFormField>
      </div>
      <div class="mt-4 flex items-center gap-4">
        <label class="flex items-center gap-2 text-sm text-gray-700 dark:text-gray-300">
          <input
            v-model="smtpConfig.tls"
            type="checkbox"
            class="rounded border-gray-300"
          >
          {{ $t('admin.email.enableTls') }}
        </label>
      </div>
      <div class="mt-6">
        <UButton
          color="primary"
          :loading="saving"
          @click="saveConfig"
        >
          {{ $t('admin.email.save') }}
        </UButton>
      </div>
    </div>

    <!-- Test Email -->
    <div class="rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-800">
      <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
        {{ $t('admin.email.testTitle') }}
      </h2>
      <div class="flex items-end gap-4">
        <UFormField
          :label="$t('admin.email.testRecipient')"
          class="flex-1"
        >
          <UInput
            v-model="testEmail"
            type="email"
            placeholder="test@example.com"
          />
        </UFormField>
        <UButton
          color="primary"
          :loading="testSending"
          @click="sendTestEmail"
        >
          {{ $t('admin.email.sendTest') }}
        </UButton>
      </div>
    </div>
  </div>
</template>
