import { useAuthStore } from '~/core/stores/auth.store'
import { apiClient, extractErrorMessage } from '~/core/api/client'
import { clearAllOnboardingDrafts } from '~/composables/useOnboardingDraft'

const TOTAL_STEPS = 4

export function useOnboarding() {
  const { t } = useI18n()
  const auth = useAuthStore()

  // onboardingStep stores the last COMPLETED step (0 = none, 1 = step 1 done, etc.)
  // So the current step to show is the NEXT one, capped at TOTAL_STEPS
  const currentStep = ref(Math.min(TOTAL_STEPS, Math.max(1, (auth.onboardingStep || 0) + 1)))
  const loading = ref(false)
  const saving = ref(false)

  const totalSteps = TOTAL_STEPS

  const isCompleted = computed(() => auth.onboardingCompleted)

  const steps = computed(() => [
    { number: 1, key: 'establishment', label: t('onboarding.steps.establishment'), icon: 'i-heroicons-building-office-2' },
    { number: 2, key: 'schoolYear', label: t('onboarding.steps.schoolYear'), icon: 'i-heroicons-calendar-days' },
    { number: 3, key: 'classes', label: t('onboarding.steps.classes'), icon: 'i-heroicons-rectangle-group' },
    { number: 4, key: 'subjects', label: t('onboarding.steps.subjects'), icon: 'i-heroicons-book-open' },
  ])

  function showError(msg: string) {
    try {
      useToast().add({ title: msg, color: 'error', icon: 'i-heroicons-exclamation-triangle' })
    }
    catch {
      // toast not available
    }
  }

  function showSuccess(msg: string) {
    try {
      useToast().add({ title: msg, color: 'success', icon: 'i-heroicons-check-circle' })
    }
    catch {
      // toast not available
    }
  }

  async function saveStep(step: number) {
    saving.value = true
    try {
      await apiClient.put('/api/auth/onboarding/step', { step })
      auth.onboardingStep = step
    }
    catch (error) {
      showError(extractErrorMessage(error))
    }
    finally {
      saving.value = false
    }
  }

  async function nextStep() {
    if (currentStep.value < totalSteps) {
      await saveStep(currentStep.value)
      currentStep.value++
    }
    else {
      await completeOnboarding()
    }
  }

  function prevStep() {
    if (currentStep.value > 1) {
      currentStep.value--
    }
  }

  async function completeOnboarding() {
    saving.value = true
    try {
      await apiClient.put('/api/auth/onboarding/skip')
      auth.onboardingStep = totalSteps
      auth.onboardingCompleted = true
      clearAllOnboardingDrafts()
      showSuccess(t('onboarding.completed'))
    }
    catch (error) {
      showError(extractErrorMessage(error))
    }
    finally {
      saving.value = false
    }
  }

  return {
    currentStep,
    totalSteps,
    steps,
    loading,
    saving,
    isCompleted,
    nextStep,
    prevStep,
    completeOnboarding,
    saveStep,
    showError,
    showSuccess,
  }
}
