<template>
  <div>
    <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">{{ $t('inscription.step3.title') }}</h2>

    <div class="space-y-5">
      <!-- Région -->
      <UFormField :label="$t('inscription.step3.region')" name="region">
        <USelect
          v-model="data.region"
          :items="regionOptions"
          :placeholder="$t('inscription.step3.regionPlaceholder')"
          class="w-full"
          @change="onRegionChange"
        />
      </UFormField>

      <!-- Préfecture / Commune (filtrée par région) -->
      <UFormField
        v-if="data.region"
        :label="prefectureLabel"
        name="prefecture"
      >
        <USelect
          v-model="data.prefecture"
          :items="prefectureOptions"
          :placeholder="$t('inscription.step3.prefecturePlaceholder')"
          :loading="loadingPrefectures"
          class="w-full"
          @change="onPrefectureChange"
        />
      </UFormField>

      <!-- Sous-préfecture (filtrée par préfecture, masquée pour Conakry) -->
      <UFormField
        v-if="data.prefecture && !isConakry"
        :label="$t('inscription.step3.sousPrefecture')"
        name="sousPrefecture"
      >
        <USelect
          v-model="data.sousPrefecture"
          :items="sousPrefectureOptions"
          :placeholder="$t('inscription.step3.sousPrefecturePlaceholder')"
          :loading="loadingSousPrefectures"
          class="w-full"
        />
      </UFormField>

      <!-- Adresse -->
      <UFormField :label="$t('inscription.step3.address')" name="address">
        <UInput v-model="data.address" class="w-full" />
      </UFormField>
    </div>
  </div>
</template>

<script setup lang="ts">
import { GUINEA_REGIONS, GUINEA_PREFECTURES, GUINEA_SOUS_PREFECTURES } from '~/utils/constants'

const props = defineProps<{
  data: {
    region: string
    prefecture: string
    sousPrefecture: string
    address: string
  }
}>()

const { t } = useI18n()

// ── Options région ────────────────────────────────────────────────────────────
const regionOptions = GUINEA_REGIONS.map(r => ({ label: r.name, value: r.code }))

// ── Détection Conakry (pas de sous-préfecture) ────────────────────────────────
const isConakry = computed(() => props.data.region === 'CKY')

// ── Label dynamique (région = commune pour Conakry) ───────────────────────────
const prefectureLabel = computed(() =>
  isConakry.value
    ? t('inscription.step3.commune')
    : t('inscription.step3.prefecture')
)

// ── Préfectures ───────────────────────────────────────────────────────────────
const loadingPrefectures = ref(false)
const prefectureOptions = ref<{ label: string; value: string }[]>([])

async function loadPrefectures(regionCode: string) {
  if (!regionCode) {
    prefectureOptions.value = []
    return
  }
  loadingPrefectures.value = true
  try {
    const res = await $fetch<{ success: boolean; data: { name: string; code: string }[] }>(
      `/api/geo/prefectures?regionCode=${regionCode}`
    )
    if (res?.success && Array.isArray(res.data)) {
      prefectureOptions.value = res.data.map(p => ({ label: p.name, value: p.code }))
      return
    }
  }
  catch {
    // Fallback sur les constantes locales
  }
  finally {
    loadingPrefectures.value = false
  }
  // Fallback
  const local = GUINEA_PREFECTURES[regionCode] ?? []
  prefectureOptions.value = local.map(p => ({ label: p.name, value: p.code }))
}

// ── Sous-préfectures ──────────────────────────────────────────────────────────
const loadingSousPrefectures = ref(false)
const sousPrefectureOptions = ref<{ label: string; value: string }[]>([])

async function loadSousPrefectures(prefectureCode: string) {
  if (!prefectureCode || isConakry.value) {
    sousPrefectureOptions.value = []
    return
  }
  loadingSousPrefectures.value = true
  try {
    const res = await $fetch<{ success: boolean; data: { name: string; code: string }[] }>(
      `/api/geo/sous-prefectures?prefectureCode=${prefectureCode}`
    )
    if (res?.success && Array.isArray(res.data)) {
      sousPrefectureOptions.value = res.data.map(sp => ({ label: sp.name, value: sp.code }))
      return
    }
  }
  catch {
    // Fallback sur les constantes locales
  }
  finally {
    loadingSousPrefectures.value = false
  }
  // Fallback
  const local = GUINEA_SOUS_PREFECTURES[prefectureCode] ?? []
  sousPrefectureOptions.value = local.map(sp => ({ label: sp.name, value: sp.code }))
}

// ── Réinitialisation en cascade ───────────────────────────────────────────────
function onRegionChange() {
  props.data.prefecture = ''
  props.data.sousPrefecture = ''
  sousPrefectureOptions.value = []
  if (props.data.region) loadPrefectures(props.data.region)
}

function onPrefectureChange() {
  props.data.sousPrefecture = ''
  if (props.data.prefecture && !isConakry.value) {
    loadSousPrefectures(props.data.prefecture)
  }
}

// ── Chargement initial si les données sont déjà remplies (retour arrière) ─────
onMounted(() => {
  if (props.data.region) {
    loadPrefectures(props.data.region)
    if (props.data.prefecture && !isConakry.value) {
      loadSousPrefectures(props.data.prefecture)
    }
  }
})
</script>
