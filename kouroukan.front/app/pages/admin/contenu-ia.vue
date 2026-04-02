<script setup lang="ts">
import { z } from 'zod'
import type { Column } from '~/shared/components/DataTable.vue'
import { useAdminContenuIA } from '~/modules/admin/composables/useAdminContenuIA'
import type { ContenuIA } from '~/modules/admin/types/admin.types'

definePageMeta({ layout: 'default' })

const { t } = useI18n()

const {
  items,
  loading,
  saving,
  paginatedData,
  fetchAll,
  create,
  update,
  remove,
  changePage,
} = useAdminContenuIA()

const showForm = ref(false)
const editingEntity = ref<ContenuIA | null>(null)
const showDeleteDialog = ref(false)
const deletingEntity = ref<ContenuIA | null>(null)
const activeRubrique = ref('all')

const formSchema = z.object({
  rubrique: z.string().min(1, t('validation.required')),
  titre: z.string().min(1, t('validation.required')),
  contenu: z.string().min(1, t('validation.required')),
})

const formState = reactive({
  rubrique: 'pedagogique',
  titre: '',
  contenu: '',
})

const formSaving = ref(false)

const isEdit = computed(() => !!editingEntity.value?.id)

watch(() => editingEntity.value, (entity) => {
  if (entity) {
    formState.rubrique = entity.rubrique ?? 'pedagogique'
    formState.titre = entity.titre ?? ''
    formState.contenu = entity.contenu ?? ''
  }
  else {
    formState.rubrique = 'pedagogique'
    formState.titre = ''
    formState.contenu = ''
  }
}, { immediate: true })

async function handleFormSubmit(): Promise<void> {
  formSaving.value = true
  try {
    if (isEdit.value && editingEntity.value) {
      const result = await update(editingEntity.value.id, {
        id: editingEntity.value.id,
        rubrique: formState.rubrique,
        titre: formState.titre,
        contenu: formState.contenu,
      })
      if (result) showForm.value = false
    }
    else {
      const result = await create({
        rubrique: formState.rubrique,
        titre: formState.titre,
        contenu: formState.contenu,
      })
      if (result) showForm.value = false
    }
  }
  finally {
    formSaving.value = false
  }
}

const rubriques = [
  { value: 'all', label: t('admin.contenuIA.toutesRubriques') },
  { value: 'pedagogique', label: t('admin.contenuIA.pedagogique') },
  { value: 'systeme', label: t('admin.contenuIA.systemeEducatif') },
  { value: 'dates', label: t('admin.contenuIA.datesImportantes') },
  { value: 'reglements', label: t('admin.contenuIA.reglements') },
]

const filteredData = computed(() => {
  if (activeRubrique.value === 'all') return items.value
  return items.value.filter(c => c.rubrique === activeRubrique.value)
})

const columns: Column[] = [
  { key: 'rubrique', label: t('admin.contenuIA.rubrique'), sortable: true },
  { key: 'titre', label: t('admin.contenuIA.titre'), sortable: true },
  { key: 'updatedAt', label: t('admin.contenuIA.dateModification'), sortable: true },
  { key: 'auteur', label: t('admin.contenuIA.auteur'), sortable: false },
  { key: 'actions', label: '', sortable: false, class: 'w-24' },
]

onMounted(() => {
  fetchAll()
})

function openCreate(): void {
  editingEntity.value = null
  showForm.value = true
}

function openEdit(entity: ContenuIA): void {
  editingEntity.value = entity
  showForm.value = true
}

function openDelete(entity: ContenuIA): void {
  deletingEntity.value = entity
  showDeleteDialog.value = true
}

async function handleDelete(): Promise<void> {
  if (!deletingEntity.value) return
  const success = await remove(deletingEntity.value.id)
  if (success) {
    showDeleteDialog.value = false
    deletingEntity.value = null
  }
}

function getRubriqueColor(rubrique: string): string {
  const colors: Record<string, string> = { pedagogique: 'primary', systeme: 'info', dates: 'warning', reglements: 'success' }
  return colors[rubrique] ?? 'neutral'
}

function getRubriqueLabel(rubrique: string): string {
  const labels: Record<string, string> = {
    pedagogique: t('admin.contenuIA.pedagogique'),
    systeme: t('admin.contenuIA.systemeEducatif'),
    dates: t('admin.contenuIA.datesImportantes'),
    reglements: t('admin.contenuIA.reglements'),
  }
  return labels[rubrique] ?? rubrique
}
</script>

<template>
  <div class="space-y-6">
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <UBreadcrumb
          :items="[
            { label: $t('admin.title'), to: '/admin' },
            { label: $t('admin.contenuIA.title') },
          ]"
        />
        <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('admin.contenuIA.title') }}
        </h1>
      </div>
      <UButton
        color="primary"
        icon="i-heroicons-plus"
        @click="openCreate"
      >
        {{ $t('admin.contenuIA.add') }}
      </UButton>
    </div>

    <!-- Rubrique Filter -->
    <div class="flex flex-wrap gap-2">
      <UButton
        v-for="r in rubriques"
        :key="r.value"
        :variant="activeRubrique === r.value ? 'solid' : 'outline'"
        size="sm"
        @click="activeRubrique = r.value"
      >
        {{ r.label }}
      </UButton>
    </div>

    <DataTable
      :columns="columns"
      :data="{ items: filteredData, totalCount: filteredData.length, page: 1, pageSize: 20 }"
      :loading="loading"
    >
      <template #cell-rubrique="{ row }">
        <UBadge
          :color="getRubriqueColor((row as ContenuIA).rubrique)"
          variant="subtle"
          size="sm"
        >
          {{ getRubriqueLabel((row as ContenuIA).rubrique) }}
        </UBadge>
      </template>
      <template #cell-actions="{ row }">
        <div class="flex gap-1">
          <UButton
            variant="ghost"
            size="xs"
            icon="i-heroicons-pencil-square"
            @click="openEdit(row as ContenuIA)"
          />
          <UButton
            variant="ghost"
            size="xs"
            color="error"
            icon="i-heroicons-trash"
            @click="openDelete(row as ContenuIA)"
          />
        </div>
      </template>
    </DataTable>

    <USlideover v-model:open="showForm">
      <template #header>
        <h3 class="text-lg font-semibold">
          {{ editingEntity ? $t('admin.contenuIA.edit') : $t('admin.contenuIA.add') }}
        </h3>
      </template>
      <template #body>
        <UForm
          :schema="formSchema"
          :state="formState"
          class="space-y-4 p-4"
          @submit="handleFormSubmit"
        >
          <UFormField
            :label="$t('admin.contenuIA.rubrique')"
            name="rubrique"
            required
          >
            <USelect
              v-model="formState.rubrique"
              class="w-full"
              :items="[
                { label: $t('admin.contenuIA.pedagogique'), value: 'pedagogique' },
                { label: $t('admin.contenuIA.systemeEducatif'), value: 'systeme' },
                { label: $t('admin.contenuIA.datesImportantes'), value: 'dates' },
                { label: $t('admin.contenuIA.reglements'), value: 'reglements' },
              ]"
            />
          </UFormField>
          <UFormField
            :label="$t('admin.contenuIA.titre')"
            name="titre"
            required
          >
            <UInput
              v-model="formState.titre"
              class="w-full"
            />
          </UFormField>
          <UFormField
            label="Contenu"
            name="contenu"
            required
          >
            <UTextarea
              v-model="formState.contenu"
              class="w-full"
              :rows="8"
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
              {{ isEdit ? $t('actions.save') : $t('actions.create') }}
            </UButton>
          </div>
        </UForm>
      </template>
    </USlideover>

    <ConfirmDialog
      :open="showDeleteDialog"
      :title="$t('admin.contenuIA.deleteTitle')"
      :description="$t('admin.contenuIA.deleteConfirm')"
      variant="danger"
      :loading="saving"
      @confirm="handleDelete"
      @cancel="showDeleteDialog = false"
      @update:open="showDeleteDialog = $event"
    />
  </div>
</template>
