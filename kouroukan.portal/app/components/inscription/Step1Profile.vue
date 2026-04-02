<template>
  <div>
    <h2 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">{{ $t('inscription.step1.title') }}</h2>

    <div class="space-y-5">
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <UFormField :label="$t('inscription.step1.firstName')" name="firstName" required>
          <UInput v-model="data.firstName" class="w-full" />
        </UFormField>
        <UFormField :label="$t('inscription.step1.lastName')" name="lastName" required>
          <UInput v-model="data.lastName" class="w-full" />
        </UFormField>
      </div>

      <!-- School name: only for etablissement -->
      <UFormField v-if="accountType === 'etablissement'" :label="$t('inscription.step1.schoolName')" name="schoolName">
        <UInput
          v-model="data.schoolName"
          :placeholder="$t('inscription.step1.schoolNamePlaceholder')"
          class="w-full"
        />
      </UFormField>

      <UFormField :label="$t('inscription.step1.phone')" name="phone" required>
        <PhoneInput v-model="data.phone" />
      </UFormField>

      <!-- Email: optional for parent, shown for others -->
      <UFormField
        :label="accountType === 'parent' ? $t('inscription.step1.emailOptional') : $t('inscription.step1.email')"
        name="email"
      >
        <UInput v-model="data.email" type="email" class="w-full" />
      </UFormField>

      <UFormField :label="$t('inscription.step1.password')" name="password" required>
        <UInput v-model="data.password" type="password" class="w-full" />
      </UFormField>

      <UFormField :label="$t('inscription.step1.confirmPassword')" name="confirmPassword" required>
        <UInput v-model="data.confirmPassword" type="password" class="w-full" />
      </UFormField>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { AccountType } from '~/utils/types'

defineProps<{
  data: {
    firstName: string
    lastName: string
    schoolName: string
    phone: string
    email: string
    password: string
    confirmPassword: string
  }
  accountType: AccountType | ''
}>()
</script>
