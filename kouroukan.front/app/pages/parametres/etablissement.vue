<script setup lang="ts">
import { useAuthStore } from '~/core/stores/auth.store'
import { apiClient } from '~/core/api/client'

const { t } = useI18n()
const auth = useAuthStore()

interface Company {
  id: number
  name: string
  role: string
}

const companies = ref<Company[]>([])
const loading = ref(true)

async function fetchCompanies() {
  loading.value = true
  try {
    const response = await apiClient.get<Company[]>('/api/users/companies')
    if (response.success && response.data) {
      companies.value = response.data
    }
  }
  finally {
    loading.value = false
  }
}

const roleLabels: Record<string, string> = {
  owner: 'Directeur (propriétaire)',
  fondateur: 'Fondateur',
  member: 'Membre',
}

onMounted(() => {
  fetchCompanies()
})
</script>

<template>
  <div>
    <div class="mb-6">
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white">{{ $t('etablissement.title') }}</h1>
      <p class="mt-1 text-sm text-gray-500">{{ $t('etablissement.subtitle') }}</p>
    </div>

    <div v-if="loading" class="flex items-center justify-center p-12">
      <UIcon name="i-heroicons-arrow-path" class="h-6 w-6 animate-spin text-gray-400" />
    </div>

    <div v-else-if="companies.length === 0" class="rounded-xl border border-gray-200 bg-white p-12 text-center dark:border-gray-700 dark:bg-gray-800">
      <UIcon name="i-heroicons-building-office-2" class="mx-auto mb-4 h-12 w-12 text-gray-300" />
      <p class="text-gray-500">{{ $t('etablissement.empty') }}</p>
    </div>

    <div v-else class="space-y-4">
      <div
        v-for="company in companies"
        :key="company.id"
        class="rounded-xl border border-gray-200 bg-white p-6 dark:border-gray-700 dark:bg-gray-800"
        :class="auth.activeCompanyId === company.id ? 'ring-2 ring-green-500' : ''"
      >
        <div class="flex items-start justify-between">
          <div>
            <div class="flex items-center gap-3">
              <div class="flex h-10 w-10 items-center justify-center rounded-lg bg-green-100 dark:bg-green-900/30">
                <UIcon name="i-heroicons-building-office-2" class="h-5 w-5 text-green-600" />
              </div>
              <div>
                <h3 class="text-lg font-semibold text-gray-900 dark:text-white">{{ company.name }}</h3>
                <UBadge :color="company.role === 'owner' ? 'primary' : 'neutral'" variant="subtle" size="sm">
                  {{ roleLabels[company.role] || company.role }}
                </UBadge>
              </div>
            </div>
          </div>

          <UButton
            v-if="auth.activeCompanyId !== company.id"
            variant="outline"
            size="sm"
            @click="auth.activeCompanyId = company.id"
          >
            {{ $t('etablissement.select') }}
          </UButton>
          <UBadge v-else color="success" variant="solid" size="sm">
            {{ $t('etablissement.active') }}
          </UBadge>
        </div>
      </div>
    </div>
  </div>
</template>
