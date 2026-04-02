import { z } from 'zod'
import type { AccountType, RegistrationPayload } from '~/utils/types'
import { MODULE_SLUGS } from '~/utils/constants'

const STORAGE_KEY = 'kouroukan_registration'

export function useRegistration() {
  const { t } = useI18n()
  const currentStep = ref(0)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const success = ref(false)

  const accountType = ref<AccountType | ''>('')

  const step1Data = reactive({
    firstName: '',
    lastName: '',
    schoolName: '',
    phone: '',
    email: '',
    password: '',
    confirmPassword: '',
  })

  const step2Data = reactive({
    region: '',
    prefecture: '',
    sousPrefecture: '',
    address: '',
  })

  const isEtablissement = computed(() => accountType.value === 'etablissement')
  const isEnseignant = computed(() => accountType.value === 'enseignant')
  const isParent = computed(() => accountType.value === 'parent')

  // Steps: 0 (type) → 1 (profile) → 2 (location) → 3 (confirmation)
  const totalSteps = computed(() => 3)

  const stepLabelsForType = computed(() => {
    return [
      t('inscription.steps.accountType'),
      t('inscription.steps.profile'),
      t('inscription.steps.location'),
      t('inscription.steps.confirmation'),
    ]
  })

  const step1Schema = computed(() => {
    const base: Record<string, z.ZodType> = {
      firstName: z.string().min(2, t('inscription.validation.required')),
      lastName: z.string().min(2, t('inscription.validation.required')),
      phone: z.string().min(1, t('inscription.validation.required')),
      password: z.string().min(8, t('inscription.validation.passwordMin')),
      confirmPassword: z.string(),
    }

    if (isParent.value) {
      base.email = z.string().email(t('inscription.validation.emailFormat')).or(z.literal(''))
    } else {
      base.email = z.string().email(t('inscription.validation.emailFormat')).or(z.literal(''))
    }

    return z.object(base).refine(data => data.password === data.confirmPassword, {
      message: t('inscription.validation.passwordMatch'),
      path: ['confirmPassword'],
    })
  })

  function nextStep() {
    if (currentStep.value < totalSteps.value) {
      currentStep.value++
      saveToSession()
    }
  }

  function prevStep() {
    if (currentStep.value > 0) {
      currentStep.value--
    }
  }

  function saveToSession() {
    try {
      sessionStorage.setItem(STORAGE_KEY, JSON.stringify({
        accountType: accountType.value,
        step1: step1Data,
        step2: step2Data,
        currentStep: currentStep.value,
      }))
    }
    catch {
      // sessionStorage unavailable
    }
  }

  function loadFromSession() {
    try {
      const stored = sessionStorage.getItem(STORAGE_KEY)
      if (stored) {
        const data = JSON.parse(stored)
        accountType.value = data.accountType || ''
        Object.assign(step1Data, data.step1 || {})
        Object.assign(step2Data, data.step2 || {})
        currentStep.value = data.currentStep || 0
      }
    }
    catch {
      // sessionStorage unavailable
    }
  }

  function formatPhone(raw: string): string {
    const digits = raw.replace(/\D/g, '')
    if (digits.startsWith('224') && digits.length === 12) {
      return digits.slice(3)
    }
    return digits
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
        modules: isEtablissement.value ? MODULE_SLUGS : [],
        accountType: accountType.value as AccountType,
        schoolName: isEtablissement.value ? (step1Data.schoolName || undefined) : undefined,
        region: step2Data.region || undefined,
        prefecture: step2Data.prefecture || undefined,
        sousPrefecture: step2Data.sousPrefecture || undefined,
        address: step2Data.address || undefined,
      }
      await $fetch('/api/auth/register', { method: 'POST', body: payload })
      success.value = true
      sessionStorage.removeItem(STORAGE_KEY)
    }
    catch (e: unknown) {
      const err = e as { data?: { message?: string, errors?: Record<string, string[]>, title?: string } }
      if (err?.data?.errors) {
        const messages = Object.values(err.data.errors).flat()
        error.value = messages.join('. ')
      }
      else {
        error.value = err?.data?.message || err?.data?.title || t('inscription.confirmation.error')
      }
    }
    finally {
      loading.value = false
    }
  }

  return {
    currentStep,
    totalSteps,
    accountType,
    isEtablissement,
    isEnseignant,
    isParent,
    stepLabelsForType,
    step1Data,
    step2Data,
    step1Schema,
    loading,
    error,
    success,
    nextStep,
    prevStep,
    submit,
    loadFromSession,
    saveToSession,
  }
}
