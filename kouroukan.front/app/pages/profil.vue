<script setup lang="ts">
import { useAuthStore } from '~/core/stores/auth.store'

definePageMeta({
  layout: 'default',
})

const { t } = useI18n()
const auth = useAuthStore()
const toast = useToast()

const saving = ref(false)
const uploadingAvatar = ref(false)
const showCurrentPassword = ref(false)
const showNewPassword = ref(false)
const showConfirmPassword = ref(false)
const { formatDateTime } = useFormatDate()

const userInitials = computed(() => {
  const first = auth.user?.firstName?.[0] ?? ''
  const last = auth.user?.lastName?.[0] ?? ''
  return `${first}${last}`.toUpperCase()
})

const form = reactive({
  firstName: auth.user?.firstName ?? '',
  lastName: auth.user?.lastName ?? '',
  email: auth.user?.email ?? '',
  currentPassword: '',
  newPassword: '',
  confirmPassword: '',
})

function triggerAvatarUpload() {
  const input = document.getElementById('avatar-file-input') as HTMLInputElement
  if (input) {
    input.value = ''
    input.click()
  }
}

async function handleAvatarChange(event: Event) {
  const input = event.target as HTMLInputElement
  const file = input.files?.[0]
  if (!file) return

  const allowedTypes = ['image/jpeg', 'image/png', 'image/webp']
  if (!allowedTypes.includes(file.type)) {
    toast.add({ title: t('profil.avatarFormatError'), color: 'error' })
    return
  }
  if (file.size > 2 * 1024 * 1024) {
    toast.add({ title: t('profil.avatarSizeError'), color: 'error' })
    return
  }

  // Upload immediatement avec fetch natif
  uploadingAvatar.value = true
  try {
    const formData = new FormData()
    formData.append('file', file)

    const response = await window.fetch('/api/auth/avatar', {
      method: 'POST',
      headers: {
        ...(auth.accessToken ? { Authorization: `Bearer ${auth.accessToken}` } : {}),
      },
      body: formData,
    })

    const data = await response.json()

    if (response.ok && data?.success && data?.data?.avatarUrl) {
      if (auth.user) {
        auth.user.avatarUrl = data.data.avatarUrl
      }
      toast.add({ title: t('profil.avatarUpdated'), color: 'success' })
    }
    else {
      toast.add({ title: data?.message || t('profil.updateError'), color: 'error' })
    }
  }
  catch (e) {
    console.error('Avatar upload error:', e)
    toast.add({ title: t('profil.updateError'), color: 'error' })
  }
  finally {
    uploadingAvatar.value = false
  }
}

async function handleSave(): Promise<void> {
  if (form.newPassword || form.currentPassword) {
    if (!form.currentPassword) {
      toast.add({ title: t('profil.currentPasswordRequired'), color: 'error' })
      return
    }
    if (form.newPassword.length < 8) {
      toast.add({ title: t('changePassword.minLength'), color: 'error' })
      return
    }
    if (form.newPassword !== form.confirmPassword) {
      toast.add({ title: t('changePassword.mismatch'), color: 'error' })
      return
    }
  }

  saving.value = true
  try {
    if (form.currentPassword && form.newPassword) {
      await auth.changePassword(form.currentPassword, form.newPassword)
      form.currentPassword = ''
      form.newPassword = ''
      form.confirmPassword = ''
    }

    toast.add({ title: t('profil.updateSuccess'), color: 'success' })
  }
  catch {
    // Le toast d'erreur est affiche par le auth store
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

    <!-- Input file global -->
    <input
      id="avatar-file-input"
      type="file"
      accept="image/jpeg,image/png,image/webp"
      style="display: none;"
      @change="handleAvatarChange"
    >

    <div class="grid grid-cols-1 gap-6 lg:grid-cols-3">
      <!-- Avatar -->
      <div class="flex flex-col items-center gap-4 rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-900">
        <div
          class="group relative cursor-pointer"
          @click="triggerAvatarUpload"
        >
          <span
            v-if="auth.user?.avatarUrl"
            class="inline-flex h-24 w-24 shrink-0 items-center justify-center overflow-hidden rounded-full"
          >
            <img
              :src="auth.user.avatarUrl"
              alt="avatar"
              class="h-full w-full object-cover"
            >
          </span>
          <span
            v-else
            class="inline-flex h-24 w-24 items-center justify-center rounded-full bg-indigo-100 text-2xl font-bold text-indigo-600 dark:bg-indigo-900/30 dark:text-indigo-400"
          >
            {{ userInitials }}
          </span>
          <div class="absolute inset-0 flex items-center justify-center rounded-full bg-black/50 opacity-0 transition-opacity group-hover:opacity-100">
            <UIcon
              :name="uploadingAvatar ? 'i-heroicons-arrow-path' : 'i-heroicons-camera'"
              :class="['h-8 w-8 text-white', uploadingAvatar ? 'animate-spin' : '']"
            />
          </div>
        </div>

        <UButton
          variant="soft"
          size="xs"
          icon="i-heroicons-camera"
          :loading="uploadingAvatar"
          @click="triggerAvatarUpload"
        >
          {{ $t('profil.clickToChangeAvatar') }}
        </UButton>

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
            {{ auth.lastLoginAt ? formatDateTime(auth.lastLoginAt) : '—' }}
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
              <UInput
                v-model="form.firstName"
                class="w-full"
              />
            </UFormField>
            <UFormField :label="$t('profil.lastName')">
              <UInput
                v-model="form.lastName"
                class="w-full"
              />
            </UFormField>
            <UFormField
              :label="$t('profil.email')"
              class="sm:col-span-2"
            >
              <UInput
                v-model="form.email"
                type="email"
                class="w-full"
                disabled
              />
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
              <UInput
                v-model="form.currentPassword"
                :type="showCurrentPassword ? 'text' : 'password'"
                class="w-full"
              >
                <template #trailing>
                  <UButton
                    variant="ghost"
                    size="xs"
                    :icon="showCurrentPassword ? 'i-heroicons-eye-slash' : 'i-heroicons-eye'"
                    @click="showCurrentPassword = !showCurrentPassword"
                  />
                </template>
              </UInput>
            </UFormField>
            <UFormField :label="$t('profil.newPassword')">
              <UInput
                v-model="form.newPassword"
                :type="showNewPassword ? 'text' : 'password'"
                class="w-full"
              >
                <template #trailing>
                  <UButton
                    variant="ghost"
                    size="xs"
                    :icon="showNewPassword ? 'i-heroicons-eye-slash' : 'i-heroicons-eye'"
                    @click="showNewPassword = !showNewPassword"
                  />
                </template>
              </UInput>
            </UFormField>
            <UFormField :label="$t('profil.confirmPassword')">
              <UInput
                v-model="form.confirmPassword"
                :type="showConfirmPassword ? 'text' : 'password'"
                class="w-full"
              >
                <template #trailing>
                  <UButton
                    variant="ghost"
                    size="xs"
                    :icon="showConfirmPassword ? 'i-heroicons-eye-slash' : 'i-heroicons-eye'"
                    @click="showConfirmPassword = !showConfirmPassword"
                  />
                </template>
              </UInput>
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
