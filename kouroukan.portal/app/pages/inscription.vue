<template>
  <div class="pt-24">
    <section class="py-16">
      <div class="max-w-2xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-8">
          <h1 class="text-3xl font-bold text-gray-900 dark:text-white">{{ $t('inscription.title') }}</h1>
          <p class="mt-2 text-gray-500">{{ $t('inscription.subtitle') }}</p>
        </div>

        <InscriptionStepIndicator :current-step="currentStep" :steps="stepLabelsForType" :step-subtitles="stepSubtitles" />

        <div class="bg-white dark:bg-gray-900 rounded-2xl border border-gray-200 dark:border-gray-800 p-6 sm:p-8 shadow-sm">
          <!-- Step 0: Account type -->
          <InscriptionStep0AccountType
            v-if="currentStep === 0"
            v-model="accountType"
          />

          <!-- Step 1: Profile -->
          <InscriptionStep1Profile
            v-else-if="currentStep === 1"
            :data="step1Data"
            :account-type="accountType"
          />

          <!-- Step 2: Location (all account types) -->
          <InscriptionStep3Location
            v-else-if="currentStep === 2"
            :data="step2Data"
          />

          <!-- Step 3: Confirmation -->
          <InscriptionStepConfirmation
            v-else-if="currentStep === 3"
            :step1="step1Data"
            :step2="step2Data"
            :account-type="accountType"
            :is-etablissement="isEtablissement"
            :success="success"
            :error="error"
          />

          <!-- Navigation -->
          <div v-if="!success" class="flex items-center justify-between mt-8 pt-6 border-t border-gray-200 dark:border-gray-800">
            <UButton
              v-if="currentStep > 0"
              variant="outline"
              @click="prevStep"
            >
              <UIcon name="i-heroicons-arrow-left" class="w-4 h-4" />
              {{ $t('inscription.navigation.previous') }}
            </UButton>
            <div v-else />

            <UButton
              v-if="currentStep < totalSteps"
              color="primary"
              :disabled="!canProceed"
              @click="handleNext"
            >
              {{ $t('inscription.navigation.next') }}
              <UIcon name="i-heroicons-arrow-right" class="w-4 h-4" />
            </UButton>
            <UButton
              v-else-if="currentStep === totalSteps && !success"
              color="primary"
              :loading="loading"
              @click="submit"
            >
              {{ $t('inscription.confirmation.submit') }}
            </UButton>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
useSeoMeta({ title: 'Inscription', robots: 'noindex' })

const route = useRoute()

const {
  currentStep,
  totalSteps,
  accountType,
  isEtablissement,
  stepLabelsForType,
  step1Data,
  step2Data,
  loading,
  error,
  success,
  nextStep,
  prevStep,
  submit,
  loadFromSession,
} = useRegistration()

const { t } = useI18n()

const accountTypeLabel = computed(() => {
  if (!accountType.value) return ''
  return t(`inscription.step0.types.${accountType.value}.name`)
})

const stepSubtitles = computed(() => [
  accountTypeLabel.value,
])

const canProceed = computed(() => {
  if (currentStep.value === 0) {
    return !!accountType.value
  }
  if (currentStep.value === 1) {
    return !!step1Data.firstName && !!step1Data.lastName && !!step1Data.phone && !!step1Data.password && step1Data.password === step1Data.confirmPassword
  }
  return true
})

function handleNext() {
  if (!canProceed.value) return
  nextStep()
}

onMounted(() => {
  const typeParam = route.query.type as string | undefined

  if (typeParam && ['etablissement', 'enseignant', 'parent'].includes(typeParam)) {
    // Type pre-selected but stay on step 0 so user sees the selection
    accountType.value = typeParam as 'etablissement' | 'enseignant' | 'parent'
    currentStep.value = 0
  }
  else {
    loadFromSession()
  }
})
</script>
