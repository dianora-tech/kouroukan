<script setup lang="ts">
import { z } from 'zod'
import type { Column } from '~/shared/components/DataTable.vue'
import { apiClient } from '~/core/api/client'

definePageMeta({ layout: 'default' })

const { t } = useI18n()

interface Enseignant {
  id: string
  nom: string
  prenom: string
  telephone: string
  specialites: string[]
  nbEtablissements: number
  forfait: string
  statut: string
}

interface MatiereOption {
  label: string
  value: string
}

const formSchema = z.object({
  prenom: z.string().min(1, t('validation.required')),
  nom: z.string().min(1, t('validation.required')),
  telephone: z.string().min(1, t('validation.required')),
  email: z.string().email(t('validation.email')).optional().or(z.literal('')),
  specialite: z.string().min(1, t('validation.required')),
})

const formState = reactive({
  prenom: '',
  nom: '',
  telephone: '',
  email: '',
  specialite: '',
})

const showForm = ref(false)
const editingEntity = ref<Enseignant | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<Enseignant | null>(null)
const formSaving = ref(false)
const matiereOptions = ref<MatiereOption[]>([])

async function fetchMatieres(): Promise<void> {
  try {
    const response = await apiClient.getPaginated<{ id: number, name: string }>('/api/pedagogie/matieres', { pageSize: 100 })
    if (response.success && response.data) {
      matiereOptions.value = response.data.items.map(m => ({ label: m.name, value: m.name }))
    }
  }
  catch {
    // ignore
  }
}

onMounted(() => {
  fetchMatieres()
})

watch(() => editingEntity.value, (entity) => {
  if (entity) {
    formState.prenom = entity.prenom ?? ''
    formState.nom = entity.nom ?? ''
    formState.telephone = entity.telephone ?? ''
    formState.email = ''
    formState.specialite = entity.specialites?.[0] ?? ''
  }
  else {
    formState.prenom = ''
    formState.nom = ''
    formState.telephone = ''
    formState.email = ''
    formState.specialite = ''
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

const fakeData = ref<Enseignant[]>([
  { id: '1', nom: 'Camara', prenom: 'Mamadou', telephone: '+224 622 11 22 33', specialites: ['Mathématiques', 'Physique'], nbEtablissements: 2, forfait: 'Premium', statut: 'Actif' },
  { id: '2', nom: 'Diallo', prenom: 'Fatoumata', telephone: '+224 628 44 55 66', specialites: ['Français', 'Histoire'], nbEtablissements: 1, forfait: 'Standard', statut: 'Actif' },
  { id: '3', nom: 'Soumah', prenom: 'Ibrahima', telephone: '+224 625 77 88 99', specialites: ['SVT'], nbEtablissements: 3, forfait: 'Premium', statut: 'Actif' },
  { id: '4', nom: 'Barry', prenom: 'Aissatou', telephone: '+224 621 00 11 22', specialites: ['Anglais'], nbEtablissements: 1, forfait: 'Basique', statut: 'Inactif' },
  { id: '5', nom: 'Condé', prenom: 'Mohamed', telephone: '', specialites: ['Mathématiques'], nbEtablissements: 1, forfait: 'Standard', statut: 'Actif' },
])

const columns: Column[] = [
  { key: 'nom', label: t('admin.enseignant.nom'), sortable: true },
  { key: 'telephone', label: t('admin.enseignant.telephone'), sortable: false },
  { key: 'specialites', label: t('admin.enseignant.specialites'), sortable: false },
  { key: 'nbEtablissements', label: t('admin.enseignant.nbEtablissements'), sortable: true },
  { key: 'forfait', label: t('admin.enseignant.forfait'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: Enseignant): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: Enseignant): void {
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
            { label: $t('admin.enseignant.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('admin.enseignant.title') }}
        </h1>
      </div>
      <UButton
        color="primary"
        icon="i-heroicons-plus"
        @click="openCreate"
      >
        {{ $t('admin.enseignant.add') }}
      </UButton>
    </div>

    <DataTable
      :columns="columns"
      :data="{ items: fakeData, totalCount: fakeData.length, page: 1, pageSize: 10 }"
      :loading="false"
    >
      <template #cell-nom="{ row }">
        {{ (row as Enseignant).nom }} {{ (row as Enseignant).prenom }}
      </template>
      <template #cell-telephone="{ row }">
        <span v-if="(row as Enseignant).telephone">{{ (row as Enseignant).telephone }}</span>
        <UBadge
          v-else
          color="warning"
          variant="subtle"
          size="xs"
        >
          {{ $t('admin.enseignant.sansTelephone') }}
        </UBadge>
      </template>
      <template #cell-specialites="{ row }">
        <div class="flex flex-wrap gap-1">
          <UBadge
            v-for="s in (row as Enseignant).specialites"
            :key="s"
            variant="subtle"
            size="xs"
          >
            {{ s }}
          </UBadge>
        </div>
      </template>
      <template #cell-actions="{ row }">
        <div class="flex gap-1">
          <UButton
            variant="ghost"
            size="xs"
            icon="i-heroicons-pencil-square"
            @click="openEdit(row as Enseignant)"
          />
          <UButton
            variant="ghost"
            size="xs"
            color="error"
            icon="i-heroicons-trash"
            @click="openDelete(row as Enseignant)"
          />
        </div>
      </template>
    </DataTable>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('admin.enseignant.edit') : $t('admin.enseignant.add') }}
        </h3>
      </template>
      <template #body>
        <UForm
          :schema="formSchema"
          :state="formState"
          class="space-y-4 p-4"
          @submit="handleFormSubmit"
        >
          <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <UFormField
              :label="$t('admin.enseignant.prenom')"
              name="prenom"
              required
            >
              <UInput
                v-model="formState.prenom"
                class="w-full"
              />
            </UFormField>
            <UFormField
              :label="$t('admin.enseignant.nom')"
              name="nom"
              required
            >
              <UInput
                v-model="formState.nom"
                class="w-full"
              />
            </UFormField>
          </div>
          <UFormField
            :label="$t('admin.enseignant.telephone')"
            name="telephone"
            required
          >
            <UInput
              v-model="formState.telephone"
              class="w-full"
            />
          </UFormField>
          <UFormField
            label="Email"
            name="email"
          >
            <UInput
              v-model="formState.email"
              type="email"
              class="w-full"
            />
          </UFormField>
          <UFormField
            :label="$t('admin.enseignant.specialites')"
            name="specialite"
            required
          >
            <USelect
              v-model="formState.specialite"
              class="w-full"
              :items="matiereOptions"
            />
          </UFormField>
          <div class="flex justify-end gap-3 pt-4">
            <UButton
              variant="outline"
              @click="showForm = false"
            >
              {{ $t('actions.cancel') }}
            </UButton>
            <UButton
              type="submit"
              color="primary"
              :loading="formSaving"
            >
              {{ editingEntity ? $t('actions.save') : $t('actions.create') }}
            </UButton>
          </div>
        </UForm>
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('admin.enseignant.deleteTitle')"
      :description="$t('admin.enseignant.deleteConfirm')"
      variant="danger"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
