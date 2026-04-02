<script setup lang="ts">
import { z } from 'zod'
import type { Badgeage, CreateBadgeagePayload, UpdateBadgeagePayload } from '../types/badgeage.types'
import { POINTS_ACCES, METHODES_BADGEAGE } from '../types/badgeage.types'
import { useBadgeage } from '../composables/useBadgeage'

const props = defineProps<{
  entity?: Badgeage | null
}>()

const emit = defineEmits<{
  (e: 'submit', payload: CreateBadgeagePayload | UpdateBadgeagePayload): void
  (e: 'cancel'): void
}>()

const { t } = useI18n()
const { typeOptions } = useBadgeage()

const isEdit = computed(() => !!props.entity?.id)

const schema = z.object({
  eleveId: z.number({ required_error: t('validation.required') }).int().positive(),
  dateBadgeage: z.string().min(1, t('validation.required')),
  heureBadgeage: z.string().min(1, t('validation.required')),
  pointAcces: z.string().min(1, t('validation.required')),
  methodeBadgeage: z.string().min(1, t('validation.required')),
  typeId: z.number({ required_error: t('validation.required') }).int().positive(),
})

type FormSchema = z.output<typeof schema>

const state = reactive<Partial<FormSchema>>({
  eleveId: props.entity?.eleveId ?? undefined,
  dateBadgeage: props.entity?.dateBadgeage ?? new Date().toISOString().split('T')[0],
  heureBadgeage: props.entity?.heureBadgeage ?? new Date().toTimeString().slice(0, 5),
  pointAcces: props.entity?.pointAcces ?? '',
  methodeBadgeage: props.entity?.methodeBadgeage ?? '',
  typeId: props.entity?.typeId ?? undefined,
})

const saving = ref(false)

const pointAccesOptions = POINTS_ACCES.map(p => ({
  label: t(`presences.badgeage.pointAcces.${p}`),
  value: p,
}))

const methodeOptions = METHODES_BADGEAGE.map(m => ({
  label: t(`presences.badgeage.methode.${m}`),
  value: m,
}))

async function onSubmit(): Promise<void> {
  saving.value = true
  try {
    if (isEdit.value && props.entity) {
      emit('submit', { id: props.entity.id, ...state } as UpdateBadgeagePayload)
    }
    else {
      emit('submit', state as CreateBadgeagePayload)
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
      :label="t('presences.badgeage.typeId')"
      name="typeId"
      required
    >
      <USelect
        v-model="state.typeId"
        :items="typeOptions"
        value-key="value"
        :placeholder="t('presences.badgeage.typePlaceholder')"
        class="w-full"
      />
    </UFormField>

    <UFormField
      :label="t('presences.badgeage.eleveId')"
      name="eleveId"
      required
    >
      <UInput
        v-model.number="state.eleveId"
        type="number"
        :placeholder="t('presences.badgeage.elevePlaceholder')"
        class="w-full"
      />
    </UFormField>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('presences.badgeage.dateBadgeage')"
        name="dateBadgeage"
        required
      >
        <UInput
          v-model="state.dateBadgeage"
          type="date"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="t('presences.badgeage.heureBadgeage')"
        name="heureBadgeage"
        required
      >
        <UInput
          v-model="state.heureBadgeage"
          type="time"
          class="w-full"
        />
      </UFormField>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <UFormField
        :label="t('presences.badgeage.pointAcces')"
        name="pointAcces"
        required
      >
        <USelect
          v-model="state.pointAcces"
          :items="pointAccesOptions"
          value-key="value"
          :placeholder="t('presences.badgeage.pointAccesPlaceholder')"
          class="w-full"
        />
      </UFormField>

      <UFormField
        :label="t('presences.badgeage.methodeBadgeage')"
        name="methodeBadgeage"
        required
      >
        <USelect
          v-model="state.methodeBadgeage"
          :items="methodeOptions"
          value-key="value"
          :placeholder="t('presences.badgeage.methodePlaceholder')"
          class="w-full"
        />
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
