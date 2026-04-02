<script setup lang="ts">
import { z } from 'zod'
import type { Column } from '~/shared/components/DataTable.vue'
import { apiClient } from '~/core/api/client'

definePageMeta({ layout: 'default' })

const { t } = useI18n()

interface Utilisateur {
  id: string
  nom: string
  prenom: string
  email: string
  telephone: string
  role: string
  etablissement: string
  statut: string
  derniereConnexion: string
}

const showDeleteDialog = ref(false)
const deletingEntity = ref<Utilisateur | null>(null)
const showForm = ref(false)
const editingEntity = ref<Utilisateur | null>(null)

const formSchema = z.object({
  prenom: z.string().min(1, t('validation.required')),
  nom: z.string().min(1, t('validation.required')),
  email: z.string().email(t('validation.email')),
  role: z.string().min(1, t('validation.required')),
})

const formState = reactive({
  prenom: '',
  nom: '',
  email: '',
  role: 'Enseignant',
})

const formSaving = ref(false)

function openEdit(entity: Utilisateur): void {
  editingEntity.value = entity
  formState.prenom = entity.prenom ?? ''
  formState.nom = entity.nom ?? ''
  formState.email = entity.email ?? ''
  formState.role = entity.role ?? 'Enseignant'
  showForm.value = true
}

async function handleFormSubmit(): Promise<void> {
  formSaving.value = true
  try {
    // TODO: wire to real API update
    showForm.value = false
  }
  finally {
    formSaving.value = false
  }
}

const fakeData = ref<Utilisateur[]>([
  { id: '1', nom: 'Camara', prenom: 'Mamadou', email: 'mamadou@kouroukan.app', telephone: '+224 622 11 22 33', role: 'Admin', etablissement: 'Toutes', statut: 'Actif', derniereConnexion: '2026-03-27 09:15' },
  { id: '2', nom: 'Diallo', prenom: 'Fatoumata', email: 'fatoumata@ecole-kipe.gn', telephone: '+224 628 44 55 66', role: 'Directeur', etablissement: 'Groupe Scolaire Kipé', statut: 'Actif', derniereConnexion: '2026-03-27 08:30' },
  { id: '3', nom: 'Barry', prenom: 'Ousmane', email: 'ousmane@ecole-kipe.gn', telephone: '+224 625 77 88 99', role: 'Enseignant', etablissement: 'Groupe Scolaire Kipé', statut: 'Actif', derniereConnexion: '2026-03-26 17:45' },
  { id: '4', nom: 'Sylla', prenom: 'Aissatou', email: 'aissatou@parent.gn', telephone: '+224 621 00 11 22', role: 'Parent', etablissement: 'Groupe Scolaire Kipé', statut: 'Actif', derniereConnexion: '2026-03-25 20:00' },
  { id: '5', nom: 'Condé', prenom: 'Ibrahim', email: 'ibrahim@lycee-fg.gn', telephone: '+224 624 33 44 55', role: 'Directeur', etablissement: 'Lycée Franco-Guinéen', statut: 'Inactif', derniereConnexion: '2026-02-15 10:30' },
  { id: '6', nom: 'Touré', prenom: 'Mariama', email: 'mariama@matam.gn', telephone: '+224 622 55 66 77', role: 'Secrétaire', etablissement: 'École Primaire de Matam', statut: 'Actif', derniereConnexion: '2026-03-27 07:50' },
])

const columns: Column[] = [
  { key: 'nom', label: t('admin.utilisateur.nom'), sortable: true },
  { key: 'email', label: t('admin.utilisateur.email'), sortable: true },
  { key: 'telephone', label: t('admin.utilisateur.telephone'), sortable: false },
  { key: 'role', label: t('admin.utilisateur.role'), sortable: true },
  { key: 'etablissement', label: t('admin.utilisateur.etablissement'), sortable: true },
  { key: 'statut', label: t('admin.utilisateur.statut'), sortable: true },
  { key: 'derniereConnexion', label: t('admin.utilisateur.derniereConnexion'), sortable: true },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

function openDelete(entity: Utilisateur): void {
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

function getRoleColor(role: string): string {
  const colors: Record<string, string> = { Admin: 'error', Directeur: 'primary', Enseignant: 'info', Parent: 'success', 'Secrétaire': 'warning' }
  return colors[role] ?? 'neutral'
}
</script>

<template>
  <div class="space-y-6">
    <div>
      <UBreadcrumb
        :items="[
          { label: $t('admin.title'), to: '/admin' },
          { label: $t('admin.utilisateur.title') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('admin.utilisateur.title') }}
      </h1>
    </div>

    <DataTable :columns="columns" :data="{ items: fakeData, totalCount: fakeData.length, page: 1, pageSize: 20 }" :loading="false">
      <template #cell-nom="{ row }">
        {{ (row as Utilisateur).nom }} {{ (row as Utilisateur).prenom }}
      </template>
      <template #cell-role="{ row }">
        <UBadge :color="getRoleColor((row as Utilisateur).role)" variant="subtle" size="sm">
          {{ (row as Utilisateur).role }}
        </UBadge>
      </template>
      <template #cell-statut="{ row }">
        <UBadge :color="(row as Utilisateur).statut === 'Actif' ? 'success' : 'neutral'" variant="subtle" size="sm">
          {{ (row as Utilisateur).statut }}
        </UBadge>
      </template>
      <template #cell-actions="{ row }">
        <div class="flex gap-1">
          <UButton variant="ghost" size="xs" icon="i-heroicons-pencil-square" @click="openEdit(row as Utilisateur)" />
          <UButton variant="ghost" size="xs" color="error" icon="i-heroicons-trash" @click="openDelete(row as Utilisateur)" />
        </div>
      </template>
    </DataTable>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">{{ $t('admin.utilisateur.edit') }}</h3>
      </template>
      <template #body>
        <UForm :schema="formSchema" :state="formState" class="space-y-4 p-4" @submit="handleFormSubmit">
          <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <UFormField :label="$t('admin.utilisateur.prenom')" name="prenom" required>
              <UInput v-model="formState.prenom" class="w-full" />
            </UFormField>
            <UFormField :label="$t('admin.utilisateur.nom')" name="nom" required>
              <UInput v-model="formState.nom" class="w-full" />
            </UFormField>
          </div>
          <UFormField :label="$t('admin.utilisateur.email')" name="email" required>
            <UInput v-model="formState.email" type="email" class="w-full" />
          </UFormField>
          <UFormField :label="$t('admin.utilisateur.role')" name="role" required>
            <USelect v-model="formState.role" class="w-full" :items="[
              { label: 'Admin', value: 'Admin' },
              { label: 'Directeur', value: 'Directeur' },
              { label: 'Enseignant', value: 'Enseignant' },
              { label: 'Parent', value: 'Parent' },
              { label: 'Secrétaire', value: 'Secrétaire' },
            ]" />
          </UFormField>
          <div class="flex justify-end gap-3 pt-4">
            <UButton variant="outline" @click="showForm = false">
              {{ $t('actions.cancel') }}
            </UButton>
            <UButton type="submit" color="primary" :loading="formSaving">
              {{ $t('actions.save') }}
            </UButton>
          </div>
        </UForm>
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('admin.utilisateur.deleteTitle')"
      :description="$t('admin.utilisateur.deleteConfirm')"
      variant="danger"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
