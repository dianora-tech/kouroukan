<script setup lang="ts">
import { useQRCode } from '~/composables/useQRCode'
import { useAuthStore } from '~/core/stores/auth.store'
import { apiClient } from '~/core/api/client'

definePageMeta({ layout: 'default' })

const { t } = useI18n()
const toast = useToast()
const auth = useAuthStore()
const { qrData, loading: qrLoading, getMyQR } = useQRCode()

const profil = reactive({
  nom: auth.user?.lastName ?? '',
  prenom: auth.user?.firstName ?? '',
  telephone: auth.user?.phone ?? '',
  email: auth.user?.email ?? '',
  identifiant: '',
  codeQR: '',
})

const saving = ref(false)

async function saveProfile(): Promise<void> {
  saving.value = true
  try {
    const response = await apiClient.put('/api/auth/profile', {
      firstName: profil.prenom,
      lastName: profil.nom,
      phone: profil.telephone,
      email: profil.email,
    })
    if (response.success) {
      toast.add({ title: t('enseignant.profil.saveSuccess'), color: 'success' })
    }
    else {
      toast.add({ title: t('enseignant.profil.saveError'), color: 'error' })
    }
  }
  catch {
    toast.add({ title: t('enseignant.profil.saveError'), color: 'error' })
  }
  finally {
    saving.value = false
  }
}

onMounted(async () => {
  const data = await getMyQR()
  if (data) {
    profil.identifiant = data.code
    profil.codeQR = data.qrCodeUrl
  }
})
</script>

<template>
  <div class="space-y-6">
    <div>
      <UBreadcrumb
        :items="[
          { label: $t('enseignant.title'), to: '/enseignant' },
          { label: $t('enseignant.profil.title') },
        ]"
      />
      <h1 class="mt-2 text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('enseignant.profil.title') }}
      </h1>
    </div>

    <div class="grid grid-cols-1 gap-6 lg:grid-cols-3">
      <!-- QR Code & Identifiant -->
      <div class="flex flex-col items-center gap-4 rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-900">
        <div class="flex h-48 w-48 items-center justify-center rounded-lg border-2 border-dashed border-gray-300 bg-gray-50 dark:border-gray-600 dark:bg-gray-800">
          <div v-if="qrLoading" class="text-center">
            <UIcon name="i-heroicons-arrow-path" class="mx-auto h-8 w-8 animate-spin text-gray-400" />
          </div>
          <img v-else-if="profil.codeQR" :src="profil.codeQR" alt="QR Code" class="h-full w-full object-contain" />
          <div v-else class="text-center">
            <UIcon name="i-heroicons-qr-code" class="mx-auto h-16 w-16 text-gray-400" />
            <p class="mt-2 text-xs text-gray-500">{{ $t('enseignant.profil.qrCode') }}</p>
          </div>
        </div>
        <div class="text-center">
          <p class="text-sm text-gray-500">{{ $t('enseignant.profil.identifiant') }}</p>
          <p class="mt-1 font-mono text-lg font-bold text-gray-900 dark:text-white">{{ profil.identifiant }}</p>
        </div>
        <UButton variant="outline" size="sm" icon="i-heroicons-share">
          {{ $t('enseignant.profil.partagerQR') }}
        </UButton>
      </div>

      <!-- Infos personnelles -->
      <div class="space-y-6 lg:col-span-2">
        <div class="rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-900">
          <h2 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">
            {{ $t('enseignant.profil.infosPersonnelles') }}
          </h2>
          <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <UFormField :label="$t('enseignant.profil.nom')">
              <UInput v-model="profil.nom" class="w-full" />
            </UFormField>
            <UFormField :label="$t('enseignant.profil.prenom')">
              <UInput v-model="profil.prenom" class="w-full" />
            </UFormField>
            <UFormField :label="$t('enseignant.profil.telephone')">
              <UInput v-model="profil.telephone" class="w-full" />
            </UFormField>
            <UFormField :label="$t('enseignant.profil.email')">
              <UInput v-model="profil.email" type="email" class="w-full" />
            </UFormField>
          </div>
          <div class="mt-4 flex justify-end">
            <UButton color="primary" icon="i-heroicons-check" :loading="saving" @click="saveProfile">
              {{ $t('enseignant.profil.enregistrer') }}
            </UButton>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
