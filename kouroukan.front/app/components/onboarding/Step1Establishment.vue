<script setup lang="ts">
import { z } from 'zod'
import { useOnboardingDraft } from '~/composables/useOnboardingDraft'

const emit = defineEmits<{
  submit: [data: Record<string, string>]
}>()

const { t } = useI18n()
const { saveDraft, loadDraft, clearDraft } = useOnboardingDraft('step1')

const typeOptions = [
  { label: t('onboarding.establishment.types.preuniversitaire'), value: 'Preuniversitaire' },
  { label: t('onboarding.establishment.types.universitaire'), value: 'Universitaire' },
  { label: t('onboarding.establishment.types.formationPro'), value: 'FormationProfessionnelle' },
]

const state = reactive({
  name: '',
  typeEtablissement: '',
  description: '',
  address: '',
  phoneNumber: '',
  email: '',
})

// Restore draft on mount
onMounted(() => {
  const draft = loadDraft<typeof state>()
  if (draft) Object.assign(state, draft)
})

// Auto-save draft on change
watch(() => ({ ...state }), (val) => {
  saveDraft(val)
}, { deep: true })

const schema = z.object({
  name: z.string().min(2, t('onboarding.validation.required')),
  typeEtablissement: z.string().min(1, t('onboarding.validation.required')),
  description: z.string().optional(),
  address: z.string().optional(),
  phoneNumber: z.string().optional(),
  email: z.string().email(t('onboarding.validation.emailFormat')).or(z.literal('')).optional(),
})

function onSubmit() {
  emit('submit', { ...state })
}

/** Expose current form data for "finish later" server-side save */
function getCurrentData() {
  return { ...state }
}
function isValid() {
  return state.name.length >= 2 && state.typeEtablissement.length > 0
}
defineExpose({ getCurrentData, isValid })
</script>

<template>
  <div>
    <h2 class="mb-2 text-xl font-bold text-gray-900 dark:text-white">
      {{ $t('onboarding.establishment.title') }}
    </h2>
    <p class="mb-6 text-sm text-gray-500 dark:text-gray-400">
      {{ $t('onboarding.establishment.description') }}
    </p>

    <UForm
      :schema="schema"
      :state="state"
      class="space-y-4"
      @submit="onSubmit"
    >
      <UFormField
        :label="$t('onboarding.establishment.name')"
        name="name"
        required
      >
        <UInput
          v-model="state.name"
          :placeholder="$t('onboarding.establishment.namePlaceholder')"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="$t('onboarding.establishment.type')"
        name="typeEtablissement"
        required
      >
        <USelect
          v-model="state.typeEtablissement"
          :items="typeOptions"
          value-key="value"
          :placeholder="$t('onboarding.establishment.typePlaceholder')"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="$t('onboarding.establishment.descriptionLabel')"
        name="description"
      >
        <UTextarea
          v-model="state.description"
          :placeholder="$t('onboarding.establishment.descriptionPlaceholder')"
          class="w-full"
        />
      </UFormField>

      <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <UFormField
          :label="$t('onboarding.establishment.phone')"
          name="phoneNumber"
        >
          <PhoneInput v-model="state.phoneNumber" />
        </UFormField>
        <UFormField
          :label="$t('onboarding.establishment.email')"
          name="email"
        >
          <UInput
            v-model="state.email"
            type="email"
            :placeholder="$t('onboarding.establishment.emailPlaceholder')"
            class="w-full"
          />
        </UFormField>
      </div>

      <UFormField
        :label="$t('onboarding.establishment.address')"
        name="address"
      >
        <UInput
          v-model="state.address"
          :placeholder="$t('onboarding.establishment.addressPlaceholder')"
          class="w-full"
        />
      </UFormField>

      <div class="flex justify-end pt-4">
        <UButton
          type="submit"
          color="primary"
        >
          {{ $t('onboarding.next') }}
        </UButton>
      </div>
    </UForm>
  </div>
</template>
