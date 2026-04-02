<script setup lang="ts">
import { z } from 'zod'
import type { Column } from '~/shared/components/DataTable.vue'
import type { PaginatedResult } from '~/core/api/types'
import { apiClient } from '~/core/api/client'

definePageMeta({ layout: 'default' })

const { t } = useI18n()

const API_PATH = '/api/pedagogie/matieres'

interface Matiere {
  id: number
  name: string
  description: string | null
  typeId: number
  code: string
}

interface MatiereType {
  id: number
  name: string
}

const formSchema = z.object({
  code: z.string().min(1, t('validation.required')),
  name: z.string().min(1, t('validation.required')),
  typeId: z.number().min(1, t('validation.required')),
})

const formState = reactive({
  code: '',
  name: '',
  typeId: null as number | null,
  description: '',
})

const showForm = ref(false)
const editingEntity = ref<Matiere | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Matiere | null>(null)
const formSaving = ref(false)
const loading = ref(false)
const items = ref<Matiere[]>([])
const totalCount = ref(0)
const currentPage = ref(1)
const pageSize = ref(20)
const types = ref<MatiereType[]>([])

const typeOptions = computed(() => types.value.map(t => ({ label: t.name, value: t.id })))

async function fetchTypes(): Promise<void> {
  try {
    const response = await apiClient.get<MatiereType[]>(`${API_PATH}/types`)
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
    const response = await apiClient.getPaginated<Matiere>(API_PATH, {
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
    formState.typeId = entity.typeId ?? null
    formState.description = entity.description ?? ''
  }
  else {
    formState.code = ''
    formState.name = ''
    formState.typeId = null
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
    }

    if (editingEntity.value) {
      await apiClient.put<boolean>(`${API_PATH}/${editingEntity.value.id}`, {
        id: editingEntity.value.id,
        ...payload,
      })
    }
    else {
      await apiClient.post<Matiere>(API_PATH, payload)
    }
    showForm.value = false
    editingEntity.value = null
    await fetchData()
  }
  finally {
    formSaving.value = false
  }
}

const columns: Column[] = [
  { key: 'code', label: t('admin.matiere.code'), sortable: true },
  { key: 'name', label: t('admin.matiere.libelle'), sortable: true },
  { key: 'typeId', label: t('admin.matiere.categorie'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

function resetForm(): void {
  formState.code = ''
  formState.name = ''
  formState.typeId = null
  formState.description = ''
}

function openCreate(): void {
  editingEntity.value = null
  resetForm()
  showForm.value = true
}

function openEdit(entity: Matiere): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Matiere): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

function getTypeName(typeId: number): string {
  return types.value.find(t => t.id === typeId)?.name ?? String(typeId)
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
</script>

<template>
  <div class="space-y-6">
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <UBreadcrumb
          :items="[
            { label: $t('admin.title'), to: '/admin' },
            { label: $t('admin.matiere.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('admin.matiere.title') }}
        </h1>
      </div>
      <UButton color="primary" icon="i-heroicons-plus" @click="openCreate">
        {{ $t('admin.matiere.add') }}
      </UButton>
    </div>

    <DataTable :columns="columns" :data="{ items, totalCount, page: currentPage, pageSize }" :loading="loading">
      <template #cell-typeId="{ row }">
        <UBadge variant="subtle" size="sm">{{ getTypeName((row as Matiere).typeId) }}</UBadge>
      </template>
      <template #cell-actions="{ row }">
        <div class="flex gap-1">
          <UButton variant="ghost" size="xs" icon="i-heroicons-pencil-square" @click="openEdit(row as Matiere)" />
          <UButton variant="ghost" size="xs" color="error" icon="i-heroicons-trash" @click="openDelete(row as Matiere)" />
        </div>
      </template>
    </DataTable>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('admin.matiere.edit') : $t('admin.matiere.add') }}
        </h3>
      </template>
      <template #body>
        <UForm :schema="formSchema" :state="formState" class="space-y-4 p-4" @submit="handleFormSubmit">
          <UFormField :label="$t('admin.matiere.code')" name="code" required>
            <UInput v-model="formState.code" class="w-full" />
          </UFormField>
          <UFormField :label="$t('admin.matiere.libelle')" name="name" required>
            <UInput v-model="formState.name" class="w-full" />
          </UFormField>
          <UFormField :label="$t('admin.matiere.categorie')" name="typeId" required>
            <USelect v-model="formState.typeId" class="w-full" :items="typeOptions" :placeholder="$t('admin.matiere.selectCategorie')" />
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
      :title="$t('admin.matiere.deleteTitle')"
      :description="$t('admin.matiere.deleteConfirm')"
      variant="danger"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
