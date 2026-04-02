<script setup lang="ts">
import { useOnboardingDraft } from '~/composables/useOnboardingDraft'

const emit = defineEmits<{
  submit: [data: { enseignants: Array<{ prenom: string, nom: string, telephone: string, specialite: string }> }]
  skip: []
  prev: []
}>()

const { t } = useI18n()
const { saveDraft, loadDraft } = useOnboardingDraft('step5')
const step4Draft = useOnboardingDraft('step4')

// ── Matières from Step4 draft for speciality dropdown ──
interface Step4Subject { code: string, nom: string, cycle: string, coefficient: number, selected: boolean }
const matieresFromStep4 = ref<Step4Subject[]>([])

// Dropdown options for speciality (from Step4 draft matières)
const specialiteOptions = computed(() => {
  // Dedupe by code
  const seen = new Set<string>()
  return matieresFromStep4.value
    .filter(m => m.selected !== false)
    .filter((m) => {
      if (seen.has(m.code)) return false
      seen.add(m.code)
      return true
    })
    .map(m => ({ label: m.nom, value: m.nom }))
})

// ── New staff to create ──
interface StaffEntry {
  id: number
  prenom: string
  nom: string
  telephone: string
  specialite: string
}

const staff = ref<StaffEntry[]>([])
let nextId = 1

onMounted(() => {
  // Load matières from Step4 draft
  const s4 = step4Draft.loadDraft<{ subjects: Step4Subject[] }>()
  if (s4?.subjects?.length) {
    matieresFromStep4.value = s4.subjects
  }

  // Load own draft
  const draft = loadDraft<{ staff: StaffEntry[] }>()
  if (draft?.staff?.length) {
    staff.value = draft.staff
    nextId = Math.max(...draft.staff.map(s => s.id), 0) + 1
  }
})

watch(staff, (val) => {
  saveDraft({ staff: val.map(s => ({ ...s })) })
}, { deep: true })

function addStaff() {
  staff.value.push({
    id: nextId++,
    prenom: '',
    nom: '',
    telephone: '',
    specialite: '',
  })
}

function removeStaff(id: number) {
  staff.value = staff.value.filter(s => s.id !== id)
}

function onSubmit() {
  emit('submit', {
    enseignants: staff.value
      .filter(s => s.prenom && s.nom)
      .map(s => ({
        prenom: s.prenom,
        nom: s.nom,
        telephone: s.telephone,
        specialite: s.specialite,
      })),
  })
}

function getCurrentData() {
  return { enseignants: staff.value.filter(s => s.prenom && s.nom).map(s => ({ prenom: s.prenom, nom: s.nom, telephone: s.telephone, specialite: s.specialite })) }
}
function isValid() {
  return staff.value.some(s => s.prenom && s.nom)
}
defineExpose({ getCurrentData, isValid })
</script>

<template>
  <div>
    <h2 class="mb-2 text-xl font-bold text-gray-900 dark:text-white">
      {{ $t('onboarding.staff.title') }}
    </h2>
    <p class="mb-6 text-sm text-gray-500 dark:text-gray-400">
      {{ $t('onboarding.staff.description') }}
    </p>

    <!-- Liste enseignants -->
    <div
      v-if="staff.length > 0"
      class="mb-4 space-y-3"
    >
      <div
        v-for="person in staff"
        :key="person.id"
        class="rounded-lg border border-gray-200 p-4 dark:border-gray-700"
      >
        <div class="grid grid-cols-1 gap-3 sm:grid-cols-2">
          <UFormField :label="$t('onboarding.staff.firstName')">
            <UInput
              v-model="person.prenom"
              :placeholder="$t('onboarding.staff.firstNamePlaceholder')"
              size="sm"
              class="w-full"
            />
          </UFormField>
          <UFormField :label="$t('onboarding.staff.lastName')">
            <UInput
              v-model="person.nom"
              :placeholder="$t('onboarding.staff.lastNamePlaceholder')"
              size="sm"
              class="w-full"
            />
          </UFormField>
          <UFormField :label="$t('onboarding.staff.phone')">
            <PhoneInput v-model="person.telephone" />
          </UFormField>
          <UFormField :label="$t('onboarding.staff.speciality')">
            <USelect
              v-if="specialiteOptions.length > 0"
              v-model="person.specialite"
              :items="specialiteOptions"
              value-key="value"
              :placeholder="$t('onboarding.staff.specialityPlaceholder')"
              class="w-full"
              size="sm"
            />
            <UInput
              v-else
              v-model="person.specialite"
              :placeholder="$t('onboarding.staff.specialityPlaceholder')"
              size="sm"
              class="w-full"
            />
          </UFormField>
        </div>
        <div class="mt-2 flex justify-end">
          <UButton
            size="xs"
            variant="ghost"
            color="error"
            icon="i-heroicons-trash"
            @click="removeStaff(person.id)"
          >
            {{ $t('onboarding.staff.remove') }}
          </UButton>
        </div>
      </div>
    </div>

    <UButton
      variant="outline"
      color="primary"
      icon="i-heroicons-plus"
      class="mb-6"
      @click="addStaff"
    >
      {{ $t('onboarding.staff.add') }}
    </UButton>

    <div class="flex justify-between pt-4">
      <div class="flex gap-2">
        <UButton
          variant="ghost"
          color="neutral"
          @click="emit('prev')"
        >
          {{ $t('onboarding.previous') }}
        </UButton>
        <UButton
          variant="ghost"
          color="neutral"
          @click="emit('skip')"
        >
          {{ $t('onboarding.skip') }}
        </UButton>
      </div>
      <UButton
        color="primary"
        @click="onSubmit"
      >
        {{ $t('onboarding.next') }}
      </UButton>
    </div>
  </div>
</template>
