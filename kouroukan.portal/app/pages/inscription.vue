<template>
  <div class="pt-24">
    <section class="py-16">
      <div class="max-w-2xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-8">
          <h1 class="text-3xl font-bold text-gray-900 dark:text-white">{{ $t('inscription.title') }}</h1>
          <p class="mt-2 text-gray-500">{{ $t('inscription.subtitle') }}</p>
        </div>

        <InscriptionStepIndicator :current-step="currentStep" :steps="stepLabels" />

        <div class="bg-white dark:bg-gray-900 rounded-2xl border border-gray-200 dark:border-gray-800 p-6 sm:p-8 shadow-sm">
          <InscriptionStep1Profile v-if="currentStep === 1" :data="step1Data" />
          <InscriptionStep3Location v-else-if="currentStep === 2" :data="step2Data" />
          <InscriptionStep3Pricing v-else-if="currentStep === 3" :data="step3Data" />
          <InscriptionStepConfirmation
            v-else
            :step1="step1Data"
            :step2="step2Data"
            :step3="step3Data"
            :success="success"
            :error="error"
          />

          <!-- Navigation -->
          <div v-if="!success" class="flex items-center justify-between mt-8 pt-6 border-t border-gray-200 dark:border-gray-800">
            <UButton
              v-if="currentStep > 1"
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

const { t } = useI18n()

const {
  currentStep,
  totalSteps,
  step1Data,
  step2Data,
  step3Data,
  loading,
  error,
  success,
  nextStep,
  prevStep,
  submit,
  loadFromSession,
} = useRegistration()

const stepLabels = computed(() => [
  t('inscription.steps.profile'),
  t('inscription.steps.location'),
  t('inscription.steps.pricing'),
  t('inscription.steps.confirmation'),
])

function handleNext() {
  if (currentStep.value === 1) {
    if (!step1Data.firstName || !step1Data.lastName || !step1Data.phone || !step1Data.password) return
    if (step1Data.password !== step1Data.confirmPassword) return
  }
  nextStep()
}

onMounted(() => {
  loadFromSession()
})
</script>
