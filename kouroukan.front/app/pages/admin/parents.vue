<script setup lang="ts">
import { z } from 'zod'
import type { Column } from '~/shared/components/DataTable.vue'
import { apiClient } from '~/core/api/client'

definePageMeta({ layout: 'default' })

const { t } = useI18n()

interface Parent {
  id: string
  nom: string
  prenom: string
  telephone: string
  nbEnfants: number
  forfait: string
  statut: string
}

const formSchema = z.object({
  prenom: z.string().min(1, t('validation.required')),
  nom: z.string().min(1, t('validation.required')),
  telephone: z.string().min(1, t('validation.required')),
  email: z.string().email(t('validation.email')).optional().or(z.literal('')),
})

const formState = reactive({
  prenom: '',
  nom: '',
  telephone: '',
  email: '',
})

const showForm = ref(false)
const editingEntity = ref<Parent | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Parent | null>(null)
const formSaving = ref(false)

watch(() => editingEntity.value, (entity) => {
  if (entity) {
    formState.prenom = entity.prenom ?? ''
    formState.nom = entity.nom ?? ''
    formState.telephone = entity.telephone ?? ''
    formState.email = ''
  }
  else {
    formState.prenom = ''
    formState.nom = ''
    formState.telephone = ''
    formState.email = ''
  }
}, { immediate: true })

async function handleFormSubmit(): Promise<void> {
  formSaving.value = true
  try {
    // TODO: wire to real API create/update
    showForm.value = false
  }
  finally {
    formSaving.value = false
  }
}

const fakeData = ref<Parent[]>([
  { id: '1', nom: 'Sylla', prenom: 'Ousmane', telephone: '+224 622 33 44 55', nbEnfants: 3, forfait: 'Premium', statut: 'Actif' },
  { id: '2', nom: 'Bah', prenom: 'Mariama', telephone: '+224 628 66 77 88', nbEnfants: 2, forfait: 'Standard', statut: 'Actif' },
  { id: '3', nom: 'Touré', prenom: 'Abdoulaye', telephone: '+224 625 99 00 11', nbEnfants: 1, forfait: 'Basique', statut: 'Actif' },
  { id: '4', nom: 'Keita', prenom: 'Kadiatou', telephone: '+224 621 22 33 44', nbEnfants: 4, forfait: 'Premium', statut: 'Inactif' },
  { id: '5', nom: 'Bangoura', prenom: 'Sékou', telephone: '+224 624 55 66 77', nbEnfants: 2, forfait: 'Standard', statut: 'Actif' },
])

const columns: Column[] = [
  { key: 'nom', label: t('admin.parent.nom'), sortable: true },
  { key: 'telephone', label: t('admin.parent.telephone'), sortable: false },
  { key: 'nbEnfants', label: t('admin.parent.nbEnfants'), sortable: true },
  { key: 'forfait', label: t('admin.parent.forfait'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Parent): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Parent): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleDelete(): Promise<void> {
  if (!deletingEntity.value) return
  try {
    await apiClient.delete(`/api/admin/users/${deletingEntity.value.id}`)
    fakeData.value = fakeData.value.filter(e => e.id !== deletingEntity.value!.id)
  }
  catch {
    // silently fail
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
            { label: $t('admin.parent.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('admin.parent.title') }}
        </h1>
      </div>
      <UButton color="primary" icon="i-heroicons-plus" @click="openCreate">
        {{ $t('admin.parent.add') }}
      </UButton>
    </div>

    <DataTable :columns="columns" :data="{ items: fakeData, totalCount: fakeData.length, page: 1, pageSize: 10 }" :loading="false">
      <template #cell-nom="{ row }">
        {{ (row as Parent).nom }} {{ (row as Parent).prenom }}
      </template>
      <template #cell-forfait="{ row }">
        <UBadge variant="subtle" size="sm">{{ (row as Parent).forfait }}</UBadge>
      </template>
      <template #cell-actions="{ row }">
        <div class="flex gap-1">
          <UButton variant="ghost" size="xs" icon="i-heroicons-pencil-square" @click="openEdit(row as Parent)" />
          <UButton variant="ghost" size="xs" color="error" icon="i-heroicons-trash" @click="openDelete(row as Parent)" />
        </div>
      </template>
    </DataTable>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('admin.parent.edit') : $t('admin.parent.add') }}
        </h3>
      </template>
      <template #body>
        <UForm :schema="formSchema" :state="formState" class="space-y-4 p-4" @submit="handleFormSubmit">
          <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <UFormField :label="$t('admin.parent.prenom')" name="prenom" required>
              <UInput v-model="formState.prenom" class="w-full" />
            </UFormField>
            <UFormField :label="$t('admin.parent.nom')" name="nom" required>
              <UInput v-model="formState.nom" class="w-full" />
            </UFormField>
          </div>
          <UFormField :label="$t('admin.parent.telephone')" name="telephone" required>
            <UInput v-model="formState.telephone" class="w-full" />
          </UFormField>
          <UFormField label="Email" name="email">
            <UInput v-model="formState.email" type="email" class="w-full" />
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
      :title="$t('admin.parent.deleteTitle')"
      :description="$t('admin.parent.deleteConfirm')"
      variant="danger"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
