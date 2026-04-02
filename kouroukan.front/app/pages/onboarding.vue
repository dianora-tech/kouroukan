<script setup lang="ts">
import { useOnboarding } from '~/composables/useOnboarding'
import { useOnboardingDraft, clearAllOnboardingDrafts } from '~/composables/useOnboardingDraft'
import { apiClient, extractErrorMessage } from '~/core/api/client'

definePageMeta({
  layout: 'onboarding',
})

const localePath = useLocalePath()

const {
  currentStep,
  totalSteps,
  steps,
  saving,
  nextStep,
  prevStep,
  completeOnboarding,
  showError,
} = useOnboarding()

const showComplete = ref(false)
const typeEtablissement = ref('')
const submitting = ref(false)

// Draft storage per step — MUST match keys used in step components
const drafts = {
  step1: useOnboardingDraft('step1'),
  step2: useOnboardingDraft('step2'),
  step3: useOnboardingDraft('step3'),
  step4: useOnboardingDraft('step4'),
}

// ── Step handlers: save to localStorage only, then go next ──

function handleStep1(data: Record<string, string>) {
  typeEtablissement.value = data.typeEtablissement || ''
  drafts.step1.saveDraft(data)
  nextStep()
}

function handleStep2(data: { anneeScolaireId: number }) {
  drafts.step2.saveDraft(data)
  nextStep()
}

function handleStep3(data: { classes: Array<{ nom: string, niveauCode: string, capacite: number }> }) {
  drafts.step3.saveDraft(data)
  nextStep()
}

// Step 4 is the last step — submit everything to the API
async function handleStep4(data: { matieres: Array<{ code: string, nom: string, coefficient: number, niveauCode?: string, classeNom?: string }> }) {
  drafts.step4.saveDraft(data)
  await submitAllToApi()
}

// ── Submit all drafts to the API at the end ──

async function submitAllToApi() {
  submitting.value = true
  try {
    // Step 1: Update company info
    const step1Data = drafts.step1.loadDraft<Record<string, string>>()
    if (step1Data) {
      await apiClient.put('/api/auth/company', step1Data)
    }

    // Step 2: Use the selected school year ID (no creation — already exists)
    const step2Data = drafts.step2.loadDraft<{ anneeScolaireId: number }>()
    const anneeScolaireId: number | null = step2Data?.anneeScolaireId ?? null

    // Step 3: Fetch niveaux-classes to map code → ID, then create classes
    const step3Data = drafts.step3.loadDraft<{ classes: Array<{ nom: string, niveauCode: string, capacite: number }> }>()

    if (step3Data?.classes?.length) {
      // Fetch niveaux to resolve niveauCode → niveauId
      const niveauCodeToId = new Map<string, number>()
      try {
        const nivRes = await apiClient.get<{ items: Array<{ id: number, code: string }> }>('/api/pedagogie/niveaux-classes?pageSize=100')
        if (nivRes?.data?.items) {
          for (const n of nivRes.data.items) {
            niveauCodeToId.set(n.code, n.id)
          }
        }
      }
      catch {
        // niveaux not available
      }

      for (const cls of step3Data.classes) {
        const niveauId = niveauCodeToId.get(cls.niveauCode)
        if (!niveauId) continue

        try {
          await apiClient.post('/api/pedagogie/classes', {
            name: cls.nom,
            niveauClasseId: niveauId,
            capacite: cls.capacite,
            effectif: 0,
            ...(anneeScolaireId ? { anneeScolaireId } : {}),
          })
        }
        catch {
          // skip failed class
        }
      }
    }

    // Step 4: Create subjects
    interface Step4Draft {
      groups?: Array<{
        niveauCode: string
        classeNom?: string
        subjects: Array<{ code: string, nom: string, coefficient: number, selected: boolean }>
      }>
      matieres?: Array<{ code: string, nom: string, coefficient: number, niveauCode?: string, classeNom?: string }>
    }
    const step4Data = drafts.step4.loadDraft<Step4Draft>()

    const matieres: Array<{ code: string, nom: string, coefficient: number }> = []
    if (step4Data?.groups?.length) {
      for (const group of step4Data.groups) {
        for (const subj of group.subjects) {
          if (!subj.selected) continue
          matieres.push({ code: subj.code, nom: subj.nom, coefficient: subj.coefficient })
        }
      }
    }
    else if (step4Data?.matieres?.length) {
      matieres.push(...step4Data.matieres)
    }

    // Dedupe by code+coefficient
    const seenCodes = new Set<string>()
    for (const matiere of matieres) {
      const key = `${matiere.code}-${matiere.coefficient}`
      if (seenCodes.has(key)) continue
      seenCodes.add(key)
      try {
        await apiClient.post('/api/pedagogie/matieres', {
          name: matiere.nom,
          code: matiere.code,
          coefficient: matiere.coefficient,
          nombreHeures: 0,
        })
      }
      catch {
        // skip failed matiere
      }
    }

    // Mark onboarding as completed
    await completeOnboarding()
    clearAllOnboardingDrafts()
    showComplete.value = true
  }
  catch (error) {
    showError(extractErrorMessage(error))
  }
  finally {
    submitting.value = false
  }
}

// ── Skip & finish later ──

async function handleFinishLater() {
  await navigateTo(localePath('/'))
}

async function handleFinish() {
  clearAllOnboardingDrafts()
  await navigateTo(localePath('/'))
}
</script>

<template>
  <div class="mx-auto max-w-3xl px-4 py-8">
    <!-- Header -->
    <div class="mb-8 text-center">
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('onboarding.title') }}
      </h1>
      <p class="mt-2 text-sm text-gray-500 dark:text-gray-400">
        {{ $t('onboarding.subtitle') }}
      </p>
      <UButton
        v-if="!showComplete"
        variant="link"
        color="neutral"
        size="sm"
        class="mt-2"
        @click="handleFinishLater"
      >
        {{ $t('onboarding.finishLater') }}
      </UButton>
    </div>

    <!-- Progress -->
    <OnboardingProgress
      v-if="!showComplete"
      :steps="steps"
      :current-step="currentStep"
    />

    <!-- Steps -->
    <div class="rounded-xl border border-gray-200 bg-white p-6 shadow-sm dark:border-gray-700 dark:bg-gray-800">
      <template v-if="showComplete">
        <OnboardingComplete @finish="handleFinish" />
      </template>
      <template v-else>
        <Step1Establishment
          v-if="currentStep === 1"
          @submit="handleStep1"
        />
        <Step2SchoolYear
          v-else-if="currentStep === 2"
          @submit="handleStep2"
          @prev="prevStep"
        />
        <Step3Classes
          v-else-if="currentStep === 3"
          :type-etablissement="typeEtablissement"
          @submit="handleStep3"
          @prev="prevStep"
        />
        <Step4Subjects
          v-else-if="currentStep === 4"
          :type-etablissement="typeEtablissement"
          @submit="handleStep4"
          @prev="prevStep"
        />
      </template>

      <!-- Loading overlay -->
      <div
        v-if="saving || submitting"
        class="mt-4 flex items-center justify-center gap-2 text-sm text-gray-500"
      >
        <UIcon
          name="i-heroicons-arrow-path"
          class="h-4 w-4 animate-spin"
        />
        {{ $t('onboarding.saving') }}
      </div>
    </div>
  </div>
</template>
