<template>
  <div>
    <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">{{ $t('inscription.step3.title') }}</h2>

    <div class="space-y-5">
      <UFormField :label="$t('inscription.step3.region')" name="region">
        <USelect
          v-model="data.region"
          :items="regionOptions"
          :placeholder="$t('inscription.step3.regionPlaceholder')"
          class="w-full"
        />
      </UFormField>

      <UFormField :label="$t('inscription.step3.prefecture')" name="prefecture">
        <UInput
          v-model="data.prefecture"
          :placeholder="$t('inscription.step3.prefecturePlaceholder')"
          class="w-full"
        />
      </UFormField>

      <UFormField :label="$t('inscription.step3.address')" name="address">
        <UInput v-model="data.address" class="w-full" />
      </UFormField>
    </div>
  </div>
</template>

<script setup lang="ts">
import { GUINEA_REGIONS } from '~/utils/constants'

defineProps<{
  data: {
    region: string
    prefecture: string
    sousPrefecture: string
    address: string
  }
}>()

const regionOptions = GUINEA_REGIONS.map(r => ({
  label: r.name,
  value: r.code
}))
</script>
