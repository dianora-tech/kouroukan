import { z } from 'zod'
import type { RegistrationPayload } from '~/utils/types'

const STORAGE_KEY = 'kouroukan_registration'

export function useRegistration() {
  const { t } = useI18n()
  const currentStep = ref(1)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const success = ref(false)

  const step1Data = reactive({
    firstName: '',
    lastName: '',
    schoolName: '',
    phone: '',
    email: '',
    password: '',
    confirmPassword: ''
  })

  const step2Data = reactive({
    modules: [] as string[]
  })

  const step3Data = reactive({
    region: '',
    prefecture: '',
    sousPrefecture: '',
    address: ''
  })

  const step1Schema = computed(() => z.object({
    firstName: z.string().min(2, t('inscription.validation.required')),
    lastName: z.string().min(2, t('inscription.validation.required')),
    phone: z.string().regex(/^\+224\s?\d{2}\s?\d{2}\s?\d{2}\s?\d{2}$/, t('inscription.validation.phoneFormat')),
    email: z.string().email(t('inscription.validation.emailFormat')).or(z.literal('')),
    password: z.string().min(8, t('inscription.validation.passwordMin')),
    confirmPassword: z.string()
  }).refine(data => data.password === data.confirmPassword, {
    message: t('inscription.validation.passwordMatch'),
    path: ['confirmPassword']
  }))

  const totalSteps = 4

  function nextStep() {
    if (currentStep.value < totalSteps) {
      currentStep.value++
      saveToSession()
    }
  }

  function prevStep() {
    if (currentStep.value > 1) {
      currentStep.value--
    }
  }

  function saveToSession() {
    try {
      sessionStorage.setItem(STORAGE_KEY, JSON.stringify({
        step1: step1Data,
        step2: step2Data,
        step3: step3Data,
        currentStep: currentStep.value
      }))
    } catch {
      // sessionStorage unavailable
    }
  }

  function loadFromSession() {
    try {
      const stored = sessionStorage.getItem(STORAGE_KEY)
      if (stored) {
        const data = JSON.parse(stored)
        Object.assign(step1Data, data.step1 || {})
        Object.assign(step2Data, data.step2 || {})
        Object.assign(step3Data, data.step3 || {})
        currentStep.value = data.currentStep || 1
      }
    } catch {
      // sessionStorage unavailable
    }
  }

  function formatPhone(raw: string): string {
    // Remove all spaces, dashes, dots
    const digits = raw.replace(/[\s\-\.]/g, '')
    // If already starts with +224, format with spaces
    if (digits.startsWith('+224') && digits.length === 12) {
      const num = digits.slice(4)
      return `+224 ${num.slice(0, 2)} ${num.slice(2, 4)} ${num.slice(4, 6)} ${num.slice(6, 8)}`
    }
    // If starts with 224 (without +)
    if (digits.startsWith('224') && digits.length === 11) {
      const num = digits.slice(3)
      return `+224 ${num.slice(0, 2)} ${num.slice(2, 4)} ${num.slice(4, 6)} ${num.slice(6, 8)}`
    }
    // If just 8 digits (local number), prepend +224
    if (/^\d{8}$/.test(digits)) {
      return `+224 ${digits.slice(0, 2)} ${digits.slice(2, 4)} ${digits.slice(4, 6)} ${digits.slice(6, 8)}`
    }
    // Return as-is, API will validate
    return raw
  }

  async function submit() {
    loading.value = true
    error.value = null
    try {
      const payload: RegistrationPayload = {
        firstName: step1Data.firstName,
        lastName: step1Data.lastName,
        phoneNumber: formatPhone(step1Data.phone),
        email: step1Data.email || undefined,
        password: step1Data.password,
        modules: step2Data.modules,
        schoolName: step1Data.schoolName || undefined,
        region: step3Data.region || undefined,
        prefecture: step3Data.prefecture || undefined,
        sousPrefecture: step3Data.sousPrefecture || undefined,
        address: step3Data.address || undefined
      }
      await $fetch('/api/auth/register', { method: 'POST', body: payload })
      success.value = true
      sessionStorage.removeItem(STORAGE_KEY)
    } catch (e: unknown) {
      const err = e as { data?: { message?: string; errors?: Record<string, string[]>; title?: string } }
      if (err?.data?.errors) {
        const messages = Object.values(err.data.errors).flat()
        error.value = messages.join('. ')
      } else {
        error.value = err?.data?.message || err?.data?.title || t('inscription.confirmation.error')
      }
    } finally {
      loading.value = false
    }
  }

  return {
    currentStep,
    totalSteps,
    step1Data,
    step2Data,
    step3Data,
    step1Schema,
    loading,
    error,
    success,
    nextStep,
    prevStep,
    submit,
    loadFromSession,
    saveToSession
  }
}
