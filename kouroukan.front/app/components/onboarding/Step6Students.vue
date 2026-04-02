<script setup lang="ts">
import { useOnboardingDraft } from '~/composables/useOnboardingDraft'

const emit = defineEmits<{
  submit: [data: { eleves: Array<{ prenom: string, nom: string, dateNaissance: string, tuteur: string, classeNom?: string }> }]
  skip: []
  prev: []
}>()

const { t } = useI18n()
const { saveDraft, loadDraft } = useOnboardingDraft('step6')
const step3Draft = useOnboardingDraft('step3')

// ── Classes from Step3 draft for dropdown ──
interface Step3Class { id: number, nom: string, niveauCode: string, capacite: number }
const classesFromStep3 = ref<Step3Class[]>([])

const classeOptions = computed(() =>
  classesFromStep3.value.map(c => ({
    label: `${c.nom} (${c.niveauCode})`,
    value: c.nom,
  })),
)

// ── New students to create ──
interface StudentEntry {
  id: number
  prenom: string
  nom: string
  dateNaissance: string
  tuteur: string
  classeNom: string
}

const students = ref<StudentEntry[]>([])
let nextId = 1

onMounted(() => {
  // Load classes from Step3 draft
  const s3 = step3Draft.loadDraft<{ classes: Step3Class[] }>()
  if (s3?.classes?.length) {
    classesFromStep3.value = s3.classes
  }

  // Load own draft
  const draft = loadDraft<{ students: StudentEntry[] }>()
  if (draft?.students?.length) {
    students.value = draft.students
    nextId = Math.max(...draft.students.map(s => s.id), 0) + 1
  }
})

watch(students, (val) => {
  saveDraft({ students: val.map(s => ({ ...s })) })
}, { deep: true })

function addStudent() {
  students.value.push({
    id: nextId++,
    prenom: '',
    nom: '',
    dateNaissance: '',
    tuteur: '',
    classeNom: '',
  })
}

function removeStudent(id: number) {
  students.value = students.value.filter(s => s.id !== id)
}

function onSubmit() {
  emit('submit', {
    eleves: students.value
      .filter(s => s.prenom && s.nom)
      .map(s => ({
        prenom: s.prenom,
        nom: s.nom,
        dateNaissance: s.dateNaissance,
        tuteur: s.tuteur,
        ...(s.classeNom ? { classeNom: s.classeNom } : {}),
      })),
  })
}

function getCurrentData() {
  return { eleves: students.value.filter(s => s.prenom && s.nom).map(s => ({ prenom: s.prenom, nom: s.nom, dateNaissance: s.dateNaissance, tuteur: s.tuteur, ...(s.classeNom ? { classeNom: s.classeNom } : {}) })) }
}
function isValid() {
  return students.value.some(s => s.prenom && s.nom)
}
defineExpose({ getCurrentData, isValid })
</script>

<template>
  <div>
    <h2 class="mb-2 text-xl font-bold text-gray-900 dark:text-white">
      {{ $t('onboarding.students.title') }}
    </h2>
    <p class="mb-6 text-sm text-gray-500 dark:text-gray-400">
      {{ $t('onboarding.students.description') }}
    </p>

    <!-- Liste eleves -->
    <div
      v-if="students.length > 0"
      class="mb-4 space-y-3"
    >
      <div
        v-for="student in students"
        :key="student.id"
        class="rounded-lg border border-gray-200 p-4 dark:border-gray-700"
      >
        <div class="grid grid-cols-1 gap-3 sm:grid-cols-2">
          <UFormField :label="$t('onboarding.students.firstName')">
            <UInput
              v-model="student.prenom"
              :placeholder="$t('onboarding.students.firstNamePlaceholder')"
              size="sm"
              class="w-full"
            />
          </UFormField>
          <UFormField :label="$t('onboarding.students.lastName')">
            <UInput
              v-model="student.nom"
              :placeholder="$t('onboarding.students.lastNamePlaceholder')"
              size="sm"
              class="w-full"
            />
          </UFormField>
          <UFormField :label="$t('onboarding.students.birthDate')">
            <UInput
              v-model="student.dateNaissance"
              type="date"
              size="sm"
              class="w-full"
            />
          </UFormField>
          <UFormField :label="$t('onboarding.students.guardian')">
            <UInput
              v-model="student.tuteur"
              :placeholder="$t('onboarding.students.guardianPlaceholder')"
              size="sm"
              class="w-full"
            />
          </UFormField>
          <UFormField
            v-if="classeOptions.length > 0"
            :label="$t('onboarding.students.class')"
            class="sm:col-span-2"
          >
            <USelect
              v-model="student.classeNom"
              :items="classeOptions"
              value-key="value"
              :placeholder="$t('onboarding.students.classPlaceholder')"
              class="w-full"
              size="sm"
            />
          </UFormField>
        </div>
        <div class="mt-2 flex justify-end">
          <UButton
            size="xs"
            variant="ghost"
            color="error"
            icon="i-heroicons-trash"
            @click="removeStudent(student.id)"
          >
            {{ $t('onboarding.students.remove') }}
          </UButton>
        </div>
      </div>
    </div>

    <UButton
      variant="outline"
      color="primary"
      icon="i-heroicons-plus"
      class="mb-6"
      @click="addStudent"
    >
      {{ $t('onboarding.students.add') }}
    </UButton>

    <p class="mb-4 text-xs text-gray-500 dark:text-gray-400">
      {{ $t('onboarding.students.hint') }}
    </p>

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
        {{ $t('onboarding.finish') }}
      </UButton>
    </div>
  </div>
</template>
