<template>
  <USelect
    v-if="companies.length > 1"
    v-model="activeCompanyId"
    :items="companyOptions"
    size="sm"
    class="w-48"
  />
</template>

<script setup lang="ts">
import { useAuthStore } from '~/core/stores/auth.store'

const auth = useAuthStore()

const companies = computed(() => auth.user?.companies ?? [])

const companyOptions = computed(() =>
  companies.value.map(c => ({
    label: c.name,
    value: String(c.id),
  })),
)

const activeCompanyId = computed({
  get: () => String(auth.activeCompanyId ?? companies.value[0]?.id ?? ''),
  set: (val: string) => {
    auth.activeCompanyId = Number(val)
  },
})
</script>
