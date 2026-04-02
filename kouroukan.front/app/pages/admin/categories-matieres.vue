<script setup lang="ts">
import { z } from 'zod'
import type { Column } from '~/shared/components/DataTable.vue'
import { apiClient } from '~/core/api/client'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const toast = useToast()

const API_PATH = '/api/pedagogie/matieres/types'

interface Categorie {
  id: number
  name: string
  description: string | null
}

const formSchema = z.object({
  name: z.string().min(1, t('validation.required')),
})

const formState = reactive({
  name: '',
  description: '',
})

const showForm = ref(false)
const editingEntity = ref<Categorie | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Categorie | null>(null)
const formSaving = ref(false)
const loading = ref(false)
const items = ref<Categorie[]>([])

async function fetchData(): Promise<void> {
  loading.value = true
  try {
    const response = await apiClient.get<Categorie[]>(API_PATH)
    if (response.success && response.data) {
      items.value = response.data
    }
  }
  catch {
    toast.add({ title: t('admin.categorie.fetchError'), color: 'error' })
  }
  finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchData()
})

watch(() => editingEntity.value, (entity) => {
  if (entity) {
    formState.name = entity.name ?? ''
    formState.description = entity.description ?? ''
  }
  else {
    formState.name = ''
    formState.description = ''
  }
}, { immediate: true })

async function handleFormSubmit(): Promise<void> {
  formSaving.value = true
  try {
    const payload = {
      name: formState.name,
      description: formState.description || null,
    }

    if (editingEntity.value) {
      await apiClient.put<boolean>(`${API_PATH}/${editingEntity.value.id}`, payload)
      toast.add({ title: t('admin.categorie.updateSuccess'), color: 'success' })
    }
    else {
      await apiClient.post<Categorie>(API_PATH, payload)
      toast.add({ title: t('admin.categorie.createSuccess'), color: 'success' })
    }
    showForm.value = false
    await fetchData()
  }
  finally {
    formSaving.value = false
  }
}

const columns: Column[] = [
  { key: 'name', label: t('admin.categorie.nom'), sortable: true },
  { key: 'description', label: t('admin.categorie.description'), sortable: false },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Categorie): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Categorie): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleDelete(): Promise<void> {
  if (!deletingEntity.value) return
  try {
    await apiClient.delete(`${API_PATH}/${deletingEntity.value.id}`)
    toast.add({ title: t('admin.categorie.deleteSuccess'), color: 'success' })
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
            { label: $t('admin.categorie.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('admin.categorie.title') }}
        </h1>
      </div>
      <UButton color="primary" icon="i-heroicons-plus" @click="openCreate">
        {{ $t('admin.categorie.add') }}
      </UButton>
    </div>

    <DataTable :columns="columns" :data="{ items, totalCount: items.length, page: 1, pageSize: 100 }" :loading="loading">
      <template #cell-description="{ row }">
        <span class="text-gray-500">{{ (row as Categorie).description || '—' }}</span>
      </template>
      <template #cell-actions="{ row }">
        <div class="flex gap-1">
          <UButton variant="ghost" size="xs" icon="i-heroicons-pencil-square" @click="openEdit(row as Categorie)" />
          <UButton variant="ghost" size="xs" color="error" icon="i-heroicons-trash" @click="openDelete(row as Categorie)" />
        </div>
      </template>
    </DataTable>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('admin.categorie.edit') : $t('admin.categorie.add') }}
        </h3>
      </template>
      <template #body>
        <UForm :schema="formSchema" :state="formState" class="space-y-4 p-4" @submit="handleFormSubmit">
          <UFormField :label="$t('admin.categorie.nom')" name="name" required>
            <UInput v-model="formState.name" class="w-full" />
          </UFormField>
          <UFormField :label="$t('admin.categorie.description')" name="description">
            <UTextarea v-model="formState.description" class="w-full" :rows="3" />
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
      :title="$t('admin.categorie.deleteTitle')"
      :description="$t('admin.categorie.deleteConfirm')"
      variant="danger"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
