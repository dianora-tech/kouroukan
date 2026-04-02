<script setup lang="ts">
import { z } from 'zod'
import type { Souscription, CreateSouscriptionPayload, UpdateSouscriptionPayload } from '../types/souscription.types'
import { STATUTS_SOUSCRIPTION } from '../types/souscription.types'
import { useServiceParent } from '../composables/useServiceParent'

const props = defineProps<{
  entity?: Souscription | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateSouscriptionPayload | UpdateSouscriptionPayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const { typeOptions: serviceOptions } = useServiceParent()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  serviceParentId: z.number({ required_error: t('validation.required') }).int().positive(),
  parentId: z.number({ required_error: t('validation.required') }).int().positive(),
  dateDebut: z.string().min(1, t('validation.required')),
  dateFin: z.string().optional().nullable(),
  statutSouscription: z.string().min(1, t('validation.required')),
  montantPaye: z.number({ required_error: t('validation.required') }).min(0),
  renouvellementAuto: z.boolean(),
  dateProchainRenouvellement: z.string().optional().nullable(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  serviceParentId: props.entity?.serviceParentId ?? undefined,
  parentId: props.entity?.parentId ?? undefined,
  dateDebut: props.entity?.dateDebut ?? new Date().toISOString().split('T')[0],
  dateFin: props.entity?.dateFin ?? null,
  statutSouscription: props.entity?.statutSouscription ?? 'Active',
  montantPaye: props.entity?.montantPaye ?? 0,
  renouvellementAuto: props.entity?.renouvellementAuto ?? false,
  dateProchainRenouvellement: props.entity?.dateProchainRenouvellement ?? null,
})

const saving = ref(false)

const statutOptions = STATUTS_SOUSCRIPTION.map(s => ({
  label: t(`servicesPremium.souscription.statut.${s}`),
  value: s,
}))

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateSouscriptionPayload)
    }
    else {
      emit('submit', state as CreateSouscriptionPayload)
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
      :label="t('servicesPremium.souscription.serviceParentId')"
      name="serviceParentId"
      required
    >
      <UInput
        v-model.number="state.serviceParentId"
        type="number"
        min="1"
        :placeholder="t('servicesPremium.souscription.serviceParentPlaceholder')"
        class="w-full"
      />
    </UFormField>

    <UFormField
      :label="t('servicesPremium.souscription.parentId')"
      name="parentId"
      required
    >
      <UInput
        v-model.number="state.parentId"
        type="number"
        min="1"
        :placeholder="t('servicesPremium.souscription.parentPlaceholder')"
        class="w-full"
      />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('servicesPremium.souscription.dateDebut')"
        name="dateDebut"
        required
      >
        <UInput
          v-model="state.dateDebut"
          type="date"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="t('servicesPremium.souscription.dateFin')"
        name="dateFin"
      >
        <UInput
          v-model="state.dateFin"
          type="date"
          class="w-full"
        />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('servicesPremium.souscription.montantPaye')"
        name="montantPaye"
        required
      >
        <UInput
          v-model.number="state.montantPaye"
          type="number"
          min="0"
          step="1"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="t('servicesPremium.souscription.statutSouscription')"
        name="statutSouscription"
        required
      >
        <USelect
          v-model="state.statutSouscription"
          :items="statutOptions"
          value-key="value"
          class="w-full"
        />
      </UFormField>
    </div>

    <UFormField
      :label="t('servicesPremium.souscription.renouvellementAuto')"
      name="renouvellementAuto"
    >
      <UToggle v-model="state.renouvellementAuto" />
    </UFormField>

    <UFormField
      v-if="state.renouvellementAuto"
      :label="t('servicesPremium.souscription.dateProchainRenouvellement')"
      name="dateProchainRenouvellement"
    >
      <UInput
        v-model="state.dateProchainRenouvellement"
        type="date"
        class="w-full"
      />
    </UFormField>

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
