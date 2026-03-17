<template>
  <div>
    <div v-if="success" class="p-6 rounded-xl bg-green-50 dark:bg-green-900/20 text-center">
      <UIcon name="i-heroicons-check-circle" class="w-12 h-12 text-green-600 mx-auto mb-4" />
      <p class="text-green-800 dark:text-green-200 font-medium">{{ $t('contact.form.success') }}</p>
    </div>

    <UForm v-else :schema="schema" :state="state" @submit="onSubmit" class="space-y-5">
      <UFormField :label="$t('contact.form.name')" name="name" required>
        <UInput v-model="state.name" :placeholder="$t('contact.form.name')" class="w-full" />
      </UFormField>

      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <UFormField :label="$t('contact.form.email')" name="email" required>
          <UInput v-model="state.email" type="email" :placeholder="$t('contact.form.email')" class="w-full" />
        </UFormField>
        <UFormField :label="$t('contact.form.phone')" name="phone">
          <UInput v-model="state.phone" :placeholder="$t('contact.form.phonePlaceholder')" class="w-full" />
        </UFormField>
      </div>

      <UFormField :label="$t('contact.form.subject')" name="subject" required>
        <USelect
          v-model="state.subject"
          :items="subjectOptions"
          :placeholder="$t('contact.form.subjectPlaceholder')"
          class="w-full"
        />
      </UFormField>

      <UFormField :label="$t('contact.form.message')" name="message" required>
        <UTextarea
          v-model="state.message"
          :placeholder="$t('contact.form.messagePlaceholder')"
          :rows="5"
          class="w-full"
        />
      </UFormField>

      <UButton type="submit" color="primary" :loading="loading" block>
        {{ $t('contact.form.submit') }}
      </UButton>

      <p v-if="error" class="text-sm text-red-600 text-center">{{ $t('contact.form.error') }}</p>
    </UForm>
  </div>
</template>

<script setup lang="ts">
import { z } from 'zod'

const { t } = useI18n()

const loading = ref(false)
const success = ref(false)
const error = ref(false)

const state = reactive({
  name: '',
  email: '',
  phone: '',
  subject: '',
  message: ''
})

const schema = z.object({
  name: z.string().min(2, t('inscription.validation.required')),
  email: z.string().email(t('inscription.validation.emailFormat')),
  phone: z.string().optional(),
  subject: z.string().min(1, t('inscription.validation.required')),
  message: z.string().min(10, t('inscription.validation.required'))
})

const subjectOptions = computed(() => [
  { label: t('contact.form.subjects.info'), value: 'info' },
  { label: t('contact.form.subjects.demo'), value: 'demo' },
  { label: t('contact.form.subjects.pricing'), value: 'pricing' },
  { label: t('contact.form.subjects.support'), value: 'support' },
  { label: t('contact.form.subjects.partnership'), value: 'partnership' },
  { label: t('contact.form.subjects.other'), value: 'other' }
])

async function onSubmit() {
  loading.value = true
  error.value = false
  try {
    await $fetch('/api/contact', { method: 'POST', body: state })
    success.value = true
  } catch {
    const stored = JSON.parse(localStorage.getItem('kouroukan_contacts') || '[]')
    stored.push({ ...state, date: new Date().toISOString() })
    localStorage.setItem('kouroukan_contacts', JSON.stringify(stored))
    success.value = true
  } finally {
    loading.value = false
  }
}
</script>
