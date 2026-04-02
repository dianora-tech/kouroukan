<script setup lang="ts">
import { z } from 'zod'
import type { MembreBDE, CreateMembreBDEPayload, UpdateMembreBDEPayload } from '../types/membre-bde.types'
import { ROLES_BDE } from '../types/membre-bde.types'
import { useAssociation } from '../composables/useAssociation'

const props = defineProps<{
  entity?: MembreBDE | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateMembreBDEPayload | UpdateMembreBDEPayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const { items: associations } = useAssociation()

const isEdit = computed(() => !!props.entity?.id)

const associationOptions = computed(() =>
  associations.value.map(a => ({ label: a.name, value: a.id })),
)

const schema = z.object({
  name: z.string().min(3, t('validation.required')),
  description: z.string().optional().nullable().or(z.literal('')),
  associationId: z.number({ required_error: t('validation.required') }).int().positive(),
  eleveId: z.number({ required_error: t('validation.required') }).int().positive(),
  roleBDE: z.string().min(1, t('validation.required')),
  dateAdhesion: z.string().min(1, t('validation.required')),
  montantCotisation: z.number().min(0).optional().nullable(),
  estActif: z.boolean(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  name: props.entity?.name ?? '',
  description: props.entity?.description ?? '',
  associationId: props.entity?.associationId ?? undefined,
  eleveId: props.entity?.eleveId ?? undefined,
  roleBDE: props.entity?.roleBDE ?? 'Membre',
  dateAdhesion: props.entity?.dateAdhesion ?? new Date().toISOString().split('T')[0],
  montantCotisation: props.entity?.montantCotisation ?? undefined,
  estActif: props.entity?.estActif ?? true,
})

const saving = ref(false)

const roleOptions = ROLES_BDE.map(r => ({
  label: t(`bde.membreBde.role.${r}`),
  value: r,
}))

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateMembreBDEPayload)
    }
    else {
      emit('submit', state as CreateMembreBDEPayload)
    }
  }
  finally {
    saving.value = false
  }
}
</script>

<template>
  <UForm
    :schema="schema"
    :state="state"
    class="space-y-4"
    @submit="onSubmit"
  >
    <UFormField
      :label="t('bde.membreBde.associationId')"
      name="associationId"
      required
    >
      <USelect
        v-model="state.associationId"
        :items="associationOptions"
        value-key="value"
        :placeholder="t('bde.membreBde.associationPlaceholder')"
        class="w-full"
      />
    </UFormField>

    <UFormField
      :label="t('bde.membreBde.name')"
      name="name"
      required
    >
      <UInput
        v-model="state.name"
        :placeholder="t('bde.membreBde.namePlaceholder')"
        class="w-full"
      />
    </UFormField>

    <UFormField
      :label="t('bde.membreBde.eleveId')"
      name="eleveId"
      required
    >
      <UInput
        v-model.number="state.eleveId"
        type="number"
        :placeholder="t('bde.membreBde.eleveIdPlaceholder')"
        class="w-full"
      />
    </UFormField>

    <UFormField
      :label="t('bde.membreBde.description')"
      name="description"
    >
      <UTextarea
        v-model="state.description"
        :placeholder="t('bde.membreBde.descriptionPlaceholder')"
        class="w-full"
      />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('bde.membreBde.roleBDE')"
        name="roleBDE"
        required
      >
        <USelect
          v-model="state.roleBDE"
          :items="roleOptions"
          value-key="value"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="t('bde.membreBde.dateAdhesion')"
        name="dateAdhesion"
        required
      >
        <UInput
          v-model="state.dateAdhesion"
          type="date"
          class="w-full"
        />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('bde.membreBde.montantCotisation')"
        name="montantCotisation"
      >
        <UInput
          v-model.number="state.montantCotisation"
          type="number"
          min="0"
          step="1"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="t('bde.membreBde.estActif')"
        name="estActif"
      >
        <UToggle v-model="state.estActif" />
      </UFormField>
    </div>

    <div class="flex justify-end gap-3 pt-4">
      <UButton
        variant="outline"
        @click="emit('cancel')"
      >
        {{ $t('actions.cancel') }}
      </UButton>
      <UButton
        type="submit"
        color="primary"
        :loading="saving"
      >
        {{ isEdit ? $t('actions.save') : $t('actions.create') }}
      </UButton>
    </div>
  </UForm>
</template>
