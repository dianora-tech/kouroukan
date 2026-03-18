<script setup lang="ts">
import { useAuthStore } from '~/core/stores/auth.store'

definePageMeta({
  layout: 'default',
})

const { t } = useI18n()
const auth = useAuthStore()
const toast = useToast()

const saving = ref(false)

const form = reactive({
  firstName: auth.user?.firstName ?? '',
  lastName: auth.user?.lastName ?? '',
  email: auth.user?.email ?? '',
  currentPassword: '',
  newPassword: '',
  confirmPassword: '',
})

async function handleSave(): Promise<void> {
  if (form.newPassword && form.newPassword !== form.confirmPassword) {
    toast.add({ title: t('profil.updateError'), color: 'error' })
    return
  }
  saving.value = true
  try {
    toast.add({ title: t('profil.updateSuccess'), color: 'success' })
  }
  catch {
    toast.add({ title: t('profil.updateError'), color: 'error' })
  }
  finally {
    saving.value = false
  }
}
</script>

<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('profil.title') }}
      </h1>
      <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
        {{ $t('profil.subtitle') }}
      </p>
    </div>

    <div class="grid grid-cols-1 gap-6 lg:grid-cols-3">
      <!-- Avatar -->
      <div class="flex flex-col items-center gap-4 rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-900">
        <div class="flex h-24 w-24 items-center justify-center rounded-full bg-indigo-100 dark:bg-indigo-900/30">
          <UIcon name="i-heroicons-user-circle" class="h-16 w-16 text-indigo-600 dark:text-indigo-400" />
        </div>
        <div class="text-center">
          <p class="font-semibold text-gray-900 dark:text-white">
            {{ auth.user?.firstName }} {{ auth.user?.lastName }}
          </p>
          <p class="text-sm text-gray-500 dark:text-gray-400">
            {{ auth.user?.email }}
          </p>
          <div class="mt-2 flex flex-wrap justify-center gap-1">
            <UBadge
              v-for="role in auth.roles"
              :key="role"
              color="indigo"
              variant="subtle"
              size="sm"
            >
              {{ role }}
            </UBadge>
          </div>
        </div>
        <div class="w-full border-t border-gray-100 pt-4 dark:border-gray-800">
          <p class="text-center text-xs text-gray-400 dark:text-gray-500">
            {{ $t('profil.lastLogin') }}:
            {{ auth.lastLoginAt ? new Date(auth.lastLoginAt).toLocaleString() : '—' }}
          </p>
        </div>
      </div>

      <!-- Forms -->
      <div class="space-y-6 lg:col-span-2">
        <!-- Personal info -->
        <div class="rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-900">
          <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
            {{ $t('profil.personalInfo') }}
          </h2>
          <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <UFormField :label="$t('profil.firstName')">
              <UInput v-model="form.firstName" class="w-full" />
            </UFormField>
            <UFormField :label="$t('profil.lastName')">
              <UInput v-model="form.lastName" class="w-full" />
            </UFormField>
            <UFormField :label="$t('profil.email')" class="sm:col-span-2">
              <UInput v-model="form.email" type="email" class="w-full" disabled />
            </UFormField>
          </div>
        </div>

        <!-- Password -->
        <div class="rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-900">
          <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
            {{ $t('profil.changePassword') }}
          </h2>
          <div class="space-y-4">
            <UFormField :label="$t('profil.currentPassword')">
              <UInput v-model="form.currentPassword" type="password" class="w-full" />
            </UFormField>
            <UFormField :label="$t('profil.newPassword')">
              <UInput v-model="form.newPassword" type="password" class="w-full" />
            </UFormField>
            <UFormField :label="$t('profil.confirmPassword')">
              <UInput v-model="form.confirmPassword" type="password" class="w-full" />
            </UFormField>
          </div>
        </div>

        <div class="flex justify-end">
          <UButton
            color="primary"
            :loading="saving"
            icon="i-heroicons-check"
            @click="handleSave"
          >
            {{ $t('profil.saveChanges') }}
          </UButton>
        </div>
      </div>
    </div>
  </div>
</template>
