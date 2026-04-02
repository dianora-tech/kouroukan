<script setup lang="ts">
import { useOnboardingDraft } from '~/composables/useOnboardingDraft'

const props = defineProps<{
  typeEtablissement?: string
}>()

const emit = defineEmits<{
  submit: [data: { classes: Array<{ nom: string, niveauCode: string, capacite: number }> }]
  prev: []
}>()

const { t } = useI18n()
const { saveDraft, loadDraft, clearDraft } = useOnboardingDraft('step3')

// ── Existing data from API ──
interface ApiNiveau { id: number, name: string, code: string, cycleEtude: string }
interface ApiClasse { id: number, name: string, niveauClasseName?: string, niveauClasseCode?: string, capacite: number }

const existingNiveaux = ref<ApiNiveau[]>([])
const existingClasses = ref<ApiClasse[]>([])
const loadingData = ref(false)

// Grouped niveaux by cycle for display
const niveauxByCycle = computed(() => {
  const groups: Record<string, ApiNiveau[]> = {}
  for (const n of existingNiveaux.value) {
    const cycle = n.cycleEtude || 'Autre'
    if (!groups[cycle]) groups[cycle] = []
    groups[cycle].push(n)
  }
  return groups
})

// ── Static niveaux par type d'etablissement ──
const allStaticNiveaux: Record<string, Array<{ label: string, children: Array<{ code: string, label: string }> }>> = {
  Preuniversitaire: [
    { label: 'Prescolaire', children: [{ code: 'PS', label: 'Petite Section' }, { code: 'MS', label: 'Moyenne Section' }, { code: 'GS', label: 'Grande Section' }] },
    { label: 'Primaire', children: [{ code: 'CP1', label: 'CP1' }, { code: 'CP2', label: 'CP2' }, { code: 'CE1', label: 'CE1' }, { code: 'CE2', label: 'CE2' }, { code: 'CM1', label: 'CM1' }, { code: 'CM2', label: 'CM2' }] },
    { label: 'College', children: [{ code: '7E', label: '7eme' }, { code: '8E', label: '8eme' }, { code: '9E', label: '9eme' }, { code: '10E', label: '10eme' }] },
    { label: 'Lycee', children: [{ code: '11E', label: '11eme' }, { code: '12E', label: '12eme' }, { code: 'TLE', label: 'Terminale' }] },
  ],
  Universitaire: [
    { label: 'Licence', children: [{ code: 'L1', label: 'Licence 1' }, { code: 'L2', label: 'Licence 2' }, { code: 'L3', label: 'Licence 3' }] },
    { label: 'Master', children: [{ code: 'M1', label: 'Master 1' }, { code: 'M2', label: 'Master 2' }] },
    { label: 'Doctorat', children: [{ code: 'D1', label: 'Doctorat 1' }, { code: 'D2', label: 'Doctorat 2' }, { code: 'D3', label: 'Doctorat 3' }] },
  ],
  FormationProfessionnelle: [
    { label: 'CAP', children: [{ code: 'CAP1', label: 'CAP 1ere annee' }, { code: 'CAP2', label: 'CAP 2eme annee' }, { code: 'CAP3', label: 'CAP 3eme annee' }] },
    { label: 'BEP', children: [{ code: 'BEP1', label: 'BEP 1ere annee' }, { code: 'BEP2', label: 'BEP 2eme annee' }] },
    { label: 'BTS', children: [{ code: 'BTS1', label: 'BTS 1ere annee' }, { code: 'BTS2', label: 'BTS 2eme annee' }] },
  ],
  FrancoArabe: [
    { label: 'Prescolaire', children: [{ code: 'PS', label: 'Petite Section' }, { code: 'MS', label: 'Moyenne Section' }, { code: 'GS', label: 'Grande Section' }] },
    { label: 'Primaire', children: [{ code: 'CP1', label: 'CP1' }, { code: 'CP2', label: 'CP2' }, { code: 'CE1', label: 'CE1' }, { code: 'CE2', label: 'CE2' }, { code: 'CM1', label: 'CM1' }, { code: 'CM2', label: 'CM2' }] },
    { label: 'College', children: [{ code: '7E', label: '7eme' }, { code: '8E', label: '8eme' }, { code: '9E', label: '9eme' }, { code: '10E', label: '10eme' }] },
    { label: 'Lycee', children: [{ code: '11E', label: '11eme' }, { code: '12E', label: '12eme' }, { code: 'TLE', label: 'Terminale' }] },
    { label: 'Etudes Arabes', children: [{ code: 'AR1', label: 'Arabe 1' }, { code: 'AR2', label: 'Arabe 2' }, { code: 'AR3', label: 'Arabe 3' }, { code: 'AR4', label: 'Arabe 4' }, { code: 'AR5', label: 'Arabe 5' }, { code: 'AR6', label: 'Arabe 6' }] },
  ],
}

// Default fallback (preuniversitaire classique)
const defaultStaticNiveaux = allStaticNiveaux.Preuniversitaire

const staticNiveaux = computed(() => {
  if (!props.typeEtablissement) return defaultStaticNiveaux
  return allStaticNiveaux[props.typeEtablissement] || defaultStaticNiveaux
})

const useApiNiveaux = computed(() => existingNiveaux.value.length > 0)

// ── New classes to create ──
interface ClassEntry {
  id: number
  nom: string
  niveauCode: string
  capacite: number
}

const classes = ref<ClassEntry[]>([])
let nextId = 1

// Restore draft on mount
onMounted(() => {
  const draft = loadDraft<{ classes: ClassEntry[] }>()
  if (draft?.classes?.length) {
    classes.value = draft.classes
    nextId = Math.max(...draft.classes.map(c => c.id), 0) + 1
  }
})

// Auto-save draft
watch(classes, (val) => {
  saveDraft({ classes: val.map(c => ({ ...c })) })
}, { deep: true })

function addClass(niveauCode: string) {
  const allClasses = [
    ...existingClasses.value.filter(c => c.niveauClasseCode === niveauCode),
    ...classes.value.filter(c => c.niveauCode === niveauCode),
  ]
  const letter = String.fromCharCode(65 + allClasses.length) // A, B, C...
  classes.value.push({
    id: nextId++,
    nom: `${niveauCode}-${letter}`,
    niveauCode,
    capacite: 40,
  })
}

function removeClass(id: number) {
  classes.value = classes.value.filter(c => c.id !== id)
}

function onSubmit() {
  emit('submit', {
    classes: classes.value.map(c => ({
      nom: c.nom,
      niveauCode: c.niveauCode,
      capacite: c.capacite,
    })),
  })
}

function getCurrentData() {
  return { classes: classes.value.map(c => ({ nom: c.nom, niveauCode: c.niveauCode, capacite: c.capacite })) }
}
function isValid() {
  return classes.value.length > 0
}
defineExpose({ getCurrentData, isValid })
</script>

<template>
  <div>
    <h2 class="mb-2 text-xl font-bold text-gray-900 dark:text-white">
      {{ $t('onboarding.classes.title') }}
    </h2>
    <p class="mb-6 text-sm text-gray-500 dark:text-gray-400">
      {{ $t('onboarding.classes.description') }}
    </p>

    <!-- Loading -->
    <div
      v-if="loadingData"
      class="mb-6 flex items-center justify-center gap-2 py-8 text-sm text-gray-500"
    >
      <UIcon
        name="i-heroicons-arrow-path"
        class="h-4 w-4 animate-spin"
      />
      {{ $t('onboarding.saving') }}
    </div>

    <template v-else>
      <!-- Classes deja enregistrees -->
      <div
        v-if="existingClasses.length > 0"
        class="mb-6"
      >
        <h3 class="mb-3 text-sm font-semibold text-gray-700 dark:text-gray-300">
          {{ $t('onboarding.classes.existing') }} ({{ existingClasses.length }})
        </h3>
        <div class="flex flex-wrap gap-2">
          <span
            v-for="cls in existingClasses"
            :key="cls.id"
            class="inline-flex items-center gap-1 rounded-full bg-green-100 px-3 py-1 text-xs font-medium text-green-700 dark:bg-green-900/30 dark:text-green-400"
          >
            <UIcon
              name="i-heroicons-check-circle"
              class="h-3.5 w-3.5"
            />
            {{ cls.name }} <span class="text-green-500">({{ cls.niveauClasseCode }})</span>
          </span>
        </div>
      </div>

      <!-- Niveaux avec boutons d'ajout rapide (API) -->
      <div
        v-if="useApiNiveaux"
        class="mb-6 space-y-4"
      >
        <div
          v-for="(niveaux, cycle) in niveauxByCycle"
          :key="cycle"
        >
          <h3 class="mb-2 text-sm font-semibold text-gray-700 dark:text-gray-300">
            {{ cycle }}
          </h3>
          <div class="flex flex-wrap gap-2">
            <UButton
              v-for="niveau in niveaux"
              :key="niveau.id"
              size="sm"
              variant="outline"
              color="primary"
              @click="addClass(niveau.code)"
            >
              + {{ niveau.name }}
            </UButton>
          </div>
        </div>
      </div>

      <!-- Niveaux avec boutons d'ajout rapide (fallback statique adapte au type) -->
      <div
        v-else
        class="mb-6 space-y-4"
      >
        <div
          v-for="groupe in staticNiveaux"
          :key="groupe.label"
        >
          <h3 class="mb-2 text-sm font-semibold text-gray-700 dark:text-gray-300">
            {{ groupe.label }}
          </h3>
          <div class="flex flex-wrap gap-2">
            <UButton
              v-for="niveau in groupe.children"
              :key="niveau.code"
              size="sm"
              variant="outline"
              color="primary"
              @click="addClass(niveau.code)"
            >
              + {{ niveau.label }}
            </UButton>
          </div>
        </div>
      </div>

      <!-- Nouvelles classes a ajouter -->
      <div
        v-if="classes.length > 0"
        class="mb-6"
      >
        <h3 class="mb-3 text-sm font-semibold text-gray-700 dark:text-gray-300">
          {{ $t('onboarding.classes.addedClasses') }} ({{ classes.length }})
        </h3>
        <div class="space-y-2">
          <div
            v-for="cls in classes"
            :key="cls.id"
            class="flex items-center gap-3 rounded-lg border border-gray-200 p-3 dark:border-gray-700"
          >
            <UInput
              v-model="cls.nom"
              class="w-32"
              size="sm"
            />
            <UInput
              v-model.number="cls.capacite"
              type="number"
              class="w-20"
              size="sm"
              :min="1"
            />
            <span class="text-xs text-gray-500">{{ $t('onboarding.classes.capacity') }}</span>
            <UButton
              size="xs"
              variant="ghost"
              color="error"
              icon="i-heroicons-trash"
              @click="removeClass(cls.id)"
            />
          </div>
        </div>
      </div>

      <div
        v-else-if="existingClasses.length === 0"
        class="mb-6 rounded-lg border-2 border-dashed border-gray-300 p-8 text-center dark:border-gray-600"
      >
        <UIcon
          name="i-heroicons-rectangle-group"
          class="mx-auto h-8 w-8 text-gray-400"
        />
        <p class="mt-2 text-sm text-gray-500">
          {{ $t('onboarding.classes.empty') }}
        </p>
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
