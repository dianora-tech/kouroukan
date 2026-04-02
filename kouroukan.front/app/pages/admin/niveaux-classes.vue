<script setup lang="ts">
import { z } from 'zod'
import type { Column } from '~/shared/components/DataTable.vue'
import type { PaginatedResult } from '~/core/api/types'
import { apiClient } from '~/core/api/client'

definePageMeta({ layout: 'default' })

const { t } = useI18n()

const API_PATH = '/api/pedagogie/niveaux-classes'

interface NiveauClasse {
  id: number
  name: string
  description: string | null
  typeId: number
  code: string
  ordre: number
  cycleEtude: string
  ageOfficielEntree: number | null
  ministereTutelle: string | null
  examenSortie: string | null
  tauxHoraireEnseignant: number | null
}

interface NiveauClasseType {
  id: number
  name: string
}

const formSchema = z.object({
  code: z.string().min(1, t('validation.required')),
  name: z.string().min(1, t('validation.required')),
  cycleEtude: z.string().min(1, t('validation.required')),
  ordre: z.number().min(1, t('validation.required')),
})

const formState = reactive({
  code: '',
  name: '',
  cycleEtude: 'Primaire',
  ordre: 1,
  typeId: 0,
  description: '',
})

const showForm = ref(false)
const editingEntity = ref<NiveauClasse | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<NiveauClasse | null>(null)
const formSaving = ref(false)
const loading = ref(false)
const items = ref<NiveauClasse[]>([])
const totalCount = ref(0)
const currentPage = ref(1)
const pageSize = ref(20)
const types = ref<NiveauClasseType[]>([])

async function fetchTypes(): Promise<void> {
  try {
    const response = await apiClient.get<NiveauClasseType[]>(`${API_PATH}/types`)
    if (response.success && response.data) {
      types.value = response.data
    }
  }
  catch {
    // ignore
  }
}

async function fetchData(): Promise<void> {
  loading.value = true
  try {
    const response = await apiClient.getPaginated<NiveauClasse>(API_PATH, {
      page: currentPage.value,
      pageSize: pageSize.value,
    })
    if (response.success && response.data) {
      items.value = response.data.items
      totalCount.value = response.data.totalCount
    }
  }
  catch {
    // error toast shown by apiClient
  }
  finally {
    loading.value = false
  }
}

onMounted(async () => {
  await Promise.all([fetchData(), fetchTypes()])
})

watch(() => editingEntity.value, (entity) => {
  if (entity) {
    formState.code = entity.code ?? ''
    formState.name = entity.name ?? ''
    formState.cycleEtude = entity.cycleEtude ?? 'Primaire'
    formState.ordre = entity.ordre ?? 1
    formState.typeId = entity.typeId ?? 0
    formState.description = entity.description ?? ''
  }
  else {
    formState.code = ''
    formState.name = ''
    formState.cycleEtude = 'Primaire'
    formState.ordre = 1
    formState.typeId = 0
    formState.description = ''
  }
}, { immediate: true })

async function handleFormSubmit(): Promise<void> {
  formSaving.value = true
  try {
    const payload = {
      name: formState.name,
      description: formState.description || null,
      typeId: formState.typeId,
      code: formState.code,
      ordre: formState.ordre,
      cycleEtude: formState.cycleEtude,
      ageOfficielEntree: null,
      ministereTutelle: null,
      examenSortie: null,
      tauxHoraireEnseignant: null,
    }

    if (editingEntity.value) {
      await apiClient.put<boolean>(`${API_PATH}/${editingEntity.value.id}`, {
        id: editingEntity.value.id,
        ...payload,
      })
    }
    else {
      await apiClient.post<NiveauClasse>(API_PATH, payload)
    }
    showForm.value = false
    await fetchData()
  }
  finally {
    formSaving.value = false
  }
}

const columns: Column[] = [
  { key: 'code', label: t('admin.niveauClasse.code'), sortable: true },
  { key: 'name', label: t('admin.niveauClasse.libelle'), sortable: true },
  { key: 'cycleEtude', label: t('admin.niveauClasse.cycle'), sortable: true },
  { key: 'ordre', label: t('admin.niveauClasse.ordre'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: NiveauClasse): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: NiveauClasse): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleDelete(): Promise<void> {
  if (!deletingEntity.value) return
  try {
    await apiClient.delete(`${API_PATH}/${deletingEntity.value.id}`)
    await fetchData()
  }
  catch {
    // error toast shown by apiClient
  }
  finally {
    showDeleteDialog.value = false
    deletingEntity.value = null
  }
}

function getCycleColor(cycle: string): string {
  const colors: Record<string, string> = { Primaire: 'info', 'Collège': 'warning', 'Lycée': 'success', College: 'warning', Lycee: 'success' }
  return colors[cycle] ?? 'neutral'
}
</script>

<template>
  <div class="space-y-6">
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <UBreadcrumb
          :items="[
            { label: $t('admin.title'), to: '/admin' },
            { label: $t('admin.niveauClasse.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('admin.niveauClasse.title') }}
        </h1>
      </div>
      <UButton color="primary" icon="i-heroicons-plus" @click="openCreate">
        {{ $t('admin.niveauClasse.add') }}
      </UButton>
    </div>

    <DataTable :columns="columns" :data="{ items, totalCount, page: currentPage, pageSize }" :loading="loading">
      <template #cell-cycleEtude="{ row }">
        <UBadge :color="getCycleColor((row as NiveauClasse).cycleEtude)" variant="subtle" size="sm">
          {{ (row as NiveauClasse).cycleEtude }}
        </UBadge>
      </template>
      <template #cell-actions="{ row }">
        <div class="flex gap-1">
          <UButton variant="ghost" size="xs" icon="i-heroicons-pencil-square" @click="openEdit(row as NiveauClasse)" />
          <UButton variant="ghost" size="xs" color="error" icon="i-heroicons-trash" @click="openDelete(row as NiveauClasse)" />
        </div>
      </template>
    </DataTable>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('admin.niveauClasse.edit') : $t('admin.niveauClasse.add') }}
        </h3>
      </template>
      <template #body>
        <UForm :schema="formSchema" :state="formState" class="space-y-4 p-4" @submit="handleFormSubmit">
          <UFormField :label="$t('admin.niveauClasse.code')" name="code" required>
            <UInput v-model="formState.code" class="w-full" />
          </UFormField>
          <UFormField :label="$t('admin.niveauClasse.libelle')" name="name" required>
            <UInput v-model="formState.name" class="w-full" />
          </UFormField>
          <UFormField :label="$t('admin.niveauClasse.cycle')" name="cycleEtude" required>
            <USelect v-model="formState.cycleEtude" class="w-full" :items="[
              { label: 'Prescolaire', value: 'Prescolaire' },
              { label: 'Primaire', value: 'Primaire' },
              { label: 'College', value: 'College' },
              { label: 'Lycee', value: 'Lycee' },
              { label: 'ETFP PostPrimaire', value: 'ETFP_PostPrimaire' },
              { label: 'Universite', value: 'Universite' },
            ]" />
          </UFormField>
          <UFormField :label="$t('admin.niveauClasse.ordre')" name="ordre" required>
            <UInput v-model.number="formState.ordre" type="number" class="w-full" />
          </UFormField>
          <div class="flex justify-end gap-3 pt-4">
            <UButton variant="outline" @click="showForm = false">
              {{ $t('actions.cancel') }}
            </UButton>
            <UButton type="submit" color="primary" :loading="formSaving">
              {{ editingEntity ? $t('actions.save') : $t('actions.create') }}
            </UButton>
          </div>
        </UForm>
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('admin.niveauClasse.deleteTitle')"
      :description="$t('admin.niveauClasse.deleteConfirm')"
      variant="danger"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
