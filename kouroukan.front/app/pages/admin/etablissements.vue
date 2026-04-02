<script setup lang="ts">
import type { Column } from '~/shared/components/DataTable.vue'
import { useAdminEtablissement } from '~/modules/admin/composables/useAdminEtablissement'
import type { EtablissementAdmin } from '~/modules/admin/types/admin.types'
import { useGeo } from '~/composables/useGeo'

definePageMeta({ layout: 'default' })

const { t } = useI18n()

const {
  items,
  loading,
  saving,
  paginatedData,
  fetchAll,
  update,
  remove,
  changePage,
} = useAdminEtablissement()

const {
  fetchRegions,
  fetchPrefectures,
  fetchSousPrefectures,
  loadingPrefectures,
  loadingSousPrefectures,
} = useGeo()

const showForm = ref(false)
const showDeleteDialog = ref(false)
const editingEntity = ref<EtablissementAdmin | null>(null)
const deletingEntity = ref<EtablissementAdmin | null>(null)
const formSaving = ref(false)

const regionOptions = ref<{ label: string; value: string }[]>([])
const prefectureOptions = ref<{ label: string; value: string }[]>([])
const sousPrefectureOptions = ref<{ label: string; value: string }[]>([])

const formState = reactive({
  name: '',
  email: '',
  phoneNumber: '',
  address: '',
  regionCode: '',
  prefectureCode: '',
  sousPrefectureCode: '',
})

const isConakry = computed(() => formState.regionCode === 'CKY')

async function loadRegionOptions() {
  const regions = await fetchRegions()
  regionOptions.value = regions.map(r => ({ label: r.name, value: r.code }))
}

async function loadPrefectureOptions(regionCode: string) {
  if (!regionCode) { prefectureOptions.value = []; return }
  const prefs = await fetchPrefectures(regionCode)
  prefectureOptions.value = prefs.map(p => ({ label: p.name, value: p.code }))
}

async function loadSousPrefectureOptions(prefectureCode: string) {
  if (!prefectureCode || isConakry.value) { sousPrefectureOptions.value = []; return }
  const sps = await fetchSousPrefectures(prefectureCode)
  sousPrefectureOptions.value = sps.map(sp => ({ label: sp.name, value: sp.code }))
}

function onRegionChange() {
  formState.prefectureCode = ''
  formState.sousPrefectureCode = ''
  sousPrefectureOptions.value = []
  if (formState.regionCode) loadPrefectureOptions(formState.regionCode)
}

function onPrefectureChange() {
  formState.sousPrefectureCode = ''
  if (formState.prefectureCode && !isConakry.value) {
    loadSousPrefectureOptions(formState.prefectureCode)
  }
}

watch(() => editingEntity.value, async (entity) => {
  if (entity) {
    formState.name = entity.name ?? ''
    formState.email = entity.email ?? ''
    formState.phoneNumber = entity.phoneNumber ?? ''
    formState.address = entity.address ?? ''
    formState.regionCode = entity.regionCode ?? ''
    formState.prefectureCode = entity.prefectureCode ?? ''
    formState.sousPrefectureCode = entity.sousPrefectureCode ?? ''
    // Load cascading options
    if (formState.regionCode) {
      await loadPrefectureOptions(formState.regionCode)
      if (formState.prefectureCode && !isConakry.value) {
        await loadSousPrefectureOptions(formState.prefectureCode)
      }
    }
  }
  else {
    formState.name = ''
    formState.email = ''
    formState.phoneNumber = ''
    formState.address = ''
    formState.regionCode = ''
    formState.prefectureCode = ''
    formState.sousPrefectureCode = ''
    prefectureOptions.value = []
    sousPrefectureOptions.value = []
  }
}, { immediate: true })

async function handleFormSubmit(): Promise<void> {
  if (!editingEntity.value) return
  formSaving.value = true
  try {
    const result = await update(editingEntity.value.id, {
      id: editingEntity.value.id,
      name: formState.name,
      email: formState.email || undefined,
      phoneNumber: formState.phoneNumber || undefined,
      address: formState.address || undefined,
      regionCode: formState.regionCode || undefined,
      prefectureCode: formState.prefectureCode || undefined,
      sousPrefectureCode: formState.sousPrefectureCode || undefined,
    })
    if (result) {
      showForm.value = false
    }
  }
  finally {
    formSaving.value = false
  }
}

const columns: Column[] = [
  { key: 'name', label: t('admin.etablissement.nom'), sortable: true },
  { key: 'directeurNom', label: t('admin.etablissement.directeur'), sortable: true },
  { key: 'email', label: 'Email', sortable: true },
  { key: 'phoneNumber', label: t('admin.etablissement.telephone'), sortable: false },
  { key: 'regionName', label: t('admin.etablissement.region'), sortable: true },
  { key: 'userCount', label: t('admin.etablissement.nbUtilisateurs'), sortable: true },
  { key: 'isActive', label: t('admin.etablissement.statut'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(() => {
  fetchAll()
  loadRegionOptions()
})

function openEdit(entity: EtablissementAdmin): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: EtablissementAdmin): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleDelete(): Promise<void> {
  if (!deletingEntity.value) return
  try {
    await remove(deletingEntity.value.id)
  }
  finally {
    showDeleteDialog.value = false
    deletingEntity.value = null
  }
}
</script>

<template>
  <div class="space-y-6">
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <UBreadcrumb
          :items="[
            { label: $t('admin.title'), to: '/admin' },
            { label: $t('admin.etablissement.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('admin.etablissement.title') }}
        </h1>
      </div>
    </div>

    <DataTable :columns="columns" :data="paginatedData" :loading="loading" @page-change="changePage">
      <template #cell-isActive="{ row }">
        <UBadge :color="(row as EtablissementAdmin).isActive ? 'success' : 'error'" variant="subtle" size="sm">
          {{ (row as EtablissementAdmin).isActive ? 'Actif' : 'Inactif' }}
        </UBadge>
      </template>
      <template #cell-directeurNom="{ row }">
        {{ (row as EtablissementAdmin).directeurNom || '—' }}
      </template>
      <template #cell-phoneNumber="{ row }">
        {{ (row as EtablissementAdmin).phoneNumber || '—' }}
      </template>
      <template #cell-regionName="{ row }">
        {{ (row as EtablissementAdmin).regionName || '—' }}
      </template>
      <template #cell-actions="{ row }">
        <div class="flex gap-1">
          <UButton variant="ghost" size="xs" icon="i-heroicons-pencil-square" @click="openEdit(row as EtablissementAdmin)" />
          <UButton variant="ghost" size="xs" color="error" icon="i-heroicons-trash" @click="openDelete(row as EtablissementAdmin)" />
        </div>
      </template>
    </DataTable>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ $t('admin.etablissement.edit') }}
        </h3>
      </template>
      <template #body>
        <form class="space-y-4 p-4" @submit.prevent="handleFormSubmit">
          <UFormField :label="$t('admin.etablissement.nom')" required>
            <UInput v-model="formState.name" class="w-full" />
          </UFormField>
          <UFormField label="Email">
            <UInput v-model="formState.email" type="email" class="w-full" />
          </UFormField>
          <UFormField :label="$t('admin.etablissement.telephone')">
            <UInput v-model="formState.phoneNumber" class="w-full" />
          </UFormField>

          <!-- Localisation -->
          <UFormField :label="$t('admin.etablissement.region')">
            <USelect
              v-model="formState.regionCode"
              :items="regionOptions"
              :placeholder="$t('admin.etablissement.selectRegion')"
              class="w-full"
              @change="onRegionChange"
            />
          </UFormField>

          <UFormField v-if="formState.regionCode" :label="isConakry ? $t('admin.etablissement.commune') : $t('admin.etablissement.prefecture')">
            <USelect
              v-model="formState.prefectureCode"
              :items="prefectureOptions"
              :placeholder="$t('admin.etablissement.selectPrefecture')"
              :loading="loadingPrefectures"
              class="w-full"
              @change="onPrefectureChange"
            />
          </UFormField>

          <UFormField v-if="formState.prefectureCode && !isConakry" :label="$t('admin.etablissement.sousPrefecture')">
            <USelect
              v-model="formState.sousPrefectureCode"
              :items="sousPrefectureOptions"
              :placeholder="$t('admin.etablissement.selectSousPrefecture')"
              :loading="loadingSousPrefectures"
              class="w-full"
            />
          </UFormField>

          <UFormField :label="$t('admin.etablissement.adresse')">
            <UInput v-model="formState.address" class="w-full" />
          </UFormField>

          <div class="flex justify-end gap-3 pt-4">
            <UButton variant="outline" @click="showForm = false">
              {{ $t('actions.cancel') }}
            </UButton>
            <UButton type="submit" color="primary" :loading="formSaving">
              {{ $t('actions.save') }}
            </UButton>
          </div>
        </form>
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('admin.etablissement.deleteTitle')"
      :description="$t('admin.etablissement.deleteConfirm')"
      variant="danger"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
