<script setup lang="ts">
import { z } from 'zod'
import type { ServiceParent, CreateServiceParentPayload, UpdateServiceParentPayload } from '../types/service-parent.types'
import { PERIODICITES } from '../types/service-parent.types'
import { useServiceParent } from '../composables/useServiceParent'

const props = defineProps<{
  entity?: ServiceParent | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateServiceParentPayload | UpdateServiceParentPayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const { typeOptions } = useServiceParent()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  code: z.string().min(1, t('validation.required')).max(50),
  tarif: z.number({ required_error: t('validation.required') }).min(0),
  periodicite: z.string().min(1, t('validation.required')),
  estActif: z.boolean(),
  periodeEssaiJours: z.number().int().min(0).optional().nullable(),
  tarifDegressif: z.boolean(),
  typeId: z.number({ required_error: t('validation.required') }).int().positive(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  code: props.entity?.code ?? '',
  tarif: props.entity?.tarif ?? 0,
  periodicite: props.entity?.periodicite ?? 'Mensuel',
  estActif: props.entity?.estActif ?? true,
  periodeEssaiJours: props.entity?.periodeEssaiJours ?? null,
  tarifDegressif: props.entity?.tarifDegressif ?? false,
  typeId: props.entity?.typeId ?? undefined,
})

const saving = ref(false)

const periodiciteOptions = PERIODICITES.map(p => ({
  label: t(`servicesPremium.serviceParent.periodicite.${p}`),
  value: p,
}))

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateServiceParentPayload)
    }
    else {
      emit('submit', state as CreateServiceParentPayload)
    }
  }
  finally {
    saving.value = false
  }
}
</script>

<template>
  <UForm :schema="schema" :state="state" class="space-y-4" @submit="onSubmit">
    <UFormField :label="t('servicesPremium.serviceParent.typeId')" name="typeId" required>
      <USelect v-model="state.typeId" :items="typeOptions" value-key="value" :placeholder="t('servicesPremium.serviceParent.typePlaceholder')" class="w-full" />
    </UFormField>

    <UFormField :label="t('servicesPremium.serviceParent.code')" name="code" required>
      <UInput v-model="state.code" :placeholder="t('servicesPremium.serviceParent.codePlaceholder')" class="w-full" />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('servicesPremium.serviceParent.tarif')" name="tarif" required>
        <UInput v-model.number="state.tarif" type="number" min="0" step="1" class="w-full" />
      </UFormField>

      <UFormField :label="t('servicesPremium.serviceParent.periodiciteLabel')" name="periodicite" required>
        <USelect v-model="state.periodicite" :items="periodiciteOptions" value-key="value" class="w-full" />
      </UFormField>
    </div>

    <UFormField :label="t('servicesPremium.serviceParent.periodeEssaiJours')" name="periodeEssaiJours">
      <UInput v-model.number="state.periodeEssaiJours" type="number" min="0" step="1" :placeholder="t('servicesPremium.serviceParent.periodeEssaiPlaceholder')" class="w-full" />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField :label="t('servicesPremium.serviceParent.estActif')" name="estActif">
        <UToggle v-model="state.estActif" />
      </UFormField>

      <UFormField :label="t('servicesPremium.serviceParent.tarifDegressif')" name="tarifDegressif">
        <UToggle v-model="state.tarifDegressif" />
      </UFormField>
    </div>

    <div class="flex justify-end gap-3 pt-4">
      <UButton variant="outline" @click="emit('cancel')">
        {{ $t('actions.cancel') }}
      </UButton>
      <UButton type="submit" color="primary" :loading="saving">
        {{ isEdit ? $t('actions.save') : $t('actions.create') }}
      </UButton>
    </div>
  </UForm>
</template>
