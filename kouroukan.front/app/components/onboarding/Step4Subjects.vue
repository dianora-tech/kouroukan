<script setup lang="ts">
import { useOnboardingDraft } from '~/composables/useOnboardingDraft'

const props = defineProps<{
  typeEtablissement?: string
}>()

const emit = defineEmits<{
  submit: [data: { matieres: Array<{ code: string, nom: string, coefficient: number, classeNom?: string, niveauCode?: string }> }]
  prev: []
}>()

const { t } = useI18n()
const { saveDraft, loadDraft } = useOnboardingDraft('step4')
const step3Draft = useOnboardingDraft('step3')

// ── Static niveaux to map niveauCode → cycle ──
const allStaticNiveaux: Record<string, Array<{ label: string, children: Array<{ code: string }> }>> = {
  Preuniversitaire: [
    { label: 'Prescolaire', children: [{ code: 'PS' }, { code: 'MS' }, { code: 'GS' }] },
    { label: 'Primaire', children: [{ code: 'CP1' }, { code: 'CP2' }, { code: 'CE1' }, { code: 'CE2' }, { code: 'CM1' }, { code: 'CM2' }] },
    { label: 'College', children: [{ code: '7E' }, { code: '8E' }, { code: '9E' }, { code: '10E' }] },
    { label: 'Lycee', children: [{ code: '11E' }, { code: '12E' }, { code: 'TLE' }] },
  ],
  Universitaire: [
    { label: 'Licence', children: [{ code: 'L1' }, { code: 'L2' }, { code: 'L3' }] },
    { label: 'Master', children: [{ code: 'M1' }, { code: 'M2' }] },
    { label: 'Doctorat', children: [{ code: 'D1' }, { code: 'D2' }, { code: 'D3' }] },
  ],
  FormationProfessionnelle: [
    { label: 'CAP', children: [{ code: 'CAP1' }, { code: 'CAP2' }, { code: 'CAP3' }] },
    { label: 'BEP', children: [{ code: 'BEP1' }, { code: 'BEP2' }] },
    { label: 'BTS', children: [{ code: 'BTS1' }, { code: 'BTS2' }] },
  ],
}

const codeToCycle = computed(() => {
  const map: Record<string, string> = {}
  const type = props.typeEtablissement || 'Preuniversitaire'
  const niveaux = allStaticNiveaux[type] || allStaticNiveaux.Preuniversitaire
  for (const groupe of niveaux) {
    for (const child of groupe.children) {
      map[child.code] = groupe.label
    }
  }
  return map
})

// Cycles where all classes of the same niveau share subjects
const sharedCycles = new Set(['Prescolaire', 'Primaire', 'College'])

// ── Subject defaults ──
interface SubjectDef {
  code: string
  nom: string
  coefficients: Record<string, number>
}

const subjectDefinitions = computed<SubjectDef[]>(() => {
  const type = props.typeEtablissement || 'Preuniversitaire'
  const defs: Record<string, SubjectDef[]> = {
    Preuniversitaire: [
      { code: 'MATH', nom: 'Mathematiques', coefficients: { Prescolaire: 1, Primaire: 3, College: 4, Lycee: 5 } },
      { code: 'FR', nom: 'Francais', coefficients: { Prescolaire: 1, Primaire: 3, College: 4, Lycee: 4 } },
      { code: 'SVT', nom: 'Sciences de la Vie et de la Terre', coefficients: { Primaire: 2, College: 2, Lycee: 3 } },
      { code: 'PC', nom: 'Physique-Chimie', coefficients: { College: 2, Lycee: 3 } },
      { code: 'HG', nom: 'Histoire-Geographie', coefficients: { Primaire: 2, College: 2, Lycee: 2 } },
      { code: 'ANG', nom: 'Anglais', coefficients: { College: 2, Lycee: 2 } },
      { code: 'EPS', nom: 'Education Physique et Sportive', coefficients: { Prescolaire: 1, Primaire: 1, College: 1, Lycee: 1 } },
      { code: 'ECM', nom: 'Education Civique et Morale', coefficients: { Primaire: 1, College: 1, Lycee: 1 } },
    ],
    Universitaire: [
      { code: 'UE1', nom: 'Unite d\'Enseignement 1', coefficients: { Licence: 3, Master: 4, Doctorat: 5 } },
      { code: 'UE2', nom: 'Unite d\'Enseignement 2', coefficients: { Licence: 3, Master: 4, Doctorat: 5 } },
      { code: 'UE3', nom: 'Unite d\'Enseignement 3', coefficients: { Licence: 3, Master: 3, Doctorat: 4 } },
      { code: 'UE4', nom: 'Unite d\'Enseignement 4', coefficients: { Licence: 2, Master: 3 } },
      { code: 'LANG', nom: 'Langues', coefficients: { Licence: 1, Master: 1 } },
      { code: 'INFO', nom: 'Informatique', coefficients: { Licence: 2, Master: 2 } },
    ],
    FormationProfessionnelle: [
      { code: 'TECH', nom: 'Technologie Professionnelle', coefficients: { CAP: 4, BEP: 4, BTS: 5 } },
      { code: 'PRAT', nom: 'Pratique Professionnelle', coefficients: { CAP: 5, BEP: 5, BTS: 4 } },
      { code: 'MATH', nom: 'Mathematiques Appliquees', coefficients: { CAP: 2, BEP: 3, BTS: 3 } },
      { code: 'FR', nom: 'Francais', coefficients: { CAP: 2, BEP: 2, BTS: 2 } },
      { code: 'ANG', nom: 'Anglais Technique', coefficients: { CAP: 1, BEP: 2, BTS: 2 } },
      { code: 'DESSIN', nom: 'Dessin Technique', coefficients: { CAP: 3, BEP: 3, BTS: 3 } },
      { code: 'EPS', nom: 'Education Physique et Sportive', coefficients: { CAP: 1, BEP: 1, BTS: 1 } },
    ],
  }
  return defs[type] || defs.Preuniversitaire
})

// ── Data model ──
interface SubjectEntry {
  id: number
  code: string
  nom: string
  coefficient: number
  selected: boolean
}

interface SubjectGroup {
  key: string
  label: string
  cycle: string
  niveauCode: string
  classeNom?: string
  isPerClass: boolean
  subjects: SubjectEntry[]
}

const groups = ref<SubjectGroup[]>([])
const activeGroupKey = ref('')

// Custom subject form
const newSubject = reactive({ code: '', nom: '' })
let nextSubjectId = 10000

// ── Build default subjects for a cycle ──
function buildDefaultSubjects(cycle: string, startId: number): SubjectEntry[] {
  const entries: SubjectEntry[] = []
  let id = startId
  for (const subj of subjectDefinitions.value) {
    const coeff = subj.coefficients[cycle]
    if (coeff === undefined) continue
    entries.push({ id: id++, code: subj.code, nom: subj.nom, coefficient: coeff, selected: true })
  }
  return entries
}

// ── Compute expected group keys from Step3 classes ──
function computeExpectedKeys(classes: Array<{ nom: string, niveauCode: string }>): Set<string> {
  const keys = new Set<string>()
  const processedNiveaux = new Set<string>()

  for (const cls of classes) {
    const cycle = codeToCycle.value[cls.niveauCode] || 'Autre'
    if (sharedCycles.has(cycle)) {
      if (!processedNiveaux.has(cls.niveauCode)) {
        processedNiveaux.add(cls.niveauCode)
        keys.add(`niveau-${cls.niveauCode}`)
      }
    }
    else {
      keys.add(`classe-${cls.nom}`)
    }
  }
  return keys
}

// ── Sync groups with Step3 classes ──
function syncWithStep3(classes: Array<{ nom: string, niveauCode: string, capacite: number }>) {
  const expectedKeys = computeExpectedKeys(classes)
  const existingMap = new Map(groups.value.map(g => [g.key, g]))

  // Remove groups whose classes no longer exist
  const keptGroups: SubjectGroup[] = []
  for (const group of groups.value) {
    if (expectedKeys.has(group.key)) {
      keptGroups.push(group)
    }
  }

  // Add new groups for classes that don't have one yet
  let nextId = Math.max(0, ...groups.value.flatMap(g => g.subjects.map(s => s.id))) + 1
  const processedNiveaux = new Set<string>()

  for (const cls of classes) {
    const cycle = codeToCycle.value[cls.niveauCode] || 'Autre'
    const isShared = sharedCycles.has(cycle)

    if (isShared) {
      const key = `niveau-${cls.niveauCode}`
      if (existingMap.has(key) || processedNiveaux.has(cls.niveauCode)) continue
      processedNiveaux.add(cls.niveauCode)

      const subjects = buildDefaultSubjects(cycle, nextId)
      nextId += subjects.length
      keptGroups.push({
        key,
        label: cls.niveauCode,
        cycle,
        niveauCode: cls.niveauCode,
        isPerClass: false,
        subjects,
      })
    }
    else {
      const key = `classe-${cls.nom}`
      if (existingMap.has(key)) continue

      const subjects = buildDefaultSubjects(cycle, nextId)
      nextId += subjects.length
      keptGroups.push({
        key,
        label: cls.nom,
        cycle,
        niveauCode: cls.niveauCode,
        classeNom: cls.nom,
        isPerClass: true,
        subjects,
      })
    }
  }

  groups.value = keptGroups

  // Fix active group
  if (!expectedKeys.has(activeGroupKey.value) && keptGroups.length > 0) {
    activeGroupKey.value = keptGroups[0].key
  }
  else if (keptGroups.length === 0) {
    activeGroupKey.value = ''
  }
}

// ── Init ──
onMounted(() => {
  // Load Step3 classes
  const s3 = step3Draft.loadDraft<{ classes: Array<{ id: number, nom: string, niveauCode: string, capacite: number }> }>()
  const classes = s3?.classes || []

  // Load own draft
  const draft = loadDraft<{ groups: SubjectGroup[] }>()
  if (draft?.groups?.length) {
    groups.value = draft.groups
    // Sync: remove stale groups, add new ones
    syncWithStep3(classes)
    if (!activeGroupKey.value && groups.value.length > 0) {
      activeGroupKey.value = groups.value[0].key
    }
  }
  else {
    // No draft — build fresh from Step3
    syncWithStep3(classes)
  }
})

// Auto-save draft
watch(groups, (val) => {
  saveDraft({ groups: JSON.parse(JSON.stringify(val)) })
}, { deep: true })

// Active group
const activeGroup = computed(() => groups.value.find(g => g.key === activeGroupKey.value))

// Group tabs organized by cycle
const groupsByCycle = computed(() => {
  const result: Record<string, SubjectGroup[]> = {}
  for (const g of groups.value) {
    if (!result[g.cycle]) result[g.cycle] = []
    result[g.cycle].push(g)
  }
  return result
})

// Add custom subject to active group
function addCustomSubject() {
  if (!newSubject.code || !newSubject.nom || !activeGroup.value) return
  activeGroup.value.subjects.push({
    id: nextSubjectId++,
    code: newSubject.code.toUpperCase(),
    nom: newSubject.nom,
    coefficient: 2,
    selected: true,
  })
  newSubject.code = ''
  newSubject.nom = ''
}

function onSubmit() {
  const matieres: Array<{ code: string, nom: string, coefficient: number, classeNom?: string, niveauCode?: string }> = []
  for (const group of groups.value) {
    for (const subj of group.subjects) {
      if (!subj.selected) continue
      matieres.push({
        code: subj.code,
        nom: subj.nom,
        coefficient: subj.coefficient,
        niveauCode: group.niveauCode,
        ...(group.classeNom ? { classeNom: group.classeNom } : {}),
      })
    }
  }
  emit('submit', { matieres })
}

function getCurrentData() {
  const matieres: Array<{ code: string, nom: string, coefficient: number, niveauCode?: string, classeNom?: string }> = []
  for (const group of groups.value) {
    for (const subj of group.subjects) {
      if (!subj.selected) continue
      matieres.push({ code: subj.code, nom: subj.nom, coefficient: subj.coefficient, niveauCode: group.niveauCode, ...(group.classeNom ? { classeNom: group.classeNom } : {}) })
    }
  }
  return { matieres }
}
function isValid() {
  return groups.value.some(g => g.subjects.some(s => s.selected))
}
defineExpose({ getCurrentData, isValid })
</script>

<template>
  <div>
    <h2 class="mb-2 text-xl font-bold text-gray-900 dark:text-white">
      {{ $t('onboarding.subjects.title') }}
    </h2>
    <p class="mb-6 text-sm text-gray-500 dark:text-gray-400">
      {{ $t('onboarding.subjects.description') }}
    </p>

    <!-- No classes created -->
    <div
      v-if="groups.length === 0"
      class="mb-6 rounded-lg border-2 border-dashed border-gray-300 p-6 text-center dark:border-gray-600"
    >
      <UIcon
        name="i-heroicons-exclamation-triangle"
        class="mx-auto h-8 w-8 text-amber-400"
      />
      <p class="mt-2 text-sm text-gray-500">
        {{ $t('onboarding.subjects.noLevels') }}
      </p>
    </div>

    <template v-else>
      <!-- Tabs par cycle, puis par groupe -->
      <div class="mb-6">
        <div
          v-for="(cycleGroups, cycle) in groupsByCycle"
          :key="cycle"
          class="mb-4"
        >
          <h3 class="mb-2 flex items-center gap-2 text-sm font-semibold text-gray-700 dark:text-gray-300">
            <UIcon
              name="i-heroicons-academic-cap"
              class="h-4 w-4 text-green-600"
            />
            {{ cycle }}
            <span
              v-if="!cycleGroups[0]?.isPerClass"
              class="text-xs font-normal text-gray-400"
            >(memes matieres par niveau)</span>
            <span
              v-else
              class="text-xs font-normal text-gray-400"
            >(matieres par classe)</span>
          </h3>

          <div class="flex flex-wrap gap-1.5">
            <UButton
              v-for="group in cycleGroups"
              :key="group.key"
              size="sm"
              :variant="activeGroupKey === group.key ? 'solid' : 'outline'"
              :color="activeGroupKey === group.key ? 'primary' : 'neutral'"
              @click="activeGroupKey = group.key"
            >
              {{ group.label }}
              <span class="ml-1 text-xs opacity-60">({{ group.subjects.filter(s => s.selected).length }})</span>
            </UButton>
          </div>
        </div>
      </div>

      <!-- Active group subjects -->
      <div
        v-if="activeGroup"
        class="mb-6"
      >
        <div class="mb-3">
          <h4 class="text-sm font-semibold text-gray-700 dark:text-gray-300">
            Matieres pour
            <span class="text-green-600">{{ activeGroup.label }}</span>
            <span
              v-if="!activeGroup.isPerClass"
              class="text-xs font-normal text-gray-400"
            >
              (s'applique a toutes les classes {{ activeGroup.niveauCode }})
            </span>
          </h4>
        </div>

        <div class="space-y-2">
          <div
            v-for="subject in activeGroup.subjects"
            :key="subject.id"
            class="flex items-center gap-3 rounded-lg border border-gray-200 p-3 transition-colors dark:border-gray-700"
            :class="{ 'bg-gray-50 opacity-50 dark:bg-gray-800/50': !subject.selected }"
          >
            <UCheckbox v-model="subject.selected" />
            <span class="w-16 font-mono text-xs text-gray-500">{{ subject.code }}</span>
            <span class="flex-1 text-sm text-gray-900 dark:text-white">{{ subject.nom }}</span>
            <div class="flex items-center gap-1">
              <span class="text-xs text-gray-500">Coeff.</span>
              <UInput
                v-model.number="subject.coefficient"
                type="number"
                class="w-16"
                size="sm"
                :min="1"
                :max="10"
                :disabled="!subject.selected"
              />
            </div>
          </div>
        </div>

        <!-- Ajouter une matiere personnalisee -->
        <div class="mt-4 rounded-lg border border-dashed border-gray-300 p-3 dark:border-gray-600">
          <div class="flex items-end gap-3">
            <UFormField
              label="Code"
              class="w-20"
            >
              <UInput
                v-model="newSubject.code"
                placeholder="ART"
                size="sm"
                class="w-full"
              />
            </UFormField>
            <UFormField
              label="Nom"
              class="flex-1"
            >
              <UInput
                v-model="newSubject.nom"
                placeholder="Nom de la matiere"
                size="sm"
                class="w-full"
              />
            </UFormField>
            <UButton
              size="sm"
              color="primary"
              variant="outline"
              icon="i-heroicons-plus"
              @click="addCustomSubject"
            >
              Ajouter
            </UButton>
          </div>
        </div>
      </div>
    </template>

    <div class="flex justify-between pt-4">
      <UButton
        variant="ghost"
        color="neutral"
        @click="emit('prev')"
      >
        {{ $t('onboarding.previous') }}
      </UButton>
      <UButton
        color="primary"
        @click="onSubmit"
      >
        {{ $t('onboarding.next') }}
      </UButton>
    </div>
  </div>
</template>
